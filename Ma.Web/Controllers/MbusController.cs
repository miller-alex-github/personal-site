using Common;
using MBusLib;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Ma.Web.Controllers
{
    public class MbusController : Controller
    {
        public IActionResult Index()
        {
            var example = new MBusData
            {
                InputText = "684D4D680801720100000096150100180000000C785600000001FD1B0002FC0348522574440D22FC0348522574F10C12FC034852257463110265B409226586091265B70901720072650000B2016500001FB316"
            };

            return View(example);
        }

        [HttpPost]
        [Route("[controller]")]
        public IActionResult ParseMbus([FromForm]MBusData data)
        {
            data.OutputText = string.Empty;
            data.Error = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var buffer = Util.HexStringToByteArray(data.InputText);
                    var response = RSP_UD.Parse(buffer);
                    data.OutputText = response.Print(data.IsExpert);                    
                }
                catch (Exception exc)
                {
                    data.Error = exc.Message;
                }
            }

            return View("Index", data);
        }
    }
}
