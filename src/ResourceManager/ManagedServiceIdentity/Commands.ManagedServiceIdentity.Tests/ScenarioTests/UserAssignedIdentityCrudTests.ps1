<#
.SYNOPSIS
Tests Managed Service Identity UserAssignedIdentities CRUD.
#>
function Test-CrudUserAssignedIdentity
{
    $rgName1 = "MSIPSTestRG1";
    $rgName2 = "MSIPSTestRG2";
    $identityType = "Microsoft.ManagedIdentity/userAssignedIdentities"
    $firstTagKey = "tag1";
    $firstTagValue = "firstTag";
    $secondTagKey = "tag2";
    $secondTagValue = "secondTag";
    $subscriptionId = "fa5fc227-a624-475e-b696-cdd604c735bc";

    try
    {
        $identityName1 = "MSIPSTestIdentity1";
        $identityName2 = "MSIPSTestIdentity2";
        $loc = "westus";

        #Create Resource Group 1
        $rg1 = New-AzureRmResourceGroup -Name $rgName1 -Location $loc;
        #Create Resource Group 2
        $rg2 = New-AzureRmResourceGroup -Name $rgName2 -Location $loc;

        #Create Identity1 under Resource Group 1
        $identity1 = New-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1 -Name $identityName1 -Location $loc;
        $expectedIdentityId1 = Get-ExpectedIdentityId -subscription $subscriptionId -rg $rgName1 -identity $identityName1;
        Assert-NotNull $identity1;
        Assert-AreEqual $identity1.ID $expectedIdentityId1;
        Assert-AreEqual $identity1.Name $identityName1;
        Assert-AreEqual $identity1.Location $loc;
        Assert-AreEqual $identity1.Type $identityType;

        #Create Identity2 under Resource Group 2
        $identity2 = New-AzureRmUserAssignedIdentity -ResourceGroupName $rgName2 -Name $identityName2 -Location $loc;
        $expectedIdentityId2 = Get-ExpectedIdentityId -subscription $subscriptionId -rg $rgName2 -identity $identityName2;
        Assert-NotNull $identity2;
        Assert-AreEqual $identity2.ID $expectedIdentityId2;
        Assert-AreEqual $identity2.Name $identityName2;
        Assert-AreEqual $identity2.Location $loc;
        Assert-AreEqual $identity2.Type $identityType;

        #Update Identity 1
        $identity1 = Set-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1 -Name $identityName1 -Tag @{$firstTagKey=$firstTagValue; $secondTagKey=$secondTagValue};
        Assert-NotNull $identity1;
        Assert-AreEqual $identity1.ID $expectedIdentityId1;
        Assert-AreEqual $identity1.Name $identityName1;
        Assert-AreEqual $identity1.Location $loc;
        Assert-AreEqual $identity1.Type $identityType;
        Assert-AreEqual $identity1.Tags.Count 2;
        Assert-AreEqual $identity1.Tags.ContainsKey($firstTagKey) True;
        Assert-AreEqual $identity1.Tags[$firstTagKey] $firstTagValue;
        Assert-AreEqual $identity1.Tags.ContainsKey($secondTagKey) True;
        Assert-AreEqual $identity1.Tags[$secondTagKey] $secondTagValue;

        #Get Identity 1
        $identity1 = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1 -Name $identityName1
        Assert-NotNull $identity1;
        Assert-AreEqual $identity1.ID $expectedIdentityId1;
        Assert-AreEqual $identity1.Name $identityName1;
        Assert-AreEqual $identity1.Location $loc;
        Assert-AreEqual $identity1.Type $identityType;
        Assert-AreEqual $identity1.Tags.Count 2;
        Assert-AreEqual $identity1.Tags.ContainsKey($firstTagKey) True;
        Assert-AreEqual $identity1.Tags[$firstTagKey] $firstTagValue;
        Assert-AreEqual $identity1.Tags.ContainsKey($secondTagKey) True;
        Assert-AreEqual $identity1.Tags[$secondTagKey] $secondTagValue;

        #Get identities under ResourceGroup 1
        $identities = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1
        Assert-AreEqual $identities.Count 1
        Assert-NotNull $identities[0];
        Assert-AreEqual $identities[0].ID $expectedIdentityId1;
        Assert-AreEqual $identities[0].Name $identityName1;
        Assert-AreEqual $identities[0].Location $loc;
        Assert-AreEqual $identities[0].Type $identityType;
        Assert-AreEqual $identities[0].Tags.Count 2;
        Assert-AreEqual $identities[0].Tags.ContainsKey($firstTagKey) True;
        Assert-AreEqual $identities[0].Tags[$firstTagKey] $firstTagValue;
        Assert-AreEqual $identities[0].Tags.ContainsKey($secondTagKey) True;
        Assert-AreEqual $identities[0].Tags[$secondTagKey] $secondTagValue;

        #Get identities under ResourceGroup 2
        $identities = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName2
        Assert-AreEqual $identities.Count 1
        Assert-NotNull $identities[0];
        Assert-AreEqual $identities[0].ID $expectedIdentityId2;
        Assert-AreEqual $identities[0].Name $identityName2;
        Assert-AreEqual $identities[0].Location $loc;
        Assert-AreEqual $identities[0].Type $identityType;

        #Delete Identity
        Remove-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1 -Name $identityName1;
        $resourceGroupIdentities = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1
        Assert-Null $resourceGroupIdentities;
    }
    finally
    {
        Clean-ResourceGroup $rgname1;
        Clean-ResourceGroup $rgname2;
    }
}

<#
.SYNOPSIS
Cleans the created resource groups
#>
function Clean-ResourceGroup($rgname)
{
    if ([Microsoft.Azure.Test.HttpRecorder.HttpMockServer]::Mode -ne [Microsoft.Azure.Test.HttpRecorder.HttpRecorderMode]::Playback)
    {
        Remove-AzureRmResourceGroup -Name $rgname -Force
    }
}

function Get-ExpectedIdentityId($subscription, $rg, $identity)
{
    return "/subscriptions/"+$subscription+"/resourcegroups/"+$rg+"/providers/Microsoft.ManagedIdentity/userAssignedIdentities/"+$identity;
}