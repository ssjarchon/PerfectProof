namespace PerfectProof;



interface IPerfectSet<TRange, TItem> 
  where TRange : IPerfectRange
  where TItem : IComparable<TItem>
{
  bool Contains(TItem item);
  bool IsSubset(IPerfectSet<TRange, TItem> other);
  bool IsSuperset(IPerfectSet<TRange, TItem> other);
  bool Overlaps(IPerfectSet<TRange, TItem> other);
  bool SetEquals(IPerfectSet<TRange, TItem> other);
}
