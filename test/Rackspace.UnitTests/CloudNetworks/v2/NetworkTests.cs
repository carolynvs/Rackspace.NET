﻿using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Rackspace.CloudNetworks.v2.Serialization;
using Rackspace.Serialization;
using Rackspace.Synchronous;
using Rackspace.Testing;
using Xunit;

namespace Rackspace.CloudNetworks.v2
{
    public class NetworkTests
    {
        private readonly CloudNetworkService _cloudNetworkService;

        public NetworkTests()
        {
            _cloudNetworkService = new CloudNetworkService(Stubs.IdentityService, "region");
        }

        [Fact]
        public void NetworkCollectionSerialization()
        {
            var networks = new NetworkCollection
            {
                Items = {new Network {Id = Guid.NewGuid()}},
                Links = {new PageLink("next", "http://api.com/next")}
            };
            string json = JsonConvert.SerializeObject(networks, Formatting.None);
            Assert.Contains("\"networks\"", json);
            Assert.DoesNotContain("\"network\"", json);

            var result = JsonConvert.DeserializeObject<NetworkCollection>(json);
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
            Assert.Equal(1, result.Items.Count());
            Assert.Equal(1, result.Links.Count());
        }

        [Fact]
        public void ListNetworks()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWithJson(new NetworkCollection {Items = {new Network {Id = networkId}}});

                var networks = _cloudNetworkService.ListNetworks();

                httpTest.ShouldHaveCalled("*/networks");
                Assert.NotNull(networks);
                Assert.Equal(1, networks.Count());
                Assert.Equal(networkId, networks.First().Id);
            }
        }

        [Fact]
        public void PageNetworks()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWithJson(new NetworkCollection
                {
                    Items = {new Network {Id = networkId}},
                    Links = {new PageLink("next", "http://api.com/next")}
                });

                IPage<Network> networks = _cloudNetworkService.ListNetworks(networkId, 10);

                httpTest.ShouldHaveCalled(string.Format($"*/networks?marker={networkId}&limit=10"));
                Assert.NotNull(networks);
                Assert.Equal(1, networks.Count());
                Assert.Equal(networkId, networks.First().Id);
                Assert.True(networks.HasNextPage);
            }
        }

        [Fact]
        public void CreateNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWithJson(new Network{Id = networkId});

                var definition = new NetworkDefinition();
                var network = _cloudNetworkService.CreateNetwork(definition);

                httpTest.ShouldHaveCalled("*/networks");
                Assert.NotNull(network);
                Assert.Equal(networkId, network.Id);
            }
        }
        
        [Fact]
        public void GetNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWithJson(new Network { Id = networkId });

                var network = _cloudNetworkService.GetNetwork(networkId);

                httpTest.ShouldHaveCalled("*/networks/" + networkId);
                Assert.NotNull(network);
                Assert.Equal(networkId, network.Id);
            }
        }

        [Fact]
        public void DeleteNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");

                _cloudNetworkService.DeleteNetwork(networkId);

                httpTest.ShouldHaveCalled("*/networks/" + networkId);
            }
        }

        [Fact]
        public void WhenDeleteNetwork_Returns404NotFound_ShouldConsiderRequestSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Not here, boss...");

                _cloudNetworkService.DeleteNetwork(networkId);

                httpTest.ShouldHaveCalled("*/networks/" + networkId);
            }
        }

        [Fact]
        public void UpdateNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWithJson(new Network {Id = networkId});

                var definition = new NetworkDefinition { Name = "new network name" };
                var network = _cloudNetworkService.UpdateNetwork(networkId, definition);

                httpTest.ShouldHaveCalled("*/networks/" + networkId);
                Assert.NotNull(network);
                Assert.Equal(networkId, network.Id);
            }
        }
    }
}
