using System.Threading.Tasks;
using Xunit;

namespace Rackspace.CloudServers.v2
{
    public class CloudServerServiceTests
    {
        [Fact]
        public async Task foo()
        {
            const string region = "IAD";

            var user = new net.openstack.Core.Domain.CloudIdentity();
            var identity = new net.openstack.Providers.Rackspace.CloudIdentityProvider(user);
            identity.Authenticate();

            var servers = new CloudServerService(identity, region);
            var results = await servers.ListServersAsync(new ServerListOptions {Name = "ci-*"});
            foreach (Server server in results)
            {
                server.Id
            }
        }
    }
}
