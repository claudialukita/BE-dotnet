using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    class DressRepository : BaseRepository<DressModel>
    {
        public DressRepository(OnBoardingSkdDbContext dbContext) : base(dbContext) { }

        //public IQueryable<Dress> GetDressByName(string name)
        //{
        //    return this.GetAll().Where(x => x.Name == name);
        //}
    }
}
