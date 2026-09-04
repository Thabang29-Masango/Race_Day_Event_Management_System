# SQL Execution and Verification Notes

## SQL Script

The database script for the RaceDay Event Management System is stored in:

`docs/RaceDay.sql`

The script creates the RaceDay database structure, including the required tables, primary keys, foreign keys and constraints.

## Execution

The SQL script should be executed using Microsoft SQL Server Management Studio (SSMS).

Before execution, the script should be reviewed to ensure that it can run successfully without errors.

## Verification

After execution, the following should be checked:

1. The `RaceDayDB` database is created successfully.
2. All required tables are created.
3. Primary keys are present on the appropriate tables.
4. Foreign key relationships are created successfully.
5. Required `NOT NULL`, `UNIQUE`, `DEFAULT` and validation constraints are applied.
6. The seed data is inserted successfully.
7. Organiser, Participant, Event, Category, Enrolment and Result relationships can be verified using SELECT queries.

## Data Verification

The inserted sample data should be checked to confirm that:

- Organisers are linked to their events.
- Categories are linked to events.
- Participants are linked to their enrolments.
- Enrolments are linked to events and categories.
- Results are linked to enrolments.

## Final Check

The SQL script should be tested in SSMS before the Part 1 video presentation. 
Any errors identified during testing should be corrected and the script should be tested again.
