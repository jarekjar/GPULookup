using GpuLookup.Models;
using GpuLookup.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Security;

namespace GpuLookup.Controllers
{
    public class UserController : ApiController
    {
        [EnableCors(origins: "http://192.168.1.112:19001", headers: "*", methods: "*")]
        [Route("api/users/"), HttpPost]
        public HttpResponseMessage InsertUser(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Request");

                UserService userService = new UserService();
                userService.InsertUser(user);

                return Request.CreateResponse(HttpStatusCode.OK, "Good Job.");

            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/login"), HttpPost]
        public HttpResponseMessage Login(User user)
        {
            try
            {
                if (!ModelState.IsValid || user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter both username and password.");

                UserService userService = new UserService();

                if (userService.Login(user))
                {
                    FormsAuthentication.SetAuthCookie(user.Username, true);
                    return Request.CreateResponse(HttpStatusCode.OK, "Logged In.");
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Incorrect Password");
                
            }
            catch(LoginException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User not found");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
