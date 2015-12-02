using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace Rackspace.CloudServers.v2
{
    /// <summary />
    public class ServerReference : IHaveExtraData, IServiceResource
    {
        /// <summary />
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        object IServiceResource.Owner { get; set; }
    }

    /// <summary />
    [JsonConverterWithConstructor(typeof (RootWrapperConverter), "server")]
    public class Server : ServerReference
    {
    }
}
