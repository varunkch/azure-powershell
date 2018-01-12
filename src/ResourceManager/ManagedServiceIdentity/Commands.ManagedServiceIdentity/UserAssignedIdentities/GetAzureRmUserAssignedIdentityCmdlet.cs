using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.ManagedServiceIdentity.Common;
using Microsoft.Azure.Commands.ManagedServiceIdentity.Models;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;

namespace Microsoft.Azure.Commands.ManagedServiceIdentity.UserAssignedIdentities
{
    [Cmdlet(VerbsCommon.Get, "AzureRmUserAssignedIdentity")]
    [OutputType(typeof (PsUserAssignedIdentity))]
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
                        this.MsiClient.UserAssignedIdentities
                            .ListBySubscriptionWithHttpMessagesAsync().GetAwaiter().GetResult();
                    var resultList = result.Body.ToList();
                    var nextPageLink = result.Body.NextPageLink;
                    while (!string.IsNullOrEmpty(nextPageLink))
                    {
                        var pageResult =
                            this.MsiClient.UserAssignedIdentities
                                .ListBySubscriptionNextWithHttpMessagesAsync(nextPageLink).GetAwaiter().GetResult();
                        resultList.AddRange(pageResult.Body.ToList());
                        nextPageLink = pageResult.Body.NextPageLink;
                    }

                    WriteIdentityList(resultList);
                }
                else if (string.IsNullOrEmpty(this.Name))
                {
                    var result =
                        this.MsiClient.UserAssignedIdentities
                            .ListByResourceGroupWithHttpMessagesAsync(this.ResourceGroupName).GetAwaiter().GetResult();
                    var resultList = result.Body.ToList();
                    var nextPageLink = result.Body.NextPageLink;
                    while (!string.IsNullOrEmpty(nextPageLink))
                    {
                        var pageResult = this.MsiClient.UserAssignedIdentities
                            .ListByResourceGroupNextWithHttpMessagesAsync(nextPageLink).GetAwaiter().GetResult();
                        resultList.AddRange(pageResult.Body.ToList());
                        nextPageLink = pageResult.Body.NextPageLink;
                    }

                    WriteIdentityList(resultList);
                }
                else
                {
                    var result =
                        this.MsiClient.UserAssignedIdentities.GetWithHttpMessagesAsync(
                            this.ResourceGroupName,
                            this.Name).GetAwaiter().GetResult();
                    WriteIdentity(result.Body);
                }
            });
        }
    }
}
