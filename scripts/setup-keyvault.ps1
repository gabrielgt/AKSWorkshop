Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$aksName,
    [parameter(Mandatory=$true)][string]$keyvaultName,
    [parameter(Mandatory=$true)][string]$clientId,
    [parameter(Mandatory=$true)][string]$clientPassword
)
  
$azAccount = $(az account show) | ConvertFrom-Json
$subsId = $azAccount.id
Write-Host "Using subscription $subsId" -ForegroundColor Green


$kv = $(az keyvault show -n $keyvaultName -g $resourceGroup) | ConvertFrom-Json

if ($null -eq $kv) {
    Write-Host "Creating KeyVault $keyvaultName" -ForegroundColor Green
    az keyvault create -g $resourceGroup -n $keyvaultName
}
else {
    Write-Host "$keyvaultName already exists. Skipping creation" -ForegroundColor Green
}

Write-Host "Assigning permissions to keyvault  $keyvaultName" -ForegroundColor Green
az role assignment create --role Reader --assignee $clientId --scope /subscriptions/$subsId/resourcegroups/$resourceGroup/providers/Microsoft.KeyVault/vaults/$keyvaultName
az keyvault set-policy -n $keyvaultName --key-permissions get --spn $clientId
az keyvault set-policy -n $keyvaultName --secret-permissions get --spn $clientId
az keyvault set-policy -n $keyvaultName --certificate-permissions get --spn $clientId
Write-Host "Assigned permissions to $clientId for keyvault $keyvaultName" -ForegroundColor Green

Write-Host "Installing Flexvol on AKS" -ForegroundColor Green
kubectl create -f https://raw.githubusercontent.com/Azure/kubernetes-keyvault-flexvol/master/deployment/kv-flexvol-installer.yaml

Write-Host "Add secret to cluster for accessing kv using clientid $clientId with pwd $clientPassword"
kubectl create secret generic keyvaultreader --from-literal clientid=$clientId --from-literal clientsecret=$clientPassword --type=azure/kv