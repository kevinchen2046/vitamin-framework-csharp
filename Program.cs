using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using vitamin;
using vitamin.utils;
namespace test
{
    class MyEvent : Event
    {
        public static string ENTER = "ENTER";
        public string name = "myname is.xxx";
        public MyEvent(string type, params object[] data) : base(type, data)
        {

        }
    }

    class EventTest
    {
        public EventTest()
        {
            EventEmitter emitter = new EventEmitter();

            emitter.on<Event>("ENTER", (Event e) =>
            {
                Logger.Log(e.ToString());
            });
            emitter.on<MyEvent>(MyEvent.ENTER, (MyEvent e) =>
            {
                Logger.Log(e.ToString());
            });

            emitter.emit<Event>("ENTER", "a", "bc", "hello!!");
            emitter.emit<MyEvent>(MyEvent.ENTER, 1, 2, 3);
        }
    }
    enum ByteArraySize
    {

        SIZE_OF_BOOLEAN = 1,

        SIZE_OF_INT8 = 1,

        SIZE_OF_INT16 = 2,

        SIZE_OF_INT32 = 4,

        SIZE_OF_UINT8 = 1,

        SIZE_OF_UINT16 = 2,

        SIZE_OF_UINT32 = 4,

        SIZE_OF_FLOAT32 = 4,

        SIZE_OF_FLOAT64 = 8
    }
    class Program
    {
        static void Main(string[] args)
        {
            //初始化 维他命
            Vitamin.initialize();
            //Vitamin.reflex(typeof(ModelUser));

            // //从框架内获取一个视图组件
            ViewMain view = Vitamin.getView<ViewMain>();

            //视图组件打开
            view.enter();

            new EventTest();

            Logger.Log(MathUtil.Random(), MathUtil.Random(), MathUtil.Random(), MathUtil.Random());

            int[] list = { 1, 2, 3, 4, 5, 6, 7 };
            CollectionUtil.Shuffle(list);
            Logger.List(list);
            Vitamin.delay(2000, (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                //视图组件关闭
                view.exit();
            });
            
            ByteArray bytes=new ByteArray();
            bytes.WriteByte(212);
            bytes.WriteDouble(5.34245121);
            bytes.WriteFloat(6.34245f);
            bytes.WriteInt(563214);
            bytes.WriteUTF("hellp!!!!");
            bytes.WriteBoolean(false);

            int len=bytes.WriteUTFBytes("some gays!!");
            bytes.Position=0;
            Logger.Log(bytes.ReadByte(),bytes.ReadDouble(),bytes.ReadFloat(),bytes.ReadInt(),bytes.ReadUTF(),bytes.ReadBoolean(),bytes.ReadUTFBytes(len));
            Console.ReadKey();
        }
        private static byte[] Sub(byte[] bytes,int position,int length)
        {
            return bytes.Skip(position).Take(length).ToArray();
        }

        private static byte[] MeagByte(params byte[][] list)
        {
            int len = 0;
            foreach (byte[] bytes in list)
            {
                len += bytes.Length;
            }
            byte[] data = new byte[len];
            int index = 0;
            foreach (byte[] bytes in list)
            {
                bytes.CopyTo(data, index);
                index += bytes.Length;
            }
            return data;
        }

        static private void eventHandler(vitamin.Event e)
        {
            Logger.Debug(e.ToString());
        }
        static private void eventHandler1(MyEvent e)
        {
            Logger.Debug(e.ToString());
        }
    }

    // 这是一个 Commnad的实例
    // 这里需要定义一个Command的路由 用于触发Command
    [CmdRoute("user.send")]
    class CommandUser : CommandBase
    {
        //在Command可以注入任意的Model
        [Model]
        public ModelUser user;

        //Command被执行
        public override void exec(params object[] args)
        {
            Logger.Debug("CommandUser:" + this.user.name);
            this.net.Request(new TestMsg(), delegate (object data)
            {
                Logger.Debug(data.ToString());
            });
        }
    }

    // 这是一个 Model的实例
    class ModelBag : ModelBase
    {
        //在Model可以注入除自身以外的任意的Model
        [Model]
        public ModelUser user;

        //单例的使用
        [Instance]
        public Manager manager;

        public ModelBag()
        {

        }

        //这里是Model的初始化方法，会在框架初始化之前被触发
        public override void initialize()
        {
            Logger.Debug("ModelBag:" + this.user.name);
            Logger.Debug("ModelBag:" + this.manager.name);
        }
    }

    // 这是另外一个 Model的实例
    class ModelUser : ModelBase
    {
        public string name = "【我是ModelUser】";
        public ModelUser()
        {

        }
    }

    // 这是一个View的实例
    class ViewMain : ViewBase
    {
        //在View里可以注入任意的Model
        [Model]
        public ModelUser user;
        public ViewMain() { }

        //视图组件打开
        public override void enter()
        {
            Logger.Log("ViewMain:enter");
            base.enter();

            Logger.Debug("ViewMain:" + this.user.name);
            this.execCommand("user.send");
            this.emit("ENTER", "event", "emit!");
        }

        //视图组件关闭
        public override void exit()
        {
            Logger.Log("ViewMain:exit");
            base.exit();
        }
    }

    class TestMsg : IUpMsg
    {
        object __data;
        public int routId { get { return 1001; } }
        public int __id__ { get { return 0; } }
        public object data { get { return this.__data; } set { this.__data = value; } }
    }

    class Manager
    {
        public string name = "Manager";
    }
}