/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCBLEIoTConnector.Core.Models
{
    public class Payload
    {
        #region --- Properties ---
        public int type { get; set; }
        public int code { get; set; }
        public float bought { get; set; }
        public float left { get; set; }
        public float used { get; set; }
        public int status { get; set; }
        public string version { get; set;}
        public string time { get; set; }
        #endregion

        #region --- Contructor ---
        public Payload()
        {

        }

        public Payload(int type, int code, float bought, float left, float used, int status, string version, string time)
        {
            this.type = type;
            this.code = code;
            this.bought = bought;
            this.left = left;
            this.used = used;
            this.status = status;
            this.version = version;
            this.time = time;
        }
        #endregion
    }
}
