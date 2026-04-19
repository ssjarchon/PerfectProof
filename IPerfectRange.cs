using System.Numerics;

namespace PerfectProof;

public interface IPerfectRange<TStart, TEnd, TOffset>
    : IPerfectRange
    where TStart : INumber<TStart>
    where TEnd : INumber<TEnd>
    where TOffset : INumber<TOffset>
{
    TStart Start { get; }
    TEnd End { get; }
    TOffset Offset { get; }
}

public interface IPerfectRange<TStartAndEnd, TOffset> = IPerfectRange<TStartAndEnd, TStartAndEnd, TOffset>
{
}

interface IPerfectRange{
}
