using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets.Scripts.Factories.DataFactories.JsonFactories;
using Data;
using Data.DataBase;
using InternalNewtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.Data.DB
{
    public class JsonProxy : IDataBaseProxy
    {
        public const string FOLDER_PATH = "JSON";

        public void Init()
        {
            //init smthng...
        }

        public void Get<T>(string collection, Action<Dictionary<string, T>> callback, bool createIfNotExist = true) where T : DataItem, new()
        {
            throw new NotImplementedException();
        }

        public void SaveCollection<T>(string collection, Dictionary<string, T> items, Action callback = null) where T : DataItem, new()
        {
            throw new NotImplementedException();
        }

        public void Save<T>(string collection, T item, string id = "", Action<T> callback = null) where T : DataItem, new()
        {
            throw new NotImplementedException();
        }

        public void Remove<T>(string collection, string id = "", Action<string> callback = null)
        {
            throw new NotImplementedException();
        }

        public event Action OnInitialized;

        public double LastUpdateTime(string tableName_)
        {
            //throw new NotImplementedException();
            return 0f;
        }

        public bool IsTableExist(string path_)
        {
            bool exists = File.Exists(path_);
            if (!exists) {
                Debug.LogWarning("FILE NOT FOUND: " + path_);
            }

            return exists;
        }

        public void SaveTableData<T>(string tableName_, Dictionary<string, T> dataDictionary_) where T : DataItem
        {
            SaveTable<T>(tableName_, dataDictionary_);
        }

        private void SaveTable<T>(string tableName_, Dictionary<string, T> dataDictionary_) where T : DataItem
        {
            var filePath = Application.streamingAssetsPath + "/" + FOLDER_PATH + "/" + tableName_ + ".json";
            if (!IsTableExist(filePath))
            {
                //return;
            }

            StreamWriter writer = File.CreateText(filePath);
            var sourceString = CreateJsonFromDict<T>(dataDictionary_);
            writer.Write(sourceString.ToCharArray());
            writer.Close();

            //save backup
            SaveBackup(tableName_, sourceString);
        }

        private void SaveTable(string tableName_, IDictionary dataDictionary_)
        {
            var filePath = Application.streamingAssetsPath + "/" + FOLDER_PATH + "/" + tableName_ + ".json";
            if (!IsTableExist(filePath))
            {
                return;
            }

            StreamWriter writer = File.CreateText(filePath);
            var sourceString = CreateJsonFromDict(dataDictionary_);
            writer.Write(sourceString.ToCharArray());
            writer.Close();

            //save backup
            SaveBackup(tableName_, sourceString);
        }

        void SaveBackup(string tableName, string source)
        {
            tableName = tableName + System.DateTime.Now.ToString("__yyyy_MM_dd__hh_mm_ss");
            var filePath = Application.streamingAssetsPath + "/" + FOLDER_PATH + "/backups/" + tableName + ".json";
            StreamWriter writer = File.CreateText(filePath);
            writer.Write(source.ToCharArray());
            writer.Close();
        }

        private string CreateJsonFromDict<T>(Dictionary<string, T> dict_) where T : DataItem
        {
            StringBuilder sb = new StringBuilder("{ \"collection\": [ \n");
            string dataStr = string.Empty;
            foreach (var dataItem in dict_.Values)
            {
                dataStr = JsonFactory.Instance.ParseData<T>((T)dataItem);
                sb.Append(dataStr);
                sb.Append(",\n");
            }
            sb.Append("\n]}");

            return sb.ToString();
        }

        private string CreateJsonFromDict(IDictionary dict_)
        {
            StringBuilder sb = new StringBuilder("{ \"collection\": [ \n");
            string dataStr = string.Empty;
            foreach (var dataItem in dict_.Values) {
                dataStr = JsonUtility.ToJson(dataItem);
                sb.Append(dataStr);
                sb.Append(",\n");
            }
            sb.Append("\n]}");

            return sb.ToString();
        }

        public void GetTableData<T>(string tableName_, Action<string, Dictionary<string, T>> callback_)
            where T : DataItem, new()
        {
            var sourceString = string.Empty;
            var filePath = Application.streamingAssetsPath + "/" + FOLDER_PATH + "/" + tableName_ + ".json";
            if (!IsTableExist(filePath)){
                callback_.Invoke(tableName_, null);
                return;
            }

            sourceString = File.ReadAllText(filePath);

            var resultDict = new Dictionary<string, T>();
            var jsonRoot = JObject.Parse(sourceString);

            foreach (var jsonObject in jsonRoot["collection"])
            {
                var dataItem = JsonFactory.Instance.Create<T>(jsonObject.ToString());
                resultDict.Add(dataItem.Id, dataItem);
            }

         /*   for (int index = 0; index < jsonRoot["collection"].Children; index++) {
                var jsonObject = jsonRoot["collection"].list[index];
                var dataItem = JsonFactory.Instance.Create<T>(jsonObject.ToString());
                resultDict.Add(dataItem.Id, dataItem);
            }*/

            callback_.Invoke(tableName_, resultDict);
        }

        public void CreateTable<T>(string tableName_)
        {
            throw new NotImplementedException();
        }
    }
}