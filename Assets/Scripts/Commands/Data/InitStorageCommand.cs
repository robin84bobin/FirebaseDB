﻿using System.Collections.Generic;
using Data;
using Data.DataBase;
using UnityEngine;

namespace Commands.Data
{

    public class InitStorageCommand<T> : Command where T : DataItem, new()
    {
        private DataStorage<T> _storage;

        public InitStorageCommand(DataStorage<T> storage)
        {
            _storage = storage;
        }

        public override void Execute()
        {
            Debug.Log(this + " --> " + _storage.CollectionName);
            DataBaseProxy.Instance.Get<T>(_storage.CollectionName, OnGetData);
        }

        private void OnGetData(Dictionary<string, T> items)
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