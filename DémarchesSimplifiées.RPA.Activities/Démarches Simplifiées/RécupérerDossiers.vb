Imports System.Activities
Imports System.ComponentModel

<DisplayName("Récupérer des dossiers")>
Public Class RécupérerDossiers
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Numéro de démarche")>
    <RequiredArgument()>
    Public Property Démarche As InArgument(Of Integer)

    <Category("Input")>
    <DisplayName("Nombre maximum de dossiers")>
    <RequiredArgument()>
    Public Property Limite As InArgument(Of Integer) = 1

    <Category("Input")>
    <DisplayName("Ordre")>
    <RequiredArgument()>
    Public Property Ordre As Ordre = Ordre.ASC

    <Category("Input")>
    <DisplayName("Statut")>
    <RequiredArgument()>
    Public Property Statut As Statut = Statut.en_construction
    
    <Category("Output")>
    <DisplayName("Dossiers récupérés")>
    <RequiredArgument()>
    Public Property Dossier As OutArgument(Of IEnumerable(Of Dossier))

    <Category("Input")>
    <DisplayName("Après le")>
    Public Property After As InArgument(Of DateTime) = Nothing

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Dossier.Set(context, DémarchesSimplifiées.RécupérerDossiers(Jeton.Get(context), Démarche.Get(context), Limite.Get(context), Ordre, Statut, After.Get(context)))
    End Sub

End Class