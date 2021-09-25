using Microsoft.AspNetCore.Identity;

namespace MyCity.DataModel
{
    public class User : IdentityUser<int>
    {
        public AccountType AccountType { get; set; }
    }
}
