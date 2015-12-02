using System.Collections.Generic;

namespace Rackspace
{
    /// <summary />
    public class PageOptions : OpenStack.Serialization.IQueryStringBuilder
    {
        /// <summary />
        public int? PageSize { get; set; }

        /// <summary />
        public Identifier StartingAt { get; set; }

        /// <summary />
        protected virtual IDictionary<string, object> BuildQueryString()
        {
            return new Dictionary<string, object>
            {
                {"marker", StartingAt},
                {"limit", PageSize}
            };
        }

        IDictionary<string, object> OpenStack.Serialization.IQueryStringBuilder.Build()
        {
            return BuildQueryString();
        }
    }
}
