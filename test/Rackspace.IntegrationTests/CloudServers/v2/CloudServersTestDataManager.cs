using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.openstack.Providers.Rackspace;

namespace Rackspace.CloudServers.v2
{
    public class CloudServersTestDataManager : IDisposable
    {
        private readonly CloudServerService _serverService;
        private readonly HashSet<object> _testData;

        public CloudServersTestDataManager(CloudServerService serverService)
        {
            _serverService = serverService;
            _testData = new HashSet<object>();
        }

        public CloudServersTestDataManager(CloudIdentityProvider identity, string region) : this(new CloudServerService(identity, region))
        {
        }

        public void Register(IEnumerable<object> testItems)
        {
            foreach (var testItem in testItems)
            {
                Register(testItem);
            }
        }

        public void Register(object testItem)
        {
            _testData.Add(testItem);
        }

        public void Dispose()
        {
            var errors = new List<Exception>();
            try
            {
                DeleteServers(_testData.OfType<Server>());
            }
            catch (AggregateException ex) { errors.AddRange(ex.InnerExceptions); }
            
            if (errors.Any())
                throw new AggregateException("Unable to remove all test data!", errors);
        }

        public Server CreateServer(Identifier networkId)
        {
            //var name = TestData.GenerateName();
            //const string flavor = "2"; // 512 MB Standard Instance
            //const string image = "09de0a66-3156-48b4-90a5-1cf25a905207"; // Ubuntu 14.04 LTS (Trusty Tahr) (PVHVM)
            //var requestedServer = _serverService.
            //var server = _serverService.GetDetails(requestedServer.Id);
            //Register(server);
            //return server;
            throw new NotImplementedException();
        }

        private void DeleteServers(IEnumerable<Server> servers)
        {
            //var deletes = servers.Select(x =>
            //    Task.Run(() =>
            //        {
            //            _serverService.DeleteServer(x.Id);
            //            _serverService.WaitForServerDeleted(x.Id);
            //        })
            //    ).ToArray();
            //Task.WaitAll(deletes);
            throw new NotImplementedException();
        }
    }
}
