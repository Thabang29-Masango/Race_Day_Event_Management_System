# RaceDay API Design Notes

## API Design Approach

The RaceDay Event Management System API is planned as a RESTful API. 
The endpoints are organised according to the main resources in the system.

## Endpoint Groups

The API is divided into the following functional areas:

- Authentication
- User Profiles
- Events
- Categories
- Event Enrolments
- Results

## Role-Based Access

The endpoint plan identifies the role required to access each operation.

The two system roles are:

- **Organiser** – manages events, categories, enrolments and participant results.
- **Participant** – manages their account, browses events, enrols in categories and views their own enrolments and results.

## HTTP Methods

The API plan uses standard HTTP methods:

- `GET` – retrieve information
- `POST` – create information
- `PUT` – update information
- `DELETE` – remove information

## Response Handling

The endpoint plan includes expected responses for successful requests and common failure situations such as invalid requests,
unauthorised access, missing resources and conflicts.
