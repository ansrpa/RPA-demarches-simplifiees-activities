Imports System.Activities
Imports System.ComponentModel

<DisplayName("Charger un dossier avec ses pièces jointes")>
Public Class ChargerDossier
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)
    
    '<OverloadGroup("dossierInput1")>
    <Category("Input")>
    <DisplayName("Numéro de dossier")>
    <RequiredArgument()>
    Public Property NuméroDossier As InArgument(Of Integer)
    
    '<Category("Input")>
    '<DisplayName("Identifiant de dossier")>
    '<OverloadGroup("dossierInput2")>
    'Public Property IdentifiantDossier As InArgument(Of String) = Nothing

    <Category("Output")>
    <DisplayName("Dossier récupéré")>
    <RequiredArgument()>
    Public Property Dossier As OutArgument(Of Dossier)


    Protected Overrides Sub Execute(context As CodeActivityContext)
        'If IdentifiantDossier IsNot Nothing Then
        '    Throw New Exception("La recherche de dossier par ID n'est pas encore implémentée.")
        'End If
        Dossier.Set(context, LireDossierParNuméro(Jeton.Get(context), NuméroDossier.Get(context)))
    End Sub

End Class