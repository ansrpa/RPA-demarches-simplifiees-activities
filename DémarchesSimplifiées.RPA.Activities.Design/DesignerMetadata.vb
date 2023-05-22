Imports System.Activities.Presentation.Metadata
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Globalization
Imports System.Reflection
Imports System.Windows.Data


Public Class EnumValueConverter
    Implements IValueConverter

    Private Shared Function GetDisplayName(EnumValue As [Enum]) As String
        Dim displayAttribute = EnumValue.GetType.GetMember([Enum].GetName(EnumValue.GetType, EnumValue)).First.GetCustomAttribute(Of DisplayAttribute)
        If displayAttribute IsNot Nothing Then
            Return displayAttribute.Name
        End If
        Return EnumValue.ToString
    End Function

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim enumValue as [Enum] = value
        If enumValue IsNot Nothing Then
            Return GetDisplayName(enumValue)
        End If
        Return value.ToString
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException() 'Return value
    End Function
End Class

Public Class DesignerMetadata
    Implements IRegisterMetadata
    
    Dim CatégorieDS = "Démarches_Simplifiées"
    Dim CatégorieREF = "Démarches_Simplifiées.REFramework"
    Public Sub Register() Implements IRegisterMetadata.Register
        Dim builder = New AttributeTableBuilder()
        builder.AddCustomAttributes(GetType(ChangerStatut), New CategoryAttribute(CatégorieDS))
        builder.AddCustomAttributes(GetType(ChangerStatut), New DesignerAttribute(GetType(ChangerStatutDesigner)))
        
        builder.AddCustomAttributes(GetType(TransférerDossier), New CategoryAttribute(CatégorieDS))
        builder.AddCustomAttributes(GetType(TransférerDossier), New DesignerAttribute(GetType(TransférerDossierDesigner)))

        builder.AddCustomAttributes(GetType(ChargerDossier), New CategoryAttribute(CatégorieDS))
        builder.AddCustomAttributes(GetType(ChargerDossier), New DesignerAttribute(GetType(ChargerDossierDesigner)))
        
        builder.AddCustomAttributes(GetType(RécupérerDossiers), New CategoryAttribute(CatégorieDS))
        builder.AddCustomAttributes(GetType(RécupérerDossiers), New DesignerAttribute(GetType(RécupérerDossiersDesigner)))
        
        builder.AddCustomAttributes(GetType(AssignerMapping), New CategoryAttribute(CatégorieDS))


        builder.AddCustomAttributes(GetType(CheckBoolean), New CategoryAttribute(CatégorieREF))
        builder.AddCustomAttributes(GetType(CheckCredentials), New CategoryAttribute(CatégorieREF))
        builder.AddCustomAttributes(GetType(CheckFile), New CategoryAttribute(CatégorieREF))
        builder.AddCustomAttributes(GetType(CheckFolder), New CategoryAttribute(CatégorieREF))
        builder.AddCustomAttributes(GetType(CheckInt), New CategoryAttribute(CatégorieREF))
        builder.AddCustomAttributes(GetType(CheckMailAddress), New CategoryAttribute(CatégorieREF))
        builder.AddCustomAttributes(GetType(CheckMailAddressCollection), New CategoryAttribute(CatégorieREF))
        builder.AddCustomAttributes(GetType(CheckStorage), New CategoryAttribute(CatégorieREF))
        builder.AddCustomAttributes(GetType(CheckString), New CategoryAttribute(CatégorieREF))
        
        MetadataStore.AddAttributeTable(builder.CreateTable())

    End Sub
End Class
