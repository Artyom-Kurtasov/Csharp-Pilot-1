using Game.Models;

namespace CultureConfig
{ 
    static class CultureConfigurator
    {
        public static void ConfigureCulture()
        {
            var culture = new CultureInfo(LanguageOptions.CurrentLanguage ?? "ru");
            Game.Properties.Resources.Culture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
