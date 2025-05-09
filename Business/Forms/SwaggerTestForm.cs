using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
namespace Business.Forms
{
    [SwaggerSchema("This form is used to test POST requests in Swagger UI. Enter a test word and submit.")]
    public class SwaggerTestForm
    {
        [Required]
        [SwaggerSchema("Any test word you want to submit. Used to verify Swagger is working.", Nullable = false)]
        public string TestWord { get; set; } = string.Empty;
    }
}
