using DataAccess.General;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.common;
using Models.General;
using Newtonsoft.Json;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Dynamic;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using static Models.ClsGloble;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Net.WebRequestMethods;

namespace Event_Resistration_APIs.Controllers.General
{
    [Route("api/[controller]")]
    [ApiController]
    public class eventRegistrationController : ControllerBase
    {
        private readonly ILogger<eventRegistrationController> _logger;
        private readonly IparticipantMaster _iclientMaster;
        private readonly IotpManager _iotpManager;
        private readonly IemailService _iEmailService;
        private LoginInUserInfo _loggedInUserInfo;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        public eventRegistrationController(ILogger<eventRegistrationController> logger, IConfiguration configuration, IparticipantMaster iclientMaster, IotpManager iotpManager
            , IemailService iEmailService
            , IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _configuration = configuration;
            _iclientMaster = iclientMaster;
            _iotpManager = iotpManager;
            _iEmailService = iEmailService;
            _hostingEnvironment = hostingEnvironment;
        }

        // [Anonymous]
        //************ ADD IN NEW CONTROLLER (EVENT_REGISTRATION) **********************

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> add([FromBody] participantMaster clientmaster)
        {
            var response = new genericResponse();
            try
            {   
                var clientMasterResponse = new participantMaster();
            
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }

                clientmaster.eventId = 1;
                response = await _iclientMaster.add(clientmaster);

                clientMasterResponse.referenceNo = response.data;

                if (response.success == (int)enumResponseCode.success && string.IsNullOrEmpty(Convert.ToString(clientMasterResponse.referenceNo)).Equals(false))
                {
                    clientmaster.mode = "ByRefenceNo";
                    clientmaster.referenceNo = clientMasterResponse.referenceNo;
                    response.data = clientMasterResponse;

                    var particiantObj = await _iclientMaster.get(clientmaster);                   
                    if (particiantObj != null)
                    {
                        emailServiceModel mailRequest = new emailServiceModel();
                        mailRequest.ishtml = true;
                        //mailRequest.subject = @"Request review notification for " + (particiantObj.email == clientmaster.email + "Event Registration") + ". | Regnum Wealth";
                        mailRequest.subject = @"Thank You for register with us"+ ". | Regnum Wealth";

                        string templateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "templates", "Event Registration template.html");
                        FileInfo file = new FileInfo(templateFilePath);

                        if (file.Exists)
                        {
                            try
                            {
                                // registration email message
                                //read html template
                                string mailBodyStr = System.IO.File.ReadAllText(file.FullName);
                                mailBodyStr = mailBodyStr.Replace("{{Name}}", particiantObj.fullName);
                                mailBodyStr = mailBodyStr.Replace("{{referenceNo}}", particiantObj.referenceNo);
                                mailRequest.body = mailBodyStr;
                               
                                mailRequest.recipents = new List<string> { particiantObj.email };
                                mailRequest.serviceType = (int)enumEmailServiceType.GeneralService;
                                var emailResponse = await _iEmailService.SendEMailAsync(mailRequest);

                                //response.success = emailResponse.success;
                                response.data = clientMasterResponse;
                                //dynamic wrapper = new ExpandoObject();
                                //wrapper.referenceNo = clientmaster.referenceNo;
                                //response.data = wrapper;
                           
                                // registration text message
                                var client = new HttpClient();
                                string apiUrl = "http://msg.msgclub.net/rest/services/sendSMS/sendGroupSms?AUTH_KEY=168939ed48874647bce2ee7721c997";
                                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

                                var smsResponse = new StringContent(JsonConvert.SerializeObject(new smsServiceModel()
                                {
                                    smsContent = "Thank you for registering with Regnum. Your reference number is " +particiantObj.referenceNo+
                                    ". Please collect your free gift from our counter. - Team Regnum Wealth !",
                                    routeId = "8",
                                    mobileNumbers = particiantObj.mobileNo,
                                    senderId = "RGNM",
                                    smsContentType = "ENGLISH",
                                }), System.Text.Encoding.UTF8, "application/json");

                                await client.PostAsync(apiUrl, smsResponse);                               
                            }
                            catch (Exception ex)
                            {
                                response.message = ex.Message.ToString();
                            }                            
                            return Ok(response);

                        }
                        else
                        {
                            response.success = (int)enumResponseCode.fail;
                            response.message = "participant registeration done.But unable to send email";
                            response.data = new participantMaster();
                        }
                    }
                    else
                    {
                        response.success = (int)enumResponseCode.fail;
                        response.message = "participant registeration done.But unable to send email";
                        response.data = new participantMaster();
                    }
                }                
                else if(response.success == (int)enumResponseCode.exist)
                {
                    response.message = "Participant is already register for event";
                    response.data = new participantMaster();
                }
                else
                {
                    response.success = (int)enumResponseCode.fail;
                    response.message = "Unable to register for event";
                    response.data = new participantMaster();
                }
                return Ok(response);

            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    string.Concat(ControllerContext.ActionDescriptor.ControllerName,
                    ControllerContext.ActionDescriptor.ActionName));
                //throw;
                return BadRequest(string.Concat("error occurs:", exception.Message));
            }
        }

        [HttpPost]
        [Route("generateEmailOTP")]
        public async Task<IActionResult> generateEmailOTP([FromBody] otpManager otpManager)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }

                Random rnd = new Random();
                int rand_num = rnd.Next(100000, 999999);
                otpManager.otpCode = rand_num.ToString();
                otpManager.mobile = string.Empty;
                otpManager.isActive = true;

                response = await _iotpManager.add(otpManager);

                if (response.success == (int)enumResponseCode.success)
                {
                    otpManager.mode = "email";
                    otpManager.email = otpManager.email;

                    var otpobj = await _iotpManager.get(otpManager);
                    string templateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "templates", "otp Notification.html");
                    FileInfo file = new FileInfo(templateFilePath);

                    if (file.Exists)
                    {
                        //read html template
                        string mailBodyStr = System.IO.File.ReadAllText(file.FullName);
                        mailBodyStr = mailBodyStr.Replace("{{email}}", otpManager.email);
                        mailBodyStr = mailBodyStr.Replace("{{otp}}", otpManager.otpCode);

                        //send mail 
                        var emailResponse = await _iEmailService.SendEMailAsync(new emailServiceModel
                        {
                            serviceType = (int)enumEmailServiceType.GeneralService,
                            parentId = -1,
                            parentType = (int)enumMailLogParentType.SignupOTPVerification,
                            recipents = new List<string> { otpManager.email },
                            subject = "One-time Password for registration",
                            body = mailBodyStr
                            //+ "Your one-time password (OTP) is " + otpManager.otpCode + "\r\n\r\n" +
                            //    "Please enter the above OTP to Event Registration." +
                            //    "\r\n\r\nThis OTP is valid for next 10 minutes only."
                        });

                        response.success = emailResponse.success;
                        response.data = emailResponse.data;
                    }
                }
                else
                {
                    response.success = (int)enumResponseCode.fail;
                    response.message = "unable to Generate Email OTP";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Concat(ControllerContext.ActionDescriptor.ControllerName
                    , ControllerContext.ActionDescriptor.ActionName));

                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };
                return Ok(response);
            }
        }

        [HttpPost]
        [Route("verifyEmailOTP")]
        public async Task<IActionResult> verifyEmailOTP([FromBody] otpManager otpManager)
        {
            var response = new genericResponse();
            try
            {
                string emOtp = "";

                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }

                if (string.IsNullOrEmpty(Convert.ToString(otpManager.otpCode)))
                {
                    return BadRequest("OTP code must required.");
                }

                if (_configuration.GetSection("MasterOTPKey") != null)
                {
                    emOtp = _configuration.GetSection("MasterOTPKey:Email_MasterOTP").Value;
                }

                if (otpManager.otpCode == emOtp)
                {
                    response.success = (int)enumResponseCode.success;
                    response.message = "Valid OTP";
                }
                else
                {
                    otpManager.mode = "email";
                    response = await _iotpManager.validOTP(otpManager);
                    if (response.success == (int)enumResponseCode.success)
                    {
                        response = await _iotpManager.verify(otpManager);
                    }
                    else
                    {
                        response.success = (int)enumResponseCode.fail;
                        response.message = "unable to verify Email OTP";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Concat(ControllerContext.ActionDescriptor.ControllerName
                    , ControllerContext.ActionDescriptor.ActionName));

                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("generateMobileOTP")]
        public async Task<IActionResult> generateMobileOTP([FromBody] otpManager otpManager)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }

                Random rnd = new Random();
                int rand_num = rnd.Next(100000, 999999);
                otpManager.otpCode = rand_num.ToString();
                otpManager.isActive = true;

                response = await _iotpManager.add(otpManager);
                if (response.success == (int)enumResponseCode.success)
                {
                    //send mail 
                    HttpClient client = new HttpClient();

                    string apiUrl = "http://msg.msgclub.net/rest/services/sendSMS/sendGroupSms?AUTH_KEY=168939ed48874647bce2ee7721c997";

                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

                    var smsResponse = new StringContent(JsonConvert.SerializeObject(new smsServiceModel()

                    {
                        smsContent = "Your OTP for registration is " + otpManager.otpCode + "." +
                        "It is valid for 10 minutes." +
                        " Please do not share this with anyone. - Team Regnum Wealth",
                        routeId = "8",
                        mobileNumbers = otpManager.mobile,
                        senderId = "RGNM",
                        smsContentType = "ENGLISH",
                    }), System.Text.Encoding.UTF8, "application/json");

                    var httpResponse = await client.PostAsync(apiUrl, smsResponse);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        response = new genericResponse
                        {
                            success = (int)enumResponseCode.success
                        };
                       
                    }
                    else
                    {
                        response = new genericResponse
                        {
                            success = (int)enumResponseCode.fail
                        };
                       
                    }
                    return Ok(response);
                }
                else
                {
                    response.success = (int)enumResponseCode.fail;
                    response.message = "unable to Generate Mobile OTP";
                }
                response.message = response.message;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Concat(ControllerContext.ActionDescriptor.ControllerName
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
        [Route("verifyMobileOTP")] // verifyOTPMobileOTP
        public async Task<IActionResult> verifyMobileOTP([FromBody] otpManager otpManager) //verifyOTPMobileOTP
        {
            var response = new genericResponse();
            try
            {
                string mmOTP = "";
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }

                if (string.IsNullOrEmpty(Convert.ToString(otpManager.otpCode)))
                {
                    return BadRequest("OTP code must required.");
                }

                if (_configuration.GetSection("MasterOTPKey") != null)
                {
                    mmOTP = _configuration.GetSection("MasterOTPKey:Mob_MasterOTP").Value;
                }

                if (otpManager.otpCode == mmOTP)
                {
                    response.success = (int)enumResponseCode.success;
                    response.message = "Valid OTP";
                }
                else
                {
                    otpManager.mode = " ";
                    response = await _iotpManager.validOTP(otpManager);
                    if (response.success == (int)enumResponseCode.success)
                    {
                        response = await _iotpManager.verify(otpManager);
                    }
                    else
                    {
                        response.success = (int)enumResponseCode.fail;
                        response.message = "unable To verify Mobile OTP";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Concat(ControllerContext.ActionDescriptor.ControllerName
                    , ControllerContext.ActionDescriptor.ActionName));

                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("getEmailOtp")]
        public async Task<IActionResult> getEmailOtp([FromBody] otpManager otpManager)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                otpManager.mode = "email";
                response.data = await _iotpManager.get(otpManager);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Concat(ControllerContext.ActionDescriptor.ControllerName
                    , ControllerContext.ActionDescriptor.ActionName));

                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("getMobileOtp")]
        public async Task<IActionResult> get([FromBody] otpManager otpManager)
        {
            var response = new genericResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                otpManager.mode = "ByRefenceNo";
                response.data = await _iotpManager.get(otpManager);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Concat(ControllerContext.ActionDescriptor.ControllerName
                    , ControllerContext.ActionDescriptor.ActionName));

                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };
            }
            return Ok(response);
        }

    }
}
