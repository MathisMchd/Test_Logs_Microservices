
Imports System.IO
Imports NLog

Module Module1
    Private logger As Logger = LogManager.GetCurrentClassLogger()

    Sub Main()

        Dim logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")
        If Not Directory.Exists(logFolder) Then Directory.CreateDirectory(logFolder)
        Console.WriteLine("Folder : " + logFolder)
        logger.Debug("Debug test log")
        logger.Info("Info test log")
        logger.Warn("Warning test log")
        logger.Error("Error test log")
        logger.Fatal("Fatal test log")

        Console.WriteLine("Logs envoyés au collector via NLog + OTLP")
        Console.ReadLine()
    End Sub
End Module
