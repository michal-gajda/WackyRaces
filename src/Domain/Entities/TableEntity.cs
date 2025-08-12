namespace WackyRaces.Domain.Entities;

using WackyRaces.Domain.Types;
using WackyRaces.Domain.Exceptions;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

public sealed class TableEntity
{
    private readonly Dictionary<Coordinate, DataValue> cells = new();

    public ReadOnlyDictionary<Coordinate, DataValue> Cells => this.cells.AsReadOnly();

    public TableId Id { get; private init; }
    public string Name { get; private set; } = string.Empty;

    public TableEntity(TableId id, string name)
    {
        this.Id = id;
        this.SetName(name);
    }

    public void SetName(string name)
    {
        var trimmedName = name.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            throw new InvalidTableNameException(name);
        }

        this.Name = trimmedName;
    }

    public void SetCell(Coordinate coordinate, DataValue value)
    {
        this.cells[coordinate] = value;
    }

    public DataValue GetCell(Coordinate coordinate)
    {
        return this.cells.GetValueOrDefault(coordinate, new DataValue(string.Empty));
    }

    public DataValue GetValue(Coordinate coordinate)
    {
        return this.Recalculate(coordinate);
    }

    private DataValue Recalculate(Coordinate coordinate)
    {
        var cellValue = this.GetCell(coordinate);

        // If it's a string that starts with "=", treat it as a formula
        if (cellValue.IsT0 && cellValue.AsT0.StartsWith("="))
        {
            return EvaluateFormula(cellValue.AsT0);
        }

        // Otherwise return the cell value as-is
        return cellValue;
    }

    private DataValue EvaluateFormula(string formula)
    {
        try
        {
            // Remove the "=" prefix
            var expression = formula.Substring(1);

            // Check if it's a function call
            if (IsFunction(expression))
            {
                return EvaluateFunction(expression);
            }

            // Convert infix to postfix (RPN)
            var rpnTokens = ConvertToRPN(expression);

            // Evaluate RPN expression
            return EvaluateRPN(rpnTokens);
        }
        catch (Exception ex)
        {
            return new DataValue($"#ERROR: {ex.Message}");
        }
    }

    private bool IsFunction(string expression)
    {
        return expression.Contains("(") && expression.Contains(")") &&
               (expression.StartsWith("SUM(") || expression.StartsWith("AVG(") || expression.StartsWith("COUNT("));
    }

    private DataValue EvaluateFunction(string expression)
    {
        // Parse function name and arguments
        var openParen = expression.IndexOf('(');
        var closeParen = expression.LastIndexOf(')');

        if (openParen == -1 || closeParen == -1 || closeParen <= openParen)
        {
            throw new InvalidFunctionSyntaxException(expression);
        }

        var functionName = expression.Substring(0, openParen).ToUpper();
        var arguments = expression.Substring(openParen + 1, closeParen - openParen - 1);

        return functionName switch
        {
            "SUM" => EvaluateSumFunction(arguments),
            "AVG" => EvaluateAvgFunction(arguments),
            "COUNT" => EvaluateCountFunction(arguments),
            _ => throw new UnknownFunctionException(functionName)
        };
    }

    private DataValue EvaluateSumFunction(string arguments)
    {
        var coordinates = ParseRange(arguments);
        var sum = new DataValue(0);

        foreach (var coord in coordinates)
        {
            var cellValue = this.GetValue(coord);
            if (cellValue.IsT1 || cellValue.IsT2) // int or decimal
            {
                sum = sum + cellValue;
            }
        }

        return sum;
    }

    private DataValue EvaluateAvgFunction(string arguments)
    {
        var coordinates = ParseRange(arguments);
        var sum = new DataValue(0);
        var count = 0;

        foreach (var coord in coordinates)
        {
            var cellValue = this.GetValue(coord);
            if (cellValue.IsT1 || cellValue.IsT2) // int or decimal
            {
                sum = sum + cellValue;
                count++;
            }
        }

        if (count == 0)
        {
            return new DataValue(0);
        }

        return sum / new DataValue(count);
    }

    private DataValue EvaluateCountFunction(string arguments)
    {
        var coordinates = ParseRange(arguments);
        var count = 0;

        foreach (var coord in coordinates)
        {
            var cellValue = this.GetValue(coord);
            if (cellValue.IsT0 is false || string.IsNullOrEmpty(cellValue.AsT0) is false) // Not empty string
            {
                count++;
            }
        }

        return new DataValue(count);
    }
    private List<Coordinate> ParseRange(string range)
    {
        var coordinates = new List<Coordinate>();

        if (range.Contains(':'))
        {
            // Range format like A1:A3
            var parts = range.Split(':');
            if (parts.Length != 2)
            {
                throw new InvalidRangeFormatException(range);
            }

            var startCoord = Coordinate.Parse(parts[0].Trim());
            var endCoord = Coordinate.Parse(parts[1].Trim());

            // For now, handle simple column ranges (same column, different rows)
            if (startCoord.ColumnId.Value == endCoord.ColumnId.Value)
            {
                var startRow = Math.Min(startCoord.RowId.Value, endCoord.RowId.Value);
                var endRow = Math.Max(startCoord.RowId.Value, endCoord.RowId.Value);

                for (int row = startRow; row <= endRow; row++)
                {
                    coordinates.Add(new Coordinate(new RowId(row), new ColumnId(startCoord.ColumnId.Value)));
                }
            }
            else
            {
                throw new UnsupportedComplexRangeException(range);
            }
        }
        else
        {
            // Single cell reference
            coordinates.Add(Coordinate.Parse(range.Trim()));
        }

        return coordinates;
    }

    private List<string> ConvertToRPN(string expression)
    {
        var output = new List<string>();
        var operatorStack = new Stack<string>();
        var tokens = TokenizeExpression(expression);

        foreach (var token in tokens)
        {
            if (IsCellReference(token) || IsNumber(token) || IsPercentage(token))
            {
                output.Add(token);
            }
            else if (IsOperator(token))
            {
                while (operatorStack.Count > 0 &&
                       operatorStack.Peek() != "(" &&
                       GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token))
                {
                    output.Add(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    output.Add(operatorStack.Pop());
                }
                if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                {
                    operatorStack.Pop(); // Remove the "("
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            output.Add(operatorStack.Pop());
        }

        return output;
    }

    private List<string> TokenizeExpression(string expression)
    {
        var tokens = new List<string>();
        var currentToken = "";

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if (char.IsWhiteSpace(c))
            {
                if (string.IsNullOrEmpty(currentToken) is false)
                {
                    tokens.Add(currentToken);
                    currentToken = "";
                }
            }
            else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')')
            {
                if (string.IsNullOrEmpty(currentToken) is false)
                {
                    tokens.Add(currentToken);
                    currentToken = "";
                }
                tokens.Add(c.ToString());
            }
            else
            {
                currentToken += c;
            }
        }

        if (string.IsNullOrEmpty(currentToken) is false)
        {
            tokens.Add(currentToken);
        }

        return tokens;
    }

    private DataValue EvaluateRPN(List<string> rpnTokens)
    {
        var stack = new Stack<DataValue>();

        foreach (var token in rpnTokens)
        {
            if (IsCellReference(token))
            {
                var coordinate = Coordinate.Parse(token);
                var cellValue = this.GetValue(coordinate);
                stack.Push(cellValue);
            }
            else if (IsNumber(token) || IsPercentage(token))
            {
                if (IsPercentage(token))
                {
                    var percentage = Percentage.Parse(token);
                    stack.Push(new DataValue(percentage));
                }
                else if (int.TryParse(token, out int intValue))
                {
                    stack.Push(new DataValue(intValue));
                }
                else if (decimal.TryParse(token, out decimal decimalValue))
                {
                    stack.Push(new DataValue(decimalValue));
                }
                else
                {
                    throw new InvalidNumberTokenException(token);
                }
            }
            else if (IsOperator(token))
            {
                if (stack.Count < 2)
                {
                    throw new InsufficientOperandsException(token);
                }

                var right = stack.Pop();
                var left = stack.Pop();

                DataValue result = token switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    _ => throw new UnknownOperatorException(token)
                };

                stack.Push(result);
            }
        }

        if (stack.Count != 1)
        {
            throw new InvalidExpressionException();
        }

        return stack.Pop();
    }

    private bool IsCellReference(string token)
    {
        return Regex.IsMatch(token, @"^[A-Z]+\d+$");
    }

    private bool IsNumber(string token)
    {
        return decimal.TryParse(token, out _);
    }

    private bool IsPercentage(string token)
    {
        return token.EndsWith('%') && Percentage.TryParse(token, out _);
    }

    private bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    private int GetPrecedence(string op)
    {
        return op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            _ => 0
        };
    }
}
