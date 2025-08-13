namespace WackyRaces.Domain.Entities;

using WackyRaces.Domain.Types;
using WackyRaces.Domain.Exceptions;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

public sealed partial class TableEntity
{
    private readonly Dictionary<Coordinate, DataValue> cells = new();
    private readonly HashSet<Coordinate> evaluationStack = new();

    [GeneratedRegex(@"^[A-Z]+\d+$", RegexOptions.None, 100)]
    private static partial Regex CellReferenceRegex();

    [GeneratedRegex(@"^[A-Z_][A-Z0-9_]*\([^)]*\)$", RegexOptions.IgnoreCase, 100)]
    private static partial Regex FunctionRegex();

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
        if (this.evaluationStack.Contains(coordinate))
        {
            throw new CircularReferenceException(coordinate);
        }

        this.evaluationStack.Add(coordinate);

        try
        {
            return this.Recalculate(coordinate);
        }
        finally
        {
            this.evaluationStack.Remove(coordinate);
        }
    }

    private DataValue Recalculate(Coordinate coordinate)
    {
        var cellValue = this.GetCell(coordinate);

        if (cellValue.IsT0 && cellValue.AsT0.StartsWith("="))
        {
            return EvaluateFormula(cellValue.AsT0);
        }

        return cellValue;
    }

    private DataValue EvaluateFormula(string formula)
    {
        try
        {
            var expression = formula.Substring(1);

            if (IsFunction(expression))
            {
                return EvaluateFunction(expression);
            }

            var rpnTokens = ConvertToRPN(expression);

            return EvaluateRPN(rpnTokens);
        }
        catch (CircularReferenceException)
        {
            throw;
        }
        catch (UnknownFunctionException)
        {
            throw;
        }
        catch (InvalidFunctionSyntaxException)
        {
            throw;
        }
        catch (InvalidRangeFormatException)
        {
            throw;
        }
        catch (UnsupportedComplexRangeException)
        {
            throw;
        }
        catch (InvalidNumberTokenException)
        {
            throw;
        }
        catch (InsufficientOperandsException)
        {
            throw;
        }
        catch (UnknownOperatorException)
        {
            throw;
        }
        catch (InvalidExpressionException)
        {
            throw;
        }
        catch (UnsupportedDataValueOperationException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new DataValue($"#ERROR: {exception.Message}");
        }
    }

    private bool IsFunction(string expression)
    {
        if (expression.Contains("(") && expression.Contains(")"))
        {
            return FunctionRegex().IsMatch(expression);
        }

        return expression.ToUpper() is "SUM" || expression.ToUpper() is "AVG" || expression.ToUpper() is "COUNT";
    }

    private DataValue EvaluateFunction(string expression)
    {
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
            "AVG" or "AVERAGE" => EvaluateAvgFunction(arguments),
            "COUNT" => EvaluateCountFunction(arguments),
            _ => throw new UnknownFunctionException(functionName),
        };
    }

    private DataValue EvaluateSumFunction(string arguments)
    {
        var coordinates = ParseRange(arguments);
        var sum = new DataValue(0);

        foreach (var coord in coordinates)
        {
            var cellValue = this.GetValue(coord);

            if (cellValue.IsT1 || cellValue.IsT2)
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

            if (cellValue.IsT1 || cellValue.IsT2)
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

            if (cellValue.IsT0 is false || string.IsNullOrEmpty(cellValue.AsT0) is false)
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
            var parts = range.Split(':');

            if (parts.Length != 2)
            {
                throw new InvalidRangeFormatException(range);
            }

            var startCoord = Coordinate.Parse(parts[0].Trim());
            var endCoord = Coordinate.Parse(parts[1].Trim());

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
            if (IsCellReference(token) || IsNumber(token) || IsPercentage(token) || IsFunctionCall(token))
            {
                output.Add(token);
            }
            else if (IsOperator(token))
            {
                while (operatorStack.Count is > 0 && operatorStack.Peek() is not "(" && GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token))
                {
                    output.Add(operatorStack.Pop());
                }

                operatorStack.Push(token);
            }
            else if (token is "(")
            {
                operatorStack.Push(token);
            }
            else if (token is ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() is not "(")
                {
                    output.Add(operatorStack.Pop());
                }
                if (operatorStack.Count > 0 && operatorStack.Peek() is "(")
                {
                    operatorStack.Pop();
                }
                else
                {
                    throw new InvalidExpressionException();
                }
            }
            else
            {
                if (token.All(char.IsLetter))
                {
                    throw new InvalidNumberTokenException(token);
                }
                else if (token.Length is 1 && !char.IsLetterOrDigit(token[0]) && token is not "(" && token is not ")")
                {
                    throw new UnknownOperatorException(token);
                }
                else
                {
                    throw new InvalidNumberTokenException(token);
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            var op = operatorStack.Pop();

            if (op is "(")
            {
                throw new InvalidExpressionException();
            }

            output.Add(op);
        }

        return output;
    }

    private List<string> TokenizeExpression(string expression)
    {
        var tokens = new List<string>();
        var currentToken = string.Empty;

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if (char.IsWhiteSpace(c))
            {
                if (string.IsNullOrEmpty(currentToken) is false)
                {
                    tokens.Add(currentToken);
                    currentToken = string.Empty;
                }
            }
            else if (c is '+' || c is '-' || c is '*' || c is '/' || c is '(' || c is ')' || c is '^' || c is '&')
            {
                if (string.IsNullOrEmpty(currentToken) is false)
                {
                    tokens.Add(currentToken);
                    currentToken = string.Empty;
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

        return this.MergeFunctionTokens(tokens);
    }

    private List<string> MergeFunctionTokens(List<string> tokens)
    {
        var mergedTokens = new List<string>();
        var i = 0;

        while (i < tokens.Count)
        {
            if (i < tokens.Count - 1 &&
                tokens[i + 1] == "(" &&
                tokens[i].All(char.IsLetter))
            {
                var functionToken = tokens[i];
                var parenthesesLevel = 0;
                i++;

                while (i < tokens.Count)
                {
                    functionToken += tokens[i];
                    if (tokens[i] == "(") parenthesesLevel++;
                    else if (tokens[i] == ")") parenthesesLevel--;

                    i++;
                    if (parenthesesLevel == 0) break;
                }

                mergedTokens.Add(functionToken);
            }
            else
            {
                mergedTokens.Add(tokens[i]);
                i++;
            }
        }

        return mergedTokens;
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
            else if (IsFunctionCall(token))
            {
                var functionResult = EvaluateFunction(token);
                stack.Push(functionResult);
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
                    "^" => DataValue.Power(left, right),
                    _ => throw new UnknownOperatorException(token),
                };

                stack.Push(result);
            }
        }

        if (stack.Count is not 1)
        {
            throw new InvalidExpressionException();
        }

        return stack.Pop();
    }

    private bool IsCellReference(string token)
    {
        return CellReferenceRegex().IsMatch(token);
    }

    private bool IsNumber(string token)
    {
        return decimal.TryParse(token, out _);
    }

    private bool IsPercentage(string token)
    {
        return token.EndsWith('%') && Percentage.TryParse(token, out _);
    }

    private bool IsFunctionCall(string token)
    {
        return token.Contains('(') && token.Contains(')') && Function.TryCreate(token, out _);
    }

    private bool IsOperator(string token)
    {
        return token is "+" || token is "-" || token is "*" || token is "/" || token is "^";
    }

    private int GetPrecedence(string op)
    {
        return op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            "^" => 3,
            _ => 0,
        };
    }
}
