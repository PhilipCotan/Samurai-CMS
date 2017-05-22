﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Samurai_CMS.Models;

namespace Samurai_CMS.CustomHandlers
{
    public class AuthorizedAttribute : AuthorizeAttribute
    {
        private readonly Roles[] _allowedRoles;

        public AuthorizedAttribute(params Roles[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //TODO(filip): Implement role guard for each attribute requests
            //if (!httpContext.User.Identity.IsAuthenticated)
            //{
            //    return false;
            //}

            //var identity = (ClaimsIdentity) httpContext.User.Identity;
            //string roleString = identity.Claims.Single(c => c.Type == ClaimTypes.Role).Value;
            //var role = (Roles) Enum.Parse(typeof (Roles), roleString);

            //if (!_allowedRoles.ToList().Contains(role) && role != Models.Roles.Administrator)
            //{
            //    httpContext.Response.StatusCode = 400; //Set bad request status code
            //}

            //return false; //Throw request to HandleUnauthorizedRequest

            return Users == httpContext.User.Identity.GetUserName();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //user is not authenticated, redirect him to login page
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else //user has no rights to access the page
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "NoRights" }));
            }
        }
    }
}