# RaceDay Event Management System

## Programming 2B POE – Part 1 and Part 2

The RaceDay Event Management System is designed to manage running and sporting events. The system allows organisers to create and manage events and categories, capture participant results, and view event enrolments. Participants can create accounts, browse available events, enter events by selecting a suitable category, view their enrolments, and track their results.

Part 1 focused on planning the system, designing the database, and preparing the REST API endpoint plan. Part 2 focuses on developing the application using C#, ASP.NET Core Web API, .NET 8, and SQL Server.

## User Roles

### Organiser

An organiser can:

- Create, edit, and delete events.
- Manage event categories.
- Capture participant results.
- View enrolments for events.

### Participant

A participant can:

- Create an account.
- Browse available events.
- Enter an event by selecting a category.
- View their own enrolments.
- Track their own results.

## Part 1: Planning and Database Design

The following planning and database documents are included in the `docs` folder:

- `RaceDay_ERD_sketch.pdf` – Entity Relationship Diagram.
- `RaceDay_API_Endpoint_Plan.docx` – REST API endpoint plan.
- `RaceDay.sql` – SQL Server database creation and sample data script.
- `Part1_Project_Structure.md` – Part 1 project documentation.

### Database Design

The database design is represented in the ERD and implemented through the SQL Server database script.

The system uses the following main entities:

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

Part 2 involves implementing the RaceDay system as a REST API using C# and ASP.NET Core Web API on .NET 8. The application connects to a SQL Server database to manage the system's information.

### Technologies Used

- C#
- ASP.NET Core Web API
- .NET 8
- SQL Server
- Entity Framework Core
- Visual Studio

### API Components

The API is organised into models, a database context, data transfer objects (DTOs), and controllers.

- **Models** represent the main system entities, including users, organisers, participants, events, categories, enrolments, and results.
- **Database Context** configures the connection between the application and the SQL Server database and defines entity relationships.
- **DTOs** define the data accepted by selected API requests.
- **Controllers** handle HTTP requests and implement operations for the system's features.

### API Functionality

The API includes components for user registration and login, event management, organiser and participant management, category management, enrolments, and results.

The available functionality depends on the implemented endpoints and their validation rules.

### Database Connection

The application uses a SQL Server connection string configured in `appsettings.json`. The database must be available for the API to perform database operations.

Database credentials and other sensitive information should not be committed to a public repository.

### Running the API

To run the application:

1. Open the API project in Visual Studio.
2. Check the database connection settings.
3. Restore the required NuGet packages.
4. Build and run the project.
5. Use Swagger, if configured, to inspect and test the available endpoints.

### Testing

The API should be tested to confirm that endpoints return the expected responses and handle valid requests, invalid input, and missing records appropriately. Testing results should reflect the tests actually performed.

## GitHub Actions / Continuous Integration

GitHub Actions is used to validate the required Part 1 repository structure and documentation files. The workflow is located at:

`.github/workflows/part1-validation.yml`

The workflow checks that the required directories and Part 1 files are present.

**Part 1 CI Status:** Passing / Green, as previously verified.

The Part 2 API should also be built and tested to confirm that the application compiles and that its required components work correctly.

## Video Presentation

**Part 1 video presentation:** YouTube link to be added before final submission.

**Part 2 video presentation:** Add the relevant link if required by the assignment.

## Project Structure

The repository contains the Part 1 planning documents and the Part 2 API source code. The intended structure is shown below:

```text
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
│       └── part1-validation.yml
│
├── RaceDayAPI/
│   ├── Controllers/
│   ├── Models/
│   ├── Data/
│   ├── DTOs/
│   ├── Program.cs
│   ├── appsettings.json
│   └── RaceDayAPI.csproj
│
└── README.md
```

The exact folder and file structure should match the files committed to GitHub.

## References

References used for the API planning and system design are documented in the API Endpoint Plan. Additional references used during Part 2 development should be included here if required by the assignment.
