using System;
namespace vitamin
{
    class Logger
    {
        static public void log(params object[] args)
        {
            Logger.to("[LOG]", ConsoleColor.White,args);
        }

        static public void info(params object[] args)
        {
            Logger.to("[INFO]", ConsoleColor.Green,args);
        }

        static public void warn(params object[] args)
        {
            Logger.to("[WARN]", ConsoleColor.Yellow,args);
        }

        static public void debug(params object[] args)
        {
            Logger.to("[DEBUG]", ConsoleColor.Cyan,args);
        }

        static public void error(params object[] args)
        {
            Logger.to("[ERROR]",ConsoleColor.Red, args);
        }

        static public void to(String tag,ConsoleColor color, params object[] args)
        {
            if (!Config.log) return;
            Console.ForegroundColor = color;
            String content = tag + " ";
            foreach (object arg in args)
            {
                content += arg.ToString() + " ";
            }
            Console.WriteLine(content);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}