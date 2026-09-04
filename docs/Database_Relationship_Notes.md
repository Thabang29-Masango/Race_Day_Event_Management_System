# RaceDay Database Relationship Notes

The RaceDay database is structured around the relationship between users, their role-specific information, events, event categories, participant enrolments, and results.

## User Relationships

A user can have role-specific information stored in either the Organisers or Participants entity. The `UserId` is used to connect the role-specific record to the main Users entity.

## Event Relationships

An organiser is associated with the events they create and manage. Each event can have multiple categories.

## Enrolment Relationships

An enrolment connects a participant to an event and the category selected for that event. This allows the system to keep track of which participants entered which events and categories.

## Result Relationships

A result is associated with an enrolment. This allows the system to record the outcome of a participant's entry while maintaining the connection between the participant, event, and category.

## Referential Integrity

Foreign keys are used to maintain relationships between related records and help prevent invalid references between the database entities.
