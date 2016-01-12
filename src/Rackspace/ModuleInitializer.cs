using OpenStack;
using Rackspace;

// ReSharper disable once CheckNamespace

/// <summary>
/// Ensures that Rackspace.NET is configured before any API calls are executed.
/// </summary>
/// <exclude />
public static class ModuleInitializer
{
    private static bool _isInitialized;

    /// <summary>
    /// Automatically called before any SDK code is executed.
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized)
            return;

        OpenStackNetConfigurationOptions.Creating += @event =>
        {
            @event.Result = new RackspaceNetConfigurationOptions();
        };
        _isInitialized = true;
    }
}