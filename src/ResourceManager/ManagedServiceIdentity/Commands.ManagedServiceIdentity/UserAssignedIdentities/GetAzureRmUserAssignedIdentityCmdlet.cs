using System.Management.Automation;
using Microsoft.Azure.Commands.ManagedServiceIdentity.Common;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.ManagedServiceIdentity.Models;

namespace Microsoft.Azure.Commands.ManagedServiceIdentity.UserAssignedIdentities
{
    [Cmdlet(VerbsCommon.Get, "AzureRmUserAssignedIdentity")]
    [OutputType(typeof(Identity))]
    public class GetAzureRmUserAssignedIdentityCmdlet : MsiBaseCmdlet
    {
        [Parameter(
            Mandatory = false,
            Position = 0,
            HelpMessage = "The resource group name.")]
        [ResourceGroupCompleter()]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 1,
            HelpMessage = "The Identity name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            ExecuteClientAction(() =>
            {
                if (string.IsNullOrEmpty(this.ResourceGroupName))
                {
                    var result =
                        this.MsiClient.ManagedServiceIdentityClient.UserAssignedIdentities
                            .ListBySubscriptionWithHttpMessagesAsync().GetAwaiter().GetResult();

                    WriteObject(result.Body);
                }
                else if (string.IsNullOrEmpty(this.Name))
                {
                    var result =
                        this.MsiClient.ManagedServiceIdentityClient.UserAssignedIdentities
                            .ListByResourceGroupWithHttpMessagesAsync(
                                this.ResourceGroupName).GetAwaiter().GetResult();

                    WriteObject(result.Body);
                }
                else
                {
                    var result = this.MsiClient.ManagedServiceIdentityClient.UserAssignedIdentities.GetWithHttpMessagesAsync(
                        this.ResourceGroupName,
                        this.Name).GetAwaiter().GetResult();

                    WriteObject(result.Body);
                }
            });
        }
    }
}
