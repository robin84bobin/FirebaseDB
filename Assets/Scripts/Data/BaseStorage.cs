using System;
using System.Collections.Generic;
using System.Linq;
using InternalNewtonsoft.Json.Linq;
using UnityEngine;

namespace Data
{
    public class BaseStorage<T> where T : Item, new()
    {
        public int Count
        {
            get { return _items.Count; }
        }


        public bool ReadOnly { get; protected set; }

        /// <summary>
        /// имя таблицы/коллекции для получения данных
        /// </summary>
        public string CollectionName { get; protected set; }

        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public T this[int id]
        {
            get { return Get(id); }
            set
            {
                if (ReadOnly)
                {
                    Debug.LogError(string.Format("Can't set data to '{0}' readOnly storage - Id:{1}",
                        typeof(T), id));
                    return;
                }

                if (_items.ContainsKey(id)) _items[id] = value;
                else _items.Add(id, value);
            }
        }

        public BaseStorage(string collectionName, bool readOnly = false)
        {
            CollectionName = collectionName;
            ReadOnly = readOnly;

            GameData.Instance.RegisterStorage(this);
            GameData.Instance.OnDataParseComplete += InitItems;
        }

        /// <summary>
        /// после того как все справочные данные загружены
        /// инициализируем
        /// </summary>
        private void InitItems()
        {
            GameData.Instance.OnDataParseComplete -= InitItems;
            foreach (var item in _items)
            {
                item.Value.Init();
            }
        }

        #region SET DATA METHODS

        public void Set(T item, int id, bool saveNow = false)
        {
            if (_items.ContainsKey(id))
            {
                _items[id] = item;
                Debug.Log(
                    string.Format("Replace data in '{0}' storage eg. objectId:{1} already exists", CollectionName, id));
            }
            else
            {
                item.id = id;
                _items.Add(id, item);
                Debug.Log(string.Format("Add data in '{0}' storage  objectId:{1}", CollectionName, id));
            }

            if (saveNow) SaveItem(item);
        }

        private void SaveItem(T item)
        {
            DataBase.DataBaseProxy.Instance.Save(CollectionName, item);
        }

        public void SaveData()
        {
            if (ReadOnly)
            {
                Debug.LogError("Can't write into read only storage: " + GetType().Name);
                return;
            }

            DataBase.DataBaseProxy.Instance.SaveCollection(CollectionName, _items);
        }

        public void SetData(Dictionary<int, T> items)
        {
            _items = items;
        }

        #endregion

        #region GET DATA METHODS

        public T GetRandom(Func<T, bool> condition = null)
        {
            List<T> listToSelect = condition == null ? GetAll() : GetAll().Where(condition).ToList();
            int randomIndex = UnityEngine.Random.Range(0, listToSelect.Count - 1);
            return listToSelect[randomIndex];
        }

        public List<T> GetAll(Func<T, bool> condition)
        {
            return GetAll().Where(condition).ToList();
        }

        public List<T> GetAll()
        {
            return _items.Values.ToList();
        }


        public T Get(int Id)
        {
            if (!_items.ContainsKey(Id))
            {
                Debug.LogError(string.Format("Can't get data from '{0}' storage - Id:{1}", typeof(T), Id));
                return default(T);
            }

            return _items[Id];
        }


        public T Get(Func<T, bool> predicate)
        {
            return _items.Values.FirstOrDefault(predicate);
        }

        #endregion

        #region foreach methods realization 

        public BaseStorageEnumerator GetEnumerator()
        {
            return new BaseStorageEnumerator(_items.Values.ToArray());
        }


        public class BaseStorageEnumerator
        {
            int _currentItem;
            int _itemsLength;
            T[] _items;

            public BaseStorageEnumerator(T[] items)
            {
                _currentItem = -1;
                _itemsLength = items.Length;
                _items = items;
            }

            public T Current
            {
                get { return _items[_currentItem]; }
            }

            public bool MoveNext()
            {
                if (_currentItem == (_itemsLength - 1))
                {
                    _currentItem = 0;
                    return false;
                }

                _currentItem++;
                return true;
            }
        }

        #endregion
    }

    public class Item
    {
        public int id = -1;

        internal virtual void Init()
        {
        }
    }
}