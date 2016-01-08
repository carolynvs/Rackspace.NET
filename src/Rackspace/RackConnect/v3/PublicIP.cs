using System;
using System.Extensions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace Rackspace.RackConnect.v3
{
    /// <summary>
    /// Represents a public IP address resource of the <see cref="RackConnectService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class PublicIP : IServiceResource
    {
        /// <summary>
        /// The public IP address identifier.
        /// </summary>
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary>
        /// Timestamp when the public IP address was allocated.
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// The server to which the public IP address has been allocated.
        /// </summary>
        [JsonProperty("cloud_server")]
        public PublicIPServerAssociation Server { get; set; }

        /// <summary>
        /// The allocated public IP address (IPv4).
        /// </summary>
        [JsonProperty("public_ip_v4")]
        public string PublicIPv4Address { get; set; }

        /// <summary>
        /// The public IP address status.
        /// </summary>
        [JsonProperty("status")]
        public PublicIPStatus Status { get; set; }

        /// <summary>
        /// Provides additional information when <see cref="Status"/> is in a failed state.
        /// </summary>
        [JsonProperty("status_detail")]
        public string StatusDetails { get; set; }

        /// <summary>
        /// Timestamp when the public IP address allocation was last updated.
        /// </summary>
        [JsonProperty("updated")]
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Determines whether a Public IP is removed from your environment once the server to which it is attached is deleted.
        /// </summary>
        [JsonProperty("retain")]
        public bool ShouldRetain { get; set; }

        /// <inheritdoc cref="RackConnectService.WaitUntilPublicIPIsActiveAsync" />
        /// <exception cref="InvalidOperationException">When the <see cref="PublicIP"/> instance was not constructed by the <see cref="RackConnectService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task WaitUntilActiveAsync(TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<RackConnectService>();
            var result = await owner.WaitUntilPublicIPIsActiveAsync(Id, refreshDelay, timeout, progress, cancellationToken).ConfigureAwait(false);
            result.CopyProperties(this);
        }

        /// <inheritdoc cref="RackConnectService.WaitUntilPublicIPIsDeletedAsync" />
        /// <exception cref="InvalidOperationException">When the <see cref="PublicIP"/> instance was not constructed by the <see cref="RackConnectService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task WaitUntilDeletedAsync(TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<RackConnectService>();
            await owner.WaitUntilPublicIPIsDeletedAsync(Id, refreshDelay, timeout, progress, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc cref="RackConnectService.DeletePublicIPAsync" />
        /// <exception cref="InvalidOperationException">When the <see cref="PublicIP"/> instance was not constructed by the <see cref="RackConnectService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<RackConnectService>();
            await owner.DeletePublicIPAsync(Id, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Assigns the Public IP to a cloud server.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the <see cref="PublicIP"/> instance was not constructed by the <see cref="RackConnectService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task AssignAsync(string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<RackConnectService>();
            var definition = new PublicIPUpdateDefinition { ServerId = serverId};
            var result = await owner.UpdatePublicIPAsync(Id, definition, cancellationToken).ConfigureAwait(false);
            result.CopyProperties(this);
        }

        /// <summary>
        /// Removes the Public IP from the server to which it is currently assigned.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the <see cref="PublicIP"/> instance was not constructed by the <see cref="RackConnectService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task UnassignAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<RackConnectService>();
            var definition = new PublicIPUpdateDefinition {ServerId = null};
            var result = await owner.UpdatePublicIPAsync(Id, definition, cancellationToken).ConfigureAwait(false);
            result.CopyProperties(this);
        }

        object IServiceResource.Owner { get; set; }
    }
}