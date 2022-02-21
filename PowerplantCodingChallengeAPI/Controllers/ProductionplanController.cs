using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PowerplantCodingChallengeAPI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerplantCodingChallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionplanController : ControllerBase
    {
        #region Variables
        private readonly IResponseBll _respbllservice;
        #endregion

        #region Constructor
        public ProductionplanController(IResponseBll respbllservice)
        {
            _respbllservice = respbllservice;
        }
        #endregion

        #region GetAll method
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_respbllservice.GetDataFromBLL().ToApiPplant());

        } 
        #endregion
    }
}
