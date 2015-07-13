using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nautabus.Domain.Model
{
    public class TopicSubscription
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        [StringLength(100)]
        public string TopicName{ get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        [StringLength(100)]
        public string SubscriptionName { get; set; }

    }
}
