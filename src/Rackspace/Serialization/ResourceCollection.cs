using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IHaveExtraData = OpenStack.Serialization.IHaveExtraData;

namespace Rackspace.Serialization
{
    /// <inheritdoc cref="OpenStack.Serialization.ResourceCollection{T}" />
    [JsonObject(MemberSerialization.OptIn)] // Using JsonObject to force the entire object to be serialized, ignoring the IEnumerable interface
    public class ResourceCollection<T> : IEnumerable<T>, IHaveExtraData
    {
        private readonly OpenStack.Serialization.ResourceCollection<T> _items = new OpenStack.Serialization.ResourceCollection<T>();

        /// <inheritdoc cref="OpenStack.Serialization.ResourceCollection{T}.Items" />
        public IList<T> Items => _items.Items;
            
        IDictionary<string, JToken> IHaveExtraData.Data
        {
            get { return ((IHaveExtraData)_items).Data; }
            set { ((IHaveExtraData)_items).Data = value; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}