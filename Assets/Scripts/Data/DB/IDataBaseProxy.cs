using System;

namespace Data
{
    internal interface IDataBaseProxy
    {
        void Init(Action <string> callback);
    }
}



