using Microsoft.AspNetCore.Mvc;
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
        private static List<DressDTO> _dressList = new List<DressDTO>();
        //private static DressDTO _dressDetail = new DressDTO();

        private readonly ILogger<DressController> _logger;

        public DressController(ILogger<DressController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Get all dress
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("/GetAll")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<DressDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult GetAll()
        {
            return new OkObjectResult(_dressList);
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
        public ActionResult GetByName([FromRoute]string dressName)
        {
            var dress = _dressList.Where(d => d.Name == dressName).FirstOrDefault();
            if(dress != null)
            {
                return new OkObjectResult(dress);
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
        public ActionResult CreateDress([FromBody]CreateDressDTO dress)
        {
            DressDTO _dressDetail = new DressDTO();
            Console.WriteLine("Before add");
            Console.WriteLine(JsonSerializer.Serialize(_dressList));
            var isExist = _dressList.Where(d => d.Name == dress.Name).Any();
            if (!isExist)
            {
                _dressDetail.Id = Guid.NewGuid();
                _dressDetail.Name = dress.Name;
                _dressDetail.Type = dress.Type;
                _dressDetail.Color = dress.Color;
                _dressDetail.Size = dress.Size;
                _dressDetail.Price = dress.Price;
                _dressList.Add(_dressDetail);
                return new OkObjectResult(_dressDetail);
            }
            return new BadRequestResult();
        }

        /// <summary>
        /// Delete dress
        /// </summary>
        /// <param name="name">dress Model.</param>
        /// <response code="200">Request ok.</response>
        [HttpDelete]
        [Route("/DeleteDress/{dressName}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<DressDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult DeleteDress([FromRoute] string dressName)
        {
            var isExist = _dressList.Where(d => d.Name == dressName).Any();
            if (isExist)
            {
                var dress = _dressList.Where(d => d.Name == dressName).First();
                _dressList.Remove(dress);
                return new OkObjectResult(_dressList);
            } else
            {
                return new NotFoundResult();
            }
        }

    }
}
