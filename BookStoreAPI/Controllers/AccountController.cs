using BookStore.Models.Account;
using BookStore.Services;
using BookStoreAPI.Filters;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BookStoreAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Account")]
    [ValidateModel]
    public class AccountController : ApiController
    {
        AccountService accountService = new AccountService();

        [AllowAnonymous]
        [Route("Authenticate/{email}/{password}")]
        [HttpGet]
        public HttpResponseMessage AuthenticateUser(string email, string password)
        {
            try
            {
                bool isValid = accountService.AuthenticateUser(email, password);

                return Request.CreateResponse(HttpStatusCode.OK, isValid, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [AllowAnonymous]
        [Route("SignUpUser")]
        [HttpPost]
        public HttpResponseMessage SignUpUser([FromBody] User user)
        {
            user.UserId = 0;
            try
            {
                if (accountService.AuthenticateUserEmail(user.Email))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email address already exists, use unique email to sign up.");
                }

                User newUser = accountService.AddUser(user);

                if (newUser.UserId > 0)
                    return Request.CreateResponse(HttpStatusCode.Created, newUser, Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User sign up failed, please try again later.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [AllowAnonymous]
        [Route("GetUserRoles")]
        [HttpGet]
        public HttpResponseMessage GetUserRoles()
        {
            try
            {
                var userRoles = accountService.GetUserRoleList();

                if (userRoles == null || userRoles.Count == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User roles not found.");

                return Request.CreateResponse(HttpStatusCode.OK, userRoles, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
    }
}