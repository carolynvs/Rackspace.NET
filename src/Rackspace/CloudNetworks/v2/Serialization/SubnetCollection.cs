using System.Collections.Generic;
using Newtonsoft.Json;
using Rackspace.Serialization;

namespace Rackspace.CloudNetworks.v2.Serialization
{
    /// <inheritdoc cref="OpenStack.Networking.v2.Serialization.SubnetCollection"/>
    public class SubnetCollection : Page<SubnetCollection, Subnet>
    {
        /// <summary>
        /// The requested subnets.
        /// </summary>
        [JsonProperty("subnets")]
        protected IList<Subnet> Subnets => Items;

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        [JsonProperty("subnets_links")]
        protected IList<PageLink> SubnetsLinks => Links;
    }
}