using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.DataBase;

namespace Assets.Scripts.Commands
{
    public class InitStorageCommand<T> : Command where T : Item, new()
    {
        private BaseStorage<T> _storage;

        public InitStorageCommand(BaseStorage<T> storage)
        {
            _storage = storage;
        }

        public override void Execute()
        {
            DataBaseProxy.Instance.Get<T>(_storage.CollectionName, OnGetData);
        }

        private void OnGetData(Dictionary<int, T> items)
        {
            _storage.SetData(items);
            Complete();
        }

        protected override void Release()
        {
            base.Release();
            _storage = null;
        }

    }
}
