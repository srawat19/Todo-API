using Microsoft.EntityFrameworkCore;
using Todo.Application.Interfaces.Repositories;
using Todo.Domain.Entities;
using Todo.Infrastructure;


namespace Todo_API.Middlewares
{
    public class UserCreationMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IUserRepository _userRepo;

       public UserCreationMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository _userRepo)
        {

            string oid = string.Empty;

            if (context != null)
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    if (context.User.FindFirst("oid") != null)
                    {
                        oid = context.User.FindFirst("oid").Value;

                    }
                    else
                    {
                        //log error 
                        throw new Exception("Object Id can't be null");
                    }
                    string? displayName = context.User.FindFirst("name")?.Value;
                    string? email = context.User.FindFirst("email")?.Value;

                    //Check if User exists, if not create.
                    var userExists = await _userRepo.GetByIdAsync(oid);


                    if (userExists == null)
                    {
                        User newUser = new User()
                        {
                            UserObjectId = oid,
                            DisplayName = displayName == null ? string.Empty : displayName,
                            Email = email == null ? string.Empty : email
                        };
                        await _userRepo.AddAsync(newUser);
                      


                    }

                    await _requestDelegate(context);



                }

            }
            else
            {
                //log error
            }

            
        }
    }
}
