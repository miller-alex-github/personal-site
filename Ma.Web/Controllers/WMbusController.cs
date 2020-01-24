using Common;
using MBusLib;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Ma.Web.Controllers
{
    public class WMbusController : Controller
    {
        public IActionResult Index()
        {
            var example = new MBusData
            {
                InputText = "5e44496a4271003988047a740050059a32cea3529700656bf1f3bb619dbfa4ef0cc1123d35ac00ae8d74a98672520e06cbfb8737a2ae2e57d253752bf86c30ee88927e374c0cef10bf4f68052b369f85413464329ffd5370a0ee3ebd287926",
                SecretKey = "E04D7077201F4372D8DCF904A4C990B2"
            };

            return View(example);
        }

        [HttpPost]
        [Route("[controller]")]
        public IActionResult Parse([FromForm]MBusData data)
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
                }
                catch (Exception exc)
                {
                    data.Error = exc.Message;
                }
            }

            return View("Index", data);
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
