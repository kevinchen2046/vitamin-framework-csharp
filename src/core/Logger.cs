using System;
namespace vitamin
{
    class Logger
    {
        static public void log(params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Logger.to("[LOG]", args);
        }

        static public void info(params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.to("[INFO]", args);
        }

        static public void warn(params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Logger.to("[WARN]", args);
        }

        static public void debug(params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Logger.to("[DEBUG]", args);
        }

        static public void error(params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Logger.to("[ERROR]", args);
        }

        static private void to(String tag, params object[] args)
        {
            if (!Config.log) return;
            String content = tag+" ";
            foreach (object arg in args)
            {
                content += arg.ToString() + " ";
            }
            Console.WriteLine(content);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}