Imports System.Activities
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq

<DisplayName("Transférer le dossier à un autre instructeur")>
Public Class TransférerDossier
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)
    
    <Category("Input")>
    <DisplayName("Instructeur")>
    <RequiredArgument()>
    Public Property Instructeur As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Dossier")>
    <RequiredArgument()>
    Public Property Dossier As InArgument(Of String)

    Protected Overrides Sub Execute(context As CodeActivityContext)
        DémarchesSimplifiées.TransférerDossier(
            Jeton.Get(context),
            Instructeur.Get(context),
            Dossier.Get(context)
        )
    End Sub

End Class