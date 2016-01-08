using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Newtonsoft.Json;

namespace Rackspace.Serialization
{
    /// <inheritdoc cref="IPage{T}" />
    /// <exclude />
    [JsonObject(MemberSerialization.OptIn)]
    public class Page<TPage, TItem> : ResourceCollection<TItem>, IPage<TItem>, OpenStack.Serialization.IPageBuilder<TPage>
        where TPage : Page<TPage, TItem>
    {
        private Func<Url, CancellationToken, Task<TPage>> _nextPageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Page{TPage,TItem}"/> class.
        /// </summary>
        public Page()
        {
            Links = new List<PageLink>();
        }

        /// <inheritdoc />
        [JsonIgnore]
        public bool HasNextPage => GetNextLink() != null;

        /// <inheritdoc />
        void OpenStack.Serialization.IPageBuilder<TPage>.SetNextPageHandler(Func<Url, CancellationToken, Task<TPage>> value)
        {
            _nextPageHandler = value;
        }

        /// <inheritdoc />
        public async Task<IPage<TItem>> GetNextPageAsync(CancellationToken cancellationToken)
        {
            var nextPageLink = GetNextLink();
            if (nextPageLink == null)
                return Empty();

            return await _nextPageHandler(new Url(nextPageLink.Url), cancellationToken);
        }

        /// <summary>
        /// Returns an empty page
        /// </summary>
        public static IPage<TItem> Empty()
        {
            return EmptyPage.Instance;
        }

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        public IList<PageLink> Links { get; set; }

        /// <summary>
        /// Finds the next link.
        /// </summary>
        protected virtual PageLink GetNextLink()
        {
            return Links.FirstOrDefault(x => x.IsNextPage);
        }

        private sealed class EmptyPage : Page<TPage, TItem>
        {
            public static readonly EmptyPage Instance = new EmptyPage();

            private EmptyPage() { }
        }
    }
}