using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.Entities;

namespace Todo.Test
{
    public static class TestHelper
    {
        public static void ValidateModel(object model, ControllerBase controller)
        {
            ValidationContext validationContext = new ValidationContext(model);
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, validationContext, validationResults))
            {
                if (validationResults != null)
                {
                    foreach (var result in validationResults)
                    {
                        foreach (var member in result.MemberNames)
                        {
                            controller.ModelState.AddModelError(member, result.ErrorMessage);
                        }
                    }
                }
            }
        }


        public static void SetAuthenticatedUser(ControllerBase controller, Dictionary<string, string> claims)
        {
            List<Claim> claim = new List<Claim>();

            if (claims != null)
            {
                foreach (var claimVal in claims)
                {
                    claim.Add(new Claim(claimVal.Key, claimVal.Value));

                    claim.Add(new Claim(claimVal.Key, claimVal.Value));

                }
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim);

            //ClaimsIdentity claimsIdentity = new ClaimsIdentity([
            //    new Claim(ClaimTypes.Name, "TestUser"),
            //    new Claim(ClaimTypes.Role,"user"),
            //    new Claim("oid","12345-asd")

            //]);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }

            };

          


        }
    }
}
