# SQL Seed Data Notes

## Purpose

The RaceDay database script includes sample data to demonstrate that the database structure can be populated and used for testing.

## Users

The seed data includes:

- 2 Organiser users.
- 2 Participant users.

The users are assigned roles that correspond with the system requirements.

## Events

The database includes 3 sample events.

Each event is linked to an organiser through the `OrganiserId` foreign key.

## Categories

Each sample event has categories associated with it.

Categories include information such as:

- Category name.
- Minimum age.
- Maximum age.
- Distance.

Each category is linked to its event using `EventId`.

## Enrolments

Sample participant enrolments are included to demonstrate the relationship between:

- Participants.
- Events.
- Categories.

The enrolment records also contain an enrolment date and status.

## Results

Sample results are included for completed enrolments.

Results contain information such as:

- Finish time.
- Finish position.
- Total finishers.
- Captured date.

Each result is linked to an enrolment using `EnrolmentId`.

## Testing Purpose

The seed data provides realistic records that can be used to verify the relationships
between the database entities and support testing of the planned API functionality in later parts of the project.
