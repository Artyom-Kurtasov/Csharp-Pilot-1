namespace GameRules
{
    static class Rules
    {
        public static void DisplayRules()
        {
            Console.WriteLine($"\n{Game.Properties.Resources.RuleString1}" +
                $"\n{Game.Properties.Resources.RuleString2}" +
                $"\n{Game.Properties.Resources.RuleString3}" +
                $"\n\n{Game.Properties.Resources.PressAnyKey}");

            Console.ReadKey();
        }
    }
}

