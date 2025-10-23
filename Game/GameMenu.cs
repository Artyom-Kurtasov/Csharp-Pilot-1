using Language;
using GameRules;
using WordGame;

namespace GameMenu
{
    internal class Menu
    {
        public static async Task DisplayMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                MenuTextManager.DisplayMenu();
                string? choice = Console.ReadLine() ?? "";

                if (await MenuSelectItem.ProcessSelectionAsync(choice))
                    break;
            }
        }
    }

    static class MenuTextManager
    {
        public static void DisplayMenu()
        {
            Console.Write($"-------- {Game.Properties.Resources.Menu} --------" +
                $"\n1. {Game.Properties.Resources.Start}" +
                $"\n2. {Game.Properties.Resources.Rules}" +
                  $"\n3. {Game.Properties.Resources.ChangeLang}" +
                 $"\n4. {Game.Properties.Resources.Exit}" +
                $"\n{Game.Properties.Resources.YourChoice}: ");
        }

        public static void DisplayError()
        {
            Console.WriteLine($"{Game.Properties.Resources.InvalidValue}");
        }
    }

    static class MenuSelectItem
    {
        public static async Task<bool> ProcessSelectionAsync(string? choice)
        {
            switch (choice)
            {
                case "1":
                    await MyGame.StartGameAsync();
                    return false;

                case "2":
                    Rules.DisplayRules();
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
}

