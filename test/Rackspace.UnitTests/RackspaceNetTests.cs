using System;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack;
using Rackspace.Testing;
using Xunit;

namespace Rackspace
{
    public class RackspaceNetTests : IDisposable
    {
        public RackspaceNetTests()
        {
            RackspaceNet.ResetDefaults();
        }

        public void Dispose()
        {
            RackspaceNet.ResetDefaults();
        }

        [Fact]
        public async Task UseBothOpenStackAndRackspace_OpenStackConfiguredFirst()
        {
            using (var httpTest = new HttpTest())
            {
                await "http://api.com".PrepareRequest().GetAsync();

                var userAgent = httpTest.CallLog[0].Request.Headers.UserAgent.ToString();

                var rackspaceMatches = new Regex("rackspace").Matches(userAgent);
                Assert.Equal(1, rackspaceMatches.Count);

                var openstackMatches = new Regex("openstack").Matches(userAgent);
                Assert.Equal(1, openstackMatches.Count);
            }
        }

        [Fact]
        public async Task UseBothOpenStackAndRackspace_RackspaceConfiguredFirst()
        {
            using (var httpTest = new HttpTest())
            {
#pragma warning disable 618
                OpenStackNet.Configure();
#pragma warning restore 618

                await "http://api.com".PrepareRequest().GetAsync();

                var userAgent = httpTest.CallLog[0].Request.Headers.UserAgent.ToString();

                var rackspaceMatches = new Regex("rackspace").Matches(userAgent);
                Assert.Equal(1, rackspaceMatches.Count);

                var openstackMatches = new Regex("openstack").Matches(userAgent);
                Assert.Equal(1, openstackMatches.Count);
            }
        }

        [Fact]
        public async Task UserAgentTest()
        {
            using (var httpTest = new HttpTest())
            {
                await "http://api.com".PrepareRequest().GetAsync();

                var userAgent = httpTest.CallLog[0].Request.Headers.UserAgent.ToString();
                Assert.Contains("rackspace.net", userAgent);
                Assert.Contains("openstack.net", userAgent);
            }
        }

        [Fact]
        public async Task UserAgentWithApplicationSuffixTest()
        {
            using (var httpTest = new HttpTest())
            {
                RackspaceNet.Configuring += options =>
                {
                    options.UserAgents.Add(new ProductInfoHeaderValue("(unit-tests)"));
                };

                await "http://api.com".PrepareRequest().GetAsync();

                var userAgent = httpTest.CallLog[0].Request.Headers.UserAgent.ToString();
                Assert.Contains("rackspace.net", userAgent);
                Assert.Contains("unit-tests", userAgent);
            }
        }
    }
}
