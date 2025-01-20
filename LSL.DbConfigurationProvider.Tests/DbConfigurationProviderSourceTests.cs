using System;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            builder.Add(new DbConfigurationProviderSource(() => new SqliteKeepAliveDbConnection(ctx.Database.GetDbConnection())));

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
            builder.AddDbConfiguration(() => new SqliteKeepAliveDbConnection(ctx.Database.GetDbConnection()));

            var config = builder.Build();
            var topLevel = config.GetChildren();
            
            topLevel.Should().BeEmpty();
        }

        [Test]
        public void GivenAnIncorrectTableNameAndALoadErrorDelegate_ItShouldCallTheDelegate()
        {
            var loadFailedCalled = false;
            LoadErrorContext capturedContext = null!;

            var ctx = CreateContext();
            var builder = new ConfigurationBuilder();
            builder.AddDbConfiguration(
                () => new SqliteKeepAliveDbConnection(ctx.Database.GetDbConnection()),
                "NotATable",
                onLoadError: ctx => 
                {
                    loadFailedCalled = true;
                    capturedContext = ctx;
                });

            var config = builder.Build();            
            var topLevel = config.GetChildren();

            topLevel.Should().BeEmpty();
            loadFailedCalled.Should().BeTrue();
            capturedContext.Exception.Should().BeOfType<SqliteException>();
        }

        [Test]
        public void GivenAnIncorrectTableNameAndNoLoadErrorDelegate_ThenItShouldSilentlyFail()
        {
            var ctx = CreateContext();
            var builder = new ConfigurationBuilder();
            builder.AddDbConfiguration(
                () => new SqliteKeepAliveDbConnection(ctx.Database.GetDbConnection()),
                "NotATable");

            var config = builder.Build();            
            var topLevel = config.GetChildren();

            topLevel.Should().BeEmpty();
        }

        [Test]
        public void GivenAnIncorrectTableNameAndALoadErrorDelegateThatExpectesTheExceptionToBeThrown_ItShouldReThrow()
        {
            var loadFailedCalled = false;
            var ctx = CreateContext();
            var builder = new ConfigurationBuilder();

            builder.AddDbConfiguration(
                () => new SqliteKeepAliveDbConnection(ctx.Database.GetDbConnection()),
                "NotATable",
                onLoadError: ctx => 
                {
                    loadFailedCalled = true;
                    ctx.RethrowException = true;
                });

            var toRun = new Action(() =>  builder.Build());
            toRun.Should().Throw<DbConfigurationProviderLoadException>()
                .WithMessage("DbConfigurationProvider load error. See InnerException for details")
                .WithInnerException<SqliteException>()
                .WithMessage("SQLite Error 1: 'no such table: NotATable'.");

            loadFailedCalled.Should().BeTrue();
        }        

        [Test]
        public void GivenDefaultSettings_ButWithAKeyPrefixAndDataItShouldReturnTheExpectedConfiguration()
        {
            var keyPrefix = "My.App:";
            var ctx = CreateContext();

            ctx.Settings.Add(new Setting
            {
                Key = $"{keyPrefix}Prefixed",
                Value = "AlsTest"
            });

            ctx.SaveChanges();

            var builder = new ConfigurationBuilder();
            builder.AddDbConfiguration(
                () => new SqliteKeepAliveDbConnection(ctx.Database.GetDbConnection()),
                keyPrefix: keyPrefix
            );

            var config = builder.Build();
            var topLevel = config.GetChildren();
            
            topLevel.Should().BeEquivalentTo(new[]
            {
                new { Key = "Prefixed", Value = "AlsTest" }
            });
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
            builder.Add(new DbConfigurationProviderSource(() => new SqliteKeepAliveDbConnection(ctx.Database.GetDbConnection()), "OtherSettings", "OtherKey", "OtherValue"));

            var config = builder.Build();
            var inner = config.GetSection("Inner").GetChildren();

            ctx.OtherSettings.Add(new OtherSetting
            {
                OtherKey = "Als",
                OtherValue = "Test2"
            });

            ctx.SaveChanges();
            
            config.Reload();

            var topLevel = config.GetChildren();

            topLevel.Should().BeEquivalentTo(new[]
            {
                new { Key = "TopLevel", Value = "Test" },
                new { Key = "Inner", Value = (string)null },
                new { Key = "Als", Value = "Test2" }
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
