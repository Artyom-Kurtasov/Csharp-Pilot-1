using Game.Models;  // Используем общую модель вместо CultureConfig

namespace Language
{
    internal class GameLanguage
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
                    // Убрали прямой вызов CultureConfigurator
                    LanguageMenuManager.DisplaySelectionMessage();
                    Console.ReadKey();
                    break;
                }

                LanguageMenuManager.DisplayError();
                Console.ReadKey();
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
            Console.Clear();
            Console.Write("Выберите язык игры / Choose the game language:" +
                "\n1. Русский (Russian)" +
                "\n2. English (Английский)" +
                "\nВаш выбор (1 или 2) / Your choice (1 or 2): ");
        }

        public static void DisplayError()
        {
            Console.Write("\nНекорректно значение! / Invalid value!" +
                "\nНажмите любую клавишу для продолжения / Press any key to continue.");
        }

        public static void DisplaySelectionMessage()
        {
            if (LanguageOptions.CurrentLanguage == "ru")
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
}
