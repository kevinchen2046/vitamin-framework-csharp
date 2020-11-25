using System;

namespace vitamin
{

    public interface IMsg
    {
        int routId { get; }
        int __id__ { get; }
        object data { get; set; }
    }

    public class Net
    {
        public void request(IMsg msg, Action<object> method)
        {

            Vitamin.delay(1000, delegate (object sender, System.Timers.ElapsedEventArgs arg)
            {
                method(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            });
        }

        public void notify(IMsg msg)
        {

        }
    }
}