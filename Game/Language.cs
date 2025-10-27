using CultureConfig;
using Game.Models;  

namespace Language
{
    internal class GameLanguage
    {
        public static void ChooseLanguage()
        {
            while (true)
            {
                LanguageMenuManager.DisplayMenu();
                string? choice = UI.ReadUserInput();

                if (LanguageValidator.IsValidChoice(choice))
                {
                    LanguageChoose.SetLanguage(choice);
                    LanguageMenuManager.DisplaySelectionMessage();
                    CultureConfigurator.ConfigureCulture();
                    UI.WaitForUser();
                    break;
                }

                LanguageMenuManager.DisplayError();
                UI.WaitForUser();
            }
        }
    }

    static class LanguageChoose
    {
        public static void SetLanguage(string? choice)
        {
            LanguageOptions.CurrentLanguage = choice == "1" ? "ru" : "en";
        }
    }

    static class LanguageValidator
    {
        public static bool IsValidChoice(string? choice)
        {
            return choice == "1" || choice == "2";
        }
    }

    static class LanguageMenuManager
    {
        public static void DisplayMenu()
        {
            UI.ClearUI();
            UI.PrintInLineToUI("Выберите язык игры / Choose the game language:" +
                "\n1. Русский (Russian)" +
                "\n2. English (Английский)" +
                "\nВаш выбор (1 или 2) / Your choice (1 or 2): ");
        }

        public static void DisplayError()
        {
            UI.PrintToUI("\nНекорректно значение! / Invalid value!" +
                "\nНажмите любую клавишу для продолжения / Press any key to continue.");
        }

        public static void DisplaySelectionMessage()
        {
            if (LanguageOptions.CurrentLanguage == "ru")
            {
                UI.PrintToUI("\nВыбран русский язык." +
                    "\nНажмите любую клавишу для продолжения.");
            }
            else
            {
                UI.PrintToUI("\nEnglish language chosen." +
                    "\nPress any key to continue.");
            }
        }
    }
}
