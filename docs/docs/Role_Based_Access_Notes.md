# RaceDay Role-Based Access Notes

The RaceDay Event Management System has two main roles: Organiser and Participant.

## Organiser

Organisers are responsible for managing events and their related information.

An organiser can:

- Create events.
- Edit events.
- Delete events.
- Manage event categories.
- View event enrolments.
- Capture participant results.
- View event results.

## Participant

Participants use the system to take part in available events.

A participant can:

- Create an account.
- Browse available events.
- Select a category when entering an event.
- View their own enrolments.
- Track their own results.

## Access Control

The API endpoint plan identifies the role required for each protected operation. Access control will be enforced at the API level during Part 2 and reflected in the MVC application during Part 3.

Public authentication operations, such as registration and login, do not require an authenticated role.
