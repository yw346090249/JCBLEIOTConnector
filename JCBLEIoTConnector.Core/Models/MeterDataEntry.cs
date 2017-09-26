/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>

using System;

//using Microsoft.WindowsAzure.Storage.Table;

namespace JCBLEIoTConnector.Core.Models
{
    public class MeterDataEntry
    {
        // <TODO>: Construct JCBLE smart meter data entry. Due to I didn't get sample data. 
        #region --- Properties ---
        public GatewayDataEntry gw { get; set; }
        public MoteDataEntry mote { get; set; }
        public AppDataEntry app { get; set; }
        #endregion
    }
}
