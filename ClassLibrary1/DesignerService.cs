using BLL.Redis;
using DAL.Model;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DesignerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;

        public DesignerService(IUnitOfWork unitOfWork, IRedisService redis)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
        }

        public bool IsDesignerExist(string designerName)
        {
            return _unitOfWork.DesignerRepository.IsExist(x => x.Name == designerName);
        }

        public async Task<DAL.Model.DesignerModel> GetDesignerByNameNoTracking(string designerName)
        {
            return await _unitOfWork.DesignerRepository.GetAll().Where(x => x.Name == designerName).AsNoTracking().FirstAsync();
        }
        
        public async Task<DAL.Model.DesignerModel> GetDesignerByName(string designerName)
        {
            return await _unitOfWork.DesignerRepository.GetAll().Where(x => x.Name == designerName).FirstAsync();
        }

        public async Task UpdateDesigner(DesignerModel _designerResult)
        {
            _unitOfWork.DesignerRepository.Edit(_designerResult);
            await _unitOfWork.SaveAsync();
        }

    }
}
