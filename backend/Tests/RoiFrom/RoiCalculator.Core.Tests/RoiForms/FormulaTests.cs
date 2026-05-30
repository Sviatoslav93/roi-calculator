using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiCalculator.Core.Tests.RoiForms;

public class FormulaTests
{
    [Theory]
    [InlineData("a + b")]
    [InlineData("revenue * 0 + cost")]
    [InlineData("(a + b) * (c - d)")]
    [InlineData("x")]
    public void Create_WithValidExpression_Succeeds(string expression)
    {
        var result = Formula.Create(expression);

        result.IsSuccess.Should().BeTrue();
        result.Value.Expression.Should().Be(expression);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyExpression_Fails(string expression)
    {
        var result = Formula.Create(expression);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.formula-cannot-be-empty");
    }

    [Theory]
    [InlineData("a + #b")]
    [InlineData("price @ cost")]
    [InlineData("value!")]
    public void Validate_WithInvalidCharacters_Fails(string expression)
    {
        var result = Formula.Create(expression);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.formula-invalid-characters");
    }

    [Theory]
    [InlineData("a + (b")]
    [InlineData("(a + b))")]
    [InlineData(")a(")]
    public void Validate_WithUnbalancedParentheses_Fails(string expression)
    {
        var result = Formula.Create(expression);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.formula-unbalanced-parentheses");
    }

    [Theory]
    [InlineData("a + b", 10, 20, 30)]
    [InlineData("a - b", 50, 20, 30)]
    [InlineData("a * b", 4, 5, 20)]
    [InlineData("a / b", 10, 2, 5)]
    public void Evaluate_WithKnownOperands_ReturnsCorrectResult(
        string expression, decimal a, decimal b, decimal expected)
    {
        var formula = Formula.Create(expression).Value;

        var result = formula.Evaluate(new Dictionary<string, decimal>
        {
            ["a"] = a,
            ["b"] = b
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }

    [Fact]
    public void Evaluate_WithDivisionByZero_Fails()
    {
        var formula = Formula.Create("a / b").Value;

        var result = formula.Evaluate(new Dictionary<string, decimal>
        {
            ["a"] = 10,
            ["b"] = 0
        });

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.formula-divide-by-zero");
    }

    [Fact]
    public void Evaluate_WithUnknownOperand_Fails()
    {
        var formula = Formula.Create("a + b").Value;

        var result = formula.Evaluate(new Dictionary<string, decimal>
        {
            ["a"] = 10
        });

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.formula-unknown-operand");
    }

    [Fact]
    public void Evaluate_WithComplexExpression_ReturnsCorrectResult()
    {
        var formula = Formula.Create("(a + b) * (c - d)").Value;

        var result = formula.Evaluate(new Dictionary<string, decimal>
        {
            ["a"] = 3,
            ["b"] = 2,
            ["c"] = 10,
            ["d"] = 5
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(25);
    }
}
