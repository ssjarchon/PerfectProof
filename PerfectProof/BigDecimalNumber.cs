using System.Numerics;

namespace PerfectProof;

public interface INumber
{
    INumber Add(INumber other);
    INumber Subtract(INumber other);
    INumber Multiply(INumber other);
    INumber Divide(INumber other, int precision = 28);
    string ToPlainString();
}

public sealed class BigDecimalNumber : INumber, IEquatable<BigDecimalNumber>, IComparable<BigDecimalNumber>
{
    private static readonly BigInteger Ten = new(10);

    public BigInteger UnscaledValue { get; }
    public int Scale { get; }

    public BigDecimalNumber(BigInteger unscaledValue, int scale)
    {
        if (scale < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale cannot be negative.");
        }

        UnscaledValue = unscaledValue;
        Scale = scale;
    }

    public static BigDecimalNumber Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(value));
        }

        value = value.Trim();
        var sign = 1;
        if (value[0] == '-')
        {
            sign = -1;
            value = value[1..];
        }
        else if (value[0] == '+')
        {
            value = value[1..];
        }

        var parts = value.Split('.', StringSplitOptions.None);
        if (parts.Length > 2 ||
            parts.Any(static p => p.Length > 0 && !p.All(char.IsDigit)) ||
            (parts.Length == 2 && parts[0].Length == 0 && parts[1].Length == 0))
        {
            throw new FormatException("Invalid decimal format.");
        }

        var integerPart = parts[0];
        var fractionalPart = parts.Length == 2 ? parts[1] : string.Empty;
        var digits = (integerPart + fractionalPart).TrimStart('0');
        if (digits.Length == 0)
        {
            return new BigDecimalNumber(BigInteger.Zero, 0);
        }

        var unscaledValue = BigInteger.Parse(digits) * sign;
        return Normalize(new BigDecimalNumber(unscaledValue, fractionalPart.Length));
    }

    public INumber Add(INumber other)
    {
        var right = GetOther(other);
        var (leftValue, rightValue, scale) = AlignScales(this, right);
        return Normalize(new BigDecimalNumber(leftValue + rightValue, scale));
    }

    public INumber Subtract(INumber other)
    {
        var right = GetOther(other);
        var (leftValue, rightValue, scale) = AlignScales(this, right);
        return Normalize(new BigDecimalNumber(leftValue - rightValue, scale));
    }

    public INumber Multiply(INumber other)
    {
        var right = GetOther(other);
        return Normalize(new BigDecimalNumber(UnscaledValue * right.UnscaledValue, Scale + right.Scale));
    }

    public INumber Divide(INumber other, int precision = 28)
    {
        var right = GetOther(other);
        if (right.UnscaledValue.IsZero)
        {
            throw new DivideByZeroException();
        }

        if (precision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision cannot be negative.");
        }

        var numerator = UnscaledValue * BigInteger.Pow(Ten, precision + right.Scale);
        var denominator = right.UnscaledValue * BigInteger.Pow(Ten, Scale);
        return Normalize(new BigDecimalNumber(numerator / denominator, precision));
    }

    public string ToPlainString()
    {
        if (UnscaledValue.IsZero)
        {
            return "0";
        }

        var sign = UnscaledValue.Sign < 0 ? "-" : string.Empty;
        var digits = BigInteger.Abs(UnscaledValue).ToString();
        if (Scale == 0)
        {
            return sign + digits;
        }

        if (digits.Length <= Scale)
        {
            digits = digits.PadLeft(Scale + 1, '0');
        }

        var decimalIndex = digits.Length - Scale;
        var whole = digits[..decimalIndex];
        var fractional = digits[decimalIndex..].TrimEnd('0');
        return fractional.Length == 0 ? sign + whole : $"{sign}{whole}.{fractional}";
    }

    public int CompareTo(BigDecimalNumber? other)
    {
        if (other is null)
        {
            return 1;
        }

        var (leftValue, rightValue, _) = AlignScales(this, other);
        return leftValue.CompareTo(rightValue);
    }

    public bool Equals(BigDecimalNumber? other)
    {
        if (other is null)
        {
            return false;
        }

        return CompareTo(other) == 0;
    }

    public override bool Equals(object? obj) => obj is BigDecimalNumber other && Equals(other);

    public override int GetHashCode()
    {
        var normalized = Normalize(this);
        return HashCode.Combine(normalized.UnscaledValue, normalized.Scale);
    }

    public override string ToString() => ToPlainString();

    private static (BigInteger Left, BigInteger Right, int Scale) AlignScales(BigDecimalNumber left, BigDecimalNumber right)
    {
        if (left.Scale == right.Scale)
        {
            return (left.UnscaledValue, right.UnscaledValue, left.Scale);
        }

        var maxScale = Math.Max(left.Scale, right.Scale);
        var leftAdjusted = left.UnscaledValue * BigInteger.Pow(Ten, maxScale - left.Scale);
        var rightAdjusted = right.UnscaledValue * BigInteger.Pow(Ten, maxScale - right.Scale);
        return (leftAdjusted, rightAdjusted, maxScale);
    }

    private static BigDecimalNumber Normalize(BigDecimalNumber value)
    {
        if (value.UnscaledValue.IsZero)
        {
            return new BigDecimalNumber(BigInteger.Zero, 0);
        }

        var unscaled = value.UnscaledValue;
        var scale = value.Scale;
        while (scale > 0 && unscaled % Ten == BigInteger.Zero)
        {
            unscaled /= Ten;
            scale--;
        }

        return new BigDecimalNumber(unscaled, scale);
    }

    private static BigDecimalNumber GetOther(INumber other) =>
        other as BigDecimalNumber ?? throw new ArgumentException("Expected a BigDecimalNumber instance.", nameof(other));
}
