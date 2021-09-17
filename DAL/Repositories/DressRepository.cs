using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DAL.Repositories
{
    public class DressRepository : BaseRepository<DressModel>
    {
        public DressRepository(OnBoardingSkdDbContext dbContext) : base(dbContext) { }

        //public IQueryable<Dress> GetDressByName(string name)
        //{
        //    return this.GetAll().Where(x => x.Name == name);
        //}

    }
}
