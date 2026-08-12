using FunctionalPrimitives.Monads.Results;
using RoiForm.Domain.FormManagement.Errors;
using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiForm.Domain.FormManagement.Utils;

public static class Tokenizer
{
    public static Result<IReadOnlyList<Token>> Tokenize(string expression)
    {
        var tokens = new List<Token>();

        var i = 0;

        while (i < expression.Length)
        {
            var c = expression[i];

            // whitespace
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            // number
            if (char.IsDigit(c))
            {
                var start = i;

                while (i < expression.Length &&
                       (char.IsDigit(expression[i]) ||
                        expression[i] == '.'))
                {
                    i++;
                }

                var number = expression[start..i];

                if (!decimal.TryParse(number, out _))
                {
                    return FormManagementErrors.FormulaInvalidCharacters();
                }

                tokens.Add(new Token(TokenType.Number, number));

                continue;
            }

            // identifier
            if (char.IsLetter(c) || c == '_')
            {
                var start = i;

                while (i < expression.Length &&
                       (char.IsLetterOrDigit(expression[i]) ||
                        expression[i] == '_'))
                {
                    i++;
                }

                tokens.Add(new Token(TokenType.Identifier, expression[start..i]));

                continue;
            }

            // operators
            if ("+-*/".Contains(c))
            {
                tokens.Add(new Token(TokenType.Operator, c.ToString()));

                i++;
                continue;
            }

            switch (c)
            {
                case '(':
                    tokens.Add(new Token(TokenType.LeftParenthesis, "("));

                    i++;
                    continue;
                case ')':
                    tokens.Add(new Token(
                        TokenType.RightParenthesis,
                        ")"));

                    i++;
                    continue;
                default:
                    return FormManagementErrors.FormulaInvalidCharacters();
            }
        }

        return tokens;
    }
}
