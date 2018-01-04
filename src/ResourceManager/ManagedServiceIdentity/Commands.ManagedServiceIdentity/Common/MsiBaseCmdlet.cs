using System;
using Microsoft.Azure.Commands.ResourceManager.Common;

namespace Microsoft.Azure.Commands.ManagedServiceIdentity.Common
{
    public class MsiBaseCmdlet : AzureRMCmdlet
    {
        private MsiClient _msiClient;
        public MsiClient MsiClient
        {
            get { return _msiClient ?? (_msiClient = new MsiClient(DefaultProfile.DefaultContext)); }

            set { _msiClient = value; }
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
