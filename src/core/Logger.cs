using System;
namespace vitamin
{
    class Logger
    {
        static public void Log(params object[] args)
        {
            Logger.to("[LOG]", ConsoleColor.White, args);
        }
        static public void List(object[] args)
        {
            Logger.to("[LIST]", ConsoleColor.White, vitamin.utils.CollectionUtil.Join(args, ","));
        }
        static public void List(int[] args)
        {
            Logger.to("[LIST]", ConsoleColor.White, vitamin.utils.CollectionUtil.Join(args, ","));
        }
        static public void List(string[] args)
        {
            Logger.to("[LIST]", ConsoleColor.White, vitamin.utils.CollectionUtil.Join(args, ","));
        }
        static public void List(float[] args)
        {
            Logger.to("[LIST]", ConsoleColor.White, vitamin.utils.CollectionUtil.Join(args, ","));
        }
        static public void Info(params object[] args)
        {
            Logger.to("[INFO]", ConsoleColor.Green, args);
        }

        static public void Warn(params object[] args)
        {
            Logger.to("[WARN]", ConsoleColor.Yellow, args);
        }

        static public void Debug(params object[] args)
        {
            Logger.to("[DEBUG]", ConsoleColor.Cyan, args);
        }

        static public void Error(params object[] args)
        {
            Logger.to("[ERROR]", ConsoleColor.Red, args);
        }

        static public void to(string tag, ConsoleColor color, params object[] args)
        {
            if (!Config.log) return;
            Console.ForegroundColor = color;
            string content = tag + " ";
            foreach (object arg in args)
            {
                content += arg.ToString() + " ";
            }
            Console.WriteLine(content);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}