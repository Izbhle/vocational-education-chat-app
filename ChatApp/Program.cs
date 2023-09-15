﻿using Avalonia.ReactiveUI;
using Avalonia;

namespace ChatApp
{
    class Program
    {
        // Instead of implementing "INotifyPropertyChanged" on our own we use "ReactiveObject" as
        // our base class. Read more about it here: https://www.reactiveui.net

        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting prototype...");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            // var db = new NotenDB("test.db", "src/infrastructure/database/sqlite/migrations");
            // var sessionManager = new SessionManager(db);
            // var operations = new OperationsController(db);
            // var entrypoint = new ConsoleEntrypoint(sessionManager, operations);
            // entrypoint.Start();
        }

        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace().UseReactiveUI();
    }
}
