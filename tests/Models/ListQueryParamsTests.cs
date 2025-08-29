using Blackbaud.HeadlessDataSync.Models;
using Xunit;

namespace Blackbaud.HeadlessDataSync.Tests.Models
{
    public class ListQueryParamsTests
    {
        [Fact]
        public void IsEmpty_WithBothPropertiesNull_ReturnsTrue()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = null,
                SortToken = null
            };

            var result = queryParams.IsEmpty();

            Assert.True(result);
        }

        [Fact]
        public void IsEmpty_WithBothPropertiesEmpty_ReturnsTrue()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = string.Empty,
                SortToken = string.Empty
            };

            var result = queryParams.IsEmpty();

            Assert.True(result);
        }

        [Fact]
        public void IsEmpty_WithLastModifiedOnly_ReturnsFalse()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = "2023-01-01T00:00:00Z",
                SortToken = null
            };

            var result = queryParams.IsEmpty();

            Assert.False(result);
        }

        [Fact]
        public void IsEmpty_WithSortTokenOnly_ReturnsFalse()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = null,
                SortToken = "test-sort-token"
            };

            var result = queryParams.IsEmpty();

            Assert.False(result);
        }

        [Fact]
        public void IsEmpty_WithBothPropertiesSet_ReturnsFalse()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = "2023-01-01T00:00:00Z",
                SortToken = "test-sort-token"
            };

            var result = queryParams.IsEmpty();

            Assert.False(result);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        public void IsEmpty_WithVariousEmptyValues_ReturnsTrue(string lastModified, string sortToken)
        {
            var queryParams = new ListQueryParams
            {
                LastModified = lastModified,
                SortToken = sortToken
            };

            var result = queryParams.IsEmpty();

            Assert.True(result);
        }

        [Theory]
        [InlineData("2023-01-01T00:00:00Z", "")]
        [InlineData("2023-01-01T00:00:00Z", null)]
        [InlineData("", "token123")]
        [InlineData(null, "token123")]
        [InlineData("2023-01-01T00:00:00Z", "token123")]
        public void IsEmpty_WithAtLeastOneNonEmptyValue_ReturnsFalse(string lastModified, string sortToken)
        {
            var queryParams = new ListQueryParams
            {
                LastModified = lastModified,
                SortToken = sortToken
            };

            var result = queryParams.IsEmpty();

            Assert.False(result);
        }

        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            var lastModified = "2023-01-01T00:00:00Z";
            var sortToken = "test-sort-token";

            var queryParams = new ListQueryParams
            {
                LastModified = lastModified,
                SortToken = sortToken
            };

            Assert.Equal(lastModified, queryParams.LastModified);
            Assert.Equal(sortToken, queryParams.SortToken);
        }

        [Fact]
        public void DefaultConstructor_InitializesWithNullValues()
        {
            var queryParams = new ListQueryParams();

            Assert.Null(queryParams.LastModified);
            Assert.Null(queryParams.SortToken);
            Assert.True(queryParams.IsEmpty());
        }
    }
}
