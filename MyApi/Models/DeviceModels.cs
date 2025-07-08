using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MyApi.Models;

/// <summary>
/// Represents a device.
/// </summary>
public record Device(
    string Name,
    [property: SwaggerSchema(Description = "The type of the device")]
    DeviceType DeviceType,
    [property: SwaggerSchema(Description = "The physical dimensions of the device")]
    Dimension Dimension);

/// <summary>
/// Represents a device type.
/// </summary>
public record DeviceType(
    [property: SwaggerSchema(Description = "The unique identifier of the device type")]
    int Id,
    [property: SwaggerSchema(Description = "The name of the device type")]
    string Name);

/// <summary>
/// Represents the physical dimensions of a device.
/// </summary>
public record Dimension(
    [property: SwaggerSchema(Description = "The width of the device in centimeters")]
    [Range(0.1, 1000, ErrorMessage = "Width must be between 0.1 and 1000 cm.")]
    double Width,
    [property: SwaggerSchema(Description = "The height of the device in centimeters")]
    [Range(0.1, 1000, ErrorMessage = "Height must be between 0.1 and 1000 cm.")]
    double Height,
    [property: SwaggerSchema(Description = "The depth of the device in centimeters")]
    [Range(0.1, 1000, ErrorMessage = "Depth must be between 0.1 and 1000 cm.")]
    double Depth)
{
    /// <summary>
    /// Gets the volume of the device in cubic centimeters.
    /// </summary>
    public double Volume => Width * Height * Depth;
}

/// <summary>
/// Request model for generating a custom device.
/// </summary>
public class CustomDeviceRequest
{
    /// <summary>
    /// The custom name for the device.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Represents an error response.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// The error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    public string? Details { get; set; }
}
