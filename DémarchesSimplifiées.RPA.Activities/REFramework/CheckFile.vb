Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check file")>
Public Class CheckFile
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

    <Category("Input")>
    <DisplayName("Must exist")>
    <RequiredArgument()>
    <DefaultValue(False)>
    Public Property MustExist As InArgument(Of Boolean)

    Protected Overrides Sub Execute(context As CodeActivityContext)
        REFramework.CheckFile(Config.Get(context), Name.Get(context), IsRequired.Get(context), MustExist.Get(context))
    End Sub

End Class