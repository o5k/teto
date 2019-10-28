using System;
using System.Collections.Generic;

namespace Teto {
    /// <summary>
    /// A not stupidly ugly console.
    /// </summary>
    public class The {
        /// <summary>
        /// The log level to display at.
        /// </summary>
        public enum LogLevel {
            Low,
            Normal,
            Warning,
            Error,
            Failure
        }

        /// <summary>
        /// Primary colors (for the text)
        /// </summary>
        static Dictionary<LogLevel, ConsoleColor> primaryColors = new Dictionary<LogLevel, ConsoleColor>() {
            { LogLevel.Low, ConsoleColor.Gray },
            { LogLevel.Normal, ConsoleColor.White },
            { LogLevel.Warning, ConsoleColor.Yellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Failure, ConsoleColor.Magenta }
        };

        /// <summary>
        /// Secondary colors (for the timestamp)
        /// </summary>
        static Dictionary<LogLevel, ConsoleColor> secondaryColors = new Dictionary<LogLevel, ConsoleColor>() {
            { LogLevel.Low, ConsoleColor.DarkGray },
            { LogLevel.Normal, ConsoleColor.Gray },
            { LogLevel.Warning, ConsoleColor.DarkYellow },
            { LogLevel.Error, ConsoleColor.DarkRed },
            { LogLevel.Failure, ConsoleColor.DarkMagenta }
        };

        /// <summary>
        /// The Teto logo for display purposes.
        /// </summary>
        static string[] Logo = new string[] {
            @"  _______________________________",
            @" /__  ___  _________  ___  ___  /",
            @"   / /  / /__      / /  / /  / / ",
            @"  / /  / ___/     / /  / /  / /  ",
            @" / /  / /________/ /  / /__/ /   ",
            @"/_/  /____________/  /______/    "
        };

        /// <summary>
        /// The logo colors. (Please make the length of this array match the length of Logo, thanks. What do you mean "your code sucks")
        /// </summary>
        static ConsoleColor[] LogoColors = new ConsoleColor[] {
            ConsoleColor.Magenta,
            ConsoleColor.DarkMagenta,
            ConsoleColor.Cyan,
            ConsoleColor.DarkCyan,
            ConsoleColor.Blue,
            ConsoleColor.DarkBlue,
        };

        /// <summary>
        /// Draw the logo and also a line under it as a header.
        /// </summary>
        public static void DrawLogo() {
            int offset = (Console.BufferWidth - Logo[0].Length) / 2;
            
            for (int i = 0; i < Logo.Length; i++) {
                Console.ForegroundColor = LogoColors[i];
                Console.WriteLine(new string(' ', offset) + Logo[i]);
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('_', Console.BufferWidth));
        }

        /// <summary>
        /// Write a log line to the console.
        /// </summary>
        /// <param name="level">The log level to be written.</param>
        /// <param name="text">The text to be logged.</param>
        public static void WriteLine(LogLevel level, string text) {
            Console.ForegroundColor = secondaryColors[level];
            Console.Write($"[{ DateTime.Now.ToLongTimeString() }] ");

            Console.ForegroundColor = primaryColors[level];
            int indentation = Console.CursorLeft;
            int availableWidth = Console.BufferWidth - indentation;

            while (text.Length > 0) {
                if (text.Length < availableWidth) {
                    Console.WriteLine(text);
                    break;
                }
                else if (text.Length == availableWidth) {
                    Console.Write(text);
                    break;
                } else {
                    Console.Write(text.Substring(0, availableWidth) + new string(' ', indentation));
                    text = text.Substring(availableWidth);
                }
            }
        }

        #region Shortcuts
        /// <summary>
        /// Write a log line with level Low.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        public static void Low(string text) {
            WriteLine(LogLevel.Low, text);
        }
        /// <summary>
        /// Write a log line with level Normal.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        public static void Normal(string text) {
            WriteLine(LogLevel.Normal, text);
        }
        /// <summary>
        /// Write a log line with level Warning.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        public static void Warning(string text) {
            WriteLine(LogLevel.Warning, text);
        }
        /// <summary>
        /// Write a log line with level Error.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        public static void Error(string text) {
            WriteLine(LogLevel.Error, text);
        }
        /// <summary>
        /// Write a log line with level Failure.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        public static void Failure(string text) {
            WriteLine(LogLevel.Failure, text);
        }
        #endregion
    }
}
