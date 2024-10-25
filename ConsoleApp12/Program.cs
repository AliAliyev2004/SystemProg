using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


class Program
{
    static List<string> fileList = new List<string> { "file1.json", "file2.json", "file3.json" }; // JSON dosya adları
    static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Select mode: 'single' for Single Mode, 'multi' for Multi Mode, 'cancel' to cancel operations.");

        string mode = Console.ReadLine()?.ToLower();

        switch (mode)
        {
            case "single":
                await LoadFilesSingleModeAsync(cancellationTokenSource.Token);
                break;
            case "multi":
                await LoadFilesMultiModeAsync(cancellationTokenSource.Token);
                break;
            case "cancel":
                CancelOperations();
                break;
            default:
                Console.WriteLine("Invalid mode selected.");
                break;
        }
    }

    static async Task LoadFilesSingleModeAsync(CancellationToken token)
    {
        Console.WriteLine("Single Mode selected. Loading files in a single thread...");

        try
        {
            foreach (var file in fileList)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Operation canceled.");
                    break;
                }

                string content = await LoadFileAsync(file, token);
                Console.WriteLine($"{file} loaded with content: {content}");
            }
            Console.WriteLine("All files loaded in Single Mode.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was canceled.");
        }
    }

    static async Task LoadFilesMultiModeAsync(CancellationToken token)
    {
        Console.WriteLine("Multi Mode selected. Loading files with multiple threads...");

        List<Task> tasks = new List<Task>();

        foreach (var file in fileList)
        {
            tasks.Add(Task.Run(async () =>
            {
                if (!token.IsCancellationRequested)
                {
                    string content = await LoadFileAsync(file, token);
                    Console.WriteLine($"{file} loaded with content: {content}");
                }
            }, token));
        }

        try
        {
            await Task.WhenAll(tasks);
            Console.WriteLine("All files loaded in Multi Mode.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was canceled.");
        }
    }

    static async Task<string> LoadFileAsync(string filePath, CancellationToken token)
    {
     
        await Task.Delay(1000, token); 
        return $"Content of {filePath}"; 
    }

    static void CancelOperations()
    {
        cancellationTokenSource.Cancel();
        Console.WriteLine("Operation cancellation requested.");
    }
}
