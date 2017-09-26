/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>

using System;

namespace JCBLEIoTConnector.Core.Models
{
    public class FinalDataEntry
    {
        #region --- Properties ---
        public MeterDataEntry originData { get; set; }
        public Payload userData { get; set; }
        #endregion

        #region --- Contructor ---
        public FinalDataEntry()
        {

        }
        #endregion
    }
}
