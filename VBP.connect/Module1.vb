Imports System.Net
Imports System.Net.Sockets
Imports System.Windows.Forms
Imports System.IO

Module Module1
    Sub Main(args() As String)
        With New App
            .Run(args)
        End With
    End Sub
End Module

Class App
    Inherits ApplicationServices.WindowsFormsApplicationBase
    Private conList As New List(Of HttpListenerResponse)
    Private imgDic As New Dictionary(Of String, Byte())
    Private server As New HttpListener()
    Sub New()
        Me.IsSingleInstance = True
    End Sub
    Protected Overrides Function OnStartup(eventArgs As ApplicationServices.StartupEventArgs) As Boolean
        server.Start()
        With New Threading.Thread(AddressOf AcceptHttp)
            .Start()
        End With
        If Me.CommandLineArgs.Count > 0 Then
            OnStartupNextInstance(New ApplicationServices.StartupNextInstanceEventArgs(eventArgs.CommandLine, Nothing))
        End If
        Return MyBase.OnStartup(eventArgs)
    End Function
    Protected Overrides Sub OnRun()
        Application.Run(Me.ApplicationContext)
    End Sub
    Protected Overrides Sub OnStartupNextInstance(eventArgs As ApplicationServices.StartupNextInstanceEventArgs)
        If eventArgs.CommandLine.Count = 4 AndAlso eventArgs.CommandLine(0) = "/id" AndAlso eventArgs.CommandLine(2) = "/f" AndAlso IO.File.Exists(eventArgs.CommandLine(3)) AndAlso Not imgDic.ContainsKey(eventArgs.CommandLine(1)) Then
            imgDic.Add(eventArgs.CommandLine(1), IO.File.ReadAllBytes(eventArgs.CommandLine(3)))
            IO.File.Delete(eventArgs.CommandLine(3))
            Update()
        End If
    End Sub
    Private Sub Update()
        For i As Integer = conList.Count - 1 To 0 Step -1
            Try
                Update(conList(i))
            Catch ex As WebException
            End Try
            conList.Remove(conList(i))
        Next
    End Sub
    Private Sub Update(r As HttpListenerResponse)
        With New StreamWriter(r.OutputStream) With {.AutoFlush = True}
            .Write(String.Join(Environment.NewLine, imgDic.Keys.ToArray()))
        End With
        r.Close()
    End Sub
    Private Sub AcceptHttp()
        server.Prefixes.Add("http://localhost:7410/")
        server.Start()
        Do
            Try
                Dim context = server.GetContext()
                Dim request = context.Request
                Dim response = context.Response
                Dim bytes() As Byte = Nothing
                If request.RawUrl.StartsWith("/getImg") AndAlso request.QueryString.AllKeys.Contains("id") AndAlso imgDic.TryGetValue(request.QueryString("id"), bytes) Then
                    response.OutputStream.Write(bytes, 0, bytes.Length)
                    imgDic.Remove(request.QueryString("id"))
                    Update()
                    response.Close()
                ElseIf request.RawUrl = "/list?first" AndAlso imgDic.Count > 0 Then
                    Update(response)
                ElseIf request.RawUrl.StartsWith("/list") Then
                    conList.Add(response)
                Else
                    response.Abort()
                End If
            Catch ex As WebException
            End Try
        Loop
    End Sub
End Class