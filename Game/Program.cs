using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Timers;
using System.Linq;
using System.Resources;
using System.Globalization;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

class Program
{
    // Current game language (ru/en)
    private static string? language { get; set; }
    // List of already used words
    private static List<string> usedWords = new List<string>();
    // Dictionary of letters and their count in the starting word
    private static Dictionary<char, int> charOfStartWord = new Dictionary<char, int>();
    // Current player (true - first, false - second)
    private static bool currentPlayer { get; set; } = true;
    // Current player name for display
    private static string? nameOfCurrentPlayer { get; set; }
    // Starting word of the game
    private static string? word { get; set; }                                                 


    public static void Main(string[] args)
    {
        ChooseLanguage();
    }

    private static void ChooseLanguage()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите язык игры / Choose the game language:");
            Console.WriteLine("1. Русский (Russian)");
            Console.WriteLine("2. English (Английский)");
            Console.Write("\nВаш выбор (1 или 2) / Your choice (1 or 2): ");
            string? choice = Console.ReadLine() ?? "";

            if (int.TryParse(choice, out int number) && (number == 1 || number == 2))
            {
                if (number == 1)
                {
                    language = "ru";
                    Console.WriteLine("\nВыбран русский язык.");
                    Console.WriteLine("\nНажмите любую клавишу для продолжения.");
                }
                else
                {
                    language = "en";
                    Console.WriteLine("\nEnglish language chosen.");
                    Console.WriteLine("\nPress any key to continue.");
                }

                Console.ReadKey();
                Console.Clear();
                break;
            }

