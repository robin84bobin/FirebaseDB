using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Commands
{
    public abstract class Command
    {
        public event Action OnComplete = delegate { };
        public abstract void Do();

        protected void Complete()
        {
            OnComplete.Invoke();
            OnComplete = delegate { };
        }
    }
}
