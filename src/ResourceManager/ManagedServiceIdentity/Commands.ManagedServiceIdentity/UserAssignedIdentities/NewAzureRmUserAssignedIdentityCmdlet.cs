using System.Collections;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.ManagedServiceIdentity.Common;
using Microsoft.Azure.Commands.ManagedServiceIdentity.Models;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Internal.Resources;
using Microsoft.Azure.Management.Internal.Resources.Models;
using Identity = Microsoft.Azure.Management.ManagedServiceIdentity.Models.Identity;

namespace Microsoft.Azure.Commands.ManagedServiceIdentity.UserAssignedIdentities
{
    [Cmdlet(VerbsCommon.New, "AzureRmUserAssignedIdentity", SupportsShouldProcess = true)]
    [OutputType(typeof(PsUserAssignedIdentity))]
    public class NewAzureRmUserAssignedIdentityCmdlet : MsiBaseCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "The resource group name.")]
        [ResourceGroupCompleter()]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "The Identity name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "The Azure region name where the Identity should be created.")]
        [LocationCompleter()]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 3,
            HelpMessage = "The Azure Resource Manager tags associated with the identity.")]
        public Hashtable Tag;

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            ExecuteClientAction(() =>
            {
                if (this.ShouldProcess(Name, VerbsCommon.New))
                {
                    var tagsDictionary = this.Tag?.Cast<DictionaryEntry>()
                        .ToDictionary(ht => (string) ht.Key, ht => (string) ht.Value);
                    var location = GetLocation();
                    Identity identityProperties = new Identity(location: location, tags: tagsDictionary);
                    var result =
                        this.MsiClient.UserAssignedIdentities
                            .CreateOrUpdateWithHttpMessagesAsync(
                                this.ResourceGroupName,
                                this.Name,
                                identityProperties).GetAwaiter().GetResult();

                    WriteIdentity(result.Body);
                }
            });
        }

        private string GetLocation()
        {
            return this.Location ?? GetResourceGroupLocation(this.ResourceGroupName);
        }

        private string GetResourceGroupLocation(string resourceGroupName)
        {
            ResourceGroup rg = ArmClient.ResourceGroups.Get(resourceGroupName);
            return rg.Location;
        }
    }
}
