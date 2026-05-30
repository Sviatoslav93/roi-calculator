using System.Diagnostics;
using FunctionalPrimitives.Monads.Results;
using FunctionalPrimitives.Monads.Results.Extensions;
using RoiForm.Domain.Common;
using RoiForm.Domain.FormManagement.Errors;
using RoiForm.Domain.FormManagement.Utils;

namespace RoiForm.Domain.FormManagement.ValueObjects;

public sealed class Formula : ValueObject
{
    // For EF Core
    private Formula() { }

    private Formula(string expression, IReadOnlyList<Token> postfixNotation)
    {
        Expression = expression;
        PostfixNotation = postfixNotation;
    }

    public string Expression { get; private set; } = null!;

    public IReadOnlyList<Token> PostfixNotation { get; private set; } = [];

    public static Result<Formula> Create(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return FormManagementErrors.FormulaCannotBeEmpty();
        }

        return from tokens in Tokenizer.Tokenize(expression)
            from _ in InfixValidator.Validate(tokens)
            let rpn = ConvertToPostfix(tokens)
            from __ in ValidatePostfix(rpn)
            select new Formula(expression, rpn);
    }

    public Result<decimal> Evaluate(IReadOnlyDictionary<string, decimal> operands)
    {
        var stack = new Stack<decimal>();

        foreach (var token in PostfixNotation)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                {
                    stack.Push(decimal.Parse(token.Value));
                    break;
                }

                case TokenType.Identifier:
                {
                    if (!operands.TryGetValue(token.Value, out var value))
                        return FormManagementErrors.FormulaUnknownOperand(token.Value);

                    stack.Push(value);
                    break;
                }

                case TokenType.Operator:
                {
                    var right = stack.Pop();
                    var left = stack.Pop();

                    if (token.Value == "/" && right == 0)
                        return FormManagementErrors.FormulaDivideByZero();

                    var result = token.Value switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => left / right,
                        _ => throw new InvalidOperationException($"Unknown operator: {token.Value}")
                    };

                    stack.Push(result);
                    break;
                }

                default:
                    throw new UnreachableException($"Unhandled token type: {token.Type}");
            }
        }

        return stack.Pop();
    }

    private static List<Token> ConvertToPostfix(IReadOnlyList<Token> tokens)
    {
        var stack = new Stack<Token>();
        var output = new List<Token>();

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                case TokenType.Identifier:
                {
                    output.Add(token);
                    break;
                }

                case TokenType.LeftParenthesis:
                {
                    stack.Push(token);
                    break;
                }

                case TokenType.RightParenthesis:
                {
                    while (stack.Count > 0 &&
                           stack.Peek().Type != TokenType.LeftParenthesis)
                    {
                        output.Add(stack.Pop());
                    }

                    // remove '('
                    stack.Pop();
                    break;
                }

                case TokenType.Operator:
                {
                    while (stack.Count > 0
                           && stack.Peek().Type == TokenType.Operator
                           && GetPrecedence(token) <= GetPrecedence(stack.Peek()))
                    {
                        output.Add(stack.Pop());
                    }

                    stack.Push(token);
                    break;
                }

                default:
                    throw new UnreachableException($"Unhandled token type: {token.Type}");
            }
        }

        while (stack.Count > 0)
        {
            output.Add(stack.Pop());
        }

        return output;
    }

    private static int GetPrecedence(Token token) =>
        token.Value switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            _ => throw new InvalidOperationException($"Unknown operator: {token.Value}")
        };

    private static Result<Unit> ValidatePostfix(IReadOnlyList<Token> tokens)
    {
        var stackCount = 0;

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                case TokenType.Identifier:
                {
                    stackCount++;
                    break;
                }

                case TokenType.Operator:
                {
                    if (stackCount < 2)
                        return FormManagementErrors.FormulaInvalidExpression();

                    // consume 2, produce 1
                    stackCount--;
                    break;
                }

                default:
                    return FormManagementErrors.FormulaInvalidExpression();
            }
        }

        if (stackCount != 1)
            return FormManagementErrors.FormulaInvalidExpression();

        return Unit.Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Expression;
    }
}
