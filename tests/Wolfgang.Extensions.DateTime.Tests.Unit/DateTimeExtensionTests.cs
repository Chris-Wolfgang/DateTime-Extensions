namespace Wolfgang.Extensions.DateTime.Tests.Unit;


using System;
using System.Globalization;
using System.Threading;
using Xunit;

    public class DateTimeExtensionsTests
    {

        [Fact]
        public void TruncateMilliseconds_successfully_removes_milliseconds_from_value()
        {
            var now = DateTime.Now;

            var actualResult = now.TruncateMilliseconds();

            var expectedResult = new DateTime
            (
                now.Year,
                now.Month,
                now.Day,
                now.Hour,
                now.Minute,
                now.Second
            );

            Assert.Equal(expectedResult, actualResult);
        }



        [Fact]
        public void TruncateMilliseconds_returns_0_milliseconds()
        {
            var now = DateTime.Now;


            var actualResult = now.TruncateMilliseconds();


            Assert.Equal(0, actualResult.Millisecond);
        }



        [Fact]
        public void TruncateSeconds_removes_seconds_from_value()
        {
            var now = DateTime.Now;

            var actualResult = now.TruncateSeconds();

            var expectedResult = new DateTime(
                now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

            Assert.Equal(expectedResult, actualResult);
        }



        [Fact]
        public void TruncateSeconds_returns_0_milliseconds()
        {
            var now = DateTime.Now;


            var actualResult = now.TruncateSeconds();


            Assert.Equal(0, actualResult.Millisecond);
        }



        [Fact]
        public void TruncateSeconds_returns_0_seconds()
        {
            var now = DateTime.Now;


            var actualResult = now.TruncateSeconds();


            Assert.Equal(0, actualResult.Second);
        }



        [Fact]
        public void TruncateSeconds_removes_seconds_and_everything_after_seconds()
        {
            var testValue = new DateTime(2018, 4, 23, 12, 47, 59)
                .AddMilliseconds(253)
                .AddTicks(15);

            var expectedResult = new DateTime(2018, 4, 23, 12, 47, 0);


            var actualResult = testValue.TruncateSeconds();


            Assert.Equal(expectedResult, actualResult);
        }



        [Theory]
        [InlineData("2016/5/13", "2016/5/1")]
        [InlineData("2016/2/13", "2016/2/1")]
        [InlineData("2016/2/1", "2016/2/1")]
        [InlineData("2016/2/29", "2016/2/1")]
        [InlineData("2016/12/31 23:59:59.9999999", "2016/12/1")]
        public void FirstOfMonth_returns_the_first_of_the_specified_month
        (
            DateTime testValue,
            DateTime expectedResult
        )
        {
            var actualResult = testValue.FirstOfMonth();

            Assert.Equal(expectedResult, actualResult);
        }



        [Theory]
        [InlineData("2016/5/13", "2016/5/31 23:59:59.9999999")]
        [InlineData("2016/2/13", "2016/2/29  23:59:59.9999999")]
        [InlineData("2017/2/1", "2017/2/28  23:59:59.9999999")]
        [InlineData("2016/2/29  23:59:59.9999999", "2016/2/29  23:59:59.9999999")]
        [InlineData("2016/12/31 23:59:59.9999999", "2016/12/31  23:59:59.9999999")]
        public void EndOfMonth_returns_the_last_date_and_time_of_the_specified_month
        (
            DateTime testValue,
            DateTime expectedResult
        )
        {
            var actualResult = testValue.EndOfMonth();

            Assert.Equal(expectedResult, actualResult);
        }



        [Theory]
        [InlineData("2016/5/13", "2016/1/1")]
        [InlineData("2020/2/29", "2020/1/1")]
        [InlineData("2018/12/31 23:59:59.9999999", "2018/1/1")]
        public void FirstOfYear_returns_the_first_of_the_specified_year
        (
            DateTime testValue,
            DateTime expectedResult
        )
        {
            var actualResult = testValue.FirstOfYear();

            Assert.Equal(expectedResult, actualResult);
        }



        [Theory]
        [InlineData("2016/5/13", "2016/12/31 23:59:59.9999999")]
        [InlineData("2020/2/29", "2020/12/31 23:59:59.9999999")]
        [InlineData("2018/12/31 23:59:59.9999999", "2018/12/31 23:59:59.9999999")]
        public void EndOfYear_returns_the_last_date_and_time_of_the_specified_year
        (
            DateTime testValue,
            DateTime expectedResult
        )
        {
            var actualResult = testValue.EndOfYear();

            Assert.Equal(expectedResult, actualResult);
        }



        [Theory]
        [InlineData("2020/2/23", DayOfWeek.Sunday, "2020/2/23")]
        [InlineData("2020/2/29", DayOfWeek.Sunday, "2020/2/23")]
        [InlineData("2020/2/23", DayOfWeek.Monday, "2020/2/17")]
        [InlineData("2020/2/29", DayOfWeek.Monday, "2020/2/24")]
        [InlineData("2020/2/24", DayOfWeek.Monday, "2020/2/24")]
        [InlineData("2020/3/2", DayOfWeek.Monday, "2020/3/2")]
        [InlineData("2020/2/23", DayOfWeek.Saturday, "2020/2/22")]
        [InlineData("2020/2/29", DayOfWeek.Saturday, "2020/2/29")]
        public void FirstOfWeek_returns_the_date_time_of_the_first_day_of_the_week_containing_the_specified_DateTime
        (
            DateTime testValue,
            DayOfWeek firstDayOfWeek,
            DateTime expectedResult
        )
        {
            var backup = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US")
                {
                    DateTimeFormat = { FirstDayOfWeek = firstDayOfWeek }
                };

                var actualResult = testValue.FirstOfWeek();

                Assert.Equal(expectedResult, actualResult);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = backup;
            }
        }



        [Theory]
        [InlineData("2020/2/23", DayOfWeek.Sunday, "2020/2/23")]
        [InlineData("2020/2/29", DayOfWeek.Sunday, "2020/2/23")]
        [InlineData("2020/2/23", DayOfWeek.Monday, "2020/2/17")]
        [InlineData("2020/2/29", DayOfWeek.Monday, "2020/2/24")]
        [InlineData("2020/2/24", DayOfWeek.Monday, "2020/2/24")]
        [InlineData("2020/3/2", DayOfWeek.Monday, "2020/3/2")]
        [InlineData("2020/2/23", DayOfWeek.Saturday, "2020/2/22")]
        [InlineData("2020/2/29", DayOfWeek.Saturday, "2020/2/29")]
        public void FirstOfWeek_DayOfWeek_is_specified_returns_the_DateTime_of_the_first_day_of_the_week_containing_the_specified_DateTime
        (
            DateTime testValue,
            DayOfWeek firstDayOfWeek,
            DateTime expectedResult
        )
        {
            var actualResult = testValue.FirstOfWeek(firstDayOfWeek);

            Assert.Equal(expectedResult, actualResult);
        }



        [Theory]
        [InlineData("2020/2/23", DayOfWeek.Sunday, "2020/2/29 23:59:59.9999999")]
        [InlineData("2020/2/29", DayOfWeek.Sunday, "2020/2/29 23:59:59.9999999")]
        [InlineData("2020/2/23", DayOfWeek.Monday, "2020/2/23 23:59:59.9999999")]
        [InlineData("2020/2/29", DayOfWeek.Monday, "2020/3/1 23:59:59.9999999")]
        [InlineData("2020/3/7", DayOfWeek.Monday, "2020/3/8 23:59:59.9999999")]
        [InlineData("2020/2/24", DayOfWeek.Monday, "2020/3/1 23:59:59.9999999")]
        [InlineData("2020/3/2", DayOfWeek.Monday, "2020/3/8 23:59:59.9999999")]
        [InlineData("2020/2/23", DayOfWeek.Saturday, "2020/2/28 23:59:59.9999999")]
        [InlineData("2020/2/29", DayOfWeek.Saturday, "2020/3/6 23:59:59.9999999")]
        [InlineData("2020/3/1", DayOfWeek.Sunday, "2020/3/7 23:59:59.9999999")]
        [InlineData("2020/3/4", DayOfWeek.Sunday, "2020/3/7 23:59:59.9999999")]
        public void EndOfWeek_returns_the_DateTime_of_the_last_day_of_the_week_containing_the_specified_DateTime
        (
            DateTime testValue,
            DayOfWeek firstDayOfWeek,
            DateTime expectedResult
        )
        {
            var backup = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US")
                {
                    DateTimeFormat = { FirstDayOfWeek = firstDayOfWeek }
                };

                var actualResult = testValue.EndOfWeek();

                Assert.Equal(expectedResult, actualResult);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = backup;
            }
        }



        [Theory]
        [InlineData("2020/2/23", DayOfWeek.Sunday, "2020/2/29 23:59:59.9999999")]
        [InlineData("2020/2/29", DayOfWeek.Sunday, "2020/2/29 23:59:59.9999999")]
        [InlineData("2020/2/23", DayOfWeek.Monday, "2020/2/23 23:59:59.9999999")]
        [InlineData("2020/2/29", DayOfWeek.Monday, "2020/3/1 23:59:59.9999999")]
        [InlineData("2020/3/7", DayOfWeek.Monday, "2020/3/8 23:59:59.9999999")]
        [InlineData("2020/2/24", DayOfWeek.Monday, "2020/3/1 23:59:59.9999999")]
        [InlineData("2020/3/2", DayOfWeek.Monday, "2020/3/8 23:59:59.9999999")]
        [InlineData("2020/2/23", DayOfWeek.Saturday, "2020/2/28 23:59:59.9999999")]
        [InlineData("2020/2/29", DayOfWeek.Saturday, "2020/3/6 23:59:59.9999999")]
        [InlineData("2020/3/1", DayOfWeek.Sunday, "2020/3/7 23:59:59.9999999")]
        [InlineData("2020/3/4", DayOfWeek.Sunday, "2020/3/7 23:59:59.9999999")]
        public void EndOfWeek_DayOfWeek_is_specified_returns_the_DateTime_of_the_last_day_of_the_week_containing_the_specified_DateTime
        (
            DateTime testValue,
            DayOfWeek firstDayOfWeek,
            System.DateTime expectedResult
        )
        {
            var actualResult = testValue.EndOfWeek(firstDayOfWeek);

            Assert.Equal(expectedResult, actualResult);
        }

    }