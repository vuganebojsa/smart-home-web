using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartHouse.Core.Model.SmartHomeDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class ModeListValueComparer : ValueComparer<List<Mode>>
    {
        public ModeListValueComparer()
            : base(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            )
        {
        }
    }
}
