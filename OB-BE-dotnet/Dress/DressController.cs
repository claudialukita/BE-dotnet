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
using System.Net.Mime;
using System.Text.Json;
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
        [Route("/GetAll")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<DressModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllAsync()
        {
            var result = await _unitOfWork.DressRepository.GetAll().ToListAsync();
            return new OkObjectResult(result);
        }

        /// <summary>
        /// Get dress by name
        /// </summary>
        /// <param name="name">dress Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("/GetDress/{dressName}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DressDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetByNameAsync([FromRoute]string dressName)
        {
            var result = await _unitOfWork.DressRepository.GetAll().Where(x => x.Name.Contains(dressName)).ToListAsync();

            if(result != null)
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
        [Route("/CreateDress")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DressDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> CreateDressAsync([FromBody]DressWithoutIdDTO dressDTO)
        {
            try
            {
                DressModel _dressDetail = new DressModel();
                //Console.WriteLine(JsonSerializer.Serialize(_dressList));

                _dressDetail.Id = Guid.NewGuid();
                _dressDetail.Name = dressDTO.Name;
                _dressDetail.Type = dressDTO.Type;
                _dressDetail.Color = dressDTO.Color;
                _dressDetail.Size = dressDTO.Size;
                _dressDetail.Price = dressDTO.Price;
                //_dressList.Add(_dressDetail);
                var dressDetail = _mapper.Map<DressModel>(_dressDetail);
                var dressResult = await _unitOfWork.DressRepository.AddAsync(dressDetail);
                await _unitOfWork.SaveAsync();
                return new OkObjectResult(dressResult);

            } catch (Exception e)
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
        [Route("/UpdateDressById/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] DressWithoutIdDTO dressWithoutIdDTO)
        {
            DressModel _dressDetail = new DressModel();
            try
            {
                _dressDetail.Id = id;
                _dressDetail.Name = dressWithoutIdDTO.Name;
                _dressDetail.Type = dressWithoutIdDTO.Type;
                _dressDetail.Color = dressWithoutIdDTO.Color;
                _dressDetail.Size = dressWithoutIdDTO.Size;
                _dressDetail.Price = dressWithoutIdDTO.Price;

                DressModel dressModel = _mapper.Map<DressModel>(_dressDetail);
                _unitOfWork.DressRepository.Edit(dressModel);
                await _unitOfWork.SaveAsync();
                return new OkResult();
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
        [Route("/DeleteDressById/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<DressDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> DeleteDressAsync([FromRoute] Guid id)
        {
            bool isExist = _unitOfWork.DressRepository.IsExist(x => x.Id == id);

            if (isExist)
            {
                _unitOfWork.DressRepository.Delete(d => d.Id == id);
                await _unitOfWork.SaveAsync();
                return new OkResult();
            } else
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
        [Route("/DeleteDressByName/{dressName}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<DressDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> DeleteDressAsync([FromRoute] string dressName)
        {
            bool isExist = _unitOfWork.DressRepository.IsExist(x => x.Name == dressName);

            if (isExist)
            {
                _unitOfWork.DressRepository.Delete(d => d.Name == dressName);
                await _unitOfWork.SaveAsync();
                return new OkResult();
            } else
            {
                return new NotFoundResult();
            }
        }

    }
}
