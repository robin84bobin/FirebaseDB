using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Commands
{
    public class InitStorageCommand<T> : Command where T : Item, new()
    {
        private DataStorage<T> _storage;

        public InitStorageCommand(DataStorage<T> storage)
        {
            _storage = storage;
        }

        public override void Do()
        {
            BaseStorage.dbProxy.Get<T>(_storage.sourceName, OnGetData);
        }

        private void OnGetData(List<T> dataList)
        {
            _storage.SetData(dataList.ToArray());
            Complete();
        }

    }
}
