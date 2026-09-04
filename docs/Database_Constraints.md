# RaceDay Database Constraints

The RaceDay database uses constraints to maintain data integrity and enforce the rules of the system.

## Primary Keys

Each main entity has a primary key that uniquely identifies its records.

- Users: `UserId`
- Organisers: `UserId`
- Participants: `UserId`
- Events: `EventId`
- Categories: `CategoryId`
- Enrolments: `EnrolmentId`
- Results: `ResultId`

## Foreign Keys

Foreign keys maintain the relationships between related entities.

- Organisers reference Users.
- Participants reference Users.
- Events reference Organisers.
- Categories reference Events.
- Enrolments reference Participants, Events, and Categories.
- Results reference Enrolments.

## Other Constraints

The SQL script also uses constraints to help maintain valid data, including:

- NOT NULL constraints for required fields.
- UNIQUE constraints where duplicate values should not be allowed.
- DEFAULT values for fields such as dates and statuses.
- CHECK constraints for values that must fall within specific rules.
