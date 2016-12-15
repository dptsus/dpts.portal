using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace DPTS.Data.IdentityEntities
{
    public class User : IdentityUser
    {
        [Required, MaxLength(256)]
        public string FirstName { get; set; }

        [Required, MaxLength(256)]
        public string LastName { get; set; }
    }
}
