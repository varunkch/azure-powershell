using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Xunit;
using Microsoft.Azure.ServiceManagemenet.Common.Models;
using Microsoft.WindowsAzure.Commands.Test.Utilities.Common;

namespace Commands.ManagedServiceIdentity.Tests.ScenarioTests
{
    public class UserAssignedIdentityCrudTests : RMTestBase
    {
        public UserAssignedIdentityCrudTests(Xunit.Abstractions.ITestOutputHelper output)
        {
            var helper = new EnvironmentSetupHelper
            {
                TracingInterceptor =new XunitTracingInterceptor(output)
            };
            XunitTracingInterceptor.AddToContext(helper.TracingInterceptor);
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestUserAssignedIdentityCrud()
        {
            TestController.NewInstance.RunPsTest("Test-CrudUserAssignedIdentity");
        }
    }
}
