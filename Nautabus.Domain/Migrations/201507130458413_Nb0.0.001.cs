namespace Nautabus.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nb00001 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TopicName = c.String(nullable: false, maxLength: 100),
                        MessageContent = c.String(nullable: false),
                        CreatedDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Topics", t => t.TopicName, cascadeDelete: true)
                .Index(t => t.TopicName);
            
            CreateTable(
                "dbo.Topics",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.SubscriptionMessages",
                c => new
                    {
                        MessageId = c.Int(nullable: false),
                        TopicName = c.String(nullable: false, maxLength: 100),
                        SubscriptionName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.MessageId, t.TopicName, t.SubscriptionName })
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .ForeignKey("dbo.TopicSubscriptions", t => new { t.TopicName, t.SubscriptionName }, cascadeDelete: true)
                .Index(t => t.MessageId)
                .Index(t => new { t.TopicName, t.SubscriptionName });
            
            CreateTable(
                "dbo.TopicSubscriptions",
                c => new
                    {
                        TopicName = c.String(nullable: false, maxLength: 100),
                        SubscriptionName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.TopicName, t.SubscriptionName });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriptionMessages", new[] { "TopicName", "SubscriptionName" }, "dbo.TopicSubscriptions");
            DropForeignKey("dbo.SubscriptionMessages", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.Messages", "TopicName", "dbo.Topics");
            DropIndex("dbo.SubscriptionMessages", new[] { "TopicName", "SubscriptionName" });
            DropIndex("dbo.SubscriptionMessages", new[] { "MessageId" });
            DropIndex("dbo.Messages", new[] { "TopicName" });
            DropTable("dbo.TopicSubscriptions");
            DropTable("dbo.SubscriptionMessages");
            DropTable("dbo.Topics");
            DropTable("dbo.Messages");
        }
    }
}
