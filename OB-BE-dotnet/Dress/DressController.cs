using AutoMapper;
using DAL.Repositories;
using DAL.Model;
using BLL.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OB_BE_dotnet.Dress.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BLL;
using BLL.Redis;
using BLL.Kafka;
using Scheduler;
using Microsoft.AspNetCore.Authorization;

namespace OB_BE_dotnet.Dress
{
    [ApiController]
    [Route("[Controller]")]
    public class DressController : ControllerBase
    {
        private static List<DressModel> _dressList = new List<DressModel>();
        //private static DressDTO _dressDetail = new DressDTO();

        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        private readonly ILogger<DressController> _logger;
        private readonly DressService _dressService;
        private readonly DesignerService _designerService;

        public DressController(ILogger<DressController> logger, UnitOfWork unitOfWork, IUnitOfWork iuow, /*IConfiguration configuration,*/ IRedisService redis, IKafkaSender kafkaSender, SchedulerService schedulerService/*, ProcessSumService processSumService*/)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

            MapperConfiguration config = new MapperConfiguration(m =>
            {
                m.CreateMap<DressDTO, DressModel>();
                m.CreateMap<DressModel, DressDTO>();
            });

            _mapper = config.CreateMapper();

            _dressService ??= new DressService(kafkaSender, iuow, /*configuration,*/ redis/*, schedulerService*//*, processSumService*//*, msgSernderFactory*/);
            _designerService ??= new DesignerService(iuow, redis);


        }

        /// <summary>
        /// Check authorization only
        /// </summary>
        /// <response code="200">Request successful.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [ProducesResponseType(200)]
        [HttpGet]
        [Route("check")]
        [Authorize]
        public ActionResult CheckAuthRole()
        {
            return Ok();
        }

        /// <summary>
        /// Get all dress
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("/Dress/GetAll")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<DressModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [Authorize]

        public async Task<ActionResult> GetAllAsync()
        {

            var result = await _dressService.GetAllDressAsync();
            //var result = await _unitOfWork.DressRepository.GetAll().Include(a => a.Designer).ToListAsync();
            return new OkObjectResult(result);
        }

