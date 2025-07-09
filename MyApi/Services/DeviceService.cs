using MyApi.Models;

namespace MyApi.Services;

/// <summary>
/// Service responsible for device business logic and operations.
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Processes a custom header and returns a response message.
    /// </summary>
    /// <param name="headerValue">The custom header value.</param>
    /// <returns>A formatted response message.</returns>
    string ProcessCustomHeader(string headerValue);

    /// <summary>
    /// Gets devices with rate limiting information.
    /// </summary>
    /// <returns>An object containing devices and rate limiting message.</returns>
    object GetDevicesWithRateLimit();

    /// <summary>
    /// Gets a standard collection of devices.
    /// </summary>
    /// <returns>A collection of devices.</returns>
    IEnumerable<Device> GetDevices();

    /// <summary>
    /// Gets public devices without authentication.
    /// </summary>
    /// <returns>A collection of devices.</returns>
    IEnumerable<Device> GetPublicDevices();

    /// <summary>
    /// Gets private devices with user information.
    /// </summary>
    /// <param name="username">The authenticated user's name.</param>
    /// <returns>An object containing user info and devices.</returns>
    object GetPrivateDevices(string username);

    /// <summary>
    /// Validates and creates a complete device.
    /// </summary>
    /// <param name="device">The device to validate and create.</param>
    /// <returns>The validated device or null if invalid.</returns>
    Device? CreateCompleteDevice(Device device);

    /// <summary>
    /// Creates a device with additional parameters.
    /// </summary>
    /// <param name="device">The base device.</param>
    /// <param name="location">The device location.</param>
    /// <param name="status">The device status (0=Offline, 1=Online).</param>
    /// <returns>The enhanced device object or null if validation fails.</returns>
    object? CreateDeviceWithParameters(Device device, string location, int status);

    /// <summary>
    /// Validates if a device exists for deletion.
    /// </summary>
    /// <param name="id">The device ID.</param>
    /// <param name="date">The device date.</param>
    /// <returns>True if the device exists and can be deleted.</returns>
    bool ValidateDeviceForDeletion(int id, DateTime date);

    /// <summary>
    /// Gets V1-specific device data.
    /// </summary>
    /// <returns>V1 device data structure.</returns>
    object GetDevicesV1();

    /// <summary>
    /// Gets V2-specific advanced device data.
    /// </summary>
    /// <returns>V2 enhanced device data structure.</returns>
    object GetDevicesV2();

    /// <summary>
    /// Gets device summary.
    /// </summary>
    /// <returns>Device summary.</returns>
    object GetDeviceSummary();
}

/// <summary>
/// Implementation of device business logic and operations.
/// </summary>
public class DeviceService : IDeviceService
{
    private static readonly string[] DeviceNames =
    [
        "Smart Sensor", "IoT Gateway", "Temperature Monitor", "Pressure Valve", "Control Panel",
        "Display Unit", "Communication Hub", "Power Module", "Safety Switch", "Data Logger"
    ];

    /// <summary>
    /// Processes a custom header and returns a response message.
    /// </summary>
    /// <param name="headerValue">The custom header value.</param>
    /// <returns>A formatted response message.</returns>
    public string ProcessCustomHeader(string headerValue)
    {
        return $"Received header: {headerValue}";
    }

    /// <summary>
    /// Gets devices with rate limiting information.
    /// </summary>
    /// <returns>An object containing devices and rate limiting message.</returns>
    public object GetDevicesWithRateLimit()
    {
        var devices = CreateDevices();
        return new
        {
            message = "This endpoint is protected by rate limiting.",
            devices
        };
    }

    /// <summary>
    /// Gets a standard collection of devices.
    /// </summary>
    /// <returns>A collection of devices.</returns>
    public IEnumerable<Device> GetDevices()
    {
        return CreateDevices();
    }

    /// <summary>
    /// Gets public devices without authentication.
    /// </summary>
    /// <returns>A collection of devices.</returns>
    public IEnumerable<Device> GetPublicDevices()
    {
        return CreateDevices();
    }

    /// <summary>
    /// Gets private devices with user information.
    /// </summary>
    /// <param name="username">The authenticated user's name.</param>
    /// <returns>An object containing user info and devices.</returns>
    public object GetPrivateDevices(string username)
    {
        return new
        {
            message = "This endpoint is protected!",
            user = username,
            devices = CreateDevices()
        };
    }

    /// <summary>
    /// Validates and creates a complete device.
    /// </summary>
    /// <param name="device">The device to validate and create.</param>
    /// <returns>The validated device or null if invalid.</returns>
    public Device? CreateCompleteDevice(Device device)
    {
        if (device == null || string.IsNullOrWhiteSpace(device.Name))
        {
            return null;
        }

        return new Device(
            device.Name,
            device.DeviceType,
            device.Dimension
        );
    }

