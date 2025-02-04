<#
.SYNOPSIS
Builds and runs the Q-Pilot frontend.
#>

Join-Path "$PSScriptRoot" '../webapp' | Set-Location
yarn install
yarn start
