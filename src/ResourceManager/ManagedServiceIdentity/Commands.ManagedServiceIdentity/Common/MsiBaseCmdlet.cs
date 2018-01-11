using System;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.ResourceManager.Common;
using Microsoft.Azure.Management.Internal.Resources;

namespace Microsoft.Azure.Commands.ManagedServiceIdentity.Common
{
    public class MsiBaseCmdlet : AzureRMCmdlet
    {
        private MsiClient _msiClient;
        private ResourceManagementClient _armClient;

        public MsiClient MsiClient
        {
            get { return _msiClient ?? (_msiClient = new MsiClient(DefaultProfile.DefaultContext)); }

            set { _msiClient = value; }
        }

        public ResourceManagementClient ArmClient
        {
            get
            {
                if (this._armClient == null)
                {
                    this._armClient = AzureSession.Instance.ClientFactory.CreateArmClient<ResourceManagementClient>(
                        context: this.DefaultContext,
                        endpoint: AzureEnvironment.Endpoint.ResourceManager);
                }
                return this._armClient;
            }
            set
            {
                this._armClient = value;
            }
        }

        protected void ExecuteClientAction(Action action)
        {
            try
            {
                action();
            }
            catch (Rest.Azure.CloudException)
            {
                try
                {
                    base.EndProcessing();
                }
                catch
                {
                    // Ignore exceptions during end processing
                }

                throw;
            }
        }
    }
}
