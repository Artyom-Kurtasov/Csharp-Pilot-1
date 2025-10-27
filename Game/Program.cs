global using System;
global using System.Collections.Generic;
global using System.Globalization;
global using System.Linq;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
using Language;
using GameMenu;

internal class Program
{
    public async static Task Main(string[] args)
    {
        GameLanguage.ChooseLanguage();
        await Menu.DisplayMenuAsync();
    }
}

static class MyGame
{
    public static async Task StartGameAsync()
    {
        PrepareUI();
        GameLogic.ClearUsedWords();
        GameLogic.SetStartWord();
        GameLogic.BuildLetterDictionary();
        GameLogic.StartGameLoop();
    }

    public static void PrepareUI()
    {
        UI.ClearUI();
    }
}

static class GameLogic
{
    public static void SetStartWord()
    {
        while (true)
        {
            UI.PrintToUI($"\n{Game.Properties.Resources.WriteF8T30}: ");
            string? input = UI.ReadUserInput();
            GameState.StartWord = input;

            string? validationMessage = GameValidatorLogic.ValidateStartWord();
            if (validationMessage is null)
            {
                AddWordToUsedWords(input ?? "");
                break;
            }

            UI.PrintToUI(validationMessage);
            UI.PrintToUI($"\n{Game.Properties.Resources.PressAnyKey}");
            UI.WaitForUser();
            UI.ClearUI();
        }
    }


    public static void StartGameLoop()
    {
        while (true)
        {
            UpdatePlayerState();
            GameTextLogic.DisplayRoundInfo();

            TimerManager.StartTimer();
            GameState.Input = UI.ReadUserInput();

            if (TimerManager.IsTimerUp)
            {
                DetermineWinner();
                GameTextLogic.DisplayEndGameMessage();
                UI.WaitForUser();
                return;
            }

            UI.PrintToUI(GameValidatorLogic.ValidateInputWord() ?? "");
        }
    }

    public static void UpdatePlayerState()
    {
        GameState.NameOfCurrentPlayer = GameState.CurrentPlayer
            ? Game.Properties.Resources.SecondPlayer
            : Game.Properties.Resources.FirstPlayer;
    }

    public static void SwitchPlayer() =>
        GameState.CurrentPlayer = !GameState.CurrentPlayer;

    public static void AddWordToUsedWords(string input) =>
        GameState.UsedWords.Add(input ?? "");

    public static void ClearUsedWords() =>
        GameState.UsedWords.Clear();

    public static void DetermineWinner()
    {
        GameState.Winner = GameState.CurrentPlayer
            ? Game.Properties.Resources.FirstPlayer
            : Game.Properties.Resources.SecondPlayer;
    }

    public static void BuildLetterDictionary()
    {
        GameState.CharOfStartWord.Clear();

        foreach (char letter in GameState.StartWord ?? "")
        {
            if (GameState.CharOfStartWord.ContainsKey(letter))
                GameState.CharOfStartWord[letter]++;
            else
                GameState.CharOfStartWord[letter] = 1;
        }
    }
}

static class GameState
{
    public static string? Input { get; set; }
    public static Dictionary<char, int> CharOfStartWord = new();
    public static string? StartWord { get; set; }
    public static string? Winner { get; set; }
    public static List<string> UsedWords = new();
    public static bool CurrentPlayer { get; set; } = false;
    public static string? NameOfCurrentPlayer { get; set; }
}

static class GameTextState
{
    public static string Winner =>
        Game.Properties.Resources.Winner.Replace("{nameOfCurrentPlayer}", $"{GameState.Winner}");

    public static string TimesUp =>
        Game.Properties.Resources.TimesUp.Replace("{nameOfCurrentPlayer}", $"{GameState.NameOfCurrentPlayer}");

    public static string WriteWord =>
        Game.Properties.Resources.WriteWord.Replace("{nameOfCurrentPlayer}", $"{GameState.NameOfCurrentPlayer}");
}

