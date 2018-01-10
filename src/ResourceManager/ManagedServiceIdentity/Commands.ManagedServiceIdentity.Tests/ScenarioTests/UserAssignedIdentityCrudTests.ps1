<#
.SYNOPSIS
Tests Managed Service Identity UserAssignedIdentities CRUD.
#>
function Test-CrudUserAssignedIdentity
{
    $rgName = "MSIPSTestRG";
    $identityType = "Microsoft.ManagedIdentity/userAssignedIdentities"
    $firstTagKey = "tag1";
    $firstTagValue = "firstTag";
    $secondTagKey = "tag2";
    $secondTagValue = "secondTag";

    try
    {
        $identityName = "MSIPSTestIdentity";
        $loc = "westus";
        $expectedIdentityID = "/subscriptions/fa5fc227-a624-475e-b696-cdd604c735bc/resourcegroups/"+$rgName+"/providers/Microsoft.ManagedIdentity/userAssignedIdentities/"+$identityName
        #$expectedIdentityID | Out-File "C:\Users\vakuma\Desktop\resultID2.txt";
        #Create new Resource Group
        $rg = New-AzureRmResourceGroup -Name $rgName -Location $loc;

        #Create new Identity
        $identity = New-AzureRmUserAssignedIdentity -ResourceGroupName $rgName -Name $identityName -Location $loc;
        Assert-NotNull $identity;
        Assert-AreEqual $identity.ID $expectedIdentityID;
        Assert-AreEqual $identity.Name $identityName;
        Assert-AreEqual $identity.Location $loc;
        Assert-AreEqual $identity.Type $identityType;
        #$identity | Out-File "C:\Users\vakuma\Desktop\resultID.txt";

        #Update Identity
        $identity = Set-AzureRmUserAssignedIdentity -ResourceGroupName $rgName -Name $identityName -Tag @{$firstTagKey=$firstTagValue; $secondTagKey=$secondTagValue};
        $identity | Out-File "C:\Users\vakuma\Desktop\resultID.txt";
        Assert-NotNull $identity;
        Assert-AreEqual $identity.ID $expectedIdentityID;
        Assert-AreEqual $identity.Name $identityName;
        Assert-AreEqual $identity.Location $loc;
        Assert-AreEqual $identity.Type $identityType;
        Assert-AreEqual $identity.Tags.Count 2;
        Assert-AreEqual $identity.Tags.ContainsKey($firstTagKey) True;
        Assert-AreEqual $identity.Tags[$firstTagKey] $firstTagValue;
        Assert-AreEqual $identity.Tags.ContainsKey($secondTagKey) True;
        Assert-AreEqual $identity.Tags[$secondTagKey] $secondTagValue;

        #Get Identity
        $identity = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName -Name $identityName
        Assert-NotNull $identity;
        Assert-AreEqual $identity.ID $expectedIdentityID;
        Assert-AreEqual $identity.Name $identityName;
        Assert-AreEqual $identity.Location $loc;
        Assert-AreEqual $identity.Type $identityType;
        Assert-AreEqual $identity.Tags.Count 2;
        Assert-AreEqual $identity.Tags.ContainsKey($firstTagKey) True;
        Assert-AreEqual $identity.Tags[$firstTagKey] $firstTagValue;
        Assert-AreEqual $identity.Tags.ContainsKey($secondTagKey) True;
        Assert-AreEqual $identity.Tags[$secondTagKey] $secondTagValue;

        #Delete Identity
        Remove-AzureRmUserAssignedIdentity -ResourceGroupName $rgName -Name $identityName;
        $resourceGroupIdentities = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName
        Assert-Null $resourceGroupIdentities;
    }
    finally
    {
        Clean-ResourceGroup $rgname;
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