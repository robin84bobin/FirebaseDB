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
        void Get<T>(string sourceName, Action<List<T>> callback) where T : Item, new();
    }
}



