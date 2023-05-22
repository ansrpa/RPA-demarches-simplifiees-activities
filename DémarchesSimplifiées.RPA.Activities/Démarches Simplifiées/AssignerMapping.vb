Imports System.Activities
Imports System.ComponentModel

<DisplayName("Assigner une table de correspondance")>
Public Class AssignerMapping
    Inherits CodeActivity

    
    <Category("Input")>
    <DisplayName("Dossier")>
    <RequiredArgument()>
    Public Property Dossier As InArgument(Of Dossier)

    <Category("Input")>
    <DisplayName("Mapping")>
    <RequiredArgument()>
    Public Property Mapping As InArgument(Of Dictionary(Of String, String))
    
    Protected Overrides Sub Execute(context As CodeActivityContext)
        Dossier.Get(context).Mapping = Mapping.Get(context)
    End Sub

End Class