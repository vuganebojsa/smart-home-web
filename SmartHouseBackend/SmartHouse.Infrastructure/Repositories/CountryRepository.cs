using SmartHouse.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Infrastructure.Repositories
{
    public class CountryRepository: RepositoryBase<Country>
    {
        public CountryRepository(DataContext dataContext) : base(dataContext)
        {

        }
    }
}
