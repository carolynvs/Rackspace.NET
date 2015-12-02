using System;
using System.Collections.Generic;

namespace Rackspace.CloudServers.v2
{
    /// <summary />
    public class ServerListOptions : PageOptions
    {
        /// <summary />
        public DateTimeOffset? UpdatedAfter { get; set; }

        /// <summary />
        public Identifier ImageId { get; set; }

        /// <summary />
        public string FlavorId { get; set; }

        /// <summary />
        public string Name { get; set; }


        /// <summary />
        protected override IDictionary<string, object> BuildQueryString()
        {
            var queryString = base.BuildQueryString();
            queryString["changes-since"] = UpdatedAfter?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            queryString["image"] = ImageId;
            queryString["flavor"] = FlavorId;
            queryString["name"] = Name;


            return queryString;
        }
    }
}
