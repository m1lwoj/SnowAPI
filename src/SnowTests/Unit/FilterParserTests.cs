using SnowBLL.Models;
using SnowDAL.Sorting;
using System;
using Xunit;

namespace SnowTests
{
    public class FilterParserTests
    {
        [Fact]
        public void FilteringValidFields()
        {
            string filter = "name::asdasd dsd||difficulty::4";
            var filterModel = FiltrationHelper.GetFilter<FilterModel>(filter);

            Assert.True(filterModel.Difficulty.HasValue);
            Assert.Equal(4, filterModel.Difficulty.Value);
            Assert.Equal("asdasd dsd", filterModel.Name);
        }

        [Fact]
        public void FilteringInvalidFields()
        {
            string filter = "name::asdasd dsd||difficulties::4";
            Assert.Throws<Exception>(() => FiltrationHelper.GetFilter<FilterModel>(filter));
        }

        [Fact]
        public void SortingAscendingOrder()
        {
            string filter = "name";
            var filterModel = FiltrationHelper.GetSorting<SortingModel>(filter);

            Assert.Equal(SortDirection.Ascending, filterModel.Direction);
            Assert.Equal("name", filterModel.Name);
        }

        [Fact]
        public void SortingDescendingOrder()
        {
            string filter = "-difficulty";
            var filterModel = FiltrationHelper.GetSorting<SortingModel>(filter);

            Assert.Equal(SortDirection.Descending, filterModel.Direction);
            Assert.Equal("difficulty", filterModel.Name);
        }

        [Fact]
        public void SortingInvalidProperty()
        {
            string filter = "-wrongpropertyname";
             Assert.Throws<Exception>(() => FiltrationHelper.GetSorting<SortingModel>(filter));
        }

        class SortingModel
        {
            public string Name { get; set; }
            public string Difficulty { get; set; }
        }

        class FilterModel
        {
            public int? Difficulty { get; set; }
            public string Name { get; set; }
        }
    }
}
