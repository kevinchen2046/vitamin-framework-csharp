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

			var net =new Net();
            foreach (var type in types)
            {
                //获取基类
                var baseType = type.BaseType;
                if (baseType == modelBaseType)
                {
					ModelBase model= Activator.CreateInstance(type) as ModelBase;
					model.initialize();
                    Vitamin.__modles.Add(type,model);
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
						CommandBase cmd= Activator.CreateInstance(type) as CommandBase;
						cmd.net=net;
                        Vitamin.__cmds.Add(des.routId,cmd);
                    }
                }
            }
            Logger.info("Vitamin Start!");
        }

        static public ViewBase getView(Type viewType)
        {
            if (Vitamin.__views.ContainsKey(viewType))
            {
                if (Vitamin.__views.GetValueOrDefault(viewType) == null)
                {
                    ViewBase view = (ViewBase)Activator.CreateInstance(viewType);
                    FieldInfo[] filedInfos = viewType.GetFields();
                    foreach (FieldInfo info in filedInfos)
                    {
                        if (Attribute.IsDefined(info, typeof(Model)))
                        {
                            info.SetValue(view, Vitamin.__modles[info.FieldType]);
                        }
                    }
                    Vitamin.__views[viewType] = view;
                }
            }
            return Vitamin.__views[viewType];
        }


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

        static public void reflex(object instance)
        {
            Logger.log(instance.ToString());
            Vitamin.logFileds(instance.GetType());
            Vitamin.logPropertys(instance.GetType());
            Vitamin.logMethods(instance.GetType());
        }
        static public void reflex(Type type)
        {
            Logger.log(type.ToString());
            Vitamin.logFileds(type);
            Vitamin.logPropertys(type);
            Vitamin.logMethods(type);
        }

        static private void logFileds(Type classType)
        {
            Logger.log(classType.GetCustomAttributes(typeof(ModelClass), true));
            FieldInfo[] filedInfos = classType.GetFields();
            Logger.log("所有字段:" + filedInfos.Length + "--------------------");
            foreach (FieldInfo info in filedInfos)
            {
                Logger.log(info.ToString() + "Model" + Attribute.IsDefined(info, typeof(ModelClass)).ToString());
            }
        }
        static private void logMethods(Type classType)
        {
            MethodInfo[] methods = classType.GetMethods();
            Logger.log("所有方法:" + methods.Length + "--------------------");
            foreach (MethodInfo info in methods)
            {
                Logger.log(info.ToString());
            }
        }
        static private void logPropertys(Type classType)
        {
            PropertyInfo[] properties = classType.GetProperties();
            Logger.log("所有属性:" + properties.Length + "--------------------");
            foreach (PropertyInfo info in properties)
            {
                Logger.log(info.ToString());
            }
        }


        static public void delay(int time, Action<object, System.Timers.ElapsedEventArgs> method)
        {
            System.Timers.Timer t = new System.Timers.Timer(time);//实例化Timer类，设置间隔时间为10000毫秒；
            t.Elapsed += new System.Timers.ElapsedEventHandler(method);//到达时间的时候执行事件；
            t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }
    }


    [AttributeUsage(AttributeTargets.Class)]
    class ModelClass : Attribute
    {
        public ModelClass() { }
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