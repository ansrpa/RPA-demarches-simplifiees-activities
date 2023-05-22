Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check int")>
Public Class CheckInt
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
    <DisplayName("Minimum value")>
    Public Property MinValue As InArgument(Of Nullable(Of Int32))

    <Category("Input")>
    <DisplayName("Maximum value")>
    Public Property MaxValue As InArgument(Of Nullable(Of Int32))


    Protected Overrides Sub Execute(context As CodeActivityContext)
        REFramework.CheckInt(Config.Get(context), Name.Get(context), IsRequired.Get(context), MinValue.Get(context), MaxValue.Get(context))
    End Sub

End Class