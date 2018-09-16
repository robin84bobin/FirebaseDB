using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public class DataStorage<T> where T : DataItem, new()
    {
        public int Count
        {
            get { return _items.Count; }
        }


        public bool ReadOnly { get; protected set; }

        /// <summary>
        /// имя таблицы/коллекции в базе данных
        /// </summary>
        public string CollectionName { get; protected set; }

        private Dictionary<string, T> _items = new Dictionary<string, T>();

        public T this[string id]
        {
            get { return Get(id); }
            set
            {
                if (ReadOnly)
                {
                    Debug.LogError(string.Format("Can't set _messageViewData to '{0}' readOnly storage - Id:{1}",
                        typeof(T), id));
                    return;
                }

                if (_items.ContainsKey(id)) _items[id] = value;
                else _items.Add(id, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"> имя таблицы/коллекции в базе данных</param>
        /// <param name="readOnly"></param>
        public DataStorage(string collectionName, bool readOnly = false)
        {
            CollectionName = collectionName;
            ReadOnly = readOnly;

            App.Data.RegisterStorage(this);
            App.Data.OnDataParseComplete += InitItems;
        }

        /// <summary>
        /// после того как все справочные данные загружены
        /// инициализируем
        /// </summary>
        private void InitItems()
        {
            App.Data.OnDataParseComplete -= InitItems;
            foreach (var item in _items)
            {
                item.Value.Init();
            }
        }

        #region SET DATA METHODS

        public void Set(T item, string id = "", bool saveNow = false)
        {
            if (_items.ContainsKey(id))
            {
                _items[id] = item;
                Debug.Log(
                    string.Format("Replace _messageViewData in '{0}' storage eg. Id:{1} already exists", CollectionName, id));
            }
            else
            {
                item.Id = id;
                _items.Add(id, item);
                Debug.Log(string.Format("Add _messageViewData in '{0}' storage  Id:{1}", CollectionName, id));
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

        
        public void SetData(Dictionary<string, T> items)
        {
            _items = items;
        }

        #endregion

        
        #region GET DATA METHODS

        
        public T GetRandom(Func<T, bool> condition = null)
        {
            var listToSelect = condition == null ? GetAll() : GetAll().Where(condition).ToList();
            var randomIndex = UnityEngine.Random.Range(0, listToSelect.Count - 1);
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


        public T Get(string id)
        {
            if (!_items.ContainsKey(id))
            {
                Debug.LogError(string.Format("Can't get _messageViewData from '{0}' storage - Id:{1}", typeof(T), id));
                return default(T);
            }

            return _items[id];
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

        public bool Exists(string id)
        {
            return _items.ContainsKey(id);
        }

        
        public void Clear()
        {
            _items.Clear();
        }

        
        public void Remove(string id, bool now = false)
        {
            if (_items.ContainsKey(id))
                _items.Remove(id);
            else
                Debug.LogError(this + ":: Try to remove unexisted item: Id = " + id);
        }
    }

    
    public class DataItem
    {
        public string Id = String.Empty;

        internal virtual void Init()
        {}
    }
}