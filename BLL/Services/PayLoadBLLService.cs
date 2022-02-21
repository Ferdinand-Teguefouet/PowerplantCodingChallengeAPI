using BLL.Interfaces;
using BLL.Models;
using BLL.Tools;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PayLoadBLLService : IPayLoadBLL
    {
        #region Variables
        private readonly IProductionPlanRepository _pplantrepo;
        private readonly ILogRepository _logRepository; 
        #endregion

        #region Constructor
        public PayLoadBLLService(IProductionPlanRepository pplantrepository, ILogRepository logRepository)
        {
            _pplantrepo = pplantrepository;
            _logRepository = logRepository;
        }
        #endregion

        #region Method to generate a production plan
        public void GetJsonFile(IFormFile file)
        {
            CheckReceivedFile(file);
            var payload = DeserializeJsonFile(file);

            var response = new List<ResponseBLL>();
            double load = payload.Load;
            ManageNegativeLoad(load);
            List<PowerplantCost> productionCostList = GetProductionCostList(payload);
            List<PowerplantCost> sortedList = SortProductionCostList(productionCostList);

            if (load == 0)
            {
                SwitchOffAllPplantForNullLoad(payload, response);
            }
            else
            {
                GenerateProductionPlan(payload, response, load, sortedList);
            }
        }
        #endregion

        #region Private methods
        private void GenerateProductionPlan(PayLoadBLL payload, List<ResponseBLL> response, double load, List<PowerplantCost> sortedList)
        {
            double percentageOfWind = payload.Fuels.Wind / 100;
            double totalLoad = 0;
            //browse sortedList
            foreach (var item in sortedList)
            {
                PowerplantName ppName = item.Name;
                var powerplant = payload.Powerplants.FirstOrDefault(pp => pp.Name == ppName);
                if (powerplant != null)
                {
                    switch (powerplant.Name)
                    {
                        case PowerplantName.gasfiredbig1:
                            var respObjgf1 = GenerateResponseBLLObject(powerplant.Name.ToString());
                            double gfPower1 = powerplant.Pmax;
                            totalLoad += gfPower1;
                            //load = 570;
                            if (totalLoad > load)
                            {
                                BalancePayload(payload, response, load, totalLoad, sortedList, powerplant, respObjgf1, gfPower1);
                                goto End;
                            }
                            AddThisPplantToList(response, respObjgf1, gfPower1);
                            if (totalLoad == load)
                            {
                                SwitchOffOtherPplants(payload, response, sortedList);
                                goto End;
                            }
                            break;
                        case PowerplantName.gasfiredbig2:
                            var respObjgf2 = GenerateResponseBLLObject(powerplant.Name.ToString());
                            double gfPower2 = powerplant.Pmax;
                            totalLoad += gfPower2;
                            //load = 570;
                            if (totalLoad > load)
                            {
                                BalancePayload(payload, response, load, totalLoad, sortedList, powerplant, respObjgf2, gfPower2);
                                goto End;
                            }
                            AddThisPplantToList(response, respObjgf2, gfPower2);
                            if (totalLoad == load)
                            {
                                SwitchOffOtherPplants(payload, response, sortedList);
                                goto End;
                            }
                            break;
                        case PowerplantName.gasfiredsomewhatsmaller:
                            var respObjgfSomeWS = GenerateResponseBLLObject(powerplant.Name.ToString());
                            double gfSomeWSPower = powerplant.Pmax;
                            totalLoad += gfSomeWSPower;
                            //load = 570;
                            if (totalLoad > load)
                            {
                                BalancePayload(payload, response, load, totalLoad, sortedList, powerplant, respObjgfSomeWS, gfSomeWSPower);
                                break;
                            }
                            AddThisPplantToList(response, respObjgfSomeWS, gfSomeWSPower);
                            if (totalLoad == load)
                            {
                                SwitchOffOtherPplants(payload, response, sortedList);
                                goto End;
                            }
                            break;
                        case PowerplantName.tj1:
                            var respObjtj1 = GenerateResponseBLLObject(powerplant.Name.ToString());
                            double tjPower = powerplant.Pmax;
                            totalLoad += tjPower;
                            //load = 570;
                            if (totalLoad > load)
                            {
                                BalancePayload(payload, response, load, totalLoad, sortedList, powerplant, respObjtj1, tjPower);
                                goto End;
                            }
                            AddThisPplantToList(response, respObjtj1, tjPower);
                            if (totalLoad < load)
                            {
                                // retourner une erreur
                                _logRepository.WriteInLogFile(DateTime.UtcNow, "BLL layer: powerplants cannot provide the requested payload");
                                throw new Exception("Powerplants cannot provide the requested payload!");
                            }
                            else
                            {
                                SwitchOffOtherPplants(payload, response, sortedList);
                            }

                            break;
                        case PowerplantName.windpark1:
                            var respObjwp1 = GenerateResponseBLLObject(powerplant.Name.ToString());
                            double wPower = CalculatePowerToProvide(powerplant.Pmax, percentageOfWind);
                            totalLoad += wPower;
                            //load = 80;
                            if (totalLoad > load)
                            {
                                BalanceWithWindTurbine(payload, response, load, totalLoad, sortedList, respObjwp1, wPower);
                                goto End;
                            }
                            AddThisPplantToList(response, respObjwp1, wPower);
                            if (totalLoad == load)
                            {
                                SwitchOffOtherPplants(payload, response, sortedList);
                                goto End;
                            }

                            break;
                        case PowerplantName.windpark2:
                            var respObjwp2 = GenerateResponseBLLObject(powerplant.Name.ToString());
                            double wPower2 = CalculatePowerToProvide(powerplant.Pmax, percentageOfWind);
                            totalLoad += wPower2;
                            //load = 110.5;
                            if (totalLoad > load)
                            {
                                BalanceWithWindTurbine(payload, response, load, totalLoad, sortedList, respObjwp2, wPower2);
                                goto End;
                            }
                            AddThisPplantToList(response, respObjwp2, wPower2);
                            if (totalLoad == load)
                            {
                                SwitchOffOtherPplants(payload, response, sortedList);
                                goto End;
                            }
                            break;
                    }
                }

            }
        End: return;
        }

        private void AddThisPplantToList(List<ResponseBLL> response, ResponseBLL respObj, double Power)
        {
            respObj.P = Power;
            response.Add(respObj);
        }

        private void BalanceWithWindTurbine(PayLoadBLL payload, List<ResponseBLL> response, double load, double totalLoad, List<PowerplantCost> sortedList, ResponseBLL respObjwp, double wPower)
        {
            var diff = totalLoad - load;
            respObjwp.P = Math.Round(wPower - diff, 1);
            response.Add(respObjwp);
            SwitchOffOtherPplants(payload, response, sortedList);
        }

        private void BalancePayload(PayLoadBLL payload, List<ResponseBLL> response, double load, double totalLoad, List<PowerplantCost> sortedList, PowerplantBLL powerplant, ResponseBLL respObjgf, double gfPower)
        {
            double gfPowerToProvide = BalanceIfTotalLoadIsBiggerThanLoad(response, load, totalLoad, powerplant, gfPower);
            respObjgf.P = gfPowerToProvide;
            response.Add(respObjgf);
            SwitchOffOtherPplants(payload, response, sortedList);
        }

        private void SwitchOffOtherPplants(PayLoadBLL payload, List<ResponseBLL> response, List<PowerplantCost> sortedList)
        {
            int i = payload.Powerplants.Count() - response.Count();
            var responseToSerialize = CompleteList(sortedList, response, i);
            var listToSendToDAL = responseToSerialize.ToDALpplanList();
            //Send list to DAL
            _pplantrepo.SerializeListToAJsonFile(listToSendToDAL);
        }

        private void CheckReceivedFile(IFormFile file)
        {
            if (file == null)
            {
                _logRepository.WriteInLogFile(DateTime.UtcNow, "BLL layer: no file found.");
                throw new ArgumentNullException();
            }
            string ext = Path.GetExtension(file.FileName);
            if (ext != ".json")
            {
                _logRepository.WriteInLogFile(DateTime.UtcNow, "BLL layer: the file format isn't json!");
                throw new FormatException("The file format must be json!");
            }
        }

        private void SwitchOffAllPplantForNullLoad(PayLoadBLL payload, List<ResponseBLL> response)
        {
            _logRepository.WriteInLogFile(DateTime.UtcNow, "BLL layer: we received null payload.");
            foreach (var pplant in payload.Powerplants)
            {
                response.Add(GenerateResponseBLLObject(pplant.Name.ToString()));
            }
            var listToSendToDAL = response.ToDALpplanList();
            //Send list to DAL
            _pplantrepo.SerializeListToAJsonFile(listToSendToDAL);
        }

        private static List<PowerplantCost> SortProductionCostList(List<PowerplantCost> productionCostList)
        {
            var sortProductionCostList = productionCostList.OrderBy(price => price.Cost);

            var sortedList = new List<PowerplantCost>();

            foreach (var prodCost in sortProductionCostList)
            {
                sortedList.Add(prodCost);
            }

            return sortedList;
        }

        private List<PowerplantCost> GetProductionCostList(PayLoadBLL payload)
        {
            var productionCostList = new List<PowerplantCost>();
            var gasPrice = payload.Fuels.Gas;

            foreach (var pplant in payload.Powerplants)
            {
                CalculatePriceOfEachPplant(payload, productionCostList, gasPrice, pplant);
            }

            return productionCostList;
        }

        private void ManageNegativeLoad(double load)
        {
            if (load < 0)
            {
                _logRepository.WriteInLogFile(DateTime.UtcNow, "BLL layer: the payload is negative!");
                throw new ArgumentOutOfRangeException("The payload cannot be negative!");
            }
        }

        private void CalculatePriceOfEachPplant(PayLoadBLL payload, List<PowerplantCost> productionCostList, double gasPrice, PowerplantBLL pplant)
        {
            switch (pplant.Type)
            {
                case PowerplantType.gasfired:
                    var eff = pplant.Efficiency;
                    var gasPriceForOneUnitOfElectricity = gasPrice / eff;
                    productionCostList.Add(GeneratePowerplantCostObject(pplant, gasPriceForOneUnitOfElectricity));
                    break;
                case PowerplantType.turbojet:
                    var kerosinePriceForOneUnitOfElectricity = payload.Fuels.Kerosine;
                    productionCostList.Add(GeneratePowerplantCostObject(pplant, kerosinePriceForOneUnitOfElectricity));
                    break;
                case PowerplantType.windturbine:
                    double windPriceForOneUnitOfElectricity = 0;
                    productionCostList.Add(GeneratePowerplantCostObject(pplant, windPriceForOneUnitOfElectricity));
                    break;
                default:
                    _logRepository.WriteInLogFile(DateTime.UtcNow, "BLL layer: this type of powerplant not exist.");
                    //Ecrire dans le log ("Nous n'avons pas ce type de central.")
                    break;
            }
        }

        private double BalanceIfTotalLoadIsBiggerThanLoad(List<ResponseBLL> response, double load, double totalLoad, PowerplantBLL powerplant, double gfPower)
        {
            var diff = totalLoad - load;
            var gfPowerToProvide = Math.Round(gfPower - diff, 1);
            if (gfPowerToProvide < powerplant.Pmin)
            {
                var toComplete = powerplant.Pmin - gfPowerToProvide;
                double accumulatepower = 0;
                for (int j = response.Count() - 1; j >= 0; j--)
                {
                    accumulatepower += response[j].P;
                    if (toComplete < response[j].P)
                    {
                        // on diminue la puissance de cette centrale
                        response[j].P = Math.Round(response[j].P - toComplete, 1);
                        goto BalanceIsOk;
                    }

                }
                BalanceIfImpossibleForOnePowerplantToDoIt(response, toComplete, accumulatepower);

            BalanceIsOk: gfPowerToProvide = Math.Round(gfPowerToProvide + toComplete, 1);
            }

            return gfPowerToProvide;
        }

        private void BalanceIfImpossibleForOnePowerplantToDoIt(List<ResponseBLL> response, double toComplete, double accumulatepower)
        {
            if (toComplete > accumulatepower)
            {
                // Ecrire dans le log
                _logRepository.WriteInLogFile(DateTime.UtcNow, "BLL layer: available powerplants cannot balance the payload.");
                throw new Exception("Available powerplants cannot balance the payload.");
            }
            else
            {
                //On diminue la puissance à fournir par les centrales activées pour respecter les consignes
                // de fonctionnement d'une centrale
                for (int k = 0; k < response.Count(); k++)
                {
                    response[k].P = Math.Round(response[k].P * toComplete / accumulatepower);
                }

            }
        }

        private double CalculatePowerToProvide(double pmax, double percwind)
        {
            var windturbinePower = pmax * percwind;
            return Math.Round(windturbinePower, 1);
        }

        private List<ResponseBLL> CompleteList(List<PowerplantCost> sortedList, List<ResponseBLL> response, int i)
        {
            while (i != 0)
            {
                foreach (var item in sortedList)
                {
                    var powerplant = response.FirstOrDefault(r => r.Name == item.Name.ToString());
                    if (powerplant == null)
                    {
                        response.Add(GenerateResponseBLLObject(item.Name.ToString()));
                    }
                }
                i--;
            }
            return response;
        }

        private PowerplantCost GeneratePowerplantCostObject(PowerplantBLL pplant, double cost)
        {
            PowerplantCost pCost = new PowerplantCost();
            pCost.Name = pplant.Name;
            pCost.Type = pplant.Type;
            pCost.Cost = Math.Round(cost, 2);
            return pCost;
        }

        private PayLoadBLL DeserializeJsonFile(IFormFile file)
        {
            using (var sReader = new StreamReader(file.FileName))
            {
                var payloadFile = JsonConvert.DeserializeObject<PayLoadBLL>(sReader.ReadToEnd());
                return payloadFile;
            }
        }

        private ResponseBLL GenerateResponseBLLObject(string ppName)
        {
            return new ResponseBLL
            {
                Name = ppName,
                P = 0
            };
        } 
        #endregion
    }
}
