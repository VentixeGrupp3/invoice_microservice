using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
namespace Business.Forms
{
    public class SwaggerTestForm
    {
        [Required]
        [SwaggerSchema("Test word", ReadOnly = true)]
        public string TestWord { get; set; }
    }
}
