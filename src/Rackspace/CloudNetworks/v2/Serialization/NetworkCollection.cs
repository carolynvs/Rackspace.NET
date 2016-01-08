using System.Collections.Generic;
using Newtonsoft.Json;
using Rackspace.Serialization;

namespace Rackspace.CloudNetworks.v2.Serialization
{
    /// <inheritdoc cref="OpenStack.Networking.v2.Serialization.NetworkCollection"/>
    public class NetworkCollection : Page<NetworkCollection, Network>
    {
        /// <summary>
        /// The requested networks.
        /// </summary>
        [JsonProperty("networks")]
        protected IList<Network> Networks => Items;

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        [JsonProperty("networks_links")]
        protected IList<PageLink> NetworksLinks => Links;
    }
}
