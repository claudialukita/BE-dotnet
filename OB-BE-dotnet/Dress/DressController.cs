using AutoMapper;
using DAL.Repositories;
using DAL.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OB_BE_dotnet.Dress.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public DressController(ILogger<DressController> logger, UnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

            MapperConfiguration config = new MapperConfiguration(m =>
            {
                m.CreateMap<DressDTO, DressModel>();
                m.CreateMap<DressModel, DressDTO>();
            });

            _mapper = config.CreateMapper();

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
        public async Task<ActionResult> GetAllAsync()
        {
            //var result = await _unitOfWork.DressRepository.GetAll().ToListAsync();
            //_repository.GetAll().Include(d => d.Child).ThenInclude(c => c.GrandChild).FirstOrDefault(x => x.Id.Equals(id));
            var result = await _unitOfWork.DressRepository.GetAll().Include(a => a.Designer).ToListAsync();
            return new OkObjectResult(result);
        }

        private object DesignerRepository(DressModel arg)
        {
            throw new NotImplementedException();
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
            var result = await _unitOfWork.DressRepository.GetAll().Where(x => x.Name.Contains(dressName)).Include(a => a.Designer).ToListAsync();

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

                //if (isExist)
                //{
                //    var result = await _unitOfWork.DesignerRepository.GetAll().Where(x => x.Name == dressBody.Designer.Name).FirstAsync();
                //    _dressDetailNoDesigner.Id = Guid.NewGuid();
                //    _dressDetailNoDesigner.Name = dressBody.Name;
                //    _dressDetailNoDesigner.Type = dressBody.Type;
                //    _dressDetailNoDesigner.Color = dressBody.Color;
                //    _dressDetailNoDesigner.Size = dressBody.Size;
                //    _dressDetailNoDesigner.Price = dressBody.Price;
                //    _dressDetailNoDesigner.DesignerId = result.Id;
                //    var dressDetail = _mapper.Map<DressNoDesignerModel>(_dressDetailNoDesigner);
                //    var dressResult = await _unitOfWork.DressNoDesignerRepository.AddAsync(dressDetail);
                //    await _unitOfWork.SaveAsync();
                //    return new OkObjectResult(dressResult);


                //} else
                //{
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
                    var dressDetail = _mapper.Map<DressModel>(_dressDetail);
                    var dressResult = await _unitOfWork.DressRepository.AddAsync(dressDetail);
                    await _unitOfWork.SaveAsync();
                    return new OkObjectResult(dressResult);
                //}

                
                

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
            DressModel _dressDetail = new DressModel();
            DesignerModel _designerDetail = new DesignerModel();

            try
            {
                bool isExist = _unitOfWork.DressRepository.IsExist(x => x.Id == id);

                if (isExist)
                {

                    var result = await _unitOfWork.DesignerRepository.GetAll().Where(x => x.Name == dressBody.Designer.Name).AsNoTracking().FirstAsync();

                    _designerDetail.Id = result.Id;
                    _designerDetail.Name = dressBody.Designer.Name;
                    _designerDetail.Email = dressBody.Designer.Email;
                    _designerDetail.UpdatedDate = DateTime.Now;

                    _dressDetail.Id = id;
                    _dressDetail.Name = dressBody.Name;
                    _dressDetail.Type = dressBody.Type;
                    _dressDetail.Color = dressBody.Color;
                    _dressDetail.Size = dressBody.Size;
                    _dressDetail.Price = dressBody.Price;
                    _dressDetail.Designer = _designerDetail;
                    _dressDetail.UpdatedDate = DateTime.Now;

                    DressModel dressModel = _mapper.Map<DressModel>(_dressDetail);
                    _unitOfWork.DressRepository.Edit(dressModel);
                    await _unitOfWork.SaveAsync();
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
            bool isExist = _unitOfWork.DressRepository.IsExist(x => x.Id == id);

            if (isExist)
            {
                _unitOfWork.DressRepository.Delete(d => d.Id == id);
                await _unitOfWork.SaveAsync();
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
            bool isExist = _unitOfWork.DressRepository.IsExist(x => x.Name == dressName);

            if (isExist)
            {
                _unitOfWork.DressRepository.Delete(d => d.Name == dressName);
                await _unitOfWork.SaveAsync();
                return new OkObjectResult($"Success delete dress: {dressName}");

            }
            else
            {
                return new NotFoundResult();
            }
        }

    }
}
