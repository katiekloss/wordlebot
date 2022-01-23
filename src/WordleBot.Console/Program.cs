using WordleBot.Core;
using WordleBot.Core.Game;

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