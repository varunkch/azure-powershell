using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Xunit;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Management.ManagedServiceIdentity;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.ServiceManagemenet.Common.Models;
using Microsoft.Azure.Test;
using Microsoft.WindowsAzure.Commands.Test.Utilities.Common;

namespace Commands.ManagedServiceIdentity.Tests.ScenarioTests
{
    public class UserAssignedIdentityCrudTests : RMTestBase
    {
        private readonly EnvironmentSetupHelper _helper;

        public UserAssignedIdentityCrudTests(Xunit.Abstractions.ITestOutputHelper output)
        {
            _helper = new EnvironmentSetupHelper
            {
                TracingInterceptor =new XunitTracingInterceptor(output)
            };
            XunitTracingInterceptor.AddToContext(_helper.TracingInterceptor);
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUserAssignedIdentityCrud()
        {
            RunPowerShellTest("Test-CrudUserAssignedIdentity");
        }

        private void RunPowerShellTest(params string[] scripts)
        {
            using (UndoContext context = UndoContext.Current)
            {
                // Configure recordings
                context.Start(TestUtilities.GetCallingClass(), TestUtilities.GetCurrentMethodName());

                // See explanation below
                SetupManagementClients();

                // Specify either ResourceManager or ServiceManagement mode
                _helper.SetupEnvironment(AzureModule.AzureResourceManager);

                // Add all ps1 files used in the test
                _helper.SetupModules(AzureModule.AzureResourceManager, "ScenarioTests\\Common.ps1",
                    "ScenarioTest\\" + this.GetType().Name + ".ps1");

                // Run actual test
                _helper.RunPowerShellTest(scripts);
            }
        }

        private void SetupManagementClients()
        {
            var resourceManagementClient = GetResourceManagementClient();
            var managedServiceIdentityClient = GetManagedServiceIdentityClient();

            _helper.SetupManagementClients(
                resourceManagementClient,
                managedServiceIdentityClient);
        }

        private ResourceManagementClient GetResourceManagementClient()
        {
            return TestBase.GetServiceClient<ResourceManagementClient>(new CSMTestEnvironmentFactory());
        }

        private ManagedServiceIdentityClient GetManagedServiceIdentityClient()
        {
            return TestBase.GetServiceClient<ManagedServiceIdentityClient>(new CSMTestEnvironmentFactory());
        }
    }
}
