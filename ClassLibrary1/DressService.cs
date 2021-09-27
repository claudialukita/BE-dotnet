using BLL.Kafka;
using BLL.Redis;
using DAL.Model;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Messaging
{
    public class DressService
    {
        private readonly IKafkaSender _kafkaSender;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IConfiguration _config;
        private readonly IRedisService _redis;
        //private readonly SchedulerService _schedulerService;

        public DressService(IKafkaSender kafkaSender, IUnitOfWork unitOfWork, /*IConfiguration config, */IRedisService redis/*, SchedulerService schedulerService*/)
        {
            _kafkaSender = kafkaSender;
            _unitOfWork = unitOfWork;
            //_config = config;
            _redis = redis;
            //_schedulerService = schedulerService;
        }

        public async Task<List<DressModel>> GetAllDressAsync()
        {
            //_schedulerService.Initialize();
            //var tokenSource = new CancellationToken();
            //await _schedulerService.StartAsync(tokenSource);
            return await _unitOfWork.DressRepository.GetAll().Include(a => a.Designer).ToListAsync();
        }
        
        public async Task<List<DressModel>> GetDressByNameAsync(string dressName)
        {
            List<DressModel> dressModelList = await _redis.GetAsync<List<DressModel>>($"dress_dressName:{dressName}");
            if(dressModelList == null)
            {
                dressModelList = await _unitOfWork.DressRepository.GetAll().Where(x => x.Name.Contains(dressName)).Include(a => a.Designer).ToListAsync();

                await _redis.SaveAsync($"dress_dressName:{dressName}", dressModelList);

            }
            return dressModelList;
        }
        
        public async Task<DressModel> GetDressIdAsync(Guid id)
        {
            DressModel dressModel = await _redis.GetAsync<DressModel>($"dress_dressId:{id}");
            if(dressModel == null)
            {
                dressModel = await _unitOfWork.DressRepository.GetAll().Where(x => x.Id == id).Include(a => a.Designer).FirstAsync();

                await _redis.SaveAsync($"dress_dressId:{id}", dressModel);

            }
            return dressModel;
        }

        public async Task<DressModel> CreateDressAsync(DressModel dressBody)
        {
            await _kafkaSender.SendAsync("tryTopic", dressBody);
            return await _unitOfWork.DressRepository.AddAsync(dressBody);
        }

        public async Task<DressModel> GetDressById(Guid id)
        {
            return await _unitOfWork.DressRepository.GetByIdAsync(id);
        }
        
        public async Task UpdateDress(DressModel _dressDetail)
        {
            _unitOfWork.DressRepository.Edit(_dressDetail);
            await _unitOfWork.SaveAsync();
        }

        public bool IsDressExist(Guid id)
        {
            return _unitOfWork.DressRepository.IsExist(x => x.Id == id);
        }

        public async Task DeleteDressById(Guid id)
        {
            _unitOfWork.DressRepository.Delete(d => d.Id == id);
            await _unitOfWork.SaveAsync();
            await _redis.DeleteAsync($"dress_dressId:{id}");
        }
        
        public async Task DeleteDressByName(string dressName)
        {
            _unitOfWork.DressRepository.Delete(d => d.Name == dressName);
            await _unitOfWork.SaveAsync();
        }
        
        public bool IsDressExistByName(string dressName)
        {
            return _unitOfWork.DressRepository.IsExist(x => x.Name == dressName);
        }

    }
}
