using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Management.ManagedServiceIdentity;

namespace Microsoft.Azure.Commands.ManagedServiceIdentity.Common
{
    public class MsiClient
    {
        public IManagedServiceIdentityClient ManagedServiceIdentityClient { get; private set; }

        public MsiClient(IAzureContext context)
            : this(AzureSession.Instance.ClientFactory.CreateArmClient<ManagedServiceIdentityClient>(
                context, AzureEnvironment.Endpoint.ResourceManager))
        {
            
        }

        public MsiClient(IManagedServiceIdentityClient managedServiceIdentityClient)
        {
            ManagedServiceIdentityClient = managedServiceIdentityClient;
        }
    }
}
