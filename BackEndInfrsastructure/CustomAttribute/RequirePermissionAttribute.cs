using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndInfrastructure.CustomAttribute
{
    public class RequirePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {

        private readonly string _permission;

        public RequirePermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var t = context.HttpContext.User.Identity.IsAuthenticated;
            var user = context.HttpContext.User;
            if (!user.HasClaim("Permission", _permission))
            {
                context.Result = new ForbidResult();
            }
        }
    }

}
