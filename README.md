# Gestionare Comenzi — Order Management

Aplicație full-stack pentru gestionarea comenzilor și generarea simulată a declarației **D100** (obligații de plată la bugetul de stat), dezvoltată ca test tehnic.

## Stack tehnologic

| Layer | Tehnologie |
|-------|------------|
| Frontend | React 18, Vite, Material UI, Redux Toolkit |
| Backend | ASP.NET Core 8 Web API, Entity Framework Core |
| Bază de date | PostgreSQL 16 |
| Containerizare | Docker, Docker Compose, Nginx |
| Declarații ANAF | XSD D100, DUKIntegrator (Docker sidecar) |

## Funcționalități

### Comenzi
- Afișare comenzi într-un tabel (toate câmpurile)
- Adăugare, editare, ștergere comandă
- Status vizual cu chip-uri colorate
- Persistență date în PostgreSQL
- Date demo la prima pornire (dacă baza e goală)

### Declarația D100 (tab separat)
- Profil companie + declarant (persistat în DB)
- Selectare perioadă (`luna` / `an`)
- **Previzualizare:** agregare comenzi cu status `Completed` din luna selectată
- **Generare XML** validat XSD (ANAF `declaratie100`)
- **Generare PDF** prin serviciul DUKIntegrator (Docker)
- Descărcare fișiere XML și PDF

> **Disclaimer simulare:** `suma_dat` = total vânzări brute (`Σ cantitate × preț`) din comenzile finalizate. Aceasta **nu** este logică fiscală reală și nu constituie consultanță fiscală. Codurile `cod_oblig` / `cod_bugetar` sunt configurabile în `appsettings.json`.

## Pornire rapidă (Docker)

