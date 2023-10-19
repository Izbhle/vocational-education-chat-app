﻿using Avalonia.ReactiveUI;
using Avalonia;
using ChatAppClient;

namespace ChatAppClient
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
        }

        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<ChatApp>().UsePlatformDetect().LogToTrace().UseReactiveUI();
    }
}
