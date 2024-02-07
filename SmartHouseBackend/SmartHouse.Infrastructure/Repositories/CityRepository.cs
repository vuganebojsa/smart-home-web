using SmartHouse.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.Repositories
{
    public class CityRepository : RepositoryBase<City>
    {
        public CityRepository(DataContext dataContext) : base(dataContext)
        {

        }
    }
}
