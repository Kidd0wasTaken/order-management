FROM python:3.12-slim

RUN apt-get update \
    && apt-get install -y --no-install-recommends xvfb openjdk-21-jre-headless \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY server.py .

RUN mkdir -p /opt/duk /exports

ENV EXPORT_ROOT=/exports
ENV DUK_JAR_PATH=/opt/duk/DUKIntegrator.jar

EXPOSE 8080

CMD ["uvicorn", "server:app", "--host", "0.0.0.0", "--port", "8080"]
