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

        [ForeignKey("TopicName")]
        public virtual Topic Topic { get; set; }

        [NotMapped]
        public string ChannelName => GetChannelName(TopicName, SubscriptionName);

        public static string GetChannelName(string topic, string subscription)
        {
            return string.Format("{1}.{0}", topic, subscription);
        }
    }
}
