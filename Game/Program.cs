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
