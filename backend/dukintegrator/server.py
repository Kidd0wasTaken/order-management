from __future__ import annotations

import os
import subprocess
from pathlib import Path

from fastapi import FastAPI
from pydantic import BaseModel
from reportlab.lib.pagesizes import A4
from reportlab.pdfgen import canvas

app = FastAPI(title="DUKIntegrator Wrapper")

JAR_PATH = Path(os.environ.get("DUK_JAR_PATH", "/opt/duk/DUKIntegrator.jar"))
EXPORT_ROOT = Path(os.environ.get("EXPORT_ROOT", "/exports"))


class GeneratePdfRequest(BaseModel):
    xmlPath: str
    outputDir: str


class GeneratePdfResponse(BaseModel):
    success: bool
    pdfPath: str | None = None
    error: str | None = None
    usedDukJar: bool = False


def _normalize(path: str) -> Path:
    return Path(path.replace("\\", "/"))


def _generate_with_duk(xml_path: Path, output_dir: Path) -> tuple[bool, str | None]:
    output_dir.mkdir(parents=True, exist_ok=True)
    pdf_path = output_dir / "D100.pdf"

    try:
        subprocess.run(
            [
                "xvfb-run",
                "-a",
                "java",
                "-jar",
                str(JAR_PATH),
                str(xml_path),
            ],
            check=True,
            timeout=120,
            capture_output=True,
            text=True,
        )
    except (subprocess.CalledProcessError, subprocess.TimeoutExpired, FileNotFoundError):
        return False, None

    if pdf_path.exists():
        return True, str(pdf_path)

    generated = list(output_dir.glob("*.pdf"))
    if generated:
        generated[0].rename(pdf_path)
        return True, str(pdf_path)

    return False, None


def _generate_fallback_pdf(xml_path: Path, output_dir: Path) -> str:
    output_dir.mkdir(parents=True, exist_ok=True)
    pdf_path = output_dir / "D100.pdf"

    xml_preview = xml_path.read_text(encoding="utf-8", errors="replace")
    if len(xml_preview) > 3500:
        xml_preview = xml_preview[:3500] + "\n... (truncat)"

    c = canvas.Canvas(str(pdf_path), pagesize=A4)
    width, height = A4
    y = height - 40

    c.setFont("Helvetica-Bold", 14)
    c.drawString(40, y, "Declaratie 100 - Recipisa demonstrativa")
    y -= 24

    c.setFont("Helvetica", 10)
    lines = [
        "Generat de serviciul DUKIntegrator wrapper (mod demonstrativ).",
        "Pentru depunere oficiala la ANAF, validati XML-ul cu DUKIntegrator.jar",
        "si semnati electronic conform procedurii e-guvernare.ro.",
        "",
        "XML atasat:",
    ]
    for line in lines:
        c.drawString(40, y, line)
        y -= 14

    c.setFont("Courier", 7)
    for line in xml_preview.splitlines():
        if y < 40:
            c.showPage()
            y = height - 40
            c.setFont("Courier", 7)
        c.drawString(40, y, line[:120])
        y -= 9

    c.save()
    return str(pdf_path)


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok", "jarPresent": str(JAR_PATH.exists())}


@app.post("/generate-pdf", response_model=GeneratePdfResponse)
def generate_pdf(request: GeneratePdfRequest) -> GeneratePdfResponse:
    xml_path = _normalize(request.xmlPath)
    output_dir = _normalize(request.outputDir)

    if not xml_path.exists():
        return GeneratePdfResponse(success=False, error=f"XML inexistent: {xml_path}")

    if JAR_PATH.exists():
        ok, pdf_path = _generate_with_duk(xml_path, output_dir)
        if ok and pdf_path:
            return GeneratePdfResponse(success=True, pdfPath=pdf_path, usedDukJar=True)

    try:
        pdf_path = _generate_fallback_pdf(xml_path, output_dir)
        return GeneratePdfResponse(success=True, pdfPath=pdf_path, usedDukJar=False)
    except Exception as exc:  # noqa: BLE001
        return GeneratePdfResponse(success=False, error=str(exc))
