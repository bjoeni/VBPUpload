Imports System.IO
Imports Microsoft.Win32

Module Module1

    Sub Main()
        Try
            Console.WriteLine("Bitte beenden Sie ""chrome.exe"" sowie ""HolzShots.exe"".")
            Do Until Process.GetProcessesByName("chrome").Length = 0 AndAlso Process.GetProcessesByName("HolzShots").Length = 0 : Loop
            Console.WriteLine("Suche Google Chrome...")
            Dim p As String = If(Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", Nothing), Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", Nothing)).ToString()
            Dim cversion As String = FileVersionInfo.GetVersionInfo(p).FileVersion
            Console.WriteLine("chrome.exe (Chrome " & cversion & ") gefunden unter """ & p & """")
            Console.WriteLine("Suche HolzShots...")
            Dim dir As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            Dim hs As String = Path.Combine(dir, "HolzShots.exe")
            Console.WriteLine("HolzShots.exe (HolzShots " & FileVersionInfo.GetVersionInfo(hs).FileVersion & ") gefunden unter """ & hs & """")
            Dim hsplugins As String = Path.Combine(dir, "Plugin")
            Console.WriteLine("Installiere HolzShots-Plugin...")
            File.WriteAllBytes(Path.Combine(hsplugins, "VBP.dll"), My.Resources.VBP)
            File.WriteAllBytes(Path.Combine(hsplugins, "VBP.connect.exe"), My.Resources.VBP_connect)
            Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", "VBPUploader", """" & Path.Combine(hsplugins, "VBP.connect.exe") & """")
            Console.WriteLine("Installiere Chrome-Extension...")
            Dim expath = Path.Combine(hsplugins, "VBP.extension.crx")
            File.WriteAllBytes(expath, My.Resources.VBP_extension)
            File.WriteAllText(Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(p), cversion), "Extensions"), "bikfiplbkfoligpokppbfkbcljpnnjkk.json"),
                              String.Format(My.Resources.json, expath.Replace("\", "\\")))
            Console.WriteLine("Schließe Installation ab...")
            Process.Start(Path.Combine(hsplugins, "VBP.connect.exe"))
            Process.Start(hs)
            Process.Start(p, My.Resources.chrome)
            Console.WriteLine("Fertig!")
            Console.WriteLine()
            Console.WriteLine("Sie können diese Anwendung entfernen, nachdem Sie sich durch betätigen einer beliebigen Taste geschlossen haben.")
            Console.ReadKey(True)
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine()
            Console.WriteLine()
            Console.WriteLine("Installation fehlgeschlagen: Der folgende Fehler trat auf:")
            Console.WriteLine()
            Console.WriteLine(ex.ToString())
            Console.ReadKey(True)
        End Try
    End Sub

End Module
