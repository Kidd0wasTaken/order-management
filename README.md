# Gestionare Comenzi — Order Management

Aplicație full-stack pentru gestionarea comenzilor, dezvoltată ca test tehnic.

## Stack tehnologic

| Layer | Tehnologie |
|-------|------------|
| Frontend | React 18, Vite, Material UI, Redux Toolkit |
| Backend | ASP.NET Core 8 Web API, Entity Framework Core |
| Bază de date | PostgreSQL 16 |
| Containerizare | Docker, Docker Compose, Nginx |

## Funcționalități

- Afișare comenzi într-un tabel (toate câmpurile)
- Adăugare comandă nouă (dialog modal)
- Editare comandă existentă
- Ștergere comandă cu confirmare
- Status vizual cu chip-uri colorate
- Persistență date în PostgreSQL
- Date demo la prima pornire (dacă baza e goală)

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

## API Endpoints

| Metodă | Rută | Descriere |
|--------|------|-----------|
| `GET` | `/api/orders` | Lista tuturor comenzilor |
| `GET` | `/api/orders/{id}` | Detalii comandă |
| `POST` | `/api/orders` | Creare comandă |
| `PUT` | `/api/orders/{id}` | Actualizare comandă |
| `DELETE` | `/api/orders/{id}` | Ștergere comandă |

### Exemplu body (POST/PUT)

```json
{
  "customerName": "Ion Popescu",
  "product": "Laptop Dell",
  "quantity": 1,
  "price": 3499.99,
  "status": "Pending",
  "notes": "Livrare urgentă"
}
```

Statusuri valide: `Pending`, `Processing`, `Completed`, `Cancelled`

## CI și teste

Pipeline GitHub Actions (`.github/workflows/ci.yml`) rulează automat la push/PR:
- **Backend:** `dotnet build` + `dotnet test` (6 teste de integrare API)
- **Frontend:** `npm ci` + `npm run build`

Rulare teste local:

```bash
cd backend
dotnet test OrderManagement.sln
```

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
│   └── OrderManagement.Api/
│       ├── Controllers/
│       ├── Data/
│       ├── DTOs/
│       ├── Mapping/
│       ├── Migrations/
│       └── Models/
└── frontend/
    ├── Dockerfile
    ├── nginx.conf
    └── src/
        ├── api/
        ├── components/
        ├── constants/
        ├── store/
        └── utils/
```

## Decizii de arhitectură

- **DTO-uri separate** de entitățile EF Core — API stabil, fără expunere directă a modelului DB
- **Validare** pe DTO-uri (Data Annotations) + validare status în controller
- **Nginx reverse proxy** în Docker — frontend și API pe același origin, fără probleme CORS
- **Health checks** pentru PostgreSQL și backend — startup ordonat în Compose
- **Migrări EF** aplicate automat la pornirea backend-ului
- **UI în română** — etichete și mesaje localizate; valorile status în API rămân în engleză (convenție REST)

## Publicare pe GitHub

Repository-ul local este inițializat pe branch-ul `main`. Pentru a crea repo-ul remote:

```bash
gh auth login
gh repo create order-management --public --description "Aplicație full-stack de gestionare comenzi" --source=. --remote=origin --push
```

Alternativ, creați manual repo-ul pe GitHub și rulați:

```bash
git remote add origin https://github.com/<utilizator>/order-management.git
git push -u origin main
```

## Licență

Proiect demonstrativ pentru evaluare tehnică.
