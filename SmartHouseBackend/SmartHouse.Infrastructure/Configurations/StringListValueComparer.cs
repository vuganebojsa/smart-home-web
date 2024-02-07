using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SmartHouse.Infrastructure.Configurations
{
    public class StringListValueComparer : ValueComparer<List<string>>
    {
        public StringListValueComparer(bool deep = false)
            : base(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => deep ? c.Select(s => new string(s.ToCharArray())).ToList() : c.ToList())
        {
        }
    }
}
