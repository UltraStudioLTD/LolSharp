using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Spectre.Console;

namespace LolSharp
{
    class Program
    {
        public static char CheckMark = '✔';
        public static char CrossMark = '✘';
        public static bool ShowVersion { get; set; }
        public static int Versionint { get; set; }
        public static bool ShowHelp { get; set; }
        public static int Helpint { get; set; }
        public static int Frequency { get; set; }
        static void Main(string[] args = null)
        {
            bool supportsTrueColor;
            if (AnsiConsole.Console.Profile.Capabilities.ColorSystem == ColorSystem.TrueColor)
            {
                supportsTrueColor = true;
            }
            else
            {
                supportsTrueColor = false;
            }
            if (supportsTrueColor)
            {
                if (args is null || args.Length == 0)
                {
                    LolSharp.HelpMessage();
                }
                else
                {
                    ShowHelp = ShowVersion = false;
                    if (args.Contains("-v") || args.Contains("--version"))
                    {
                        ShowVersion = true;
                        Versionint = args.Contains("-v") ? args.ToList().IndexOf("-v") : args.ToList().IndexOf("--version");
                    }
                    if (args.Contains("-h") || args.Contains("--help"))
                    {
                        ShowHelp = true;
                        Helpint = args.Contains("-h") ? args.ToList().IndexOf("--help") : args.ToList().IndexOf("--version");
                    }
                    if (ShowHelp && ShowVersion)
                    {
                        if (Versionint > Helpint)
                        {
                            LolSharp.VersionDisplay();
                            Environment.Exit(0);
                        }
                        else
                        {
                            LolSharp.HelpMessage();
                            Environment.Exit(0);
                        }
                    }
                    else if (ShowHelp && !ShowVersion)
                    {
                        LolSharp.HelpMessage();
                        Environment.Exit(0);
                    }
                    else if (ShowVersion && !ShowHelp)
                    {
                        LolSharp.VersionDisplay();
                        Environment.Exit(0);
                    }
                    else if (!ShowVersion && !ShowHelp)
                    {
                        Frequency = 220;
                        if (args.Contains("-f") || args.Contains("--frequency"))
                        {
                            if (args.Length < 3)
                            {
                                AnsiConsole.MarkupLine("[bold red]Need At least 3 Arguments During Frequency Changing![/]");
                                Environment.Exit(0);
                            }
                            else
                            {
                                try
                                {
                                    Frequency = Convert.ToInt32(args[1]);
                                }
                                catch (Exception ex)
                                {
                                    AnsiConsole.MarkupLine("[bold red]Frequency Must Be Valid INT![/]");
                                    AnsiConsole.WriteException(ex);
                                    Environment.Exit(0);
                                }
                                if (args.Length > 3)
                                {
                                    List<string> arguments = args.ToList();
                                    arguments.RemoveAt(0);
                                    arguments.RemoveAt(0);
                                    LolSharp.Run(string.Join(" ", arguments.ToArray()), Frequency);
                                }
                                else
                                {
                                    LolSharp.RunFromFile(args[2], Frequency);
                                }
                            }
                        }
                        else
                        {
                            if (args.Length > 1)
                            {
                                List<string> arguments = args.ToList();
                                LolSharp.Run(string.Join(" ", arguments.ToArray()), Frequency);
                            }
                            else
                            {
                                LolSharp.RunFromFile(args[0], Frequency);
                            }
                        }
                    }
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold red]{CrossMark}[/][italic red] Terminal Color System is not supported![/]");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine($"Your Current Terminal Support Level Is {AnsiConsole.Console.Profile.Capabilities.ColorSystem}\nYour Terminal Must Support True Color aka 16m (24-bit)!");
                Console.ResetColor();
            }
        }
    }
    public static class LolSharp
    {
        /// <summary>
        /// LolSharp runner (string)
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="freq">Frequency</param>
        public static void Run(string text, int freq = 220, string end = "\n")
        {
            foreach ((int i, string c) in Extensions.Enumerate(text))
            {
                foreach ((int j, _) in Extensions.Enumerate(c))
                {
                    AnsiConsole.Markup($"[rgb({Rainbow(freq, (i * 10) + j).Item1},{Rainbow(freq, (i * 10) + j).Item2},{Rainbow(freq, (i * 10) + j).Item3})]" + c + "[/]");
                }
            }
            AnsiConsole.Markup(end);
        }
        public static string RainbowTextGenerator(string text, int freq = 220, string end = "\n")
        {
            StringBuilder @string = new();
            foreach ((int i, string c) in Extensions.Enumerate(text))
            {
                foreach ((int j, _) in Extensions.Enumerate(c))
                {
                    //AnsiConsole.Markup($"[rgb({Rainbow(freq, (i * 10) + j).Item1},{Rainbow(freq, (i * 10) + j).Item2},{Rainbow(freq, (i * 10) + j).Item3})]" + c + "[/]");
                    @string.Append($"[rgb({Rainbow(freq, (i * 10) + j).Item1},{Rainbow(freq, (i * 10) + j).Item2},{Rainbow(freq, (i * 10) + j).Item3})]" + c + "[/]");
                }
            }
            @string.Append(end);
            return @string.ToString();
        }
        public static void RunFromFile(string path, int freq = 220)
        {
            if (File.Exists(path))
            {
                Run(File.ReadAllText(path), freq);
            }
            else if (File.Exists(Path.Combine(Environment.CurrentDirectory, path)))
            {
                Run(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, path)), freq);
            }
            else
            {
                Run(path, freq);
            }
        }
        /// <summary>
        /// LolSharp runner (strings)
        /// </summary>
        /// <param name="text">texts</param>
        /// <param name="freq">Frequency</param>
        public static void Run(string[] text, int freq = 220, string end = "\n")
        {
            foreach ((int i, string c) in Extensions.Enumerate(text))
            {
                AnsiConsole.Markup($"[rgb({Rainbow(freq, i).Item1},{Rainbow(freq, i).Item2},{Rainbow(freq, i).Item3})]" + c + "[/]");
            }
            AnsiConsole.Markup(end);
        }
        /// <summary>
        /// LolSharp runner (character)
        /// </summary>
        /// <param name="character">character</param>
        /// <param name="freq">Frequency</param>
        public static void Run(char character, int freq = 220, string end = "\n") => Run(character.ToString(), freq, end);
        /// <summary>
        /// LolSharp runner (characters)
        /// </summary>
        /// <param name="characters">characters</param>
        /// <param name="freq">Frequency</param>
        public static void Run(char[] characters, int freq = 220, string end = "\n")
        {
            foreach ((int i, string c) in Extensions.Enumerate(characters))
            {
                AnsiConsole.Markup($"[rgb({Rainbow(freq, i).Item1},{Rainbow(freq, i).Item2},{Rainbow(freq, i).Item3})]" + c + "[/]");
            }
            AnsiConsole.Markup(end);
        }
        /// <summary>
        /// Creates RGB values
        /// </summary>
        /// <param name="freq">Frequency, more the value; more the colours</param>
        /// <param name="i">Current character position, used to set colours at character level</param>
        /// <returns>RGB values in tuple</returns>
        public static Tuple<int, int, int> Rainbow(int freq, int i) => new((int)((Math.Sin((freq * i) + 0) * 127) + 128), (int)((Math.Sin((freq * i) + (2 * Math.PI / 3)) * 127) + 128), (int)((Math.Sin((freq * i) + (4 * Math.PI / 3)) * 127) + 128));
        public static void VersionDisplay()
        {
            PanelHeader panelHeader = new("Version");
            Panel panel = new(RainbowTextGenerator("LolSharp", end: "") + " Version [red]v[/][orangered1]1[/][yellow].[/][green]0[/][cyan].[/][violet]0[/]")
            {
                UseSafeBorder = true,
                Header = panelHeader,
                Border = BoxBorder.Rounded
            };
            panel.BorderColor(Color.Aqua);
            AnsiConsole.Render(panel);
        }
        public static void HelpMessage()
        {
            string RAINBOW = RainbowTextGenerator("RAINBOW", end: "");
            List<Tuple<string, string, string>> ArgumentList = new()
            {
                new("[yellow]Optional[/]", "[red]-h[/],[red] --help[/]", "Prints [red]help[/] message"),
                new("[yellow]Optional[/]", "[yellow]-v[/], [yellow]--version[/]", "Prints [yellow]version[/] of LolSharp"),
                new("[yellow]Optional[/]", "[green]-f[/], [green]--frequency[/] [bold italic green]FREQUENCY[/]", "Frequency, higher the value == more the colours"),
                new("[red]Positional[/]", "[cyan]TEXT[/]", $"Display's {RAINBOW} version of [cyan]TEXT[/]"),
                new("[red]Positional[/]", "[violet]FILE[/]", $"Display's {RAINBOW} version of [violet]FILE[/]'s contents")
            };
            Table argumentList = new()
            {
                UseSafeBorder = true,
                Border = TableBorder.Heavy,
                Title = new TableTitle(RainbowTextGenerator("Arguments List", end: ""))
            };
            argumentList.BorderColor(Color.Aquamarine1);
            argumentList.AddColumns("[bold italic green]Argument Type[/]", "[bold italic blue]Arguments[/]", "[bold italic purple]Argument Description[/]");
            foreach (Tuple<string, string, string> argument in ArgumentList)
            {
                argumentList.AddRow(argument.Item1, argument.Item2, argument.Item3);
            }
            Panel panel = new(argumentList)
            {
                Header = new PanelHeader($"{ RainbowTextGenerator("LolSharp", end: "") } [red]v[/][orangered1]1[/][yellow].[/][green]0[/][cyan].[/][violet]0[/]")
            };
            AnsiConsole.MarkupLine(RainbowTextGenerator("LolSharp - C# version of https://github.com/busyloop/lolcat \n\t--> Usage: lolsharp (ARGUMENT)... (FILE | TEXT)", end: ""));
            AnsiConsole.Render(panel);
        }
    }
    public static class Extensions
    {
        /// <summary>
        /// Enumerator (string)
        /// </summary>
        /// <param name="obj">string object</param>
        /// <returns>Tuple(int, string)</returns>
        public static IEnumerable<Tuple<int, string>> Enumerate(string obj) => obj.Select((x, i) => new Tuple<int, string>(i, x.ToString()));
        /// <summary>
        /// Enumerator (strings)
        /// </summary>
        /// <param name="obj">strings array</param>
        /// <returns>Tuple(int, string)</returns>
        public static IEnumerable<Tuple<int, string>> Enumerate(string[] obj) => obj.Select((x, i) => new Tuple<int, string>(i, x));
        /// <summary>
        /// Enumerator (characters)
        /// </summary>
        /// <param name="obj">characters array</param>
        /// <returns>Tuple(int, string)</returns>
        public static IEnumerable<Tuple<int, string>> Enumerate(char[] obj) => obj.Select((x, i) => new Tuple<int, string>(i, x.ToString()));
    }
}