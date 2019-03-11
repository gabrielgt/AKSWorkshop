Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$aksName,
    [parameter(Mandatory=$false)][string]$baseName="my",
    [parameter(Mandatory=$false)][string]$imageTag="latest",
    [parameter(Mandatory=$true)][string]$acrName
)

$aks = $(az aks show -n $aksName -g $resourceGroup) | ConvertFrom-Json

if ($null -eq $aks) {
    Write-Host "AKS $aksName not found in RG: $resourceGroup" -ForegroundColor Red
    exit 1
}


$acr = $(az acr show -n $acrName -g $resourceGroup) | ConvertFrom-Json

if ($null -eq $acr) {
    Write-Host "ACR $acr not found in RG: $resourceGroup" -ForegroundColor Red
    exit 1
}

$hostName = $aks.addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
$acrLoginServer=$acr.loginServer

Write-Host "Running charts" -ForegroundColor Green

cmd /c "helm install -f gvalues.yaml -n $baseName-catalog  --set ingress.hosts={$hostName} --set ingress.tls[0].hosts={$hostName} --set image.repository=$acrLoginServer/catalog --set image.tag=$imageTag  catalog-api"
cmd /c "helm install -f gvalues.yaml -n $baseName-basket  --set ingress.hosts={$hostName} --set ingress.tls[0].hosts={$hostName} --set image.repository=$acrLoginServer/basket --set image.tag=$imageTag basket-api"
cmd /c "helm install -f gvalues.yaml -n $baseName-order  --set ingress.hosts={$hostName} --set ingress.tls[0].hosts={$hostName} --set image.repository=$acrLoginServer/order --set image.tag=$imageTag order-api"
cmd /c "helm install -f gvalues.yaml -n $baseName-website  --set ingress.hosts={$hostName} --set ingress.tls[0].hosts={$hostName} --set image.repository=$acrLoginServer/website --set image.tag=$imageTag website"

Write-Host "Charts deployed" -ForegroundColor Green