Imports System.ComponentModel.DataAnnotations
Imports System.Data
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.Http.Json
Imports System.Text
Imports Newtonsoft.Json.Linq

Public Enum Statut
    <Display(Name:="En construction")>
    en_construction
    <Display(Name:="En instruction")>
    en_instruction
    <Display(Name:="Accepté")>
    accepte
    <Display(Name:="Refusé")>
    refuse
    <Display(Name:="Sans suite")>
    sans_suite
End Enum

Public Enum Ordre
    <Display(Name:="Les plus récents")>
    DESC
    <Display(Name:="Les plus anciens")>
    ASC
End Enum


Public MustInherit Class DossierException
    Inherits Exception

    Public MustOverride ReadOnly Property Type As String
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class

Public Class RetournerDossierException
    Inherits DossierException
    Public Overrides ReadOnly Property Type As String = "RetournerDossier"
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class

Public Class RefuserDossierException
    Inherits DossierException
    Public Overrides ReadOnly Property Type As String = "RefuserDossier"
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class

Public Class TransférerDossierException
    Inherits DossierException
    Public Overrides ReadOnly Property Type As String = "TransférerDossier"
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class


Public Class Dossier
    Inherits JObject

    Public ids As IEnumerable(Of String)
    Private _mapping As Dictionary(Of String, String)

    Public Property Mapping() As Dictionary(Of String, String)
        Get
            Return _mapping
        End Get
        Set(value As Dictionary(Of String, String))
            For Each id In value.Values
                If Not ids.Contains(id) Then
                    Throw New Exception($"Erreur de mapping: le dossier n'a pas de champ {id}.")
                End If
            Next
            _mapping = value
        End Set
    End Property

    Public Sub New(other As JObject)
        MyBase.New(other)
        If Not ContainsKey("champs") Then
            Throw New Exception("Erreur lors de la conversion du dossier: la liste des champs n'a pas été trouvée.")
        End If
        ids = From field As JToken In Item("champs") Let s = field("id").ToString Select s
    End Sub


    Function Champ(Nom As String) As JToken
        If Mapping Is Nothing Then
            Throw New Exception("Une table de correspondance des noms n'a pas été fournie pour ce dossier.")
        ElseIf Not Mapping.ContainsKey(Nom) Then
            Throw New Exception($"Pas de correspondance établie pour le champ nommé ""{Nom}"".")
        End If
        For Each field As JObject In Item("champs")
            If field("id").ToString.Equals(Mapping(Nom)) Then
                Return field
            End If
        Next
        Throw New Exception($"Le champ ""{Nom}"" n'a pas été trouvé pour ce dossier.")
    End Function

    Shared Function Normaliser(InputString As String) As String
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
        Dim tempBytes = Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(InputString.ToUpper)
        Return Text.Encoding.UTF8.GetString(tempBytes).Replace("'"," ")
    End Function

    Function ChampNormalisé(Nom As String) As String
        Return Dossier.Normaliser(Champ(Nom).Value(Of String)("stringValue"))
    End Function

    Function ChampParId(Id As String) As JToken
        For Each field As JObject In Item("champs")
            If field("id").ToString.Equals(Id) Then
                Return field
            End If
        Next
        Throw New Exception($"Le champ identifié ""{Id}"" n'a pas été trouvé pour ce dossier.")
    End Function

    Function ChampParLabel(Label As String) As JToken
        For Each field As JObject In Item("champs")
            If field("label").ToString.Equals(Label) Then
                Return field
            End If
        Next
        Throw New Exception($"Le champ labellisé ""{Label}"" n'a pas été trouvé pour ce dossier.")
    End Function

