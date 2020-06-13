using System;
using System.Reflection;
using vitamin;
namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            //初始化 维他命
            Vitamin.initialize();
            //Vitamin.reflex(typeof(ModelUser));

            //从框架内获取一个视图组件
            ViewMain view = (ViewMain)Vitamin.getView(typeof(ViewMain));

            //测试事件机制
            view.on("ENTER",Program.eventHandler);

            //视图组件打开
            view.enter();

            Vitamin.delay(2000, delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                //视图组件关闭
                view.exit();
            });
            Console.ReadKey();
        }
        
        static private void eventHandler(params object[] args){
            Logger.debug(args);
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
            Logger.debug("CommandUser:"+this.user.name);
            this.net.request(new TestMsg(), delegate (object data)
            {
                Logger.debug(data.ToString());
            });
        }
    }

    // 这是一个 Model的实例
    class ModelBag : ModelBase
    {
        //在Model可以注入除自身以外的任意的Model
        [Model]
        public ModelUser user;
        public ModelBag()
        {

        }

        //这里是Model的初始化方法，会在框架初始化之前被触发
        public override void initialize(){
            Logger.debug("ModelBag:"+this.user.name);
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
            Logger.log("ViewMain:enter");
            base.enter();

            Logger.debug("ViewMain:"+this.user.name);
            this.execCommand("user.send");
            this.emit("ENTER", "event", "emit!");
        }

        //视图组件关闭
        public override void exit()
        {
            Logger.log("ViewMain:exit");
            base.exit();
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