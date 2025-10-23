namespace WordGame
{
    static class MyGame
    {
        public static async Task StartGameAsync()
        {
            Console.Clear();
            GameLogic.ClearUsedWords();

            GameLogic.SetStartWord();
            GameLogic.BuildLetterDictionary();

            GameLogic.StartGameLoop();
        }
    }

    static class GameLogic
    {
        public static void SetStartWord()
        {
            while (true)
            {
                Console.Write($"\n{Game.Properties.Resources.WriteF8T30}: ");
                GameInput.StartWordInput();

                if (GameValidatorLogic.IsLengthValid(GameState.StartWord) &&
                    !GameValidatorLogic.ContainsInvalidCharacters(GameState.StartWord))
                {
                    AddWordToUsedWords(GameState.StartWord);
                    break;
                }

                Console.WriteLine(GameValidatorLogic.ValidateStartWord());
                Console.WriteLine($"\n{Game.Properties.Resources.PressAnyKey}");
                Console.ReadKey();
                Console.Clear();
            }
        }

        public static void StartGameLoop()
        {
            while (true)
            {
                UpdatePlayerState();
                GameTextLogic.DisplayRoundInfo();

                TimerManager.StartTimer();
                GameInput.ReadInput();

                if (TimerManager.IsTimerUp)
                {
                    DetermineWinner();
                    GameTextLogic.DisplayEndGameMessage();
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine(GameValidatorLogic.ValidateInputWord());
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

    static class GameInput
    {
        public static void StartWordInput() =>
            GameState.StartWord = Console.ReadLine()?.ToLower() ?? "";
        public static void ReadInput() =>
            GameState.Input = Console.ReadLine()?.ToLower() ?? "";
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
            Console.WriteLine($"\n{GameTextState.TimesUp} {GameTextState.Winner}" +
                $"\n{Game.Properties.Resources.PressAnyKey}");
        }

        public static void DisplayRoundInfo()
        {
            Console.WriteLine($"\n{Game.Properties.Resources.StartWord}{GameState.StartWord}");
            Console.Write($"{GameTextState.WriteWord}");
        }
    }

    static class GameValidatorLogic
    {
        private static string? _errorMessage;
        private static int _countOfLetter;

        public static bool HasMoreLettersThanStartWord(Dictionary<char, int> charOfStartWord, int countOfLetter, char letterOfWord) =>
            countOfLetter > charOfStartWord[letterOfWord];

        public static bool ContainsLetter(Dictionary<char, int> charOfStartWord, char letterOfWord) =>
            charOfStartWord.ContainsKey(letterOfWord);

        public static bool IsWordAlreadyUsed(List<string> usedWords, string input) =>
            usedWords.Contains(input ?? "");

        public static bool ContainsInvalidCharacters(string input) =>
            Regex.IsMatch(input, @"[^a-zA-Zа-яА-Я]");

        public static bool IsLengthValid(string word) =>
            word.Length >= 8 && word.Length <= 30;

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

    static class TimerManager
    {
        public static Task? Timer;
        public static bool IsTimerUp => Timer?.IsCompleted ?? false;

        public static void StartTimer() =>
            Timer = StartTimerAsync(20);

        private static async Task StartTimerAsync(int seconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            using var timer = new System.Timers.Timer(1000);

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
}
