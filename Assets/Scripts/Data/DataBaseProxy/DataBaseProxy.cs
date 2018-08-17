using System;
using System.Collections.Generic;

namespace Data
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
        /// <param name="sourceName"> data table/collection name in database</param>
        /// <param name="callback"> returns data list </param>
        void Get<T>(string sourceName, Action<Dictionary<int, T>> callback) where T : Item, new();

        /// <summary>
        /// save data items to database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceName"></param>
        /// <param name="items"></param>
        void SaveCollection<T>(string sourceName, Dictionary<int, T> items) where T : Item, new();
    }

    public class DataBaseProxy
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
            return new FireBaseDBProxy();
        }
    }
}



