using System;
using System.Collections.Generic;
using System.Linq;
using InternalNewtonsoft.Json.Linq;
using UnityEngine;

namespace Data
{

    public class BaseStorage<T>  where T : Item, new()
    {
        public int Count
        {
            get { return _items.Count; }
        }


        public bool readOnly { get; protected set; }
        /// <summary>
        /// имя таблицы/коллекции для получения данных
        /// </summary>
        public string sourceName { get; protected set; }

        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public T this[int Id]
        {
            get { return Get(Id); }
            set
            {
                if (readOnly)
                {
                    Debug.LogError(string.Format("Can't set data to '{0}' readOnly storage - Id:{1}",
                        typeof(T), Id));
                    return;
                }

                if (_items.ContainsKey(Id)) _items[Id] = value;
                else _items.Add(Id, value);
            }
        }

        public BaseStorage(string sourceName, bool readOnly = true)
        {
            this.sourceName = sourceName;
            this.readOnly = readOnly;

            GameData.Instance.RegisterStorage(this);
            GameData.Instance.OnDataParseComplete += InitItems;
        }

        /// <summary>
        /// после того как все справочные данные загружены
        /// инициализируем
        /// </summary>
        private void InitItems()
        {
            GameData.Instance.OnInitSuccess -= InitItems;
            foreach (var item in _items)
            {
                item.Value.Init();
            }
        }


        public void SetData(Dictionary<int, T> items)
        {
            _items = items;
        }

        public T GetRandom(Func<T, bool> condition = null)
        {
            List<T> listToSelect = condition == null ?
                GetAll() :
                GetAll().Where(condition).ToList();
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
        public int Id { get; internal set; }

        internal virtual void Init()
        {
        }
    }
}