using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PageLink = OpenStack.Serialization.PageLink;

namespace Rackspace.Serialization
{
    /// <inheritdoc />
    [JsonObject(MemberSerialization.OptIn)]
    public class Page<TPage, TItem> : IPage<TItem>
        where TPage : Page<TPage, TItem>
    {
        private readonly PageWrapper _page;

        private Page(PageWrapper page)
        {
            _page = page;
        } 

        /// <summary>
        /// Initializes a new instance of the <see cref="Page{TPage, TItem}"/> class.
        /// </summary>
        public Page() : this(new PageWrapper())
        { }
        
        /// <summary>
        /// The requested items.
        /// </summary>
        protected IList<TItem> Items
        {
            get { return _page.Items; }
            set { _page.Items = value; }
        }

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        protected IList<PageLink> Links
        {
            get { return _page.Links; }
            set { _page.Links = value; }
        }

        /// <inheritdoc />
        public IEnumerator<TItem> GetEnumerator()
        {
            return _page.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_page).GetEnumerator();
        }

        /// <inheritdoc />
        public bool HasNextPage => _page.HasNextPage;

        /// <inheritdoc />
        public async Task<IPage<TItem>> GetNextPageAsync(CancellationToken cancellation = new CancellationToken())
        {
            var nextPageAsync = (PageWrapper)(await _page.GetNextPageAsync(cancellation));
            return new Page<TPage, TItem>(nextPageAsync);
        }

        // This is a wee bit of black magic which allows us to refer to a recurive generic type
        // I couldn't figure out how to have _page use the underlying type directly :-)
        private class PageWrapper : OpenStack.Serialization.Page<PageWrapper, TItem, PageLink>
        {

        }
    }
}