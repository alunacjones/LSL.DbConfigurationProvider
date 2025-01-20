using System.Data;
using System.Data.Common;
using FluentAssertions;
using NUnit.Framework;

namespace LSL.DbConfigurationProvider.Tests
{
    public class CommandBuilderExtensionsTests
    {
        [Test]
        public void NullSafeQuoteIdentifier_GivenANullCommandBuilder_ItShouldReturnTheExpectedResult()
        {
            DbCommandBuilder builder = null;
            builder.NullSafeQuoteIdentifier("an identifier").Should().Be("an identifier");
        }

        [Test]
        public void NullSafeQuoteIdentifier_GivenANonNullCommandBuilder_ItShouldReturnTheExpectedResult()
        {
            DbCommandBuilder builder = new NonNullCommandBuilder();
            builder.NullSafeQuoteIdentifier("an identifier").Should().Be("[an identifier]");
        }

        private class NonNullCommandBuilder : DbCommandBuilder
        {
            public override string QuoteIdentifier(string unquotedIdentifier) => $"[{unquotedIdentifier}]";
            protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause) => throw new System.NotImplementedException();
            protected override string GetParameterName(int parameterOrdinal) => throw new System.NotImplementedException();
            protected override string GetParameterName(string parameterName) => throw new System.NotImplementedException();
            protected override string GetParameterPlaceholder(int parameterOrdinal) => throw new System.NotImplementedException();
            protected override void SetRowUpdatingHandler(DbDataAdapter adapter) => throw new System.NotImplementedException();
        }
    }    
}