        private object DesignerRepository(DressModel arg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get dress by id
        /// </summary>
        /// <param name="id">dress Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("/Dress/GetDress/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DressModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetById([FromRoute] Guid id)
        {
            //var result = await _unitOfWork.DressRepository.GetAll().Where(x => x.Name.Contains(dressName)).Include(a => a.Designer).ToListAsync();
            var result = await _dressService.GetDressIdAsync(id);

            if (result != null)
            {
                return new OkObjectResult(result);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Get dress by name
        /// </summary>
        /// <param name="name">dress Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("/Dress/GetSpecific/{dressName}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DressModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetByNameAsync([FromRoute] string dressName)
        {
            //var result = await _unitOfWork.DressRepository.GetAll().Where(x => x.Name.Contains(dressName)).Include(a => a.Designer).ToListAsync();
            var result = await _dressService.GetDressByNameAsync(dressName);

            if (result != null)
            {
                return new OkObjectResult(result);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Create dress
        /// </summary>
        /// <param name="dress">dress Model.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("/Dress/Create")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DressByDesignerDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> CreateDressAsync([FromBody] DressByDesignerNoIdDTO dressBody)
        {
            try
            {
                DressModel _dressDetail = new DressModel();
                DesignerModel _designerDetail = new DesignerModel();

                //bool isExist = _unitOfWork.DesignerRepository.IsExist(x => x.Name == dressBody.Designer.Name);
                bool isExist = _designerService.IsDesignerExist(dressBody.Designer.Name);

                if (isExist)
                {
                    var result = await _designerService.GetDesignerByNameNoTracking(dressBody.Designer.Name);
                    _dressDetail.Id = Guid.NewGuid();
                    _dressDetail.Name = dressBody.Name;
                    _dressDetail.Type = dressBody.Type;
                    _dressDetail.Color = dressBody.Color;
                    _dressDetail.Size = dressBody.Size;
                    _dressDetail.Price = dressBody.Price;
                    _dressDetail.DesignerId = result.Id;
                    //untuk set
                    //var dressDetail = _mapper.Map<DressModel>(_dressDetail);
                    //dressDetail.Designer = new DesignerModel
                    //{
                    //    Id = result.Id,
                    //    Name = "Gucci"
                    //};
                    //var dressResult = await _unitOfWork.DressRepository.AddAsync(_dressDetail);
                    var dressResult = await _dressService.CreateDressAsync(_dressDetail);
                    await _unitOfWork.SaveAsync();
                    return new OkObjectResult(dressResult);
                }
                else
                {
                    _designerDetail.Id = Guid.NewGuid();
                    _designerDetail.Name = dressBody.Designer.Name;
                    _designerDetail.Email = dressBody.Designer.Email;

                    _dressDetail.Id = Guid.NewGuid();
                    _dressDetail.Name = dressBody.Name;
                    _dressDetail.Type = dressBody.Type;
                    _dressDetail.Color = dressBody.Color;
                    _dressDetail.Size = dressBody.Size;
                    _dressDetail.Price = dressBody.Price;
                    _dressDetail.Designer = _designerDetail;

                    //var dressResult = await _unitOfWork.DressRepository.AddAsync(_dressDetail);
                    var dressResult = await _dressService.CreateDressAsync(_dressDetail);
                    await _unitOfWork.SaveAsync();
                    return new OkObjectResult(dressResult);
                }


                //}
                //else
                //{
                //    return new NotFoundObjectResult("Designer not found");
                //}

                //Console.WriteLine(JsonSerializer.Serialize(_dressList));

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// Update dress by id
        /// </summary>
        /// <param name="dressDTO">author data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPut]
        [Route("/Dress/UpdateById/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] DressByDesignerNoIdDTO dressBody)
        {

            try
            {
                //var _dressDetail = await _unitOfWork.DressRepository.GetByIdAsync(id);
                var _dressDetail = await _dressService.GetDressById(id);

                if (_dressDetail != null)
                {

                    //var result = await _unitOfWork.DesignerRepository.GetAll().Where(x => x.Name == dressBody.Designer.Name).FirstAsync();
                    var result = await _designerService.GetDesignerByName(dressBody.Designer.Name);

                    result.Name = dressBody.Designer.Name;
                    result.Email = dressBody.Designer.Email;
                    result.UpdatedDate = DateTime.Now;

                    _dressDetail.Name = dressBody.Name;
                    _dressDetail.Type = dressBody.Type;
                    _dressDetail.Color = dressBody.Color;
                    _dressDetail.Size = dressBody.Size;
                    _dressDetail.Price = dressBody.Price;
                    _dressDetail.UpdatedDate = DateTime.Now;

                    //_unitOfWork.DressRepository.Edit(_dressDetail);
                    //_unitOfWork.DesignerRepository.Edit(result);
                    //await _unitOfWork.SaveAsync();

                    await _dressService.UpdateDress(_dressDetail);
                    await _designerService.UpdateDesigner(result);
                    return new OkObjectResult($"Success update dress with id: {id}");

                }
                else
                {
                    return new NotFoundObjectResult("Designer not found");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new NotFoundResult();
            }
        }

        /// <summary>
        /// Delete dress by id
        /// </summary>
        /// <param name="name">dress Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpDelete]
        [Route("/Dress/DeleteById/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> DeleteDressAsync([FromRoute] Guid id)
        {
            //bool isExist = _unitOfWork.DressRepository.IsExist(x => x.Id == id);
            bool isExist = _dressService.IsDressExist(id);

            if (isExist)
            {
                //_unitOfWork.DressRepository.Delete(d => d.Id == id);
                //await _unitOfWork.SaveAsync();
                await _dressService.DeleteDressById(id);
                return new OkObjectResult($"Success delete dress with id: {id}");

            }
            else
            {
                return new NotFoundResult();
            }
        }

        /// <summary>
        /// Delete dress by name
        /// </summary>
        /// <param name="name">dress Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpDelete]
        [Route("/Dress/DeleteByName/{dressName}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> DeleteDressAsync([FromRoute] string dressName)
        {
            //bool isExist = _unitOfWork.DressRepository.IsExist(x => x.Name == dressName);
            bool isExist = _dressService.IsDressExistByName(dressName);

            if (isExist)
            {
                //_unitOfWork.DressRepository.Delete(d => d.Name == dressName);
                //await _unitOfWork.SaveAsync();
                await _dressService.DeleteDressByName(dressName);
                return new OkObjectResult($"Success delete dress: {dressName}");

            }
            else
            {
                return new NotFoundResult();
            }
        }

    }
}
