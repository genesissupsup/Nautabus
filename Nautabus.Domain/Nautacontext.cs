﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nautabus.Domain.Model;

namespace Nautabus.Domain
{
    public class Nautacontext : DbContext
    {
        public Nautacontext() : this("Nautacontext") { }

        public Nautacontext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<TopicSubscription> TopicSubscriptions { get; set; }

        public DbSet<SubscriptionMessage> SubscriptionMessages { get; set; }
    }
}