    /// <summary>
    /// Creates a device with additional parameters.
    /// </summary>
    /// <param name="device">The base device.</param>
    /// <param name="location">The device location.</param>
    /// <param name="status">The device status (0=Offline, 1=Online).</param>
    /// <returns>The enhanced device object or null if validation fails.</returns>
    public object? CreateDeviceWithParameters(Device device, string location, int status)
    {
        if (device == null || string.IsNullOrWhiteSpace(device.Name))
        {
            return null;
        }

        if (status < 0 || status > 1)
        {
            return null;
        }

        return new
        {
            device.Name,
            device.DeviceType,
            device.Dimension,
            Location = location,
            Status = status == 1 ? "Online" : "Offline"
        };
    }

    /// <summary>
    /// Validates if a device exists for deletion.
    /// </summary>
    /// <param name="id">The device ID.</param>
    /// <param name="date">The device date.</param>
    /// <returns>True if the device exists and can be deleted.</returns>
    public bool ValidateDeviceForDeletion(int id, DateTime date)
    {
        // Simulate validation logic - IDs 1-5 exist
        return id >= 1 && id <= 5;
    }

    /// <summary>
    /// Gets V1-specific device data.
    /// </summary>
    /// <returns>V1 device data structure.</returns>
    public object GetDevicesV1()
    {
        return new
        {
            Version = "1.0",
            Message = "This is a V1-only endpoint",
            Data = CreateDevices().Take(3), // V1 returns only 3 devices
            Features = new[] { "Basic device listing", "Simple data structure" }
        };
    }

    /// <summary>
    /// Gets V2-specific advanced device data.
    /// </summary>
    /// <returns>V2 enhanced device data structure.</returns>
    public object GetDevicesV2()
    {
        return new
        {
            Version = "2.0",
            Message = "This is a V2-only endpoint with enhanced features",
            Data = CreateDevices().Select(d => new
            {
                d.Name,
                d.DeviceType,
                d.Dimension,
                d.Dimension.Volume,
                // V2 additions
                Status = Random.Shared.Next(0, 2) == 1 ? "Online" : "Offline",
                BatteryLevel = Random.Shared.Next(10, 100),
                LastSeen = DateTime.UtcNow.AddMinutes(-Random.Shared.Next(1, 1440))
            }),
            Features = new[] { "Advanced device monitoring", "Extended data structure", "Additional device metrics" },
            Metadata = new
            {
                DataSource = "Advanced Device API v2",
                LastUpdated = DateTime.UtcNow,
                Accuracy = "High precision"
            }
        };
    }

    /// <summary>
    /// Gets device summary.
    /// </summary>
    /// <returns>Device summary.</returns>
    public object GetDeviceSummary()
    {

        return new
        {
            Version = "1.0",
            Summary = "Basic device summary",
            TotalDevices = 5,
            AverageVolume = CreateDevices().Average(d => d.Dimension.Volume),
            AvailableLocations = new[] { "Factory A", "Warehouse B", "Office C" }
        };
    }

    /// <summary>
    /// Create a list of sample devices.
    /// </summary>
    /// <returns>A list of devices.</returns>
    private static IEnumerable<Device> CreateDevices()
    {
        var deviceTypes = new[]
        {
            new DeviceType(1, "Sensor"),
            new DeviceType(2, "Controller"),
            new DeviceType(3, "Display"),
            new DeviceType(4, "Gateway"),
            new DeviceType(5, "Monitor")
        };

        return Enumerable.Range(1, 5).Select(index => new Device
        (
            DeviceNames[Random.Shared.Next(DeviceNames.Length)],
            deviceTypes[Random.Shared.Next(deviceTypes.Length)],
            new Dimension(
                Math.Round(Random.Shared.NextDouble() * 50 + 5, 2), // Width: 5-55 cm
                Math.Round(Random.Shared.NextDouble() * 30 + 3, 2), // Height: 3-33 cm
                Math.Round(Random.Shared.NextDouble() * 20 + 2, 2)  // Depth: 2-22 cm
            )
        ))
        .ToArray();
    }

    /// <summary>
    /// Helper method to get device status based on name.
    /// </summary>
    /// <param name="deviceName">Device name.</param>
    /// <returns>Status indicator for the device.</returns>
    private static string GetDeviceStatus(string? deviceName) => deviceName?.ToLower() switch
    {
        var n when n?.Contains("sensor") == true => "🟢 Online",
        var n when n?.Contains("gateway") == true => "🔵 Connected",
        var n when n?.Contains("monitor") == true => "🟡 Monitoring",
        var n when n?.Contains("control") == true => "🟠 Active",
        var n when n?.Contains("safety") == true => "🔴 Alert",
        _ => "⚪ Unknown"
    };
}
