Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check credentials")>
Public Class CheckCredentials
    Inherits CodeActivity

    <Category("Input/Output")>
    <DisplayName("Credentials dictionary")>
    <RequiredArgument()>
    Public Property Credentials As InOutArgument(Of Dictionary(Of String, Tuple(Of String, Security.SecureString)))

    <Category("Input")>
    <DisplayName("Name")>
    <RequiredArgument()>
    Public Property Name As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Is required")>
    <RequiredArgument()>
    <DefaultValue(True)>
    Public Property IsRequired As InArgument(Of Boolean)

    Protected Overrides Sub Execute(context As CodeActivityContext)
        REFramework.CheckCredentials(Credentials.Get(context), Name.Get(context), IsRequired.Get(context))
    End Sub

End Class