Cerințe: [Docker Desktop](https://www.docker.com/products/docker-desktop/)

```bash
docker compose up --build
```

| Serviciu | URL |
|----------|-----|
| Frontend (UI) | http://localhost:3000 |
| Backend API | http://localhost:5000/api/orders |
| Swagger (doar Development) | http://localhost:5000/swagger |
| Health check | http://localhost:5000/health |

Oprire:

```bash
docker compose down
```

Ștergere date persistente:

```bash
docker compose down -v
```

## Flux D100 pentru testeri

1. Porniți aplicația cu `docker compose up --build`.
2. În tab **Comenzi**, creați comenzi și setați statusul **Finalizată** (`Completed`) pentru luna dorită.
3. Comutați la tab **Declarația D100**.
4. Verificați profilul companiei (pre-completat cu date demo) și salvați dacă modificați.
5. Selectați `luna` / `an` și apăsați **Previzualizare** — vedeți totalul comenzilor și `suma_dat` simulată.
6. Apăsați **Generează XML** — la succes, statusul devine `Validated`.
7. Apăsați **Generează PDF** — serviciul `dukintegrator` produce PDF-ul.
8. Descărcați XML și PDF cu butoanele dedicate.

### DUKIntegrator (PDF oficial)

JAR-ul oficial ANAF nu este inclus în repo (licență / distribuție). Pentru PDF generat de ANAF:

1. Descărcați `DUKIntegrator.jar` de pe [pagina ANAF](https://static.anaf.ro/static/DUKIntegrator/DUKIntegrator.htm).
2. Plasați fișierul în `backend/dukintegrator/jar/DUKIntegrator.jar`.
3. Reporniți stack-ul Docker.

Fără JAR, serviciul `dukintegrator` generează un **PDF demonstrativ** cu conținutul XML (util pentru testare tehnică).

## API Endpoints

### Comenzi

| Metodă | Rută | Descriere |
|--------|------|-----------|
| `GET` | `/api/orders` | Lista tuturor comenzilor |
| `GET` | `/api/orders/{id}` | Detalii comandă |
| `POST` | `/api/orders` | Creare comandă |
| `PUT` | `/api/orders/{id}` | Actualizare comandă |
| `DELETE` | `/api/orders/{id}` | Ștergere comandă |

### D100

| Metodă | Rută | Descriere |
|--------|------|-----------|
| `GET` | `/api/d100/company` | Profil companie |
| `PUT` | `/api/d100/company` | Actualizare profil |
| `POST` | `/api/d100/preview` | Previzualizare calcul `{ luna, an }` |
| `POST` | `/api/d100/generate` | Calcul + XML + validare XSD |
| `GET` | `/api/d100/{id}` | Detalii declarație |
| `GET` | `/api/d100/{id}/xml` | Descărcare XML |
| `POST` | `/api/d100/{id}/pdf` | Generare PDF |
| `GET` | `/api/d100/{id}/pdf` | Descărcare PDF |

Statusuri declarație: `Draft`, `Validated`, `PdfGenerated`, `Failed`

### Exemplu body comandă (POST/PUT)

```json
{
  "customerName": "Ion Popescu",
  "product": "Laptop Dell",
  "quantity": 1,
  "price": 3499.99,
  "status": "Completed",
  "notes": "Livrare urgentă"
}
```

Statusuri valide: `Pending`, `Processing`, `Completed`, `Cancelled`

## Configurare D100

`backend/OrderManagement.Api/appsettings.json`:

```json
"D100": {
  "CodOblig": "103",
  "CodBugetar": "20A010101X",
  "TipOblig": "1",
  "SimulationNote": "suma_dat = total vanzari comenzi finalizate (demo)",
  "DukIntegratorUrl": "http://dukintegrator:8080",
  "ExportRoot": "/exports",
  "XsdFileName": "d100.xsd"
}
```

Variabile de mediu în Docker: `D100__CodOblig`, `D100__CodBugetar`, `D100__DukIntegratorUrl`, `D100__ExportRoot`.

## CI și teste

Pipeline GitHub Actions (`.github/workflows/ci.yml`) rulează automat la push/PR:
- **Backend:** `dotnet build` + `dotnet test` (16 teste: comenzi + D100 calcul/XML/XSD/API)
- **Frontend:** `npm ci` + `npm run build`

Rulare teste local:

```bash
cd backend
dotnet test OrderManagement.sln
```

Test smoke DUKIntegrator (opțional, local): marcat `[Trait("Category", "DukIntegrator")]` — omis în CI dacă JAR-ul lipsește.

## Dezvoltare locală (fără Docker)

### Backend

```bash
cd backend
dotnet restore
dotnet ef database update --project OrderManagement.Api
dotnet run --project OrderManagement.Api
```

Necesită PostgreSQL local pe `localhost:5432` (vezi `appsettings.Development.json`).

### Frontend

```bash
cd frontend
npm install
npm run dev
```

UI disponibil la http://localhost:5173 (proxy API către `:5000`).

## Structura proiectului

```
order-management/
├── docker-compose.yml
├── backend/
│   ├── Dockerfile
│   ├── dukintegrator/          # DUKIntegrator wrapper (Python + JRE)
│   └── OrderManagement.Api/
│       ├── Anaf/D100/          # XSD + documentație
│       ├── Controllers/
│       ├── Services/D100/
│       ├── Data/
│       ├── DTOs/
│       ├── Migrations/
│       └── Models/
└── frontend/
    ├── Dockerfile
    ├── nginx.conf
    └── src/
        ├── api/
        ├── components/
        │   └── d100/
        ├── constants/
        ├── store/
        └── utils/
```

## Decizii de arhitectură

- **DTO-uri separate** de entitățile EF Core — API stabil, fără expunere directă a modelului DB
- **Validare** pe DTO-uri (Data Annotations) + validare XSD pentru XML D100
- **Nginx reverse proxy** în Docker — frontend și API pe același origin, fără probleme CORS
- **Volume partajat** `d100_exports` între backend și `dukintegrator` pentru XML/PDF
- **Health checks** pentru PostgreSQL, backend și dukintegrator
- **Migrări EF** aplicate automat la pornirea backend-ului
- **UI în română** — etichete și mesaje localizate

## Resurse ANAF

- [Declarația 100](http://static.anaf.ro/static/10/Anaf/Declaratii_R/100.html)
- XSD vendored: `backend/OrderManagement.Api/Anaf/D100/Schemas/d100.xsd` (namespace `mfp:anaf:dgti:d100:declaratie:v2`, țintă OPANAF 57/2026)
- Structura XML: vezi `Anaf/D100/docs/` sau PDF-ul oficial ANAF

## Licență

Proiect demonstrativ pentru evaluare tehnică.
