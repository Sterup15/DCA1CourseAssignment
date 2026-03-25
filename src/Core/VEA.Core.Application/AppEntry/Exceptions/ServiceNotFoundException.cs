namespace VEA.Core.Application.AppEntry.Exceptions;

public class ServiceNotFoundException(string serviceName)
    : Exception($"No handler registered for: {serviceName}");
