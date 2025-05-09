using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Forms
{
    [SwaggerSchema("Form used by Admins to request all invoices for a specific user.")]
    public class UserIdForm
    {
        [Required]
        [MinLength(1)]
        [SwaggerSchema(
            Description = "The unique identifier of the user whose invoices should be retrieved. Example  - user-123",
            Nullable = false
        )]
        public string UserId { get; set; } = null!;
    }
}
