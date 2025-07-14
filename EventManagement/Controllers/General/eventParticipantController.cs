using Azure;
using DataAccess.General;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MimeKit;
using Models.common;
using Models.General;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.IdentityModel.Tokens.Jwt;
using Utilities;
using static Models.ClsGloble;

namespace Event_Resistration_APIs.Controllers.General
{
    [Route("api/[controller]")]
    [ApiController]
    public class eventParticipantController : ControllerBase
    {
        private readonly ILogger<eventParticipantController> _logger;
        private readonly IparticipantMaster _iclientMaster;
        private readonly IotpManager _iotpManager;
        private readonly IemailService _iEmailService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private LoginInUserInfo _loggedInUserInfo;
        public eventParticipantController(ILogger<eventParticipantController> logger, IparticipantMaster iclientMaster, IotpManager iotpManager, IemailService iEmailService, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _iclientMaster = iclientMaster;
            _iotpManager = iotpManager;
            _iEmailService = iEmailService;
            _hostingEnvironment = hostingEnvironment;
        }



        // [Authorise]
        //************ ADD IN NEW CONTROLLER (EVENT_Participant) ***********************        
       
        [HttpPost]
        [Route("tokenIssueStatusUpdate")]
        public async Task<IActionResult> tokenIssueStatusUpdate([FromBody] participantMaster clientmaster)
        {
            var response = new genericResponse(); 
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }

                _loggedInUserInfo = JwtHelper.getLoginUserInfoByJwtToken(Convert.ToString(Request.Headers["authorization"]));
                if (_loggedInUserInfo == null)
                {
                    return Unauthorized();
                }

                clientmaster.userid = (int)_loggedInUserInfo.userId;
                var result = await _iclientMaster.tokenIssueStatusUpdate(clientmaster);

                #region << Comment >>
                //if (result.success == (int)enumResponseCode.success)
                //{
                //    clientmaster.mode = "ByID";
                //    clientmaster.clientId = clientmaster.clientId;

                //    var particiantObj = await _iclientMaster.get(clientmaster);
                //    if (particiantObj != null)
                //    {
                //        emailServiceModel mailRequest = new emailServiceModel();
                //        mailRequest.ishtml = true;
                //        //mailRequest.subject = @"Request review notification for " + (particiantObj.email == clientmaster.email + "Event Registration") + ". | Regnum Wealth";
                //        mailRequest.subject = @"Thank You for register with our event" + ". | Regnum Wealth";

                //        string templateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "templates", "Event Registration template.html");
                //        FileInfo file = new FileInfo(templateFilePath);

                //        if (file.Exists)
                //        {
                //            //read html template
                //            string mailBodyStr = System.IO.File.ReadAllText(file.FullName);
                //            mailBodyStr = mailBodyStr.Replace("{{Name}}", particiantObj.fullName);
                //            mailBodyStr = mailBodyStr.Replace("{{referenceNo}}", particiantObj.referenceNo);
                //            mailRequest.body = mailBodyStr;

                //            mailRequest.recipents = new List<string> { particiantObj.email };
                //            mailRequest.serviceType = (int)enumEmailServiceType.GeneralService;
                //            var emailResponse = await _iEmailService.SendEMailAsync(mailRequest);

                //            response.success = emailResponse.success;
                //            response.data = clientmaster;
                //            //dynamic wrapper = new ExpandoObject();
                //            //wrapper.referenceNo = clientmaster.referenceNo;
                //            //response.data = wrapper;

                //        }
                //        else
                //        {
                //            response.success = (int)enumResponseCode.fail;
                //            response.message = "participant registeration done.But unable to send email";
                //            response.data = new participantMaster();
                //        }
                //    }
                //    else
                //    {
                //        response.success = (int)enumResponseCode.fail;
                //        response.message = "participant registeration done.But unable to send email";
                //        response.data = new participantMaster();
                //    }
                //}
                //else if (response.success == (int)enumResponseCode.exist)
                //{
                //    response.message = "Participant is already register for event";
                //    response.data = new participantMaster();
                //}
                //else
                //{
                //    response.success = (int)enumResponseCode.fail;
                //    response.message = "Unable to register for event";
                //    response.data = new participantMaster();
                //}
                //return Ok(response);

                #endregion

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    string.Concat(ControllerContext.ActionDescriptor.ControllerName,
                    ControllerContext.ActionDescriptor.ActionName));

                return BadRequest(string.Concat("error occurs:", exception.Message));
            }
        }

        [HttpPost]
        [Route("getById")]
        public async Task<IActionResult> partcipantDetailGet([FromBody] participantMaster clientmaster)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                clientmaster.mode = "ById";
                response.data = await _iclientMaster.get(clientmaster);
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
        [Route("getByReferenceNo")]
        public async Task<IActionResult> getByReferenceNo([FromBody] participantMaster clientmaster)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                clientmaster.mode = " ";
                response.data = await _iclientMaster.get(clientmaster);
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
        [Route("getList")]
        public async Task<IActionResult> getList([FromBody] participantMaster clientmaster)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }

                response.data = await _iclientMaster.getList(clientmaster);
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
        [Route("getTokenIssuedStatus")]
        public async Task<IActionResult> getTokenIssuedCount([FromBody] participantMaster clientmaster)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                clientmaster.mode = "issued";
                response.data = await _iclientMaster.tokenGiftIssuedStatus(clientmaster);
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
        [Route("getTokenNotIssuedStatus")]
        public async Task<IActionResult> getTokenNotIssuedCount([FromBody] participantMaster clientmaster)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                clientmaster.mode = "notIssued";
                response.data = await _iclientMaster.tokenGiftIssuedStatus(clientmaster);
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
