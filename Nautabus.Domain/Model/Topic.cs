using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nautabus.Domain.Model
{
    public class Topic
    {
        [Key]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
