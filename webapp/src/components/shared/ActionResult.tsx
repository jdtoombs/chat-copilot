/**
 * Represents the result of an action, typically an HTTP action in the context of RESTful APIs.
 * It includes a status code to indicate the result of the action, and optionally, a body for additional information.
 *
 * @interface ActionResult
 * @property {number} statusCode - The HTTP status code returned by the operation.
 *                                E.g., 204 for No Content, 404 for Not Found, 200 for OK, etc.
 * @property {*} [body] - Optional body which might contain additional data or messages from the server.
 */
export interface ActionResult {
    statusCode: number;
    body?: any;
}
