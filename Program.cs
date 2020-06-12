using System;
using System.Reflection;
using vitamin;
namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Vitamin.initialize();
            //Vitamin.reflex(typeof(ModelUser));

            ViewMain view = (ViewMain)Vitamin.getView(typeof(ViewMain));
            view.enter();

            Vitamin.delay(2000, delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                view.exit();
            });

            Console.ReadKey();
        }
    }


    [CmdRoute("user.send")]
    class CommandUser : CommandBase
    {
        public override  void exec(params object[] args)
        {
            Logger.info("调用到了 CommandUser.");
            this.net.request(new TestMsg(), delegate (object data)
            {
                Logger.log(data.ToString());
            });
        }
    }

    class ModelUser : ModelBase
    {
        public string age = "这里是ModelUser的属性";
        public ModelUser()
        {
            
        }
    }

    class ViewMain : ViewBase
    {
        [Model]
        public ModelUser user;
        public ViewMain() { }

        public override  void enter()
        {
            Logger.log("ViewMain:enter");
            base.enter();

            Logger.info(this.user.age);
            this.execCommand("user.send");
            // var a = new A();
            // a.On("bb", this.eventHandler);
            // a.Emit("bb", "event emit!");
        }

        public override  void exit()
        {
            Logger.log("ViewMain:exit");
            base.exit();
        }

        void eventHandler(object args)
        {
            Console.WriteLine(args);
        }
    }

    class TestMsg : IMsg
    {
        object __data;
        public int routId { get { return 1001; } }
        public int __id__ { get { return 0; } }
        public object data { get { return this.__data; } set { this.__data = value; } }
    }

}