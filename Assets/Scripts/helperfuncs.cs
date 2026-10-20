
using System;
using System.Collections.Generic;
using static MiniHelpers.HelperFuncs;

class Program
{
    static void Main()
    {
        // Shortened printing
        P("Hello, world!");
        W("Loading...");
        P();

        // Input
        string name = Input("Enter your name: ");
        int age = AskInt("Enter your age: ");

        P($"Hello {Cap(name)}! You're {age}.");

        // Mathematics
        int health = Clamp(150, 0, 100);
        P($"Health: {health}");

        double progress = Map(50, 0, 100, 0, 1);
        P($"Progress: {progress}");

        // Random numbers (1 to 6 inclusive)
        int dice = RandInt(1, 7);
        P($"Dice rolled: {dice}");

        // Probability
        if (Chance(0.25))
            P("You got lucky!");

        // Random collection item
        var weapons = new List<string>
        {
            "Sword", "Bow", "Staff"
        };

        P($"Selected weapon: {Pick(weapons)}");

        Shuffle(weapons);
        P($"Shuffled: {Join(weapons)}");

        // Shortened loops
        Times(5, i => P($"Iteration {i}"));

        // String utilities
        P(Repeat("=", 20));
        P(ContainsI("HELLO WORLD", "hello"));
        P(Truncate("Programming", 7));

        // Safe parsing
        int number = ParseIntOr("invalid", 42);
        P(number); // 42

        // Files
        WriteText("save.txt", "Player: " + name);
        P(ReadText("save.txt"));

        // Performance measurement
        TimeSpan elapsed = Measure(() =>
        {
            Times(100000, i =>
            {
                double result = Math.Sqrt(i);
            });
        });

        P($"Elapsed: {elapsed.TotalMilliseconds} ms");
    }
}
