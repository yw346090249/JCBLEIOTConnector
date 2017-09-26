using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCBLEIoTConnector.Core.Models
{
    public class GatewayDataEntry
    {
        #region --- Properties --- 
        public string eui { get; set; }
        public string loraregion { get; set; }
        public Position posn { get; set; }
        #endregion

        #region --- Constructors ---
        public GatewayDataEntry() { }
        #endregion
    }

    public class Position
    {
        #region --- Properties ---
        public bool gps { get; set; }
        public double lati { get; set; }
        public double lon { get; set; }
        public double alti { get; set; }
        public double tolh { get; set; }
        public double tolv { get; set; }
        #endregion

        #region --- Constructors ---
        public Position() { }
        #endregion
    }
}
