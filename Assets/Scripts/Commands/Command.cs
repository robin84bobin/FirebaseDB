using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Commands
{
    public abstract class Command
    {
        public event Action OnComplete = delegate { };
        public abstract void Execute();

        protected virtual void Release()
        {
            OnComplete = delegate { };
        }

        protected void Complete()
        {
            OnComplete.Invoke();
            Release();
        }
    }
}
