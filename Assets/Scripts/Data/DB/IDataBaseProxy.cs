using System;
using System.Collections.Generic;
using Data;

namespace Assets.Scripts.Data.DB
{
    public interface IDataBaseProxy
    {
        void Init();
        double LastUpdateTime (string tableName_);
        bool IsTableExist(string tableName_);
        void SaveTableData<T>(string tableName_, Dictionary<string, T> dataDictionary_) where T : DataItem;
        void GetTableData<TBaseData> (string tableName_, Action<string, Dictionary<string, TBaseData>> callback_) where TBaseData:DataItem, new();
        void CreateTable<T>(string tableName_);
    }
}


