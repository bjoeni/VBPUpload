Imports System.IO

Public Class VBP
    Inherits HolzPlugin.Net.Upload.NoSettingsImageHoster
    Private Image As String

    Public Overrides ReadOnly Property MaxFileSize As Long
        Get
            Return 1000000
        End Get
    End Property

    Public Overrides ReadOnly Property PluginInfo As HolzPlugin.Plugin.PluginInformation
        Get
            Return New HolzPlugin.Plugin.PluginInformation(
                "VB-Paradise",
                "Stellt den Screenshot zum Einfügen in einen Post auf VB-Paradise bereit.",
                New Version(1, 0),
                Nothing,
                "BjöNi",
                My.Resources.vbp_small,
                Nothing
            )
        End Get
    End Property

    Public Overrides Sub PrepareRequest(postData() As Byte)
        Image = Path.GetTempFileName()
        IO.File.WriteAllBytes(Image, postData)
    End Sub

    Public Overrides Function UploadImage() As HolzPlugin.Net.Upload.UploadResult
        Dim input As New HolzShots.Dialogs.AeroInputbox()
        input.ShowDialog()
        Process.Start(Path.Combine("Plugin", "VBP.connect.exe"), "/id " & input.Text & ".png /f " & Image)
        Return New HolzPlugin.Net.Upload.UploadResult(
            Me,
            False,
            "Das Bild steht nun zum Upload auf VBP bereit." & Environment.NewLine &
            "Sie finden es unter ""Dateianhänge"">""Screenshot hinzufügen"""
        )
    End Function
End Class
