using System;
using System.Collections.Generic;

namespace Data.DataBase
{
    public interface IDataBaseProxy
    {
        /// <summary>
        /// initialize connection to db etc.
        /// </summary>
        /// <param name="callback"></param>
        void Init(Action callback);

        /// <summary>
        /// get data list of current type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"> data table/collection name in database</param>
        /// <param name="callback"> returns data list </param>
        void Get<T>(string collectionName, Action<Dictionary<string, T>> callback, bool createIfNotExist = true) where T : DataItem, new();

        /// <summary>
        /// save data items to database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="items"></param>
        void SaveCollection<T>(string collectionName, Dictionary<string, T> items) where T : DataItem, new();

        /// <summary>
        /// save data item to database
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        void Save<T>(string collectionName, T item, string id = "") where T : DataItem, new();

    }

    public static class DataBaseProxy
    {
        private static IDataBaseProxy _instance;

        public static IDataBaseProxy Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetInstance();
                }
                return _instance;
            }
        }

        /// <summary>
        /// feel free to make IDataBaseProxy instances if you need ;)
        /// </summary>
        /// <returns></returns>
        private static IDataBaseProxy GetInstance()
        {
            return new FireBaseDbProxy();
        }
    }
}



