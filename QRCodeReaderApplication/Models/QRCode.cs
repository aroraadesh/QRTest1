using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRCodeReaderApplication.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class QRCode
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<symbol> symbol { get; set; }
    }

    public class symbol
    {
        public int seq { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string error { get; set; }
    }

    public enum FileType
    {
        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Description("QR")]
        qrcode =10,

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Description("QR")]
        barcode = 20,
    }
}