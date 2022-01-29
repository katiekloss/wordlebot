using System.IO.Abstractions;
using Autofac;
using Microsoft.Extensions.Configuration;
using WordleBot.Core.Game;
using WordleBot.Core.Intelligence;
using WordleBot.Core.Utilities;

ConfigurationBuilder configBuilder = new();
configBuilder.AddJsonFile("appsettings.json");
IConfiguration config = configBuilder.Build();

ContainerBuilder builder = new();
builder.RegisterAssemblyTypes(typeof(IFileSystem).Assembly)
    .AsImplementedInterfaces()
    .SingleInstance();

builder.RegisterType<HttpClient>()
    .AsSelf()
    .InstancePerDependency();

builder.RegisterInstance(config);

builder.RegisterType<WordList>()
    .AsSelf()
    .OnActivating(async e => await e.Instance.Load())
    .InstancePerDependency();

builder.RegisterType<SolverEngine>()
    .AsSelf()
    .InstancePerDependency();

var game = new GameHost("could");
builder.RegisterInstance(game)
    .AsSelf();

builder.Register(c =>
{
    var wordList = c.Resolve<WordList>();
    return new GameHost(wordList.Current[new Random().Next(0, wordList.Current.Count)]);
}).AsSelf();

using IContainer root = builder.Build();
SolverEngine engine = root.Resolve<SolverEngine>();
(string? answer, int guesses) = engine.Solve();

if (answer is not null)
    Console.WriteLine($"Won with {answer} in {guesses} guess{(guesses == 1 ? "" : "es")}");
else
    Console.WriteLine("I lost");

/*
Console.Write("Enter a word: ");
string? answer = Console.ReadLine();
if (answer is null) return;

var game = new GameHost(answer);

while (true)
{
    Console.Write("Guess: ");
    string? guess = Console.ReadLine();
    if (guess is null) return;

    GuessResult result;
    try
    {
        result = game.Guess(guess);
    }
    catch (Exception x)
    {
        Console.WriteLine(x.Message);
        continue;
    }

    for (int i = 0; i < guess.Length; i++)
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = result.Slots[i] switch
        {
            SlotStatus.Correct => ConsoleColor.Green,
            SlotStatus.In => ConsoleColor.DarkYellow,
            SlotStatus.Out => ConsoleColor.DarkGray,
            _ => ConsoleColor.Black
        };
        
        Console.Write(guess[i]);
        Console.ResetColor();
    }

    Console.WriteLine();

    if (result.Won) break;
}
*/
