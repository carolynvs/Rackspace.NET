using OpenStack.Authentication;

namespace Rackspace.CloudServers.v2
{
    internal class ComputeApiBuilder : OpenStack.Compute.v2_1.ComputeApiBuilder
    {
        public ComputeApiBuilder(IAuthenticationProvider authenticationProvider, string region) 
            : base(ServiceType.CloudServers, authenticationProvider, region, null)
        {
        }
    }
}