using System;
using System.Diagnostics;
using System.Extensions;
using System.Net.Http.Headers;
using OpenStack;

namespace Rackspace
{
    /// <summary>
    /// A static container for global configuration settings affecting Rackspace.NET behavior.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public static class RackspaceNet
    {
        /// <summary>
        /// Global configuration which affects OpenStack.NET's behavior.
        /// <para>Customize using the <see cref="Configuring"/> event.</para>
        /// </summary>
        public static RackspaceNetConfigurationOptions Configuration => (RackspaceNetConfigurationOptions)OpenStackNet.Configuration;

        /// <summary>
        /// Occurs when initializing the global configuration for Rackspace.NET.
        /// </summary>
        public static event Action<RackspaceNetConfigurationOptions> Configuring
        {
            add { RackspaceNetConfigurationOptions.Initializing += value; }
            remove { RackspaceNetConfigurationOptions.Initializing -= value; }
        }

        /// <summary>
        /// <par>Resets all configuration (OpenStack.NET, Flurl and Json.NET).</par>
        /// <para>After this is called, you must re-register any <see cref="Configuring"/> event handlers.</para>
        /// </summary>
        public static void ResetDefaults()
        {
            RackspaceNetConfigurationOptions.ResetDefaults();
            OpenStackNet.ResetDefaults();
        }

        /// <inheritdoc cref="OpenStack.OpenStackNet.Tracing" />
        public static class Tracing
        {
            /// <inheritdoc cref="OpenStack.OpenStackNet.Tracing.Http" />
            public static readonly TraceSource Http = OpenStackNet.Tracing.Http;
        }
    }

    /// <summary>
    /// <para>A set of properties that affect the SDK's behavior.</para>
    /// <para>To customize, register an event handler for <see cref="OpenStackNet.Configuring"/>.</para>
    /// </summary>
    public class RackspaceNetConfigurationOptions : OpenStackNetConfigurationOptions
    {
        internal static event Action<RackspaceNetConfigurationOptions> Initializing;

        internal static void ResetDefaults()
        {
            Initializing = null;
        }

        /// <summary />
        protected override void OnCompleteInitialization()
        {
            Initializing?.Invoke(this);
            UserAgents.Add(new ProductInfoHeaderValue("rackspace.net", typeof(RackspaceNetConfigurationOptions).GetAssemblyFileVersion()));
        }
    }
}
