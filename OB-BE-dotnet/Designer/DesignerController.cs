using AutoMapper;
using DAL.Model;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OB_BE_dotnet.Designer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OB_BE_dotnet.Designer
{

    [ApiController]
    [Route("[Controller]")]
    public class DesignerController : ControllerBase
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        private readonly ILogger<DesignerController> _logger;

        public DesignerController(ILogger<DesignerController> logger, UnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

            MapperConfiguration config = new MapperConfiguration(m =>
            {
                m.CreateMap<DesignerDTO, DesignerModel>();
                m.CreateMap<DesignerModel, DesignerDTO>();
            });

            _mapper = config.CreateMapper();
        }

        /// <summary>
        /// Get all designers
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("/Designer/GetAll")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<DesignerDressesDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllAsync()
        {
            var result = await _unitOfWork.DesignerRepository.GetAll().ToListAsync();
            return new OkObjectResult(result);
        }


        /// <summary>
        /// Get designer by name
        /// </summary>
        /// <param name="name">designer Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("/Designer/GetSpecific/{designerName}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DesignerDressesDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetByNameAsync([FromRoute] string designerName)
        {
            var result = await _unitOfWork.DesignerRepository.GetAll().Where(x => x.Name.Contains(designerName)).ToListAsync();

            if (result != null)
            {
                return new OkObjectResult(result);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Create designer
        /// </summary>
        /// <param name="designer">designer Model.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("/Designer/Create")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DesignerDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> CreateDressAsync([FromBody] DesignerWithoutIdDTO designerBody)
        {
            try
            {
                DesignerModel _designerDetail = new DesignerModel();

                _designerDetail.Name = designerBody.Name;
                _designerDetail.Email = designerBody.Email;

                var designerDetail = _mapper.Map<DesignerModel>(_designerDetail);
                var designerResult = await _unitOfWork.DesignerRepository.AddAsync(designerDetail);
                await _unitOfWork.SaveAsync();
                return new OkObjectResult(designerResult);

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// Update designer by id
        /// </summary>
        /// <param name="designerDTO">author data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPut]
        [Route("/Designer/UpdateById/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] DesignerWithoutIdDTO designerBody)
        {
            DesignerModel _designerDetail = new DesignerModel();
            try
            {
                _designerDetail.Id = id;
                _designerDetail.Name = designerBody.Name;
                _designerDetail.Email = designerBody.Email;
                _designerDetail.UpdatedDate = DateTime.Now;

                DesignerModel designerModel = _mapper.Map<DesignerModel>(_designerDetail);
                _unitOfWork.DesignerRepository.Edit(designerModel);
                await _unitOfWork.SaveAsync();
                return new OkObjectResult($"Success update designer with id: {id}");

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new NotFoundResult();
            }
        }

        /// <summary>
        /// Delete designer by id
        /// </summary>
        /// <param name="name">designer Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpDelete]
        [Route("/Designer/DeleteById/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> DeleteByIdAsync([FromRoute] Guid id)
        {
            bool isExist = _unitOfWork.DesignerRepository.IsExist(x => x.Id == id);

            if (isExist)
            {
                _unitOfWork.DesignerRepository.Delete(d => d.Id == id);
                await _unitOfWork.SaveAsync();
                return new OkObjectResult($"Success delete designer with id: {id}");
            }
            else
            {
                return new NotFoundResult();
            }
        }

        /// <summary>
        /// Delete designer by name
        /// </summary>
        /// <param name="name">designer Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpDelete]
        [Route("/Designer/DeleteByName/{designerName}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> DeleteByNameAsync([FromRoute] string designerName)
        {
            bool isExist = _unitOfWork.DesignerRepository.IsExist(x => x.Name == designerName);

            if (isExist)
            {
                _unitOfWork.DesignerRepository.Delete(d => d.Name == designerName);
                await _unitOfWork.SaveAsync();
                return new OkObjectResult($"Success delete designer: {designerName}");

            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