End Class
Public Module DémarchesSimplifiées

    Dim client As HttpClient = New HttpClient With {
        .BaseAddress = New Uri("https://www.demarches-simplifiees.fr/api/v2/graphql")
    }

    Private Function Requêter(Jeton As String, Requête As Object) As JObject

        ' Authentification
        client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", Jeton)

        ' Envoi de la requête
        Dim responseRaw = client.PostAsJsonAsync("", Requête).Result

        Dim r = responseRaw.Content.ReadAsStringAsync.Result
        

        Dim response As JObject
        ' Récupération de la réponse (synchronisée)
        response = JObject.Parse(r)


        ' Si elle est présente, la propriété "data" devient la racine de l'objet réponse car elle renferme le contenu le plus intéressant
        If response.ContainsKey("data") AndAlso response("data").ToObject(Of Object) IsNot Nothing Then
            response = response("data").ToObject(Of JObject)
        End If

        ' Mise en forme des erreurs si elles sont présentes
        Dim errors = New List(Of String)
        If response.ContainsKey("errors") AndAlso response("errors").ToObject(Of Object) IsNot Nothing Then
            errors.Add(String.Join(" ; ", From e As JObject In response("errors") Select e("message")))
        Else
            For Each item In response
                Dim value = item.Value.ToObject(Of JObject)
                If value.ContainsKey("errors") AndAlso value("errors").ToObject(Of Object) IsNot Nothing Then
                    errors.Add(item.Key + " : " + String.Join(" ; ", From e As JObject In value("errors") Select """" + e("message").ToString + """"))
                End If
            Next
        End If

        ' S'il y a des erreurs, une exception est levée
        If errors.Count > 0 Then
            Dim e = New Exception(String.Join(vbCrLf, errors))
            e.Data("response") = response
            Throw e
        End If

        ' Renvoi de la réponse
        Return response
    End Function

    Function LireDossierParNuméro(Jeton As String, Numéro As Integer) As Dossier

        Dim jDossier As JObject = Requêter(Jeton, New With {
            .query = My.Resources.getDossier,
            .variables = New With {
                .dossierNumber = Numéro
            }
          })("dossier")


        If jDossier IsNot Nothing
            Dim dossier = New Dossier(jDossier) ' Dans le cas où la fonction Requêter() retourne,
            For Each field As JObject In dossier("champs")
                If field.ContainsKey("files") Then
                    For Each file As JObject In field("files")
                        If file.ContainsKey("filename") And file.ContainsKey("url") And Not String.IsNullOrEmpty(file.Value(Of String)("url")) Then
                            Dim client = New HttpClient()
                            file("filebytes") = client.GetAsync(file.Value(Of String)("url")).Result.Content.ReadAsByteArrayAsync.Result
                        End If
                    Next
                End If
            Next
            Return dossier
        End If

        Throw New Exception($"Impossible de récupérer le dossier {Numéro}.")
        ' c'est qu'il n'y a pas d'exception et qu'une propriété "dossier" est présente a minima
    End Function

    Function RécupérerDossiers(Jeton As String, Démarche As Integer, Optional Limite As Integer = 1, Optional Ordre As Ordre = Ordre.ASC, Optional Statut As Statut = Statut.en_construction, Optional After As DateTime = Nothing) As IEnumerable(Of Dossier)
        Dim nextPage = True
        Dim cursor As String = Nothing

        Dim dossiers = New List(Of Dossier)
        
        If After.Equals(Nothing):
            After = DateTime.MinValue
        End If

        While nextPage
            Dim requête = New With {
                .query = My.Resources.getDemarche,
                .variables = If(cursor Is Nothing,
                New With {
                    .demarcheNumber = Démarche,
                    .order = Ordre.GetName(Ordre),
                    .state = Statut.GetName(Statut),
                    .updatedSince = After.ToString("O")
                },
                New With {
                    .demarcheNumber = Démarche,
                    .order = Ordre.GetName(Ordre),
                    .state = Statut.GetName(Statut),
                    .updatedSince = After.ToString("O"),
                    .after = cursor
                })
            }

            Dim response As JObject
            While True
                Try
                    response = Requêter(Jeton, requête)
                    Exit While
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    throw
                End Try
            End While

            cursor = response("demarche")("dossiers")("pageInfo")("endCursor")
            nextPage = response("demarche")("dossiers")("pageInfo")("hasNextPage")

            
            For Each jDossier As Object In response("demarche")("dossiers")("nodes")
                If jDossier IsNot Nothing Then
                    dossiers.Add(New Dossier(jDossier))
                    If dossiers.Count.Equals(Limite) Then
                        Exit While
                    End If
                End If
            Next
            
        End While
        
        Return dossiers

    End Function

    Function ChangerStatut(Jeton As String, InstructeurId As String, DossierId As String, Statut As Statut, Optional Motivation As String = Nothing, Optional Notifier As Boolean = True) As JObject

        ' Assemblage des paramètres...
        Dim query
        Dim disableNotification = Not Notifier
        Dim input As Object = New With {
            instructeurId,
            dossierId,
            motivation,
            disableNotification
        }

        ' ...en fonction du type de changement
        Select Case Statut
            Case Statut.en_construction
                query = My.Resources.dossierRepasserEnConstruction
                input = New With {instructeurId, dossierId}
            Case Statut.en_instruction
                query = My.Resources.dossierPasserEnInstruction
                input = New With {instructeurId, dossierId}
            Case Statut.accepte
                query = My.Resources.dossierAccepter
            Case Statut.refuse
                query = My.Resources.dossierRefuser
            Case Statut.sans_suite
                query = My.Resources.dossierClasserSansSuite
        End Select
        Dim variables = New With {input}

        ' Requête vers l'API
        Dim response = Requêter(Jeton, New With {query, variables})

        ' Renvoi de la réponse
        Return response

    End Function
    
    Sub TransférerDossier(Jeton As String, InstructeurId As String, DossierId As String)

        ' Assemblage des paramètres...
        Dim query = My.Resources.dossierRepasserEnConstruction
        Dim input As Object = New With {
            instructeurId,
            dossierId,
            .disableNotification = True
        }
        Dim variables = New With {input}

        ' Requête vers l'API
        Requêter(Jeton, New With {query, variables})

        
        query = My.Resources.dossierPasserEnInstruction
        Requêter(Jeton, New With {query, variables})

    End Sub

End Module
