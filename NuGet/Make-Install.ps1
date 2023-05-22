# Configuration
$LIBDIR="lib\net6.0-windows"

$NUGET_FEED="$env:USERPROFILE\.nuget\packages"
$NUGET_EXE="NuGet.exe"

# Nettoyage
Remove-Item -Path *.nupkg -Force -ErrorAction Ignore
Remove-Item -Path lib -Recurse -Force -ErrorAction Ignore
mkdir $LIBDIR -Force > $null

# Détermination du nom du package
Get-ChildItem -Path "." -Filter "*.nuspec" | ForEach-Object {$NAME="$_" -replace ".nuspec", ""}

# Assemblage des fichiers à compiler
Copy-Item -Path ..\\$NAME*\\bin\\Debug\\*\\$NAME*.dll -Destination $LIBDIR -Force

# Compilation du NuPkg
& $NUGET_EXE Pack -Exclude "*.ps1" -Exclude "*\*\*.deps.json" > $null

# Récupération du nom du NuPkg
Get-ChildItem -Path "." -Filter "*.nupkg" | ForEach-Object {$NUPKG="$_" -replace ".nupkg", ""}

# Suppression de l'ancien paquet dans le feed NuGet s'il existe
$OUTDIR=-join("$NUGET_FEED\$NAME.Activities\", $NUPKG.replace("$NAME.Activities.",""))
Remove-Item -Path "$OUTDIR" -Recurse -Force -ErrorAction Ignore

# Déploiement du nouveau paquet
& $NUGET_EXE Add "$NUPKG.nupkg" -Source "$NUGET_FEED" -Expand

if ($?) { "Le déploiement a réussi." } else { "Le déploiement a échoué ; vérifiez qu'il ne reste pas de projet ouvert dans le Studio UiPath." }
