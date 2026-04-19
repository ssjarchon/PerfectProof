namespace PerfectProof;

interface IPerfectRange{
}

interface IPerfectSet<TRange, TItem> 
  where TRange : IPerfectRange
  where TItem : IComparable<TItem>
{
  
}