static class GameTextLogic
{
    public static void DisplayEndGameMessage()
    {
        UI.PrintToUI($"\n{GameTextState.TimesUp} {GameTextState.Winner}" +
            $"\n{Game.Properties.Resources.PressAnyKey}");
    }

    public static void DisplayRoundInfo()
    {
        UI.PrintToUI($"\n{Game.Properties.Resources.StartWord}{GameState.StartWord}");
        UI.PrintToUI($"{GameTextState.WriteWord}");
    }
}

static class GameValidatorLogic
{
    private const int _minCharactersInWord = 8;
    private const int _maxCharactersInWord = 30;
    private const string _validSymbols = "[^a-zA-Zа-яА-Я]";
    private static string? _errorMessage;
    private static int _countOfLetter;

    public static bool HasMoreLettersThanStartWord(Dictionary<char, int> charOfStartWord, int countOfLetter, char letterOfWord) =>
        countOfLetter > charOfStartWord[letterOfWord];

    public static bool ContainsLetter(Dictionary<char, int> charOfStartWord, char letterOfWord) =>
        charOfStartWord.ContainsKey(letterOfWord);

    public static bool IsWordAlreadyUsed(List<string> usedWords, string input) =>
        usedWords.Contains(input ?? "");

    public static bool ContainsInvalidCharacters(string input) =>
        Regex.IsMatch(input, _validSymbols);

    public static bool IsLengthValid(string word) =>
        word.Length >= _minCharactersInWord && word.Length <= _maxCharactersInWord;

    public static string? ValidateStartWord()
    {
        if (ContainsInvalidCharacters(GameState.StartWord ?? ""))
            _errorMessage = Game.Properties.Resources.NonAlphabet;
        else
            _errorMessage = $"\n{Game.Properties.Resources.InvalidLength}";
        return _errorMessage;
    }

    public static string? ValidateInputWord()
    {
        if (ContainsInvalidCharacters(GameState.Input ?? ""))
            return $"\n{Game.Properties.Resources.NonAlphabet}";

        if (string.IsNullOrWhiteSpace(GameState.Input))
            return $"\n{Game.Properties.Resources.CantBeNull}";

        if (IsWordAlreadyUsed(GameState.UsedWords, GameState.Input))
            return $"\n{Game.Properties.Resources.TryAnother}";

        foreach (char letterOfWord in GameState.Input.Distinct())
        {
            _countOfLetter = GameState.Input.Count(letter => letter == letterOfWord);

            if (!ContainsLetter(GameState.CharOfStartWord, letterOfWord))
                return $"\n{Game.Properties.Resources.NoLetter.Replace("{letterOfWord}", $"{letterOfWord}")}";
            else if (HasMoreLettersThanStartWord(GameState.CharOfStartWord, _countOfLetter, letterOfWord))
                return $"\n{Game.Properties.Resources.MoreThen.Replace("{letterOfWord}", $"{letterOfWord}")}";
        }

        GameLogic.SwitchPlayer();
        GameLogic.AddWordToUsedWords(GameState.Input);
        return $"\n{Game.Properties.Resources.CorrectWord}";
    }
}

static class UI
{
    public static string? ReadUserInput() =>
        Console.ReadLine()?.ToLower() ?? "";
    public static void ClearUI() =>
        Console.Clear();
    public static void WaitForUser() =>
        Console.ReadKey();
    public static void PrintToUI(string content) =>
        Console.WriteLine(content);
    public static void PrintInLineToUI(string content) =>
        Console.Write(content);
}

static class TimerManager
{
    private const int _turnTimeLimit = 20;
    private const int _timerIntervalMs = 1000;
    public static Task? Timer;
    public static bool IsTimerUp => Timer?.IsCompleted ?? false;
    public static void StartTimer() =>
        Timer = StartTimerAsync(_turnTimeLimit);

    private static async Task StartTimerAsync(int seconds)
    {
        var tcs = new TaskCompletionSource<bool>();
        using var timer = new System.Timers.Timer(_timerIntervalMs);

        timer.Elapsed += (s, e) =>
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