using System;
using System.IO;
using System.Linq;
using LegacyTest = Microsoft.Azure.Test;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Management.ManagedServiceIdentity;
using Microsoft.Azure.Test.HttpRecorder;
using Microsoft.Rest.ClientRuntime.Azure.TestFramework;
using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Microsoft.Azure.Management.Resources;

namespace Commands.ManagedServiceIdentity.Tests
{
    public class TestController
    {
        private LegacyTest.CSMTestEnvironmentFactory _csmTestFactory;
        private readonly EnvironmentSetupHelper _helper;

        public ResourceManagementClient ResourceManagementClient { get; private set; }

        public ManagedServiceIdentityClient ManagedServiceIdentityClient { get; private set; }

        public static TestController NewInstance
        {
            get
            {
                return new TestController();
            }
        }

        public TestController()
        {
            _helper = new EnvironmentSetupHelper();
        }

        public void RunPsTest(params string[] scripts)
        {
            var callingClassType = TestUtilities.GetCallingClass(2);
            var mockName = TestUtilities.GetCurrentMethodName(2);

            RunPsTestWorkflow(
                () => scripts,
                // no custom initializer
                null,
                // no custom cleanup 
                null,
                callingClassType,
                mockName);
        }

        public void RunPsTestWorkflow(
            Func<string[]> scriptBuilder,
            Action<LegacyTest.CSMTestEnvironmentFactory> initialize,
            Action cleanup,
            string callingClassType,
            string mockName)
        {
            HttpMockServer.RecordsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SessionRecords");

            using (MockContext context = MockContext.Start(callingClassType, mockName))
            {

                _csmTestFactory = new LegacyTest.CSMTestEnvironmentFactory();

                initialize?.Invoke(_csmTestFactory);

                _helper.SetupEnvironment(AzureModule.AzureResourceManager);

                var callingClassName = callingClassType
                                        .Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                        .Last();
                _helper.SetupModules(AzureModule.AzureResourceManager,
                    _helper.RMProfileModule,
                    _helper.RMResourceModule,
                    _helper.RMManagedServiceIdentityModule,
                    "ScenarioTests\\" + callingClassName + ".ps1");

                try
                {
                    if (scriptBuilder != null)
                    {
                        var psScripts = scriptBuilder();

                        if (psScripts != null)
                        {
                            _helper.RunPowerShellTest(psScripts);
                        }
                    }
                }
                finally
                {
                    cleanup?.Invoke();
                }
            }
        }
    }
}
