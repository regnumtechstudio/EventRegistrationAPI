using DataAccess.General;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.common;
using Models.General;
using static Models.ClsGloble;

namespace Event_Resistration_APIs.Controllers.General
{
    [Route("api/[controller]")]
    [ApiController]
    public class eventController : ControllerBase
    {
        private readonly ILogger<eventController> _logger;
        private readonly IeventMaster _ieventmaster;
        private LoginInUserInfo _loggedInUserInfo;
        public eventController(ILogger<eventController> logger, IeventMaster ieventmaster)
        {
            _logger = logger;
            _ieventmaster = ieventmaster;
        }


        [HttpPost]
        [Route("eventGet")]
        public async Task<IActionResult> eventGet([FromBody] eventMaster eventmaster)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                response.data = await _ieventmaster.eventGet(eventmaster);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex
                    , string.Concat(ControllerContext.ActionDescriptor.ControllerName
                    , ControllerContext.ActionDescriptor.ActionName));

                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };
                return Ok(response);
            }
            return Ok(response);
        }


        [HttpPost]
        [Route("eventList")]
        public async Task<IActionResult> eventList([FromBody] eventMaster eventmaster)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                response.data = await _ieventmaster.eventGetList(eventmaster);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex
                    , string.Concat(ControllerContext.ActionDescriptor.ControllerName
                    , ControllerContext.ActionDescriptor.ActionName));

                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };
                return Ok(response);
            }
            return Ok(response);
        }
    }
}
