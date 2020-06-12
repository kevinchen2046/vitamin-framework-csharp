using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
namespace vitamin
{
    class Config
    {
        static public bool log = true;
    }

    class Vitamin
    {
        static Dictionary<Type, ViewBase> __views;
        static Dictionary<Type, ModelBase> __modles;
        static Dictionary<string, CommandBase> __cmds;
        static public void initialize()
        {
            Vitamin.__views = new Dictionary<Type, ViewBase>();
            Vitamin.__modles = new Dictionary<Type, ModelBase>();
            Vitamin.__cmds = new Dictionary<string, CommandBase>();

            var types = Assembly.GetCallingAssembly().GetTypes();
            var modelBaseType = typeof(ModelBase);
            var viewBaseType = typeof(ViewBase);
            var cmdBaseType = typeof(CommandBase);

            var net = new Net();
            foreach (var type in types)
            {
                //获取基类
                var baseType = type.BaseType;
                if (baseType == modelBaseType)
                {
                    ModelBase model = Activator.CreateInstance(type) as ModelBase;
                    Vitamin.__modles.Add(type, model);
                }
                else if (baseType == viewBaseType)
                {
                    Vitamin.__views.Add(type, null);
                }
                else if (baseType == cmdBaseType)
                {
                    CmdRoute des = (CmdRoute)type.GetCustomAttribute(typeof(CmdRoute));
                    if (des == null)
                    {
                        Logger.error(baseType.ToString() + "没有添加描述信息!");
                    }
                    else
                    {
                        CommandBase cmd = Activator.CreateInstance(type) as CommandBase;
                        cmd.net = net;
                        Vitamin.__cmds.Add(des.routId, cmd);
                    }
                }
            }

            while (true)
            {
                var allInject = true;
                foreach (var model in Vitamin.__modles)
                {
                    bool result = Vitamin.injectModel(model.Value, model.Value.GetType());
                    if (!result) allInject = false;
                }
                if (allInject)
                {
                    foreach (var model in Vitamin.__modles){
                        model.Value.initialize();
                    }
                    break;
                }
            }
            while (true)
            {
                var allInject = true;
                foreach (var cmd in Vitamin.__cmds)
                {
                    bool result = Vitamin.injectModel(cmd.Value, cmd.Value.GetType());
                    if (!result) allInject = false;
                }
                if (allInject){
                    break;
                }
            }
            Logger.info("Vitamin Start!");
        }
        static private bool injectModel(object target, Type type)
        {
            bool result = true;
            var modelType=typeof(Model);
            FieldInfo[] filedInfos = type.GetFields();
            foreach (FieldInfo info in filedInfos)
            {
                if (Attribute.IsDefined(info,modelType))
                {
                    if (!Vitamin.__modles.ContainsKey(info.FieldType))
                    {
                        result = false;
                        continue;
                    }
                    info.SetValue(target, Vitamin.__modles.GetValueOrDefault(info.FieldType));
                }
            }
            return result;
        }
        /// <summary>
        /// 获取组件
        /// 通过框架接口获取的组件才会有相关的依赖注入
        /// </summary>
        static public ViewBase getView(Type viewType)
        {
            if (Vitamin.__views.ContainsKey(viewType))
            {
                if (Vitamin.__views.GetValueOrDefault(viewType) == null)
                {
                    ViewBase view = (ViewBase)Activator.CreateInstance(viewType);
                    Vitamin.injectModel(view, viewType);
                    Vitamin.__views[viewType] = view;
                }
            }
            return Vitamin.__views[viewType];
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        static public void execCommand(string cmdRoute, params object[] args)
        {

            if (Vitamin.__cmds.ContainsKey(cmdRoute))
            {
                CommandBase cmd = Vitamin.__cmds.GetValueOrDefault(cmdRoute);
                if (cmd != null)
                {
                    cmd.exec(args);
                }
                else
                {
                    Logger.error("无法执行命令:" + cmdRoute);
                }
            }
        }

        /// <summary>
        /// 输出对象反射信息
        /// </summary>
        static public void reflex(object instance)
        {
            Logger.log(instance.ToString());
            Vitamin.logFileds(instance.GetType());
            Vitamin.logPropertys(instance.GetType());
            Vitamin.logMethods(instance.GetType());
        }

        /// <summary>
        /// 输出类型反射信息
        /// </summary>
        static public void reflex(Type type)
        {
            Logger.log("//=============反射"+type.ToString()+"=============");
            Vitamin.logFileds(type);
            Vitamin.logPropertys(type);
            Vitamin.logMethods(type);
            Logger.log("===========================================//");
        }

        static private void logFileds(Type classType)
        {
            FieldInfo[] filedInfos = classType.GetFields();
            Logger.log("/---------"+"字段:" + filedInfos.Length + "---------");
            foreach (FieldInfo info in filedInfos)
            {
                Logger.log(info.Name);
            }
            Logger.log("--------------------------/");
        }
        static private void logMethods(Type classType)
        {
            MethodInfo[] methods = classType.GetMethods();
            Logger.log("/---------"+"方法:" + methods.Length + "---------");
            foreach (MethodInfo info in methods)
            {
                Logger.log(info.Name);
            }
            Logger.log("--------------------------/");
        }
        static private void logPropertys(Type classType)
        {
            PropertyInfo[] properties = classType.GetProperties();
            Logger.log("/---------"+"属性:" + properties.Length + "---------");
            foreach (PropertyInfo info in properties)
            {
                Logger.log(info.Name);
            }
            Logger.log("--------------------------/");
        }

        static public void delay(int time, Action<object, System.Timers.ElapsedEventArgs> method)
        {
            System.Timers.Timer t = new System.Timers.Timer(time);//实例化Timer类，设置间隔时间为10000毫秒；
            t.Elapsed += new System.Timers.ElapsedEventHandler(method);//到达时间的时候执行事件；
            t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        static public void loop(int time, Action<object, System.Timers.ElapsedEventArgs> method)
        {
            System.Timers.Timer t = new System.Timers.Timer(time);//实例化Timer类，设置间隔时间为10000毫秒；
            t.Elapsed += new System.Timers.ElapsedEventHandler(method);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    class CmdRoute : Attribute
    {
        public string routId;
        public CmdRoute(string routeId)
        {
            this.routId = routeId;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    class Model : Attribute
    {
        public Model() { }
    }
}