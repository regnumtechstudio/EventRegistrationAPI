
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.common;
using Models.General;
using static Models.ClsGloble;
using Utilities;
using DataAccess.General;

namespace Event_Resistration_APIs.Controllers.General
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly ILogger<userController> _logger;
        private readonly IuserMaster _iuserMaster;
       
        public userController(IuserMaster iuserMaster, ILogger<userController> logger)
        {
            _iuserMaster = iuserMaster;
            _logger = logger;
        }

        [HttpPost, Route("login")]
        [AllowAnonymous]
        public IActionResult login([FromBody] loginModel loginModel)
        {
            var response = new genericResponse();
            try
            {
                if (loginModel == null)
                {
                    return BadRequest("Invalid client request");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model object");
                }

                //authenicate User
                loginModel.password = CryptoHelper.encryptHash(loginModel.password);
                var verifyUserParam = new userMaster
                {
                    username = loginModel.userName,
                    password = loginModel.password,
                    userType = (int)enumUserType.admin
                };
                int isAuthenticate = _iuserMaster.verifyUser(verifyUserParam).Result;
                if (isAuthenticate == (int)enumResponseCode.success)
                {
                    //sign & get Login Information.
                    response = _iuserMaster.userSignIn(loginModel).Result;

                }
                else
                {
                    response.success = (int)enumResponseCode.fail;
                    response.message = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                  string.Concat(ControllerContext.ActionDescriptor.ControllerName,
                  ControllerContext.ActionDescriptor.ActionName));

                return BadRequest(string.Concat("error occurs:", ex.Message));
            }
            return Ok(response);
        }


        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> add([FromBody] userMaster usermaster)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                if (string.IsNullOrEmpty(Convert.ToString(Request.Headers["userid"])))
                {
                    return BadRequest("User Not Found");
                }
                usermaster.userType = (int)enumUserType.admin;
                usermaster.entrybyuserid = Convert.ToInt32(Request.Headers["userid"]);
                //encrypt pwd
                usermaster.password = Encryptionclass.EncryptHash(usermaster.password);//pwd: 123; return: laksj92387##
                var result = await _iuserMaster.add(usermaster);
                return Ok(result);
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
        [Route("modify")]
        public async Task<IActionResult> modify([FromBody] userMaster usermaster)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("invalid model object");
                }
                if (string.IsNullOrEmpty(Convert.ToString(Request.Headers["userid"])))
                {
                    return BadRequest("User Not Found");
                }
                usermaster.entrybyuserid = Convert.ToInt32(Request.Headers["userid"]);

                var result = await _iuserMaster.modify(usermaster);
                return Ok(result);
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

        [HttpGet]
        [Route("get")]
        public IActionResult get()
        {
            return Ok("1");
        }
    }
}
