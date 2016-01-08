using System.Collections.Generic;
using Newtonsoft.Json;
using Rackspace.Serialization;

namespace Rackspace.CloudNetworks.v2.Serialization
{
    /// <inheritdoc cref="OpenStack.Networking.v2.Serialization.PortCollection"/>
    public class PortCollection : Page<PortCollection, Port>
    {
        /// <summary>
        /// The requested ports.
        /// </summary>
        [JsonProperty("ports")]
        protected IList<Port> Ports => Items;

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        [JsonProperty("ports_links")]
        protected IList<PageLink> PortsLinks => Links;
    }
}