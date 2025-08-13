namespace WackyRaces.Domain.Types;

using WackyRaces.Domain.Exceptions;

public readonly record struct Function
{
    public string Value { get; private init; }
    public Type Format { get; private init; }

    public Function(string value, Type? format = null)
    {
        ValidateFunction(value);
        this.Value = value;
        this.Format = format ?? typeof(decimal);
    }

    private static void ValidateFunction(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidFunctionValueException(value);
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.StartsWith("="))
        {
            throw new InvalidFunctionValueException($"Function value should not start with '='. Use '{trimmedValue.Substring(1)}' instead of '{trimmedValue}'.");
        }

        if (IsValidFunctionExpression(trimmedValue) is false)
        {
            throw new UnknownFunctionException(value);
        }
    }

    private static bool IsValidFunctionExpression(string expression)
    {
        var validFunctions = new HashSet<string>
        {
            "SUM",
            "AVG",
            "AVERAGE",
            "COUNT",
            "MIN",
            "MAX",
            "CONCAT",
        };

        if (expression.Contains('('))
        {
            var openParenIndex = expression.IndexOf('(');
            var functionName = expression.Substring(0, openParenIndex).ToUpper();

            return validFunctions.Contains(functionName);
        }

        return validFunctions.Contains(expression.ToUpper());
    }

    public override string ToString()
    {
        return this.Value;
    }

    public string GetFunctionName()
    {
        var trimmedValue = this.Value.Trim();

        if (trimmedValue.Contains('('))
        {
            var openParenIndex = trimmedValue.IndexOf('(');

            return trimmedValue.Substring(0, openParenIndex).ToUpper();
        }

        return trimmedValue.ToUpper();
    }

    public string GetArguments()
    {
        var trimmedValue = this.Value.Trim();

        if (trimmedValue.Contains('(') is false)
        {
            return string.Empty;
        }

        var openParen = trimmedValue.IndexOf('(');
        var closeParen = trimmedValue.LastIndexOf(')');

        if (openParen is -1 || closeParen is -1 || closeParen <= openParen)
        {
            throw new InvalidFunctionSyntaxException(trimmedValue);
        }

        return trimmedValue.Substring(openParen + 1, closeParen - openParen - 1);
    }

    public List<string> GetArgumentTokens()
    {
        var arguments = this.GetArguments();

        if (string.IsNullOrWhiteSpace(arguments))
        {
            return new List<string>();
        }

        return this.TokenizeArguments(arguments);
    }

    private List<string> TokenizeArguments(string arguments)
    {
        var tokens = new List<string>();
        var currentToken = string.Empty;
        var parenthesesLevel = 0;
        var isInRange = false;

        for (int i = 0; i < arguments.Length; i++)
        {
            char c = arguments[i];

            if (c is '(')
            {
                parenthesesLevel++;
                currentToken += c;
            }
            else if (c is ')')
            {
                parenthesesLevel--;
                currentToken += c;
            }
            else if (char.IsWhiteSpace(c) && parenthesesLevel == 0)
            {
                if (string.IsNullOrEmpty(currentToken) is false)
                {
                    tokens.Add(currentToken.Trim());
                    currentToken = string.Empty;
                }
            }
            else if (c is ',' && parenthesesLevel == 0 && isInRange is false)
            {
                if (string.IsNullOrEmpty(currentToken) is false)
                {
                    tokens.Add(currentToken.Trim());
                    currentToken = string.Empty;
                }
            }
            else if (c is ':')
            {
                currentToken += c;
                isInRange = true;
            }
            else
            {
                currentToken += c;

                if (isInRange && (i == arguments.Length - 1 || arguments[i + 1] is ',' || char.IsWhiteSpace(arguments[i + 1])))
                {
                    isInRange = false;
                }
            }
        }

        if (string.IsNullOrEmpty(currentToken) is false)
        {
            tokens.Add(currentToken.Trim());
        }

        return tokens.Where(t => string.IsNullOrEmpty(t) is false).ToList();
    }

    public bool HasArguments()
    {
        return this.Value.Contains('(') && this.Value.Contains(')');
    }

    public bool IsNestedFunction()
    {
        var arguments = this.GetArguments();

        if (string.IsNullOrWhiteSpace(arguments))
        {
            return false;
        }

        return arguments.Contains('(') && arguments.Contains(')');
    }

    public List<Function> GetNestedFunctions()
    {
        var tokens = this.GetArgumentTokens();
        var nestedFunctions = new List<Function>();

        foreach (var token in tokens)
        {
            if (token.Contains('(') && token.Contains(')'))
            {
                if (TryCreate(token, this.Format, out Function nestedFunction))
                {
                    nestedFunctions.Add(nestedFunction);
                }
            }
        }

        return nestedFunctions;
    }

    public static bool TryCreate(string value, out Function function)
    {
        return TryCreate(value, typeof(decimal), out function);
    }

    public static bool TryCreate(string value, Type? format, out Function function)
    {
        try
        {
            function = new Function(value, format);

            return true;
        }
        catch
        {
            function = default;

            return false;
        }
    }
}
