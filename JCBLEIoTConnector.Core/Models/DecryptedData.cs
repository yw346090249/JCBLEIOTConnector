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
    class DecryptedData
    {
        #region --- Properties ---
        public uint ProtocolVersion { get; set; }
        public int Token { get; set; }              // int ?
        public uint OperationCode { get; set; }
        public string GatewayEUI { get; set; }      // string type?
        public string RawJSON { get; set; }
        #endregion

        #region --- Methods ---
        public void Parse(byte[] data)
        {
            if (null == data)
                return; // empty.
            if (12 >= data.Length)
                return; // Invalid data.
            int index = 0;
            ProtocolVersion = (uint)data[index];
            index++;
            Token = BitConverter.ToInt16(data, index);
            index += 2;
            OperationCode = (uint)data[index];
            index++;
            GatewayEUI = BitConverter.ToString(data, index, 4);
            index += 4;
            RawJSON = BitConverter.ToString(data, 12);
        }
        #endregion
    }
}
