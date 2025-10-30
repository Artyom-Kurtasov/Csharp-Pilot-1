using Language;
using GameRules;

namespace GameMenu
{
    internal class Menu
    {
        public static async Task DisplayMenuAsync()
        {
            while (true)
            {
                UI.ClearUI();
                MenuTextManager.DisplayMenu();
                string? choice = UI.ReadUserInput();

                if (await MenuSelectItem.ProcessSelectionAsync(choice))
                    break;
            }
        }
    }

    static class MenuTextManager
    {
        public static void DisplayMenu()
        {
            UI.PrintInLineToUI($"-------- {Game.Properties.Resources.Menu} --------" +
                $"\n1. {Game.Properties.Resources.Start}" +
                $"\n2. {Game.Properties.Resources.Rules}" +
                $"\n3. {Game.Properties.Resources.ChangeLang}" +
                $"\n4. {Game.Properties.Resources.Exit}" +
                $"\n{Game.Properties.Resources.YourChoice}: ");
        }

       public static void DisplayError()
        {
            UI.PrintToUI($"\n{Game.Properties.Resources.InvalidValue}" +
                $"\n{Game.Properties.Resources.PressAnyKey}");
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
                    UI.WaitForUser();
                    return false;
            }
        }
    }
}


