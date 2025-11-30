using System.Net;

namespace TestClientResponse.Tests.Tests.Components;

public class UniStatusCodeTests
{
    #region Range ctor

    public static TheoryData<Range> CorrectRangeTestCases =>
    [
        (200..200),
        (200..299),
        (400..599),
        (100..599),
    ];
    [Theory, MemberData(nameof(CorrectRangeTestCases))]
    public void CtorRange_CorrectValues_ShouldNotThrow(Range statusCodeRange)
    {
        var action = () => new UniStatusCode(statusCodeRange);
        
        action.Should().NotThrow<ArgumentException>();
    }

    public static TheoryData<Range> IncorrectRangeTestCases =>
    [
        (200..),
        (..299),
        (^200..299),
        (200..^299),
        (200..600),
        (99..500),
        (299..200)
    ];
    [Theory, MemberData(nameof(IncorrectRangeTestCases))]
    public void CtorRange_IncorrectValues_ShouldThrow(Range statusCodeRange)
    {
        var action = () => new UniStatusCode(statusCodeRange);
        
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CtorRange_RangeIsSingleValue_ShouldSaySingleValue()
    {
        var testStatusCode = new UniStatusCode(200..200);
        
        testStatusCode.Range.Should().Be(200..200);
        testStatusCode.IsSingleValue.Should().BeTrue();
        testStatusCode.ToString().Should().Be("200");
    }
    
    [Fact]
    public void CtorRange_RangeIsNotSingleValue_ShouldSaySingleValue()
    {
        var testStatusCode = new UniStatusCode(400..599);
        
        testStatusCode.Range.Should().Be(400..599);
        testStatusCode.IsSingleValue.Should().BeFalse();
        testStatusCode.ToString().Should().Be("400..599");
    }

    #endregion

    #region Enum ctor

    public static TheoryData<HttpStatusCode> CorrectEnumTestCases =>
    [
        HttpStatusCode.OK,
        HttpStatusCode.Continue,
        HttpStatusCode.InternalServerError,
        (HttpStatusCode)599
    ];
    [Theory, MemberData(nameof(CorrectEnumTestCases))]
    public void CtorEnum_CorrectValues_ShouldNotThrow(HttpStatusCode statusCodeRange)
    {
        var action = () => new UniStatusCode(statusCodeRange);
        
        action.Should().NotThrow<ArgumentException>();
    }

    public static TheoryData<HttpStatusCode> IncorrectEnumTestCases =>
    [
        (HttpStatusCode)(-1),
        (HttpStatusCode)0,
        (HttpStatusCode)99,
        (HttpStatusCode)600
    ];
    [Theory, MemberData(nameof(IncorrectEnumTestCases))]
    public void CtorEnum_IncorrectValues_ShouldThrow(HttpStatusCode statusCodeRange)
    {
        var action = () => new UniStatusCode(statusCodeRange);
        
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CtorEnum_ShouldMakeCorrectRange()
    {
        var testStatusCode = new UniStatusCode(HttpStatusCode.OK);
        
        testStatusCode.Range.Should().Be(200..200);
        testStatusCode.IsSingleValue.Should().BeTrue();
        testStatusCode.ToString().Should().Be("200");
    }

    #endregion

    #region Int ctor

    public static TheoryData<int> CorrectIntTestCases => [200, 100, 500, 599];
    [Theory, MemberData(nameof(CorrectIntTestCases))]
    public void CtorInt_CorrectValues_ShouldNotThrow(int statusCodeRange)
    {
        var action = () => new UniStatusCode(statusCodeRange);
        
        action.Should().NotThrow<ArgumentException>();
    }

    public static TheoryData<int> IncorrectIntTestCases => [ -1, 0, 99, 600];
    [Theory, MemberData(nameof(IncorrectIntTestCases))]
    public void CtorInt_IncorrectValues_ShouldThrow(int statusCodeRange)
    {
        var action = () => new UniStatusCode(statusCodeRange);
        
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void CtorInt_ShouldMakeCorrectRange()
    {
        var testStatusCode = new UniStatusCode(200);
        
        testStatusCode.Range.Should().Be(200..200);
        testStatusCode.IsSingleValue.Should().BeTrue();
        testStatusCode.ToString().Should().Be("200");
    }

    #endregion

    #region IsMatch

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData((HttpStatusCode)299)]
    public void IsMatch_StatusCodeMatches_ShouldReturnTrue(HttpStatusCode statusCode)
    {
        UniStatusCode testStatusCode = 200..299;
        
        var result = testStatusCode.IsMatch(statusCode);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public void IsMatch_StatusCodeDoesNotMatch_ShouldReturnFalse(HttpStatusCode statusCode)
    {
        UniStatusCode testStatusCode = 400..499;
        
        var result = testStatusCode.IsMatch(statusCode);

        result.Should().BeFalse();
    }

    #endregion
}
