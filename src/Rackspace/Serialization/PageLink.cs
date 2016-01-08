using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace Rackspace.Serialization
{
    /// <summary />
    public class PageLink : IPageLink, IHaveExtraData
    {
        /// <summary />
        public PageLink(string relationship, string url)
        {
            Relationship = relationship;
            Url = url;
        }

        /// <summary />
        [JsonProperty("href")]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string Url { get; private set; }

        /// <summary />
        [JsonIgnore]
        public bool IsNextPage => Relationship == "next";

        /// <summary />
        [JsonProperty("rel")]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string Relationship { get; private set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();
    }
}