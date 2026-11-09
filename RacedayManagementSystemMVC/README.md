# RaceDay – Part 3: MVC Web Application

The RaceDay web front end is an ASP.NET Core MVC application that consumes the Part 2 REST API. It gives Organisers and Participants a clean, branded, role-aware interface. Image uploads are stored in Azure Blob Storage (through the API), and the MVC app runs in Docker.

## Architecture

```
Browser ──> MVC web app ──HttpClient (JSON)──> RaceDay API ──> SQL Server
                                                    │
                                                    └──> Azure Blob Storage (images)
```

- The MVC project contains **no database access and no replicated business logic**. Every data operation goes through the API using a typed `HttpClient` service (`RaceDayApiService`).
- The MVC project **never communicates with Azure**. Images are posted to the API, which stores them in Blob Storage and returns the URL.
- After login, the API returns a JWT. The MVC app keeps the token, user id, role and name in **server-side session** and sends the token with every API request.

## MVC Structure

| Folder | Purpose |
|---|---|
| `Controllers/` | `Account` (register, login, logout), `Home` (filterable upcoming events), `Events` (event details, enrol), `Participant` (my enrolments, my results, profile), `Organiser` (dashboard, events, categories, enrolments, results) |
| `Services/` | `RaceDayApiService` (all API calls), `AuthTokenHandler` (adds the JWT to requests), `ApiResult` (success/error wrapper) |
| `Filters/` | `[RequireRole]` – session-based access control; sends unauthenticated users to login |
| `Models/` | API data shapes and validated view models |
| `Views/` | Razor views with a role-specific navigation menu |
| `wwwroot/css/` | RaceDay theme (navy, orange and lime; Barlow Condensed and Inter fonts) on top of Bootstrap |

## Features

### Authentication and navigation
- Register and log in as an Organiser or Participant through the API.
- Session keeps the user's authenticated state and role on every page.
- Guests, Participants and Organisers each see a different navigation menu.
- Protected pages redirect unauthenticated users to the login page; a user with the wrong role is sent to an "Access denied" page.
- Logging out ends the session and redirects to the login page.

### Organiser interface
- **Dashboard:** events created, upcoming events, total enrolments per event and the next event date.
- **Events:** create, edit and delete events (name, description, date, location, distance, event type) with banner image upload.
- **Categories:** add and remove age or distance categories for each event.
- **Enrolments:** participants enrolled for an event, with their category and colour-coded status.
- **Results entry:** capture finish times, finishing positions and total finishers (available once the event date has arrived).

### Participant interface
- **Home:** upcoming events with filters (name, type, location, date range, maximum distance).
- **Event details:** full event information, banner image and available categories.
- **Enrol:** choose a category and enrol; the enrolment is recorded immediately. Upcoming enrolments can be cancelled.
- **My Enrolments:** all enrolments with colour-coded status (green = Confirmed, amber = Pending, red = Cancelled).
- **My Results:** event, date, category, finish time and position, for example "47th out of 312".
- **Profile:** update personal details and upload a profile picture.

### Quality
- Server-side validation with friendly messages; raw API or server errors are never shown to users.
- Consistent Bootstrap layout, fonts, colours and spacing on every page, responsive on mobile.

Blob Storage is integrated into the **API** (`BlobStorageService` and `ImagesController`):

| Endpoint | Role | Purpose |
|---|---|---|
| `POST api/events/{id}/banner` | Organiser | Upload an event banner |
| `POST api/participants/{id}/picture` | Authenticated | Upload a profile picture |

Images (JPG, PNG, WEBP or GIF, up to 5 MB) are stored in the `raceday-images` container. The returned URL is saved on the event or participant and displayed on the event details and profile pages.

## Configuration

Never commit secrets. In the **API** project, use user secrets:

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

## Running Locally

1. Start the API and note its URL.
2. Check that `RaceDayApi:BaseUrl` in the MVC `appsettings.json` matches it.
3. In Visual Studio choose **Solution → Set Startup Projects → Multiple startup projects**, set the API and the MVC project to **Start**, and run. (Or run `dotnet run` in each project folder.)


Build the image from the repository root:

```bash
docker build -t raceday-mvc ./RaceDayManagementSystemMVC
```

Run the container (the API must be running on the host on port 5106):

```bash
docker run --rm -p 8080:8080 --add-host=host.docker.internal:host-gateway -e RaceDayApi__BaseUrl=http://host.docker.internal:5106/ --name raceday-mvc raceday-mvc
```

Open http://localhost:8080.

Notes:
- `RaceDayApi__BaseUrl` overrides the value in `appsettings.json` (a double underscore stands for `:`).
- Use the API's **http** address inside Docker to avoid development-certificate errors.
- If the app cannot reach the API, start the API on all interfaces: `dotnet run --urls "http://0.0.0.0:5106"`.
- `--add-host` is only needed on Linux; Docker Desktop on Windows and Mac already provides `host.docker.internal`.

## GitHub Actions

`.github/workflows/dotnet.yml` restores, builds and tests the API, builds the MVC project and builds the Docker image on every push.
