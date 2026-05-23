using Microsoft.AspNetCore.Http;

namespace Armageddon.Server.Common.Dtos
{
    /// <summary>
    /// Standardized envelope for all API responses.
    /// Ensures uniform structure for success and error cases across the entire API.
    /// </summary>
    /// <typeparam name="TData">The type of the payload data (use object? or omit for no-data responses).</typeparam>
    public class ApiResponse<TData>
    {
        /// <summary>
        /// Indicates whether the operation completed successfully from a business perspective.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// The HTTP status code that should be returned (mirrors the controller's response).
        /// </summary>
        public int StatusCode { get; init; }

        /// <summary>
        /// A concise, human-readable message describing the outcome.
        /// Success: usually a positive confirmation.
        /// Failure: clear explanation suitable for logging and client debugging.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Optional machine-readable code for programmatic handling or localization.
        /// Examples: "INVALID_CATEGORY", "DUPLICATE_COMMUNITY_NAME", "INSUFFICIENT_BALANCE"
        /// </summary>
        public string? ErrorCode { get; init; }

        /// <summary>
        /// The primary payload (e.g. created ID, fetched entity, list of items).
        /// Null or default when the operation did not produce meaningful data.
        /// </summary>
        public TData? Data { get; init; }

        /// <summary>
        /// Optional list of specific validation or business-rule violation messages.
        /// Commonly used when Success = false and StatusCode = 400.
        /// </summary>
        public IReadOnlyList<string>? Errors { get; init; }

        /// <summary>
        /// Optional metadata (pagination info, timestamps, trace ID, etc.).
        /// Use anonymous objects or dedicated metadata classes when needed.
        /// </summary>
        public object? Metadata { get; init; }

        // ───────────────────────────────────────────────────────────────
        // Factory methods – preferred way to create instances
        // ───────────────────────────────────────────────────────────────

        public static ApiResponse<TData> Successful(
            TData data,
            string message = "Operation completed successfully",
            int statusCode = StatusCodes.Status200OK,
            object? metadata = null)
        {
            return new ApiResponse<TData>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Metadata = metadata
            };
        }

        public static ApiResponse<TData> Created(
            TData data,
            string message = "Resource created successfully",
            object? metadata = null)
        {
            return Successful(data, message, StatusCodes.Status201Created, metadata);
        }

        public static ApiResponse<TData> Error(
            string message,
            int statusCode = StatusCodes.Status400BadRequest,
            string? errorCode = null,
            IEnumerable<string>? errors = null)
        {
            return new ApiResponse<TData>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                ErrorCode = errorCode,
                Errors = errors?.ToList().AsReadOnly(),
                Data = default
            };
        }

        public static ApiResponse<TData> NotFound(string message = "Resource not found")
        {
            return Error(message, StatusCodes.Status404NotFound, "NOT_FOUND");
        }

        public static ApiResponse<TData> Unauthorized(
            string message = "Authentication required",
            string? errorCode = "UNAUTHORIZED")
        {
            return Error(message, StatusCodes.Status401Unauthorized, errorCode);
        }

        public static ApiResponse<TData> Forbidden(
            string message = "Access forbidden",
            string? errorCode = "FORBIDDEN")
        {
            return Error(message, StatusCodes.Status403Forbidden, errorCode);
        }

        public static ApiResponse<TData> BadRequest(
            string message = "Invalid Payload",
            string? errorCode = "BAD_REQUEST")
        {
            return Error(message, StatusCodes.Status400BadRequest, errorCode);
        }

        /// <summary>
        /// Creates a 400 Bad Request response specifically for validation failures.
        /// This method is optimized for cases where ModelState or FluentValidation produces
        /// a list of field-specific error messages.
        /// </summary>
        /// <param name="message">General message (optional – defaults to standard validation text).</param>
        /// <param name="errors">List of individual validation error messages (e.g. from ModelState).</param>
        /// <param name="errorCode">Optional custom error code (defaults to "VALIDATION_ERROR").</param>
        public static ApiResponse<TData> ValidationError(
            string message = "One or more validation errors occurred",
            IEnumerable<string>? errors = null,
            string? errorCode = "VALIDATION_ERROR")
        {
            return Error(
                message: message,
                statusCode: StatusCodes.Status400BadRequest,
                errorCode: errorCode,
                errors: errors);
        }

        public static ApiResponse<TData> ServerError(
            string message = "An unexpected error occurred. Please try again later.",
            string? errorCode = "INTERNAL_ERROR")
        {
            return Error(message, StatusCodes.Status500InternalServerError, errorCode);
        }
    }

    /// <summary>
    /// Non-generic convenience version when no data is returned.
    /// </summary>
    public class ApiResponse : ApiResponse<object?>
    {
        // Inherits all factory methods
    }
}