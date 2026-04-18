namespace PerfectProof;

public interface IPerfectRange<TStart, TEnd, TOffset> where TStart : INumber where TEnd : INumber where TOffset where INumber
{
    TStart Start { get; }
    TEnd End { get; }
    TOffset Offset { get; }
}
