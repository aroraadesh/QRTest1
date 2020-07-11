using QRCodeReaderApplication.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace QRCodeReaderApplication.Controllers
{
    /// <summary>
    /// API to
    /// </summary>
    [RoutePrefix("api/Reader")]
    public class QRReadController : ApiController
    {
        private static int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  
        private static IList<string> AllowedvalidatExtention = new List<string> { ".png", ".jpg", "jpeg" };
        private static string tempFilePath = WebConfigurationManager.AppSettings["tempFilePath"];
        private static string qrReadApi = WebConfigurationManager.AppSettings["qrReadApi"];
        public static object Strings { get; private set; }

        [Route("QRRead")]
        [HttpPost]
        public HttpResponseMessage FileReader()
        {
            string returnValue = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            try
            {
                if (ValidateVideoRequest(httpRequest, true).IsSuccessStatusCode)
                {
                    var postedFile = httpRequest.Files["file"];
                    string filePath = tempFilePath + postedFile.FileName;
                    Byte[] fileInfo = WriteFileToTempLocation(postedFile, filePath);

                    if (null != postedFile && postedFile.ContentLength > 0)
                    {
                        returnValue = GetQRCodeValue(filePath,qrReadApi);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid input");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid input"); 
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, returnValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetQRCodeValue(string filePath , string url)
        {
            string QRCodeValue = string.Empty;
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("file", filePath);
            IRestResponse response = client.Execute(request);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<QRCode> listQRCode = serializer.Deserialize<List<QRCode>>(response.Content);
            foreach (QRCode item in listQRCode)
            {
                if (item.type == "qrcode")
                {
                    foreach (symbol symbol in item.symbol)
                    {
                        if (string.IsNullOrEmpty(symbol.error))
                        {
                            QRCodeValue = symbol.data;
                        }
                        else
                        {
                            QRCodeValue = symbol.error;
                        }
                    }
                }
            }
            return QRCodeValue;
        }

        private HttpResponseMessage ValidateVideoRequest(HttpRequest request, bool validateExtention)
        {
            var postedFile = request.Files["file"];
            if (postedFile != null && postedFile.ContentLength > 0)
            {

                var extension = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.')).ToLower();
                if (validateExtention && !AllowedvalidatExtention.Contains(extension))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Please upload image with valid Extension");
                }
                else if (postedFile.ContentLength > MaxContentLength)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Input file is too large. Please upload file size upto " + MaxContentLength + "MB");
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);

        }


        private Byte[] WriteFileToTempLocation(HttpPostedFile file, string filePath)
        {
            if (null != file && file.ContentLength > 0)
            {
                // Process file 
                Stream fs = file.InputStream;
                BinaryReader br = new BinaryReader(fs);  //reads the   binary files
                Byte[] bytes = br.ReadBytes((Int32)fs.Length);  //converting file into bytes
                File.WriteAllBytes(filePath, bytes); //storing file in temp location 
                return bytes;

            }
            return null;
        }
    }
}
