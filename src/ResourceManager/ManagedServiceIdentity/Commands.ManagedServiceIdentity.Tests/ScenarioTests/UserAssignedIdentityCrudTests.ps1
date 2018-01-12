<#
.SYNOPSIS
Tests Managed Service Identity UserAssignedIdentities CRUD.
#>
function Test-CrudUserAssignedIdentity
{
    $rgName1 = Get-RandomTestResourceName;
    $rgName2 = Get-RandomTestResourceName;
    $identityName1 = Get-RandomTestResourceName;
    $identityName2 = Get-RandomTestResourceName;
    $identityType = "Microsoft.ManagedIdentity/userAssignedIdentities";
    $locationWestUS = "westus";
    $locationCentralUS = "centralus";

    try
    {
        #Create Resource Group 1
        $rg1 = New-AzureRmResourceGroup -Name $rgName1 -Location $locationWestUS;
        #Create Resource Group 2
        $rg2 = New-AzureRmResourceGroup -Name $rgName2 -Location $locationWestUS;

        #Create Identity1 under Resource Group 1
        $identity1 = New-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1 -Name $identityName1;
        Assert-AreEqual $identity1.ResourceGroupName $rgName1
        Assert-AreEqual $identity1.Name $identityName1;
        Assert-AreEqual $identity1.Location $locationWestUS;
        Assert-AreEqual $identity1.Type $identityType;

        #Create Identity2 under Resource Group 2
        $identity2 = New-AzureRmUserAssignedIdentity -ResourceGroupName $rgName2 -Name $identityName2 -Location $locationCentralUS;
        Assert-AreEqual $identity2.ResourceGroupName $rgName2;
        Assert-AreEqual $identity2.Name $identityName2;
        Assert-AreEqual $identity2.Location $locationCentralUS;
        Assert-AreEqual $identity2.Type $identityType;

        #Get Identity 1
        $identity1 = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1 -Name $identityName1
        Assert-NotNull $identity1;
        Assert-AreEqual $identity1.ResourceGroupName $rgName1;
        Assert-AreEqual $identity1.Name $identityName1;
        Assert-AreEqual $identity1.Location $locationWestUS;
        Assert-AreEqual $identity1.Type $identityType;

        #Get identities under ResourceGroup 1
        $identities = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName1
        Assert-AreEqual $identities.Count 1
        Assert-AreEqual $identities[0].ResourceGroupName $rgName1;
        Assert-AreEqual $identities[0].Name $identityName1;
        Assert-AreEqual $identities[0].Location $locationWestUS;
        Assert-AreEqual $identities[0].Type $identityType;

        #Get identities under ResourceGroup 2
        $identities = Get-AzureRmUserAssignedIdentity -ResourceGroupName $rgName2
        Assert-AreEqual $identities.Count 1
        Assert-AreEqual $identities[0].ResourceGroupName $rgName2;
        Assert-AreEqual $identities[0].Name $identityName2;
        Assert-AreEqual $identities[0].Location $locationCentralUS;
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

function Get-RandomTestResourceName
{
    $prefix = "MSITestPS"

    $str = $prefix + ((Get-Random) % 10000);
    return $str;
}