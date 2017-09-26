/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JCBLEIoTConnector.Core.Models;
using log4net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;

namespace JCBLEIoTConnector.Core
{
    public class StorageConnector
    {
        #region --- Private members ---
        private CloudStorageAccount _storageAccount = null;
        private string _storageAccountConnectionString = string.Empty;
        private CloudBlobClient _dataBlob = null;
        private CloudBlobContainer _dataContainer = null;
        private static string _containerName = "data";
        private ILog _infoLogger = null;
        private ILog _errorLogger = null;
        #endregion

        #region --- Constructors ---
        public StorageConnector(string connectionString)
        {
            _storageAccountConnectionString = connectionString;
            _infoLogger = LogManager.GetLogger(this.GetType());
            _errorLogger = LogManager.GetLogger(this.GetType());
        }
        #endregion

        #region --- Methods ---
        public void Open()
        {
            try
            {
                _storageAccount = CloudStorageAccount.Parse(_storageAccountConnectionString);

                _dataBlob = _storageAccount.CreateCloudBlobClient();
                _dataContainer = _dataBlob.GetContainerReference(_containerName);
                _dataContainer.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("Opening Storage Account encounter error : {0}", 
                    ex.ToString().Replace("\r\n", " "));
            }
        }

        public void Close()
        {
            // Do nothing.
        }

        public async Task SaveDataToBlobAsync(string data)
        {
            try
            {
                CloudAppendBlob blob = _dataContainer.GetAppendBlobReference(GetBlobName());
                if (!blob.Exists())
                {
                    blob.CreateOrReplace();
                }
                await blob.AppendTextAsync(data);
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("SaveDataToBlobAsync function encounter error : {0}",
                    ex.ToString().Replace("\r\n", " "));
            }

        }

        private string GetBlobName()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + "/data.json";
        }
        #endregion
    }
}
