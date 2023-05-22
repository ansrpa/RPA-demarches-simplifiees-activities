Imports System.IO
Imports System.Net.Mail
'Imports UiPath.Core.Activities.Storage


Public Module REFramework
    Sub CheckBoolean(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception("Le dictionnaire de configuration n’est pas initialisé.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom du paramètre est invalide.")
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("Le paramètre « {0} » est manquant.", Name))
            End If
            Return
        End If

        Dim value As Boolean
        If Not Boolean.TryParse(Config(Name), value) Then
            Throw New Exception(String.Format("Paramètre « {0} » : la valeur « {1} » ne ressemble pas à un booléen.", Name, Config(Name)))
        End If
    End Sub

    Sub CheckCredentials(Credentials As Dictionary(Of String, Tuple(Of String, Security.SecureString)), Name As String, IsRequired As Boolean)
        If Credentials Is Nothing Then
            Throw New Exception("Le dictionnaire des identifiants n’est pas initialisé.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom de l’identifiant est invalide.")
        End If

        If Not Credentials.ContainsKey(Name) OrElse Credentials(Name) Is Nothing Then
            Credentials(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("L’identifiant « {0} » est manquant.", Name))
            End If
            Return
        End If
    End Sub

    Sub CheckFile(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean, MustExist As Boolean)
        If Config Is Nothing Then
            Throw New Exception("Le dictionnaire de configuration n’est pas initialisé.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom du paramètre est invalide.")
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("Le paramètre « {0} » est manquant.", Name))
            End If
            Return
        End If

        Config(Name) = Environment.ExpandEnvironmentVariables(Config(Name))

        Dim info As FileInfo
        Try
            info = New FileInfo(Config(Name))
        Catch e As ArgumentException
            Throw New Exception(String.Format("Paramètre « {0} » : la valeur « {1} » ne ressemble pas à un nom de fichier.", Name, Config(Name)))
        End Try

        If MustExist And Not info.Exists Then
            Throw New Exception(String.Format("Paramètre « {0} » : le fichier « {1} » n’existe pas.", Name, Config(Name)))
        End If
    End Sub

    Sub CheckFolder(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean, CreateIfNotExists As Boolean)
        If Config Is Nothing Then
            Throw New Exception("Le dictionnaire de configuration n’est pas initialisé.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom du paramètre est invalide.")
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("Le paramètre « {0} » est manquant.", Name))
            End If
            Return
        End If

        Config(Name) = Environment.ExpandEnvironmentVariables(Config(Name))

        Dim info As DirectoryInfo
        Try
            info = New DirectoryInfo(Config(Name))
        Catch e As ArgumentException
            Throw New Exception(String.Format("Paramètre « {0} » : la valeur « {1} » ne ressemble pas à un nom de dossier.", Name, Config(Name)))
        End Try

        If Not info.Exists Then
            If CreateIfNotExists Then
                Try
                    Directory.CreateDirectory(Config(Name))
                Catch e As Exception
                    Throw New Exception(String.Format("Paramètre « {0} » : erreur lors de la création du dossier « {1} ».", Name, Config(Name)))
                End Try
            Else
                Throw New Exception(String.Format("Paramètre « {0} » : le dossier « {1} » n’existe pas.", Name, Config(Name)))
            End If
        End If

        Try
            Using fs As FileStream = File.Create(Path.Combine(Config(Name), Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose)
                ' Will throw a System.UnauthorizedAccessException if folder cannot be written.
            End Using
        Catch e As System.UnauthorizedAccessException
            Throw New Exception(String.Format("Paramètre « {0} »  : le dossier « {1} »  n'est pas accessible en écriture.", Name, Config(Name)))
        End Try
    End Sub

    Sub CheckInt(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean, minValue As Nullable(Of Int32), maxValue As Nullable(Of Int32))
        If Config Is Nothing Then
            Throw New Exception("Le dictionnaire de configuration n’est pas initialisé.")
        End If

        If Not IsNothing(minValue) AndAlso Not IsNothing(maxValue) AndAlso Not (minValue < maxValue) Then
            Throw New Exception("La valeur minimale doit être strictement inférieure à la valeur maximale.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom du paramètre est invalide.")
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("Le paramètre « {0} » est manquant.", Name))
            End If
            Return
        End If

        If Not Information.IsNumeric(Config(Name)) Then
            Throw New Exception(String.Format("Paramètre « {0} » : la valeur « {1} » ne ressemble pas à un nombre.", Name, Config(Name)))
        End If

        Dim value As Int32
        If Not Integer.TryParse(Config(Name), value) Then
            Throw New Exception(String.Format("Paramètre « {0} » : la valeur {1} n’est pas un nombre entier.", Name, Config(Name)))
        End If

        If Not IsNothing(minValue) AndAlso value < minValue Then
            Throw New Exception(String.Format("Paramètre « {0} » : la valeur {1} est inférieure à la valeur minimale {2}.", Name, Config(Name), minValue))
        End If

        If Not IsNothing(maxValue) AndAlso value > maxValue Then
            Throw New Exception(String.Format("Paramètre « {0} » : la valeur {1} est supérieure à la valeur maximale {2}.", Name, Config(Name), maxValue))
        End If
    End Sub

    Sub CheckMailAddress(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception("Le dictionnaire de configuration n’est pas initialisé.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom du paramètre est invalide.")
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("Le paramètre « {0} » est manquant.", Name))
            End If
            Return
        End If

        Dim value As MailAddress
        Try
            value = New MailAddress(Config(Name))
        Catch
            Throw New Exception(String.Format("Paramètre « {0} » : la valeur « {1} » ne ressemble pas à une adresse de messagerie.", Name, Config(Name)))
        End Try
    End Sub

    Sub CheckMailAddressCollection(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception("Le dictionnaire de configuration n’est pas initialisé.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom du paramètre est invalide.")
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("Le paramètre « {0} » est manquant.", Name))
            End If
            Return
        End If

        Dim value As MailAddress
        For Each str As String In Config(Name).Split(";"c)
            Try
                value = New MailAddress(str)
            Catch
                Throw New Exception(String.Format("Paramètre « {0} » : la chaîne de caractères « {1} » ne ressemble pas à une adresse de messagerie.", Name, str))
            End Try
        Next

    End Sub

    Sub CheckStorage(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception("Le dictionnaire de configuration n’est pas initialisé.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom du paramètre est invalide.")
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("Le paramètre « {0} » est manquant.", Name))
            End If
            Return
        End If


        'Try
        '    Dim listStorageFilesActivity As ListStorageFiles = New ListStorageFiles With {
        '        .Directory = "/",
        '        .Recursive = True,
        '        .StorageBucketName = Config(Name)
        '    }
        '    Dim invoker As WorkflowInvoker = New WorkflowInvoker(listStorageFilesActivity)
        '    invoker.Invoke()
        'Catch e As Exception
        '    'Throw New Exception(String.Format("Paramètre « {0} » : le compartiment de stockage « {1} » est inaccessible en lecture. Message reçu : {2}", Name, Config(Name), e.Message))
        'End Try
    End Sub

    Sub CheckString(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception("Le dictionnaire de configuration n’est pas initialisé.")
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception("Le nom du paramètre est invalide.")
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(String.Format("Le paramètre « {0} » est manquant.", Name))
            End If
            Return
        End If
    End Sub
End Module
