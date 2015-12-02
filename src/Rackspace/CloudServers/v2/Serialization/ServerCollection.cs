using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rackspace.Serialization;

namespace Rackspace.CloudServers.v2.Serialization
{
    /// <summary>
    /// Represents a collection of server resources of the <see cref="CloudServerService"/>.
    /// </summary>
    /// <exclude />
    public class ServerCollection<TPage, TItem> : Page<TPage, TItem>
        where TPage : ServerCollection<TPage, TItem>
        where TItem : OpenStack.Serialization.IServiceResource
    {
        /// <summary>
        /// The requested servers.
        /// </summary>
        [JsonProperty("servers")]
        protected IList<TItem> Servers => Items;

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        [JsonProperty("servers_links")]
        protected IList<PageLink> ServerLinks => Links;
    }

    /// <summary>
    /// Represents a collection of references to server resources of the <see cref="CloudServerService"/>.
    /// </summary>
    /// <exclude />
    public class ServerReferenceCollection : ServerCollection<ServerReferenceCollection, ServerReference>
    { }

    /// <inheritdoc cref="ServerCollection{TPage, TItem}" />
    /// <exclude />
    public class ServerCollection : ServerCollection<ServerCollection, Server>
    { }
}
