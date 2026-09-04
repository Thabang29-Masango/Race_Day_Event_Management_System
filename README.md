# RaceDay Event Management System

## Programming 2B POE – Part 1

The RaceDay Event Management System is designed to manage running and sporting events. The system allows organisers to create and manage events and categories, capture participant results, and view event enrolments. Participants can create accounts, browse available events, enter events by selecting a suitable category, view their enrolments, and track their results.

## User Roles

### Organiser

An organiser can:

* Create, edit, and delete events.
* Manage event categories.
* Capture participant results.
* View enrolments for events.

### Participant

A participant can:

* Create an account.
* Browse available events.
* Enter an event by selecting a category.
* View their own enrolments.
* Track their own results.

## Part 1 Documentation

The following planning and database documents are included in the `docs` folder:

* `RaceDay_ERD_sketch.pdf` – Entity Relationship Diagram.
* `RaceDay_API_Endpoint_Plan.docx` – REST API endpoint plan.
* `RaceDay.sql` – SQL Server database creation and sample data script.
* `Part1_Project_Structure.md` – Part 1 project documentation.

## Database Design

The database design is based on the entities required for the RaceDay Event Management System. The ERD defines the relationships between users, organisers, participants, events, categories, enrolments, and results.

The SQL Server script creates the database tables, primary keys, foreign keys, constraints, and sample data.

## REST API Planning

The API Endpoint Plan defines the RESTful endpoints that will be implemented in Part 2. The plan identifies the HTTP method, route, purpose, required role, request body, and expected response for each endpoint.

## GitHub Actions / CI

GitHub Actions is used to validate the required Part 1 repository structure and documentation files. The workflow is located at:

`.github/workflows/part1-validation.yml`

The workflow checks that the required directories and Part 1 files are present.

**CI Status:** Passing / Green

## Video Presentation

Part 1 video presentation:

**YouTube Link:** To be added before final submission.

## Project Structure

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
└── README.md
```

## References

References used for the API planning and system design are documented in the API Endpoint Plan.