            Console.Write("\nНекорректно значение! / ");
            Console.WriteLine("Invalid value!");
            Console.Write("\nНажмите любую клавишу для продолжения / ");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
        Menu();
        return;
    }

    private static void Menu()
    {
        Console.Clear();

        Thread.CurrentThread.CurrentUICulture = new CultureInfo(language ?? "ru");

        Console.WriteLine($"-------- {Game.Properties.Resources.Menu} --------");
        Console.WriteLine($"1. {Game.Properties.Resources.Start}");
        Console.WriteLine($"2. {Game.Properties.Resources.Rules}");
        Console.WriteLine($"3. {Game.Properties.Resources.ChangeLang}");
        Console.WriteLine($"4. {Game.Properties.Resources.Exit}");
        Console.Write($"\n{Game.Properties.Resources.YourChoice}: ");

        while (true)
        {
            string choice = Console.ReadLine() ?? "";

            if (int.TryParse(choice, out int number))
            {
                switch (number)
                {
                    case 1: _ = StartGameAsync(); break;
                    case 2: Rules(); break;
                    case 3: ChooseLanguage(); Menu(); break;
                    case 4: break;
                }
            }
            break;
        }
        return;
    }

    private static void Rules()
    {
        Console.WriteLine($"\n{Game.Properties.Resources.RuleString1}");  
        Console.WriteLine(Game.Properties.Resources.RuleString2);  
        Console.WriteLine(Game.Properties.Resources.RuleString3);  
        Console.WriteLine($"\n{Game.Properties.Resources.PressAnyKey}");
        Console.ReadKey();
        Menu();
        return;
    }

    private static async Task StartGameAsync()
    {
        Console.Clear();
        usedWords.Clear();

        while (true)
        {
            Console.Write($"\n{Game.Properties.Resources.WriteF8T30}: ");
            currentPlayer = false;
            word = Console.ReadLine()?.ToLower() ?? "";

            if (word.Length >= 8 && word.Length <= 30 && !ContainsInvalidCharacters(word))
            {
                usedWords.Add(word);
                break;
            }

            if (ContainsInvalidCharacters(word))
            {
                Console.WriteLine(Game.Properties.Resources.NonAlphabet);
            }
            else
            {
                Console.WriteLine($"\n{Game.Properties.Resources.InvalidLength}");
            }

            Console.WriteLine($"\n{Game.Properties.Resources.PressAnyKey}");
            Console.ReadKey();
            Console.Clear();
        }

        foreach (char letter in word)
        {
            if (charOfStartWord.ContainsKey(letter))
                charOfStartWord[letter]++;
            else
                charOfStartWord[letter] = 1;
        }

        string? input;

        while (true)
        {
            nameOfCurrentPlayer = currentPlayer
                ? Game.Properties.Resources.SecondPlayer
                : Game.Properties.Resources.FirstPlayer;
            
            Console.WriteLine($"\n{Game.Properties.Resources.StartWord}{word}");
            Console.Write($"{Game.Properties.Resources.WriteWord.Replace("{nameOfCurrentPlayer}", $"{nameOfCurrentPlayer}")}");

            var timerTask = TimerAsync(5);

            input = Console.ReadLine()?.ToLower() ?? "";

            if (timerTask.IsCompleted)
            {
                endGame();
                Menu();
                return;
            }

            if (ContainsInvalidCharacters(input))
            {
                Console.WriteLine($"\n{Game.Properties.Resources.NonAlphabet}");
                continue;
            }

            if (IsStringNullOrWhiteSpace(input))
            {
                continue;
            }

            if (IsWordAlreadyUsed(input))
            {
                continue;
            }

            bool validWord = true;

            foreach (char letterOfWord in input.Distinct())
            {
                int countOfLetter = input.Count(letter =>
                    letter == letterOfWord);

                if (ContainsLetterOrNot(letterOfWord))
                {
                    validWord = false;
                    break;
                }
                else if (WordHasMoreLetter(countOfLetter, letterOfWord))
                {
                    validWord = false;
                    break;
                }
            }

            if (validWord)
            {
                Console.WriteLine($"\n{Game.Properties.Resources.CorrectWord}");
                currentPlayer = !currentPlayer;
                usedWords.Add(input ?? "");
            }
        }
    }

    // Display time's up message and game results (winner/loser)
    private static void endGame()
    {
        Console.WriteLine($"\n{Game.Properties.Resources.TimesUp.Replace("{nameOfCurrentPlayer}", $"{nameOfCurrentPlayer}")}");
        string winner = currentPlayer
            ? Game.Properties.Resources.FirstPlayer
            : Game.Properties.Resources.SecondPlayer;
        Console.WriteLine($"{Game.Properties.Resources.Winner.Replace("{nameOfCurrentPlayer}", $"{winner}")}");
        Console.WriteLine(Game.Properties.Resources.PressAnyKey);
        Console.ReadKey();
        return;
    }

    // Checks if the entered word contains non-alphabet characters
    private static bool ContainsInvalidCharacters(string input)
    {
        if (Regex.IsMatch(input, @"[^a-zA-Zа-яА-Я]"))
        {
            return true;
        }
        return false;
    }

    // Checks if the entered word contains more of a specific letter than the original word
    private static bool WordHasMoreLetter(int countOfLetter, char letterOfWord)
    {
        if (countOfLetter > charOfStartWord[letterOfWord])
        {
            Console.WriteLine($"\n{Game.Properties.Resources.MoreThen.Replace("{letterOfWord}", $"{letterOfWord}")}");
            return true;
        }
        return false;
    }

    // Checks if the letter is contained in the original word
    private static bool ContainsLetterOrNot(char letterOfWord)
    {
        if (!charOfStartWord.ContainsKey(letterOfWord))
        {
            Console.WriteLine($"\n{Game.Properties.Resources.NoLetter.Replace("{letterOfWord}", $"{letterOfWord}")} ");
            return true;
        }
        return false;
    }

    // Checks if the entered string is empty or null
    private static bool IsStringNullOrWhiteSpace(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine($"\n{Game.Properties.Resources.CantBeNull}");
            return true;
        }
        return false;
    }

    // Checks if the word has already been used in the game
    private static bool IsWordAlreadyUsed(string input)
    {
        if (usedWords.Contains(input ?? ""))
        {
            Console.WriteLine($"\n{Game.Properties.Resources.TryAnother}");
            return true;
        }
        return false;
    }

    // Async timer
    private static async Task TimerAsync(int seconds)
    {
        var tcs = new TaskCompletionSource<bool>();
        using var timer = new System.Timers.Timer(1000);

        timer.Elapsed += (sender, e) =>
        {
            seconds--;

            if (seconds <= 0)
            {
                timer.Stop();
                tcs.TrySetResult(true);
            }
        };
        timer.Start();
        await tcs.Task;
    }
   
}
