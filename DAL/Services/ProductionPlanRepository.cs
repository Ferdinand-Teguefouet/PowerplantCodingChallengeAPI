using DAL.Entities;
using DAL.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class ProductionPlanRepository : IProductionPlanRepository
    {
        #region Variables
        private readonly ILogRepository _logrepository;
        private readonly string JsonFilePath = Environment.CurrentDirectory + "/productionplan.json";
        #endregion

        #region Constructor
        public ProductionPlanRepository(ILogRepository logrepository)
        {
            _logrepository = logrepository;
        }
        #endregion

        #region GetDataFromDAL method (deserialize JSON file)
        public List<ProductionPlan> GetDataFromDAL()
        {
            try
            {
                string jsonFromFile;
                using (var sReader = new StreamReader(JsonFilePath))
                {
                    jsonFromFile = sReader.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<List<ProductionPlan>>(jsonFromFile);
            }
            catch (Exception)
            {
                _logrepository.WriteInLogFile(DateTime.UtcNow, "Error in deserializing the json file.");
                throw new FileNotFoundException("A problem occurred while reading!");
            }

        }
        #endregion

        #region SerializeListToAJsonFile method
        // Serialize response list and save to a json file
        public void SerializeListToAJsonFile(List<ProductionPlan> list)
        {
            try
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();

                if (File.Exists(JsonFilePath))
                {
                    File.Delete(JsonFilePath);
                }

                using (StreamWriter jsonToSave = File.CreateText(JsonFilePath))
                {
                    using (var jsonwritter = new JsonTextWriter(jsonToSave))
                    {
                        jsonwritter.Formatting = Formatting.Indented;
                        serializer.Serialize(jsonwritter, list);
                    }
                    jsonToSave.Close();
                }
            }
            catch (Exception)
            {
                _logrepository.WriteInLogFile(DateTime.UtcNow, "Error of serializing.");
                throw new FileNotFoundException("A problem occurred while writing!"); ;
            }


        } 
        #endregion
    }
}
