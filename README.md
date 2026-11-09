# RaceDay Event Management System

## Programming 2B POE – Parts 1, 2 and 3

The RaceDay Event Management System is designed to manage running and sporting events. Organisers can create and manage events and categories, capture participant results, and view event enrolments. Participants can create accounts, browse available events, enter events by selecting a suitable category, view their enrolments, and track their results.

- **Part 1** – planning, database design and the REST API endpoint plan.
- **Part 2** – the REST API (C#, ASP.NET Core Web API, .NET 8, SQL Server).
- **Part 3** – the ASP.NET Core MVC web front end, Azure Blob Storage image uploads, Docker and CI/CD.

## User Roles

### Organiser

An organiser can:

- Create, edit, and delete events (including uploading an event banner image).
- Manage event categories.
- View enrolments for events.
- Capture and publish participant results.

### Participant

A participant can:

- Create an account.
- Browse and filter upcoming events.
- Enter an event by selecting a category.
- View and cancel their own enrolments.
- Track their own results.
- Update their profile and upload a profile picture.

## Part 1: Planning and Database Design

The following planning and database documents are included in the `docs` folder:

- `RaceDay_ERD_sketch.pdf` – Entity Relationship Diagram.
- `RaceDay_API_Endpoint_Plan.docx` – REST API endpoint plan.
- `RaceDay.sql` – SQL Server database creation and sample data script.
- `Part1_Project_Structure.md` – Part 1 project documentation.

### Database Design

The database design is represented in the ERD and implemented through the SQL Server database script. The system uses the following main entities:

- **Users** – stores account and role information.
- **Organisers** – stores organiser-specific information.
- **Participants** – stores participant-specific information.
- **Events** – stores event information created by organisers.
- **Categories** – stores the categories available for each event.
- **Enrolments** – records participants entering events and selecting categories.
- **Results** – records the results associated with participant enrolments.

The database uses primary keys, foreign keys, constraints, and relationships to maintain consistent data.

### REST API Planning

The API Endpoint Plan defines the RESTful endpoints planned for implementation in Part 2. It identifies the HTTP method, route, purpose, required role, request body, and expected response for each endpoint.

## Part 2: API Development

Part 2 implements the RaceDay system as a REST API using C# and ASP.NET Core Web API on .NET 8, connected to a SQL Server database.

### Technologies Used

- C#
- ASP.NET Core Web API
- .NET 8
- SQL Server
- Entity Framework Core
- JWT authentication
- Visual Studio

### API Components

- **Models** represent the main system entities (users, organisers, participants, events, categories, enrolments, results).
- **Database Context** configures the connection to SQL Server and defines entity relationships.
- **DTOs** define the data accepted or returned by selected API requests.
- **Controllers** handle HTTP requests and implement the system's features.

### API Functionality

The API includes user registration and login (JWT), event management, organiser and participant management, category management, enrolments, results, and (added in Part 3) image upload endpoints and enrolment lookups by event and by participant.

### Database Connection

The application uses a SQL Server connection string configured in `appsettings.json`. The database must be available for the API to perform database operations. Database credentials and other sensitive information must not be committed to a public repository.

### Running the API

1. Open the API project in Visual Studio.
2. Run `docs/RaceDay.sql` against your SQL Server instance (or apply the EF Core migrations).
3. Check the connection string in `appsettings.json`.
4. Set the JWT key using user secrets (see Configuration below).
5. Restore NuGet packages, then build and run the project.
6. Use Swagger to inspect and test the endpoints.

### Testing

The API has unit tests in the `RaceDayAPI1.Tests` project. Run them with:

```bash
dotnet test
```

[Describe the tests you actually ran and their results here, including manual Swagger/Postman tests.]

## Part 3: MVC Web Application

Part 3 adds a role-aware ASP.NET Core MVC web application that consumes the Part 2 API. It gives Organisers and Participants a clean, branded interface, adds image uploads through Azure Blob Storage, and is containerised with Docker.

### Architecture

```
Browser ──> MVC web app ──HttpClient (JSON)──> RaceDay API ──> SQL Server
                                                    │
                                                    └──> Azure Blob Storage (images)
```

- The MVC project contains **no database access and no replicated business logic**. All data operations go through the API using a typed `HttpClient` service (`RaceDayApiService`).
- The MVC project **never communicates with Azure directly**. Images are posted to the API, which stores them in Azure Blob Storage and returns the URL.
- After login, the API returns a JWT. The MVC app stores the token, user id, role and name in **server-side session** and sends the token with every API request.

### MVC Structure

| Folder | Purpose |
|---|---|
| `Controllers/` | `Account` (register, login, logout), `Home` (filterable upcoming events), `Events` (event details, enrol), `Participant` (my enrolments, my results, profile), `Organiser` (dashboard, events, categories, enrolments, results) |
| `Services/` | `RaceDayApiService` (all API calls), `AuthTokenHandler` (adds the JWT to requests), `ApiResult` (success/error wrapper) |
| `Filters/` | `[RequireRole]` – session-based access control that redirects unauthenticated users to the login page |
| `Models/` | API data shapes and validated view models |
| `Views/` | Razor views with a role-specific navigation menu |
| `wwwroot/css/` | RaceDay theme (navy, orange and lime; Barlow Condensed and Inter fonts) using Bootstrap |

### Features

**Authentication and navigation**

- Register and log in as an Organiser or Participant through the API.
- Session keeps the user's authenticated state and role across pages.
- Guests, Participants and Organisers each see a different menu.
- Protected pages redirect unauthenticated users to login; the wrong role is sent to an "Access denied" page.
- Log out ends the session and redirects to the login page.

**Organiser interface**

- Dashboard: events created, upcoming events, total enrolments per event and the next event date.
- Events management: create, edit and delete events (name, description, date, location, distance, event type) with banner image upload.
- Category management: add and remove age or distance categories per event.
- Enrolments: view participants enrolled for an event with category and colour-coded status.
- Results entry: capture finish times and positions and the total number of finishers (available once the event date has arrived).

**Participant interface**

- Home page with upcoming events and filters (name, type, location, date range, maximum distance).
- Event details page with full information and categories.
- Enrol for an event by choosing a category (recorded immediately); cancel upcoming enrolments.
- My Enrolments with colour-coded status (green = Confirmed, amber = Pending, red = Cancelled).
- My Results: event, date, category, finish time and position, e.g. "47th out of 312".
- Profile page to update personal details and upload a profile picture.

**Quality**

- Server-side validation with friendly messages; raw API or server errors are never shown to users.
- Consistent Bootstrap-based layout, fonts, colours and spacing, responsive on mobile.

### Azure Blob Storage

Blob Storage is integrated into the **API** (`BlobStorageService` and `ImagesController`):

- `POST api/events/{id}/banner` – Organiser uploads an event banner.
- `POST api/participants/{id}/picture` – Participant uploads a profile picture.
- Images (JPG, PNG, WEBP or GIF, up to 5 MB) are stored in the `raceday-images` container; the returned URL is saved on the event or participant and displayed on the event details and profile pages.

### Configuration

Never commit secrets. Set them with user secrets in the **API** project:

```bash
cd RaceDayAPI1
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<random string of at least 32 characters>"
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "<Azure storage connection string>"
```

In the **MVC** project, set the API address in `appsettings.json`:

```json
"RaceDayApi": { "BaseUrl": "https://localhost:7247/" }
```

### Running Locally

1. Start the API (`RaceDayAPI1`) and note its URL.
2. Make sure `RaceDayApi:BaseUrl` in the MVC `appsettings.json` matches it.
3. In Visual Studio choose **Solution → Set Startup Projects → Multiple startup projects** and start both the API and the MVC project (or run `dotnet run` in each folder).

### Running with Docker

Build the image from the repository root:

```bash
docker build -t raceday-mvc ./RaceDayManagementSystemMVC
```

Run the container (the API must be running on the host on port 5106):

```bash
docker run --rm -p 8080:8080 --add-host=host.docker.internal:host-gateway -e RaceDayApi__BaseUrl=http://host.docker.internal:5106/ --name raceday-mvc raceday-mvc
```

Then open http://localhost:8080.

Notes:

- `RaceDayApi__BaseUrl` overrides the value in `appsettings.json` (a double underscore stands for `:`).
- Use the API's **http** address inside Docker to avoid development-certificate errors.
- If the app cannot reach the API, start the API on all interfaces: `dotnet run --urls "http://0.0.0.0:5106"`.
- `--add-host` is only required on Linux; Docker Desktop on Windows/Mac already provides `host.docker.internal`.

## GitHub Actions / Continuous Integration

- **Part 1:** `.github/workflows/part1-validation.yml` checks that the required directories and Part 1 files are present.
- **Parts 2 and 3:** `.github/workflows/dotnet.yml` restores, builds and tests the API and now also builds the MVC project and the Docker image on every push.

**CI status:** [add the status of your latest run after checking the Actions tab]

## Video Presentations

- **Part 1:** [YouTube link]
- **Part 2:** [YouTube link, if required]
- **Part 3 (unlisted):** [YouTube link] – demonstrates the full running application, the MVC structure, the Blob Storage integration and the Docker setup.

## Project Structure

```
Race_Day_Event_Management_System/
│
├── docs/
│   ├── RaceDay.sql
│   ├── RaceDay_API_Endpoint_Plan.docx
│   ├── RaceDay_ERD_sketch.pdf
│   └── Part1_Project_Structure.md
│
├── .github/
│   └── workflows/
│       ├── part1-validation.yml
│       └── dotnet.yml
│
├── RaceDayAPI1/                        (Part 2 API + Part 3 additions)
│   ├── Controllers/
│   ├── Models/
│   ├── Data/
│   ├── DTOs/
│   ├── Services/                       (BlobStorageService)
│   ├── Program.cs
│   ├── appsettings.json
│   └── RaceDayAPI1.csproj
│
├── RaceDayAPI1.Tests/                  (unit tests)
│
├── RaceDayManagementSystemMVC/         (Part 3 MVC web app)
│   ├── Controllers/
│   ├── Filters/
│   ├── Models/
│   ├── Services/
│   ├── Views/
│   ├── wwwroot/
│   ├── Dockerfile
│   ├── .dockerignore
│   ├── Program.cs
│   ├── appsettings.json
│   └── RaceDayManagementSystemMVC.csproj
│
└── README.md
```

The exact folder and file structure should match the files committed to GitHub.
