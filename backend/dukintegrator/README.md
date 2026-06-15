# DUKIntegrator wrapper

Place the official `DUKIntegrator.jar` from ANAF at `/opt/duk/DUKIntegrator.jar` inside the container
(volume mount `./dukintegrator/jar:/opt/duk`). If you extracted the full ANAF zip, the JAR is usually at
`jar/dist/DUKIntegrator.jar` — `docker-compose.yml` sets `DUK_JAR_PATH` accordingly.

Without the JAR, the service generates a demonstrative PDF with the XML content for review purposes.
