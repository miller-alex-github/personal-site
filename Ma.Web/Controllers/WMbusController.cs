using Common;
using Common.Report;
using MBusLib;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ma.Web.Controllers
{
    public class WMbusController : Controller
    {
        public IActionResult Index()
        {
            var example = new MBusData
            {
                InputText = "6644496A3100015514377203926314496A00075000500598A78E0D71AA6358EEBD0B20BFDF99EDA2D22FA25314F3F1B84470898E495303923770BA8DDA97C964F0EA6CE24F5650C0A6CDF3DE37DE33FBFBEBACE4009BB0D8EBA2CBE80433FF131328206020B1BF",
                SecretKey = "F8B24F12F9D113F680BEE765FDE67EC0"
            };

            return View(example);
        }

        [HttpPost]
        [Route("[controller]")]
        public IActionResult Parse([FromForm]MBusData data, string command)
        {
            data.OutputText = string.Empty;
            data.Error = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var buffer = Util.HexStringToByteArray(data.InputText);
                    var frame = WMBusFrame.Parse(buffer, new Keys(data.SecretKey));                                                                            
                    data.OutputText = frame.Print(data.IsExpert);

                    if (command.Equals("pdf"))
                    {
                        var stream = new MemoryStream();                        
                        ReportMeterExpert.Create(frame, stream, false);
                        stream.Position = 0;

                        var productName = string.IsNullOrEmpty(frame.ProductName) ? string.Empty : "_" + MakeValidFileName(frame.ProductName);
                        productName = productName.Replace(" ", "_");
                        var fileName = $"WMBUS_{frame.Header.ID_BCD.ToString("X8")}{productName}.pdf";

                        HttpContext.Response.Headers.Add("Content-Disposition", $"attachment;filename={fileName}");
                        return new FileStreamResult(stream, "application/pdf"); 
                    }
                }
                catch (Exception exc)
                {
                    data.Error = exc.Message;
                }
            }

            return View("Index", data);
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        private class Keys : IKeyContainer
        {
            private readonly string secretKey;

            public Keys(string secretKey) => this.secretKey = secretKey;

            public byte[] GetKey(uint iD_BCD, ushort manufacturer, byte generation, Medium medium)
            {
                return string.IsNullOrEmpty(secretKey) ? null : Util.HexStringToByteArray(secretKey);
            }
        }
    }
}
