using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nautabus.Domain.Model
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TopicName { get; set; }
      
        [Required]
        public string MessageContent { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public DateTimeOffset CreatedDate { get; set; }

        public virtual Topic Topic { get; set; }

    }
}
