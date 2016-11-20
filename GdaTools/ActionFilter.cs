using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdaTools
{
    public class ActionFilter
    {
        private DateTime _lastAction = DateTime.MinValue;

        public TimeSpan Delta { get; set; } = new TimeSpan(0,0,0,0,250);

        public void MaybeExecute(Action action)
        {
            DateTime now = DateTime.Now;
            if((now - _lastAction).TotalMilliseconds > Delta.TotalMilliseconds)
            {
                _lastAction = now;
                action();
            }
        }
    }
}
