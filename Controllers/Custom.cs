using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CustomController:BaseApiController
    {

        [HttpGet]
        public AppUser GetName()
        {
            var user = new AppUser{
                UserName="Kerim"
            };
            return user;
        }
        
    }
}