using FunctionalPrimitives.Monads.Results.Extensions;
using RoiForm.Domain.FormManagement.Errors;
using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiForm.Domain.FormManagement.Utils;

public static class InfixValidator
{
    public static UnitResult Validate(IReadOnlyList<Token> tokens)
    {
        return tokens.Count == 0
            ? FormManagementErrors.FormulaCannotBeEmpty()
            : (from _ in ValidateParentheses(tokens)
            from __ in ValidateTokenOrder(tokens)
            select Unit.Value);
    }

    private static UnitResult ValidateParentheses(IReadOnlyList<Token> tokens)
    {
        var count = 0;

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.LeftParenthesis:
                    count++;
                    break;
                case TokenType.RightParenthesis:
                {
                    count--;

                    if (count < 0)
                        return FormManagementErrors.FormulaUnbalancedParentheses();

                    break;
                }
            }
        }

        return count != 0 ? FormManagementErrors.FormulaUnbalancedParentheses() : Unit.Value;
    }

    private static UnitResult ValidateTokenOrder(IReadOnlyList<Token> tokens)
    {
        Token? previous = null;

        foreach (var current in tokens)
        {
            if (previous != null)
            {
                // operator after operator
                if (previous.Type == TokenType.Operator &&
                    current.Type == TokenType.Operator)
                {
                    return FormManagementErrors.FormulaInvalidExpression();
                }

                // operand after operand
                if (IsOperand(previous) && IsOperand(current))
                {
                    return FormManagementErrors.FormulaInvalidExpression();
                }

                // "( *"
                if (previous.Type == TokenType.LeftParenthesis &&
                    current.Type == TokenType.Operator)
                {
                    return FormManagementErrors.FormulaInvalidExpression();
                }

                // "3 ("
                if (IsOperand(previous) &&
                    current.Type == TokenType.LeftParenthesis)
                {
                    return FormManagementErrors.FormulaInvalidExpression();
                }
            }

            previous = current;
        }

        var last = tokens[^1];

        return last.Type == TokenType.Operator ? FormManagementErrors.FormulaInvalidExpression() : Unit.Value;
    }

    private static bool IsOperand(Token token) =>
        token.Type is TokenType.Number or TokenType.Identifier;
}
