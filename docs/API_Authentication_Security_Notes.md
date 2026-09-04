# API Authentication and Security Notes

## 1. Authentication

The RaceDay Event Management System will provide authentication endpoints for user registration and login.

- `POST /api/auth/register` – allows a new user to register.
- `POST /api/auth/login` – verifies user credentials and provides authenticated access.

Authentication is required for protected operations after login.

## 2. Role-Based Access

The system supports two main roles:

- **Organiser** – manages events, categories and event-related results.
- **Participant** – manages their profile and enrolments and views their results.

Access to protected endpoints will be controlled according to the user's assigned role.

## 3. Password Security

User passwords will not be stored as plain text. The database stores a `PasswordHash` value instead.

## 4. Data Protection

User and event information should only be accessed or modified by authorised users. 
API endpoints will validate the user's permissions before performing protected operations.

## 5. Input Validation

The API should validate incoming request data before saving it to the database.
Invalid values should return an appropriate HTTP error response rather than being stored.

## 6. API Security Considerations

The implementation should consider:

- Authentication before accessing protected resources.
- Role-based authorisation.
- Secure password handling.
- Validation of request data.
- Appropriate HTTP status codes for authentication and authorisation failures.
- Protection of user and event information.

These notes support the authentication, user profile, event, enrolment and results endpoint planning for Part 1.
