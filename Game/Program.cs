using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    public static async Task Main(string[] args)
    {
        GameLanguage.ChooseLanguage();
        await Menu.MenuAsync();
    }
}
    static class GameLanguage
    {
        public static void ChooseLanguage()
        {
            while (true)
            {
                LanguageMenuManager.DisplayMenu();
                string? choice = Console.ReadLine() ?? "";

                if (LanguageValidator.IsValidChoice(choice))
                {
                    LanguageChoose.SetLanguage(choice);
                    CultureConfigurator.CultureConfig();
                    LanguageMenuManager.DisplaySelectionMessage();
                    Console.ReadKey();
                    break;
                }
                LanguageMenuManager.DisplayError();
                Console.ReadKey();
            }
        }
    }

    static class Menu
    {
        public static async Task MenuAsync()
        {
            while (true)
            {
                Console.Clear();
                MenuTextManager.DisplayMenu();
                string? choice = Console.ReadLine() ?? "";

                if (await MenuSelectItem.Select(choice))
                {
                    break;
                }
            }
        }
    }

    static class GameRules
    {
        public static void Rules()
        {
            Console.WriteLine($"\n{Game.Properties.Resources.RuleString1}" +
                $"\n{Game.Properties.Resources.RuleString2}" +
                $"\n{Game.Properties.Resources.RuleString3}" +
                $"\n\n{Game.Properties.Resources.PressAnyKey}");

            Console.ReadKey();
        }
    }

    static class MyGame
    {
        public static async Task StartGameAsync()
        {
            Console.Clear();
            GameLogic.ClearList();

            while (true)
            {
                Console.Write($"\n{Game.Properties.Resources.WriteF8T30}: ");
                GameState.word = Console.ReadLine()?.ToLower() ?? "";

                if (GameValidator.IsLengthValid(GameState.word) && !GameValidator.IsContainsInvalidCharacters(GameState.word))
                {
                    GameLogic.AddWordsToList(GameState.word);
                    break;
                }

                if (GameValidator.IsContainsInvalidCharacters(GameState.word))
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

            GameLogic.BuildLetterDictionary();
            string? input;

            while (true)
            {
                GameLogic.PlayerState();
                GameTextManager.DisplayRoundInfo();

                TimerManager.StartTimer();
                input = Console.ReadLine()?.ToLower() ?? "";

                if (TimerManager.IsTimerUp)
                {
                    GameLogic.WinnerIs();
                    GameTextManager.DisplayEndGameMessage();
                    Console.ReadKey();
                    return;
                }

                if (GameValidator.IsContainsInvalidCharacters(input))
                {
                    Console.WriteLine($"\n{Game.Properties.Resources.NonAlphabet}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"\n{Game.Properties.Resources.CantBeNull}");
                    continue;
                }

                if (GameValidator.IsWordAlreadyUsed(GameState.usedWords, input))
                {
                    Console.WriteLine($"\n{Game.Properties.Resources.TryAnother}");
                    continue;
                }

                bool validWord = true;

                foreach (char letterOfWord in input.Distinct())
                {
                    int countOfLetter = input.Count(letter => letter == letterOfWord);

                    if (GameValidator.IsContainsLetterOrNot(GameState.charOfStartWord, letterOfWord))
                    {
                        Console.WriteLine($"\n{Game.Properties.Resources.NoLetter.Replace("{letterOfWord}", $"{letterOfWord}")} ");
                        validWord = false;
                        break;
                    }
                    else if (GameValidator.IsThereMoreLetterOrNot(GameState.charOfStartWord, countOfLetter, letterOfWord))
                    {
                        Console.WriteLine($"\n{Game.Properties.Resources.MoreThen.Replace("{letterOfWord}", $"{letterOfWord}")}");
                        validWord = false;
                        break;
                    }
                }

                if (validWord)
                {
                    Console.WriteLine($"\n{Game.Properties.Resources.CorrectWord}");
                    GameLogic.ReversePlayerState();
                    GameLogic.AddWordsToList(input);
                }
            }
        }
    }

static class LanguageChoose
{
    public static string? language { get; private set; }

    public static void SetLanguage(string? choice)
    {
        language = choice == "1" ? "ru" : "en";
    }
}

static class LanguageValidator
{
    public static bool IsValidChoice(string? choice)
    {
        return choice == "1" || choice == "2";
    }
}

static class CultureConfigurator
{
    public static void CultureConfig()
    {
        var culture = new CultureInfo(LanguageChoose.language ?? "ru");
        Game.Properties.Resources.Culture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}

static class LanguageMenuManager
{
    public static void DisplayMenu()
    {
        Console.Clear();
        Console.WriteLine("Выберите язык игры / Choose the game language:" +
            "\n1. Русский (Russian)" +
            "\n2. English (Английский)" +
            "\nВаш выбор (1 или 2) / Your choice (1 or 2): ");
    }

    public static void DisplayError()
    {
        Console.Write("\nНекорректно значение! / " +
            "Invalid value!" +
            "\nНажмите любую клавишу для продолжения / " +
            "Press any key to continue.");
    }

    public static void DisplaySelectionMessage()
    {
        if (LanguageChoose.language == "ru")
        {
            Console.WriteLine("\nВыбран русский язык." +
                "\nНажмите любую клавишу для продолжения.");
        }
        else
        {
            Console.WriteLine("\nEnglish language chosen." +
                "\nPress any key to continue.");
        }
    }
}

static class MenuTextManager
{
    public static void DisplayMenu()
    {
        Console.WriteLine($"-------- {Game.Properties.Resources.Menu} --------" +
            $"\n1. {Game.Properties.Resources.Start}" +
            $"\n2. {Game.Properties.Resources.Rules}" +
            $"\n3. {Game.Properties.Resources.ChangeLang}" +
            $"\n4. {Game.Properties.Resources.Exit}" +
            $"\n{Game.Properties.Resources.YourChoice}:");
    }

    public static void DisplayError()
    {
        Console.WriteLine($"{Game.Properties.Resources.InvalidValue}");
    }
}

static class MenuSelectItem
{
    public static async Task<bool> Select(string? choice)
    {
        switch (choice)
        {
            case "1":
                await MyGame.StartGameAsync();
                return false;

            case "2":
                GameRules.Rules();
                return false;

            case "3":
                GameLanguage.ChooseLanguage();
                return false;

            case "4":
                return true;

            default:
                MenuTextManager.DisplayError();
                Console.ReadKey();
                return false;
        }
    }
}

static class GameState
{
    public static Dictionary<char, int> charOfStartWord = new Dictionary<char, int>();
    public static string? word { get; set; }
    public static string? winner { get; set; }
    public static List<string> usedWords = new List<string>();
    public static bool currentPlayer { get; set; } = false;
    public static string? NameOfCurrentPlayer { get; set; }
}
static class GameLogic
{
    public static void PlayerState()
    {
        GameState.NameOfCurrentPlayer = GameState.currentPlayer
               ? Game.Properties.Resources.SecondPlayer
               : Game.Properties.Resources.FirstPlayer;
    }

    public static void ReversePlayerState()
    {
        GameState.currentPlayer = !GameState.currentPlayer;
    }

    public static void AddWordsToList(string input)
    {
        GameState.usedWords.Add(input ?? "");
    }

    public static void ClearList()
    {
        GameState.usedWords.Clear();
    }

    public static void WinnerIs()
    {
        GameState.winner = GameState.currentPlayer
            ? Game.Properties.Resources.FirstPlayer
            : Game.Properties.Resources.SecondPlayer;
    }

    public static void BuildLetterDictionary()
    {
        foreach (char letter in GameState.word ?? "")
        {
            if (GameState.charOfStartWord.ContainsKey(letter))
                GameState.charOfStartWord[letter]++;
            else
                GameState.charOfStartWord[letter] = 1;
        }
    }
}

static class GameTextManager
{
    public static string Winner => Game.Properties.Resources.Winner.Replace("{nameOfCurrentPlayer}", $"{GameState.winner}");
    public static string TimesUp => Game.Properties.Resources.TimesUp.Replace("{nameOfCurrentPlayer}", $"{GameState.NameOfCurrentPlayer}");
    public static string WriteWord => Game.Properties.Resources.WriteWord.Replace("{nameOfCurrentPlayer}", $"{GameState.NameOfCurrentPlayer}");

    public static void DisplayEndGameMessage()
    {
        Console.WriteLine($"\n{TimesUp} {Winner}" +
            $"\n{Game.Properties.Resources.PressAnyKey}");
    }

    public static void DisplayRoundInfo()
    {
        Console.WriteLine($"\n{Game.Properties.Resources.StartWord}{GameState.word}");
        Console.Write($"{WriteWord}");
    }
}

static class GameValidator
{
    public static bool IsThereMoreLetterOrNot(Dictionary<char, int> charOfStartWord, int countOfLetter, char letterOfWord)
    {
        return countOfLetter > charOfStartWord[letterOfWord];
    }

    public static bool IsContainsLetterOrNot(Dictionary<char, int> charOfStartWord, char letterOfWord)
    {
        return !charOfStartWord.ContainsKey(letterOfWord);
    }

    public static bool IsWordAlreadyUsed(List<string> usedWords, string input)
    {
        return usedWords.Contains(input ?? "");
    }

    public static bool IsContainsInvalidCharacters(string input)
    {
        return Regex.IsMatch(input, @"[^a-zA-Zа-яА-Я]");
    }

    public static bool IsLengthValid(string word)
    {
        return word.Length >= 8 && word.Length <= 30;
    }
}

static class TimerManager
{
    public static Task? timer;
    public static bool IsTimerUp => timer?.IsCompleted ?? false;

    public static void StartTimer()
    {
        timer = TimerAsync(20);
    }

    public static async Task TimerAsync(int seconds)
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