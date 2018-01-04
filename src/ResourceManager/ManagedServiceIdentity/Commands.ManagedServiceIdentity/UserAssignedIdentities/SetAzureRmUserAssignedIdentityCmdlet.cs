using System.Collections;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.ManagedServiceIdentity.Common;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.ManagedServiceIdentity.Models;

namespace Microsoft.Azure.Commands.ManagedServiceIdentity.UserAssignedIdentities
{
    [Cmdlet(VerbsCommon.Set, "AzureRmUserAssignedIdentity")]
    [OutputType(typeof(Identity))]
    public class SetAzureRmUserAssignedIdentityCmdlet : MsiBaseCmdlet
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
            HelpMessage = "The Azure Resource Manager tags associated with the identity.")]
        public Hashtable Tag;

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            ExecuteClientAction(() =>
            {
                var tagsDictionary = this.Tag?.Cast<DictionaryEntry>().ToDictionary(ht => (string)ht.Key, ht => (string)ht.Value);
                Identity identityProperties = new Identity(tags: tagsDictionary);
                var result = this.MsiClient.ManagedServiceIdentityClient.UserAssignedIdentities.UpdateWithHttpMessagesAsync(
                    this.ResourceGroupName,
                    this.Name,
                    identityProperties).GetAwaiter().GetResult();

                WriteObject(result.Body);
            });
        }
    }
}
