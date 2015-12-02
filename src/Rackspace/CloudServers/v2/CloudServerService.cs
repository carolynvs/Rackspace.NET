using System.Threading;
using System.Threading.Tasks;
using OpenStack.Authentication;
using OpenStack.Serialization;
using Rackspace.CloudServers.v2.Serialization;

namespace Rackspace.CloudServers.v2
{
    /// <summary />
    public class CloudServerService
    {
        private readonly ComputeApiBuilder _computeApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudServerService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        public CloudServerService(IAuthenticationProvider authenticationProvider, string region)
        {
            _computeApi = new ComputeApiBuilder(authenticationProvider, region);
        }

        #region Servers
        /// <inheritdoc cref="OpenStack.Compute.v2_1.ComputeApiBuilder.ListServersAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public async Task<IPage<ServerReference>> ListServerReferencesAsync(ServerListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServersAsync<ServerReferenceCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="OpenStack.Compute.v2_1.ComputeApiBuilder.ListServersAsync{TPage}(IQueryStringBuilder,CancellationToken)" />
        public async Task<IPage<Server>> ListServersAsync(ServerListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _computeApi.ListServersAsync<ServerCollection>(options, cancellationToken);
        }

        #endregion
    }
}
