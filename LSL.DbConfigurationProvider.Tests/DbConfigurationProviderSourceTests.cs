using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace LSL.DbConfigurationProvider.Tests
{
    public class DbConfigurationProviderSourceTests
    {
        [Test]
        public void GivenDefaultSettings_ItShouldReturnTheExpectedConfiguration()
        {
            var ctx = CreateContext();
            ctx.Settings.Add(new Setting
            {
                Key = "TopLevel",
                Value = "Test"
            });

            ctx.Settings.Add(new Setting
            {
                Key = "Inner:MyKey",
                Value = "Another"
            });

            ctx.SaveChanges();

            var builder = new ConfigurationBuilder();
            builder.Add(new DbConfigurationProviderSource(() => ctx.Database.GetDbConnection()));

            var config = builder.Build();
            var topLevel = config.GetChildren();
            var inner = config.GetSection("Inner").GetChildren();

            topLevel.Should().BeEquivalentTo(new[]
            {
                new { Key = "TopLevel", Value = "Test" },
                new { Key = "Inner", Value = (string)null }
            });

            inner.Should().BeEquivalentTo(new[]
            {
                new { Key = "MyKey", Value = "Another" },
            });            
        }

        [Test]
        public void GivenDefaultSettings_ButNoDbSettingsItShouldReturnTheExpectedConfiguration()
        {
            var ctx = CreateContext();
            var builder = new ConfigurationBuilder();
            builder.AddDbConfiguration(() => ctx.Database.GetDbConnection());

            var config = builder.Build();
            var topLevel = config.GetChildren();
            
            topLevel.Should().BeEmpty();
        }

        [Test]
        public void GivenCustomSettings_ItShouldReturnTheExpectedConfiguration()
        {
            var ctx = CreateContext();
            ctx.OtherSettings.Add(new OtherSetting
            {
                OtherKey = "TopLevel",
                OtherValue = "Test"
            });

            ctx.OtherSettings.Add(new OtherSetting
            {
                OtherKey = "Inner:MyKey",
                OtherValue = "Another"
            });

            ctx.SaveChanges();

            var builder = new ConfigurationBuilder();
            builder.Add(new DbConfigurationProviderSource(() => ctx.Database.GetDbConnection(), "OtherSettings", "OtherKey", "OtherValue"));

            var config = builder.Build();
            var topLevel = config.GetChildren();
            var inner = config.GetSection("Inner").GetChildren();

            topLevel.Should().BeEquivalentTo(new[]
            {
                new { Key = "TopLevel", Value = "Test" },
                new { Key = "Inner", Value = (string)null }
            });

            inner.Should().BeEquivalentTo(new[]
            {
                new { Key = "MyKey", Value = "Another" },
            });
        }

        private DefaultTestContext CreateContext() 
        {
            var closedBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(typeof(DefaultTestContext));
            var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(closedBuilderType);            

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            optionsBuilder.UseSqlite(connection);

            var result = (DefaultTestContext)Activator.CreateInstance(typeof(DefaultTestContext), optionsBuilder.Options);

            result.Database.EnsureDeleted();
            result.Database.EnsureCreated();

            return result;
        }

        internal class DefaultTestContext : DbContext
        {
            public DefaultTestContext(DbContextOptions<DefaultTestContext> options) : base(options) {}

            public DbSet<Setting> Settings { get; set; }
            public DbSet<OtherSetting> OtherSettings { get; set; }
        }

        internal class OtherSetting
        {
            [Key]
            public string OtherKey { get; set; }
            public string OtherValue { get; set; }
        }

        internal class Setting
        {
            [Key]
            public string Key { get; set; }
            public string Value { get; set; }
        }        
    }
}
