Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check storage")>
Public Class CheckStorage
    Inherits CodeActivity

    <Category("Input/Output")>
    <DisplayName("Config dictionary")>
    <RequiredArgument()>
    Public Property Config As InOutArgument(Of Dictionary(Of String, String))

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
        REFramework.CheckStorage(Config.Get(context), Name.Get(context), IsRequired.Get(context))
    End Sub

End Class