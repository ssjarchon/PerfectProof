namespace PerfectProof;

interface IPerfectRange{
}

interface IPerfectSet<TRange, TItem> 
  where TRange : IPerfectRange
  where TItem : IComparable<TItem>
{
  bool Contains(TItem item);
  bool IsSubsetOf(IEnumerable<TItem> other);
  bool IsSupersetOf(IEnumerable<TItem> other);
  bool IsProperSubsetOf(IEnumerable<TItem> other);
  bool IsProperSupersetOf(IEnumerable<TItem> other);
  bool Overlaps(IEnumerable<TItem> other);
  bool SetEquals(IEnumerable<TItem> other);
}
