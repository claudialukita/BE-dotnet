using DAL.Model;

namespace DAL.Repositories
{
    class DesignerRepository : BaseRepository<DesignerModel>
    {
        public DesignerRepository(OnBoardingSkdDbContext dbContext) : base(dbContext) { }

        //public IQueryable<Dress> GetDressByName(string name)
        //{
        //    return this.GetAll().Where(x => x.Name == name);
        //}
    }
}
