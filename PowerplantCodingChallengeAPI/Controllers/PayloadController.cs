using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PowerplantCodingChallengeAPI.Models;
using PowerplantCodingChallengeAPI.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PowerplantCodingChallengeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayloadController : ControllerBase
    {
        #region Variables
        private readonly IPayLoadBLL _plservice;
        private readonly ILogRepository _logRepository;
        #endregion

        #region Constructor
        public PayloadController(IPayLoadBLL plserv, ILogRepository logRepository)
        {
            _plservice = plserv;
            _logRepository = logRepository;
        }
        #endregion

        #region UploadFile method
        [HttpPost]
        public void UploadFile(IFormFile file)
        {
            if (file == null)
            {
                _logRepository.WriteInLogFile(DateTime.UtcNow, "API layer: no file found.");
                throw new ArgumentNullException();
            }
            string ext = Path.GetExtension(file.FileName);
            if (ext != ".json")
            {
                _logRepository.WriteInLogFile(DateTime.UtcNow, "API layer: the file format isn't json!");
                throw new FormatException("The file format must be json!");
            }
            _plservice.GetJsonFile(file);
        } 
        #endregion
    }
}
