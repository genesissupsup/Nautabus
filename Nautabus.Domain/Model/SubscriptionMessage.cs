using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nautabus.Domain.Model
{
    public class SubscriptionMessage
    {
        [Key]
        [Column(Order = 0)]
        public int MessageId { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        [StringLength(100)]
        public string TopicName { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required]
        [StringLength(100)]
        public string SubscriptionName { get; set; }

        public virtual TopicSubscription TopicSubscription { get; set; }


        public virtual Message Message { get; set; }


    }
}
