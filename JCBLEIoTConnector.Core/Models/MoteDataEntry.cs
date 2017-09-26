using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace JCBLEIoTConnector.Core.Models
{
    public class MoteDataEntry : TableEntity
    {
        #region --- Private members ---
        private string _eui = string.Empty;
        #endregion

        #region --- Properties ---
        public string eui { get { return _eui; } set { _eui = value; RowKey = value; }}
        public JOIN join { get; set; }
        #endregion

        #region --- Constructors ---
        public MoteDataEntry()
        {
            PartitionKey = "JCBLE";
            RowKey = _eui;
        }
        #endregion
    }

    public class JOIN
    {
        #region --- Properties ---
        public string appeui { get; set; }
        #endregion
    }
}
