﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Authentication;

namespace Rackspace.RackConnect.v3
{
    /// <summary>
    /// The Rackspace RackConnect (Hybrid Cloud) service.
    /// </summary>
    /// <seealso href="http://www.rackspace.com/cloud/hybrid/rackconnect">Rackspace Hybrid Cloud / RackConnect Overview</seealso>
    /// <seealso href="http://docs.rcv3.apiary.io/">RackConnect v3 API</seealso>
    /// <threadsafety static="true" instance="false"/>
    public class RackConnectService
    {
        /// <summary />
        private readonly IAuthenticationProvider _authenticationProvider;

        /// <summary />
        private readonly ServiceUrlBuilder _urlBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="RackConnectService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        public RackConnectService(IAuthenticationProvider authenticationProvider, string region)
        {
            if (authenticationProvider == null)
                throw new ArgumentNullException("authenticationProvider");
            if (string.IsNullOrEmpty(region))
                throw new ArgumentException("region cannot be null or empty", "region");

            _authenticationProvider = authenticationProvider;
            _urlBuilder = new ServiceUrlBuilder(ServiceType.RackConnect, authenticationProvider, region);
        }

        private void SetOwner(IServiceResource<RackConnectService> resource)
        {
            resource.Owner = this;
        }

        #region Public IPs

        /// <summary>
        /// Lists all public IP addresses associated with the account.
        /// </summary>
        /// <param name="serverId">Filter the results to only those associated with the specified server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A collection of public IP addresses associated with the account.
        /// </returns>
        public async Task<IEnumerable<PublicIP>> ListPublicIPsAsync(string serverId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            var ips = await endpoint
                .AppendPathSegments("public_ips")
                .SetQueryParam("cloud_server_id", serverId)
                .Authenticate(_authenticationProvider)
                .GetJsonAsync<IEnumerable<PublicIP>>(cancellationToken)
                .ConfigureAwait(false);

            foreach (var ip in ips)
            {
                SetOwner(ip);
            }

            return ips;
        }

        /// <summary>
        /// Provisions a public IP address.
        /// </summary>
        /// <param name="definition">The</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The identifer of the public IP address while it is being provisioned. Use <see cref="WaitUntilPublicIPIsActiveAsync"/> to wait for the IP address to be full active.</returns>
        public async Task<PublicIP> ProvisionPublicIPAsync(PublicIPDefinition definition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            Url endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);
            
            var ip = await endpoint
                .AppendPathSegments("public_ips")
                .Authenticate(_authenticationProvider)
                .PostJsonAsync(definition, cancellationToken)
                .ReceiveJson<PublicIP>()
                .ConfigureAwait(false);

            SetOwner(ip);

            return ip;
        }

        /// <summary>
        /// Gets the specified public IP address.
        /// </summary>
        /// <param name="publicIPId">The public IP address identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<PublicIP> GetPublicIPAsync(Identifier publicIPId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);
            
            var ip = await endpoint
                .AppendPathSegments("public_ips", publicIPId)
                .Authenticate(_authenticationProvider)
                .GetJsonAsync<PublicIP>(cancellationToken)
                .ConfigureAwait(false);

            SetOwner(ip);

            return ip;
        }

        /// <summary>
        /// Waits for the public IP address to become active.
        /// </summary>
        /// <param name="publicIPId">The public IP address identifier.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task<PublicIP> WaitUntilPublicIPIsActiveAsync(Identifier publicIPId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(publicIPId))
                throw new ArgumentNullException("publicIPId");

            refreshDelay = refreshDelay ?? TimeSpan.FromSeconds(5);
            timeout = timeout ?? TimeSpan.FromMinutes(5);

            using (var timeoutSource = new CancellationTokenSource(timeout.Value))
            using (var rootCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token))
            {
                while (true)
                {
                    PublicIP ip = await GetPublicIPAsync(publicIPId, cancellationToken).ConfigureAwait(false);
                    if (ip.Status == PublicIPStatus.AddFailed)
                        throw new ServiceOperationFailedException(ip.StatusDetails);

                    bool complete = ip.Status == PublicIPStatus.Active;

                    progress?.Report(complete);

                    if (complete)
                        return ip;

                    try
                    {
                        await Task.Delay(refreshDelay.Value, rootCancellationToken.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (timeoutSource.IsCancellationRequested)
                            throw new TimeoutException($"The requested timeout of {timeout.Value.TotalSeconds} seconds has been reached while waiting for the public IP ({publicIPId}) to become active.", ex);

                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Waits for the public IP address to be removed from a server.
        /// </summary>
        /// <param name="publicIPId">The public IP address identifier.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public async Task WaitUntilPublicIPIsRemovedAsync(Identifier publicIPId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(publicIPId))
                throw new ArgumentNullException("publicIPId");

            refreshDelay = refreshDelay ?? TimeSpan.FromSeconds(5);
            timeout = timeout ?? TimeSpan.FromMinutes(5);

            using (var timeoutSource = new CancellationTokenSource(timeout.Value))
            using (var rootCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token))
            {
                while (true)
                {
                    bool complete;
                    try
                    {
                        PublicIP ip = await GetPublicIPAsync(publicIPId, cancellationToken).ConfigureAwait(false);
                        if(ip.Status == PublicIPStatus.RemoveFailed)
                            throw new ServiceOperationFailedException(ip.StatusDetails);

                        complete = ip.Status == PublicIPStatus.Removed;
                    }
                    catch (FlurlHttpException httpError)
                    {
                        if (httpError.Call.HttpStatus == HttpStatusCode.NotFound)
                            complete = true;
                        else
                            throw;
                    }

                    progress?.Report(complete);

                    if (complete)
                        return;

                    try
                    {
                        await Task.Delay(refreshDelay.Value, rootCancellationToken.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (timeoutSource.IsCancellationRequested)
                            throw new TimeoutException($"The requested timeout of {timeout.Value.TotalSeconds} seconds has been reached while waiting for the public IP ({publicIPId}) to be removed.", ex);

                        throw;
                    }
                }
            }
        }
        
        /// <summary>
        /// Removes the public IP address.
        /// </summary>
        /// <param name="publicIPId">The public IP address identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task RemovePublicIPAsync(Identifier publicIPId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (publicIPId == null)
                throw new ArgumentNullException("publicIPId");

            Url endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);
            
            await endpoint
                .AppendPathSegments("public_ips", publicIPId)
                .Authenticate(_authenticationProvider)
                .DeleteAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        #endregion

        #region Networks
        /// <summary>
        /// Lists Cloud Networks associated with a RackConnect Configuration.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A collection of networks associated with the account.
        /// </returns>
        public async Task<IEnumerable<NetworkReference>> ListNetworksAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return await endpoint
                .AppendPathSegments("cloud_networks")
                .Authenticate(_authenticationProvider)
                .GetJsonAsync<IEnumerable<NetworkReference>>(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the specified RackConnect Cloud Network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<NetworkReference> GetNetworkAsync(Identifier networkId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await _urlBuilder.GetEndpoint(cancellationToken).ConfigureAwait(false);

            return await endpoint
                .AppendPathSegments("cloud_networks", networkId)
                .Authenticate(_authenticationProvider)
                .GetJsonAsync<NetworkReference>(cancellationToken)
                .ConfigureAwait(false);
        }
        #endregion

    }
}