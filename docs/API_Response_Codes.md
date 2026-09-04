# RaceDay API Response Codes

The RaceDay API endpoint plan uses standard HTTP response codes to communicate the outcome of API requests.

## Successful Responses

- **200 OK** – The request was successfully processed.
- **201 Created** – A new resource was successfully created.
- **204 No Content** – The request was successfully processed and no response body is required.

## Client Errors

- **400 Bad Request** – The request contains invalid or missing information.
- **401 Unauthorized** – Authentication is required or the supplied credentials are invalid.
- **403 Forbidden** – The authenticated user does not have permission to perform the requested operation.
- **404 Not Found** – The requested resource could not be found.
- **409 Conflict** – The request conflicts with an existing resource or database rule.

## Server Error

- **500 Internal Server Error** – An unexpected error occurred while processing the request.

These response codes provide consistent feedback to clients consuming the REST API and support the role-based access requirements of the system.
