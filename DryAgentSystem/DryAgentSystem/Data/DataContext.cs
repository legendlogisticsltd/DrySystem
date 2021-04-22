using System.Data.SqlClient;
using System.Configuration;
using DryAgentSystem.Models;
using Dapper;
using System.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Globalization;
using System.Data.Odbc;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Data
{
    public class DataContext
    {
        public static ErrorLog errorLog = new ErrorLog();
        public static bool GetValidUser(UserDetails userDetails)
        {
            string currentPassword = userDetails.Password;

            var parameters = new DynamicParameters();
            parameters.Add("Name", userDetails.Name, DbType.String, ParameterDirection.Input);

            try
            {
                //parameters.Add("@Password", currentPassword, DbType.String, ParameterDirection.Input);
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var valid = conn.Query<UserDetails>("Select Name, Password from UserAccountManagement where Name = ?", parameters);

                    if (valid.Any())
                    {
                        string dbPassword = valid.First(v => v.Name == userDetails.Name).Password;

                        if (dbPassword == currentPassword)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            catch (Exception)
            {

                throw;
            }
        }

        public static string GetRolesForUser(string username)
        {
            string currentUserName = username;
            string userRole = string.Empty;
            try
            {
                var userRoleParam = new DynamicParameters();
                userRoleParam.Add("Name", currentUserName, DbType.String, ParameterDirection.Input);

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var roles = conn.Query<string>("Select privilegerole From UserAccountManagement  Where Name = ?", userRoleParam);

                    userRole = roles.ToString();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return userRole;
        }

        public static UserDetails GetUserDetailsForUser(string username)
        {

            string currentUserName = username;
            
            UserDetails userdetail = new UserDetails();
            try
            {
                var userRoleParam = new DynamicParameters();
                userRoleParam.Add("Name", currentUserName, DbType.String, ParameterDirection.Input);

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {

                    string query = "Select IDCompany From UserAccountManagement  Where Name = ?";

                    var userdetails = conn.Query<UserDetails>(query, userRoleParam);
                    foreach (var item in userdetails)
                    {
                        userdetail.UserID = item.UserID;
                        userdetail.Name = item.Name;
                        userdetail.EmailID = item.EmailID;
                        userdetail.IDCompany = item.IDCompany;
                        userdetail.Location = item.Location;
                        userdetail.Role = item.Role;
                        userdetail.RoleType = item.RoleType;

                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return userdetail;
        }

        public static List<RateRequest> GetAllRateRequestData(SearchParameters filter)
        {
            string username = HttpContext.Current.User.Identity.Name;
            List<RateRequest> rateRequests = new List<RateRequest>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                   string selectquery = "Select RR.RateID, RR.CompanyName, RR.ShipmentTerm, RR.LoadPort, RR.DischargePort, RR.PlaceOfReceipt, RR.TransshipmentPort, RR.PlaceOfDelivery, RR.POLFreeDays, RR.PODFreeDays, RR.EffectiveDate, " +
                        "RR.ValidityDate, RR.Status, RR.Quantity, RR.EquipmentType, RR.CargoType, RR.Rate, RR.RateCountered, RR.AgentName, RR.ModifiedDate From RateRequest RR INNER JOIN UserAccountManagement UM ON RR.IDCompany = UM.IDCompany WHERE UM.Name = '" + username + "' AND";

                    if((filter.CompanyName != null) || (filter.Status != null) || (filter.DischargePort != null) || (filter.LoadPort != null) || (filter.RateID != null))
                    {
                        //selectquery += " WHERE ";

                        if (filter.CompanyName != null)
                        {
                            selectquery += " RR.CompanyName = '" + filter.CompanyName + "' AND";
                        }
                        if (filter.Status != null)
                        {
                            selectquery += " RR.Status = '" + filter.Status + "' AND";
                        }
                        if (filter.DischargePort != null)
                        {
                            selectquery += " RR.DischargePort = '" + filter.DischargePort + "' AND";
                        }
                        if (filter.LoadPort != null)
                        {
                            selectquery += " RR.LoadPort = '" + filter.LoadPort + "' AND";
                        }
                        if (filter.RateID != null)
                        {
                            filter.RateID = filter.RateID.ToUpperInvariant();
                            selectquery += " RR.RateID = '" + filter.RateID + "' AND";
                        }

                        
                    }

                    if (selectquery.LastIndexOf("AND") > 0)
                    {
                        selectquery = selectquery.Remove(selectquery.LastIndexOf("AND"));
                    }

                    selectquery += " Order By RR.RateID DESC";// FETCH FIRST 10 ROWS ONLY";


                    var rateRequestData = conn.Query<RateRequest>(selectquery);
                    if (rateRequestData != null)
                    {
                        foreach (var rateRequest in rateRequestData)
                        {
                            rateRequests.Add(new RateRequest
                            {
                                RateID = rateRequest.RateID,
                                CompanyName = rateRequest.CompanyName,
                                ShipmentTerm = rateRequest.ShipmentTerm,
                                LoadPort = rateRequest.LoadPort,
                                DischargePort = rateRequest.DischargePort,
                                PlaceOfReceipt = rateRequest.PlaceOfReceipt,
                                PlaceOfDelivery = rateRequest.PlaceOfDelivery,
                                TransshipmentPort = rateRequest.TransshipmentPort,
                                POLFreeDays = rateRequest.POLFreeDays,
                                PODFreeDays = rateRequest.PODFreeDays,
                                EffectiveDate = rateRequest.EffectiveDate,
                                ValidityDate = rateRequest.ValidityDate,
                                Status = rateRequest.Status,
                                Quantity = rateRequest.Quantity,
                                EquipmentType = rateRequest.EquipmentType,
                                CargoType = rateRequest.CargoType,
                                Rate = rateRequest.Rate,
                                RateCountered = rateRequest.RateCountered,
                                AgentName = rateRequest.AgentName,
                                ModifiedDate = rateRequest.ModifiedDate
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                    throw ex;
            }
            return rateRequests;
        }
        public static List<CountryPort> GetCountryPorts()
        {
            List<CountryPort> ports = new List<CountryPort>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var allports = conn.Query<CountryPort>("Select Port, PortCode, Country from CountryPort Order By Port FETCH FIRST 10 ROWS ONLY");// FETCH FIRST 10 ROWS ONLY");
                    if (allports != null)
                    {
                        foreach (var por in allports)
                        {
                            ports.Add(new CountryPort
                            {
                                Port = por.Port,
                                PortCode = por.PortCode,
                                Country = por.Country
                            });
                        }
                    }
                }
                return ports;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<SelectListItem> GetCollectionPorts(string Port)
        {
            List<SelectListItem> ports = new List<SelectListItem>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    string IDDepot = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
                    var parameters = new DynamicParameters();
                    parameters.Add("Port", Port, DbType.String, ParameterDirection.Input);
                    parameters.Add("IDDepot", IDDepot, DbType.String, ParameterDirection.Input);



                    string query = "Select DepotName from DepotDetails WHERE Port = ? AND IDDepot = ? Order By DepotName";
                    var allports = conn.Query<string>(query, parameters);


                    if (allports != null)
                    {
                        foreach (var por in allports)
                        {
                            ports.Add(new SelectListItem
                            {
                                Text = por
                                
                            });
                        }
                    }
                }
                return ports;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static Booking GetCollectionAddress(string DepotName, string Port)
        {
            Booking booking = new Booking();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    string IDDepot = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
                    var parameters = new DynamicParameters();
                    parameters.Add("Port", Port, DbType.String, ParameterDirection.Input);
                    parameters.Add("IDDepot", IDDepot, DbType.String, ParameterDirection.Input);
                    parameters.Add("DepotName", DepotName, DbType.String, ParameterDirection.Input);



                    string query = "Select Address,Email,PhoneNo from DepotDetails WHERE Port = ? AND IDDepot = ? AND DepotName = ?";
                    var DepoDetails = conn.Query<Booking>(query, parameters);
                    foreach (var item in DepoDetails)
                    {
                        booking.Address = item.Address;
                        booking.Email = item.Email;
                        booking.PhoneNo = item.PhoneNo;
                    }
                 }
                return booking;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<SelectListItem> GetCompany()
        {
            string username = HttpContext.Current.User.Identity.Name;
            List<SelectListItem> companyname = new List<SelectListItem>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Name", username, DbType.String, ParameterDirection.Input);
                    string query = "Select AGB.CompanyName from AgentAddressBook AGB INNER JOIN UserAccountManagement UM ON " +
                        "UM.IDCompany = AGB.IDCompany Where UM.Name = ? Order By CompanyName";

                    var allcompanies = conn.Query<string>(query, parameters);
                    if (allcompanies != null)
                    {
                        foreach (var com in allcompanies)
                        {
                            companyname.Add(new SelectListItem
                            {
                                Text = com
                            });
                        }
                    }
                }
                return companyname;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static UpdateRateRequest GetRateRequestFromRateID(string rateID)
        {
            UpdateRateRequest rateRequest = new UpdateRateRequest();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("RateID", rateID, DbType.String, ParameterDirection.Input);

                    string query = "Select * from RateRequest where RateID = ?";

                    var rateRequestData = conn.Query<UpdateRateRequest>(query, parameters);
                    foreach (var item in rateRequestData)
                    {
                        rateRequest.AgentName = item.AgentName;
                        rateRequest.CargoType = item.CargoType;
                        rateRequest.UNNo = item.UNNo;
                        rateRequest.IMO = item.IMO;
                        rateRequest.CompanyName = item.CompanyName;
                        rateRequest.AgencyType = item.AgencyType;
                        rateRequest.GrossWt = item.GrossWt;
                        rateRequest.GrossWtUnit = item.GrossWtUnit;
                        rateRequest.ShipperName = item.ShipperName;
                        rateRequest.TransshipmentType = item.TransshipmentType;
                        rateRequest.TransshipmentPort = item.TransshipmentPort;
                        rateRequest.RateType = item.RateType;
                        rateRequest.Temperature = item.Temperature;
                        rateRequest.Humidity = item.Humidity;
                        rateRequest.Ventilation = item.Ventilation;
                        rateRequest.DischargePort = item.DischargePort;
                        rateRequest.EffectiveDate = item.EffectiveDate;
                        rateRequest.EquipmentType = item.EquipmentType;
                        rateRequest.LoadPort = item.LoadPort;
                        rateRequest.PlaceOfDelivery = item.PlaceOfDelivery;
                        rateRequest.GTotalSalesCal = item.GTotalSalesCal;
                        rateRequest.PlaceOfReceipt = item.PlaceOfReceipt;
                        rateRequest.PODFreeDays = item.PODFreeDays;
                        rateRequest.POLFreeDays = item.POLFreeDays;
                        rateRequest.Quantity = item.Quantity;
                        rateRequest.Rate = item.Rate;
                        rateRequest.RateCountered = item.RateCountered;
                        rateRequest.RateID = item.RateID;
                        rateRequest.Remark = item.Remark;
                        rateRequest.PrincipalRemark = item.PrincipalRemark;
                        rateRequest.ShipmentTerm = item.ShipmentTerm;
                        rateRequest.Status = item.Status;
                        rateRequest.ValidityDate = item.ValidityDate;
                        rateRequest.AgentCostLI = item.AgentCostLI;
                        rateRequest.SelectedExportLocalCharges = ConvertToList(rateRequest.AgentCostLI);
                        rateRequest.SelectedImportLocalCharges = ConvertToList(rateRequest.AgentCostLI);
                        rateRequest.ExportLocalCharges = GetExportChargesList(rateRequest).Select(x => new SelectListItem { Text = x.ChargeDescription, Value = x.ChargeDescription });
                        rateRequest.ImportLocalCharges = GetImportChargesList(rateRequest).Select(x => new SelectListItem { Text = x.ChargeDescription, Value = x.ChargeDescription });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return rateRequest;
        }

        public static ErrorLog SaveRateRequest(RateRequest rateRequest)
        {
            string username = HttpContext.Current.User.Identity.Name;
            string status = "DRAFT";
            string agentCostLi = ConvertToString(rateRequest.SelectedExportLocalCharges) + "," + ConvertToString(rateRequest.SelectedImportLocalCharges);

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CargoType", rateRequest.CargoType, DbType.String, ParameterDirection.Input);
                    parameters.Add("UNNo", rateRequest.UNNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("IMO", rateRequest.IMO, DbType.String, ParameterDirection.Input);
                    parameters.Add("CompanyName", rateRequest.CompanyName, DbType.String, ParameterDirection.Input);
                    parameters.Add("AgencyType", rateRequest.AgencyType, DbType.String, ParameterDirection.Input);
                    parameters.Add("TranshipmentType", rateRequest.TransshipmentType, DbType.String, ParameterDirection.Input);
                    parameters.Add("TranshipmentPort", rateRequest.TransshipmentPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("RateType", rateRequest.RateType, DbType.String, ParameterDirection.Input);
                    parameters.Add("Temperature", rateRequest.Temperature, DbType.String, ParameterDirection.Input);
                    parameters.Add("Humidity", rateRequest.Humidity, DbType.String, ParameterDirection.Input);
                    parameters.Add("Ventilation", rateRequest.Ventilation, DbType.String, ParameterDirection.Input);
                    parameters.Add("ShipperName", rateRequest.ShipperName, DbType.String, ParameterDirection.Input);
                    parameters.Add("GrossWt", rateRequest.GrossWt, DbType.String, ParameterDirection.Input);
                    parameters.Add("GrossWtUnit", rateRequest.GrossWtUnit, DbType.String, ParameterDirection.Input);
                    parameters.Add("DischargePort", rateRequest.DischargePort, DbType.String, ParameterDirection.Input);
                    parameters.Add("EquipmentType", rateRequest.EquipmentType, DbType.String, ParameterDirection.Input);
                    parameters.Add("LoadPort", rateRequest.LoadPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("PlaceOfDelivery", rateRequest.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                    parameters.Add("PlaceOfReceipt", rateRequest.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                    parameters.Add("Quantity", rateRequest.Quantity.ToString(), DbType.String, ParameterDirection.Input);
                    parameters.Add("Rate", rateRequest.Rate.ToString(), DbType.String, ParameterDirection.Input);
                    // parameters.Add("RateCountered", quoteref.RateCountered.ToString(), DbType.String, ParameterDirection.Input);
                    parameters.Add("ShipmentTerm", rateRequest.ShipmentTerm, DbType.String, ParameterDirection.Input);
                    parameters.Add("POLFreeDays", rateRequest.POLFreeDays, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("PODFreeDays", rateRequest.PODFreeDays, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("EffectiveDate", rateRequest.EffectiveDate, DbType.Date, ParameterDirection.Input);
                    parameters.Add("ValidityDate", rateRequest.ValidityDate, DbType.Date, ParameterDirection.Input);
                    //parameters.Add("QuoteType", quoteref.QuoteType, DbType.String, ParameterDirection.Input);
                    parameters.Add("Status", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("AgentName", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("Remark", rateRequest.Remark, DbType.String, ParameterDirection.Input);
                    parameters.Add("AgentCostLI", agentCostLi, DbType.String, ParameterDirection.Input);

                    string odbcQuery = "Insert Into RateRequest(CargoType,UNNo,IMO,CompanyName,AgencyType,TransshipmentType,TransshipmentPort,RateType,Temperature,Humidity,Ventilation,ShipperName,GrossWt,GrossWtUnit,DischargePort,EquipmentType,LoadPort,PlaceOfDelivery,PlaceOfReceipt,Quantity,Rate,ShipmentTerm,POLFreeDays,PODFreeDays,EffectiveDate,ValidityDate,Status,AgentName,Remark,AgentCostLI)" +
                        //"ModifiedBy, ModificationTimestamp) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);

                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                    string query = "Select MAX (RateID) as RateID from RateRequest where AgentName= '" + username + "'";

                    var rateRequestData = conn.Query<RateRequest>(query, parameters);
                    foreach (var item in rateRequestData)
                    {
                        rateRequest.RateID = item.RateID;
                    }

                }

                errorLog.IsError = false;
                errorLog.ErrorMessage = rateRequest.RateID;
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog UpdateRateRequest(UpdateRateRequest rateRequest)
        {
            string username = HttpContext.Current.User.Identity.Name;
            string agentCostLi = ConvertToString(rateRequest.SelectedExportLocalCharges) + "," + ConvertToString(rateRequest.SelectedImportLocalCharges);

            try
            {
                if (rateRequest.Status == Constant.Draft)
                {

                    using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("CargoType", rateRequest.CargoType, DbType.String, ParameterDirection.Input);
                        parameters.Add("UNNo", rateRequest.UNNo, DbType.String, ParameterDirection.Input);
                        parameters.Add("IMO", rateRequest.IMO, DbType.String, ParameterDirection.Input);
                        parameters.Add("CompanyName", rateRequest.CompanyName, DbType.String, ParameterDirection.Input);
                        parameters.Add("AgencyType", rateRequest.AgencyType, DbType.String, ParameterDirection.Input);
                        parameters.Add("TransshipmentType", rateRequest.TransshipmentType, DbType.String, ParameterDirection.Input);
                        parameters.Add("TransshipmentPort", rateRequest.TransshipmentPort, DbType.String, ParameterDirection.Input);
                        parameters.Add("RateType", rateRequest.RateType, DbType.String, ParameterDirection.Input);
                        parameters.Add("Temperature", rateRequest.Temperature, DbType.String, ParameterDirection.Input);
                        parameters.Add("Humidity", rateRequest.Humidity, DbType.String, ParameterDirection.Input);
                        parameters.Add("Ventilation", rateRequest.Ventilation, DbType.String, ParameterDirection.Input);
                        parameters.Add("ShipperName", rateRequest.ShipperName, DbType.String, ParameterDirection.Input);
                        parameters.Add("GrossWt", rateRequest.GrossWt, DbType.String, ParameterDirection.Input);
                        parameters.Add("GrossWtUnit", rateRequest.GrossWtUnit, DbType.String, ParameterDirection.Input);
                        parameters.Add("DischargePort", rateRequest.DischargePort, DbType.String, ParameterDirection.Input);
                        parameters.Add("EquipmentType", rateRequest.EquipmentType, DbType.String, ParameterDirection.Input);
                        parameters.Add("LoadPort", rateRequest.LoadPort, DbType.String, ParameterDirection.Input);
                        parameters.Add("PlaceOfDelivery", rateRequest.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                        parameters.Add("PlaceOfReceipt", rateRequest.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                        parameters.Add("Quantity", rateRequest.Quantity.ToString(), DbType.String, ParameterDirection.Input);
                        parameters.Add("Rate", rateRequest.Rate.ToString(), DbType.String, ParameterDirection.Input);
                        parameters.Add("RateCountered", rateRequest.RateCountered.ToString(), DbType.String, ParameterDirection.Input);
                        parameters.Add("ShipmentTerm", rateRequest.ShipmentTerm, DbType.String, ParameterDirection.Input);
                        parameters.Add("POLFreeDays", rateRequest.POLFreeDays, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("PODFreeDays", rateRequest.PODFreeDays, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("EffectiveDate", rateRequest.EffectiveDate, DbType.Date, ParameterDirection.Input);
                        parameters.Add("ValidityDate", rateRequest.ValidityDate, DbType.Date, ParameterDirection.Input);
                        //parameters.Add("QuoteType", quoteref.QuoteType, DbType.String, ParameterDirection.Input);
                        //parameters.Add("Status", status, DbType.String, ParameterDirection.Input);
                        parameters.Add("AgentName", username, DbType.String, ParameterDirection.Input);
                        parameters.Add("Remark", rateRequest.Remark, DbType.String, ParameterDirection.Input);
                        parameters.Add("AgentCostLI", agentCostLi, DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Update RateRequest Set CargoType = ?, UNNo = ?, IMO = ?, CompanyName = ?, AgencyType = ?, TransshipmentType = ?, TransshipmentPort = ?, RateType = ?, " +
                            "Temperature = ?, Humidity = ?, Ventilation = ?, ShipperName = ?, GrossWt = ?, GrossWtUnit = ?, DischargePort = ?, EquipmentType = ?, LoadPort = ? ," +
                            "PlaceOfDelivery = ?, PlaceOfReceipt = ?, Quantity = ?, Rate = ?, RateCountered = ?, ShipmentTerm = ? ,POLFreeDays = ?,PODFreeDays = ?, " +
                            "EffectiveDate = ?, ValidityDate = ?,AgentName = ?, Remark = ?, AgentCostLI = ? " +
                            "Where RateID = '" + rateRequest.RateID + "'";

                        int rowsAffected = conn.Execute(odbcQuery, parameters);

                        if (rowsAffected > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }
                    }
                    
                }
                else if (rateRequest.OfferRate > 0 && rateRequest.Status == Constant.CounterOffer)
                {
                    rateRequest.RateCountered = rateRequest.OfferRate;
                    using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RateCountered", rateRequest.RateCountered.ToString(), DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Update RateRequest Set RateCountered = ?, Status = 'APPROVAL REQUIRED' Where RateID = '" + rateRequest.RateID + "'";

                        int rowsAffected = conn.Execute(odbcQuery, parameters);
                        if (rowsAffected > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }
                    }
                    
                }
                else
                {
                    errorLog.IsError = true;
                    errorLog.ErrorMessage = "Rate Request needs to be in Draft for Making any changes";
                    return errorLog;
                }
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;   
        }

        public static ErrorLog ApprovalRequest(string rateID)
        {

            string status = Constant.ApprovalRequired;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();


                    parameters.Add("Status", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("RateID", rateID, DbType.String, ParameterDirection.Input);


                    string odbcQuery = "Update RateRequest Set Status = ? Where RateID = ?";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                }
                
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog IssueBooking(string BookingID)
        {

            string status = Constant.Issued;
            
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();


                    parameters.Add("BookingStatus", status, DbType.String, ParameterDirection.Input);
                    

                    parameters.Add("BookingID", BookingID, DbType.String, ParameterDirection.Input);


                    string odbcQuery = "Update BookingConfirmation Set BookingStatus = ? Where BookingID = ?";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);

                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }
                }

            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog ConfirmBooking(string BookingID)
        {

            string status = Constant.Confirmed;
            string AccMonth = DateTime.Today.ToString("MMMyy");
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();


                    parameters.Add("BookingStatus", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("AccMonth", AccMonth, DbType.String, ParameterDirection.Input);
                    
                    parameters.Add("BookingID", BookingID, DbType.String, ParameterDirection.Input);


                    string odbcQuery = "Update BookingConfirmation Set BookingStatus = ?, AccMonth = ? Where BookingID = ?";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);

                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                    var parameters1 = new DynamicParameters();
                    parameters1.Add("BookingDate", DateTime.Today, DbType.Date, ParameterDirection.Input);
                    parameters1.Add("BookingID", BookingID, DbType.String, ParameterDirection.Input);

                    string Query = "Update BookingConfirmation Set BookingDate = ? Where BookingID = ?";

                    int rowsAffected1 = conn.Execute(Query, parameters1);



                }
                
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog EditBooking(string BookingID)
        {

            string status = Constant.Draft;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();


                    parameters.Add("BookingStatus", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("BookingDate", null, DbType.Date, ParameterDirection.Input);
                    parameters.Add("AccMonth", string.Empty, DbType.String, ParameterDirection.Input);
                    parameters.Add("BookingID", BookingID, DbType.String, ParameterDirection.Input);


                    string odbcQuery = "Update BookingConfirmation Set BookingStatus = ?, BookingDate = ?, AccMonth = ? Where BookingID = ?";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }
                }
                
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog SaveBooking(Booking booking)
        {
            string username = HttpContext.Current.User.Identity.Name;
            string status = "DRAFT";
            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("LoadPort", booking.LoadPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("DischargePort", booking.DischargePort, DbType.String, ParameterDirection.Input); //Change field name to DischargePort
                    parameters.Add("QuoteRefID", booking.QuoteRefID, DbType.String, ParameterDirection.Input);
                    parameters.Add("CompanyName", booking.CompanyName, DbType.String, ParameterDirection.Input);
                    //parameters.Add("EffectiveDate", booking.EffectiveDate, DbType.Date, ParameterDirection.Input); //changed to type text
                    //parameters.Add("Validity", booking.Validity, DbType.Date, ParameterDirection.Input);//changed to type text
                    //parameters.Add("LoadAgent", booking.LoadAgent, DbType.String, ParameterDirection.Input);
                    //parameters.Add("DischAgent", booking.DischAgent, DbType.String, ParameterDirection.Input);
                    //parameters.Add("LoadTerminal", booking.LoadTerminal, DbType.String, ParameterDirection.Input);
                    //parameters.Add("DischargeTerminal", booking.DischargeTerminal, DbType.String, ParameterDirection.Input);
                    parameters.Add("CutoffDate", booking.CutoffDate, DbType.Date, ParameterDirection.Input);
                    parameters.Add("CROPickUpDate", booking.CROPickUpDate, DbType.Date, ParameterDirection.Input);
                    parameters.Add("CutoffDateandTime", booking.CutoffDate.TimeOfDay, DbType.Time, ParameterDirection.Input);
                    parameters.Add("ServiceMode", booking.ServiceMode, DbType.String, ParameterDirection.Input);
                    parameters.Add("GrossweightMeasurement", booking.GrossweightMeasurement, DbType.String, ParameterDirection.Input);
                    parameters.Add("Grossweight", booking.Grossweight, DbType.String, ParameterDirection.Input);
                    parameters.Add("Commodity", booking.Commodity, DbType.String, ParameterDirection.Input);
                    parameters.Add("CommodityGroup", booking.CommodityGroup, DbType.String, ParameterDirection.Input);
                    parameters.Add("RateType", booking.RateType, DbType.String, ParameterDirection.Input);
                    parameters.Add("Temperature", booking.Temperature, DbType.String, ParameterDirection.Input);
                    parameters.Add("Humidity", booking.Humidity, DbType.String, ParameterDirection.Input);
                    parameters.Add("Ventilation", booking.Ventilation, DbType.String, ParameterDirection.Input);
                    parameters.Add("Remark", booking.Remark, DbType.String, ParameterDirection.Input);
                    parameters.Add("CRORemarks", booking.CRORemarks, DbType.String, ParameterDirection.Input);
                    parameters.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("BookingStatus", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("PaymentTerms", booking.PaymentTerms, DbType.String, ParameterDirection.Input);
                    parameters.Add("Volume", booking.Volume, DbType.String, ParameterDirection.Input);
                    //parameters.Add("SOC", booking.SOC, DbType.String, ParameterDirection.Input);
                    parameters.Add("AddressAttn", booking.AddressAttn, DbType.String, ParameterDirection.Input);
                    parameters.Add("AddressFax", booking.AddressFax, DbType.String, ParameterDirection.Input);
                    parameters.Add("AddressTel", booking.AddressTel, DbType.String, ParameterDirection.Input);
                    parameters.Add("AddressTo", booking.AddressTo, DbType.String, ParameterDirection.Input);
                    parameters.Add("CollectionYard", booking.CollectionYard, DbType.String, ParameterDirection.Input);                    

                    string odbcQuery = "Insert Into BookingConfirmation (LoadPort, DischargePort, QuoteRefID, CompanyName, " +
                        "CutoffDate, CROPickUpDate, CutoffDateandTime, ServiceMode, GrossweightMeasurement, Grossweight, Commodity, CommodityGroup, RateType, Temperature, Humidity, " +
                        "Ventilation, Remark, CRORemarks, CreatedBy, BookingStatus, PaymentTerms, Volume, AddressAttn, AddressFax, AddressTel, AddressTo, CollectionYard) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                    //string query = "Select MAX (BookingID) as BookingID from BookingConfirmation where CreatedBy= '" + username + "'";
                    string query = "SELECT BookingID FROM BookingConfirmation WHERE CreatedBy = '" + username + "' ORDER BY BKID DESC FETCH FIRST 1 ROWS ONLY";

                    var bookingreference = conn.Query<Booking>(query, parameters);
                    foreach (var item in bookingreference)
                    {
                        booking.BookingID = item.BookingID;
                    }

                }

                
                errorLog.ErrorMessage = booking.BookingID;
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog UpdateBooking(Booking booking)
        {
            string username = HttpContext.Current.User.Identity.Name;
            
            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);

            try
            {
                if (booking.BookingStatus == Constant.Draft)
                {
                    using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("LoadPort", booking.LoadPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("DischargePort", booking.DischargePort, DbType.String, ParameterDirection.Input); //Change field name to DischargePort
                    parameters.Add("QuoteRefID", booking.QuoteRefID, DbType.String, ParameterDirection.Input);
                    parameters.Add("CompanyName", booking.CompanyName, DbType.String, ParameterDirection.Input);
                    //parameters.Add("EffectiveDate", booking.EffectiveDate, DbType.Date, ParameterDirection.Input); //changed to type text
                    //parameters.Add("Validity", booking.Validity, DbType.Date, ParameterDirection.Input);//changed to type text
                    //parameters.Add("LoadAgent", booking.LoadAgent, DbType.String, ParameterDirection.Input);
                    //parameters.Add("DischAgent", booking.DischAgent, DbType.String, ParameterDirection.Input);
                    //parameters.Add("LoadTerminal", booking.LoadTerminal, DbType.String, ParameterDirection.Input);
                    //parameters.Add("dischargeTerminal", booking.DischargeTerminal, DbType.String, ParameterDirection.Input);
                    parameters.Add("CutoffDate", booking.CutoffDate, DbType.Date, ParameterDirection.Input);
                    parameters.Add("CROPickUpDate", booking.CROPickUpDate, DbType.Date, ParameterDirection.Input);
                    parameters.Add("CutoffDateandTime", booking.CutoffDate.TimeOfDay, DbType.Time, ParameterDirection.Input);
                    parameters.Add("ServiceMode", booking.ServiceMode, DbType.String, ParameterDirection.Input);
                    parameters.Add("GrossweightMeasurement", booking.GrossweightMeasurement, DbType.String, ParameterDirection.Input);
                    parameters.Add("Grossweight", booking.Grossweight, DbType.String, ParameterDirection.Input);
                    parameters.Add("Commodity", booking.Commodity, DbType.String, ParameterDirection.Input);
                    parameters.Add("CommodityGroup", booking.CommodityGroup, DbType.String, ParameterDirection.Input);
                    parameters.Add("RateType", booking.RateType, DbType.String, ParameterDirection.Input);
                    parameters.Add("Temperature", booking.Temperature, DbType.String, ParameterDirection.Input);
                    parameters.Add("Humidity", booking.Humidity, DbType.String, ParameterDirection.Input);
                    parameters.Add("Ventilation", booking.Ventilation, DbType.String, ParameterDirection.Input);
                    parameters.Add("Remark", booking.Remark, DbType.String, ParameterDirection.Input);
                    parameters.Add("CRORemarks", booking.CRORemarks, DbType.String, ParameterDirection.Input);
                    parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("PaymentTerms", booking.PaymentTerms, DbType.String, ParameterDirection.Input);
                    parameters.Add("Volume", booking.Volume, DbType.String, ParameterDirection.Input);
                    //parameters.Add("SOC", booking.SOC, DbType.String, ParameterDirection.Input);
                    parameters.Add("AddressAttn", booking.AddressAttn, DbType.String, ParameterDirection.Input);
                    parameters.Add("AddressFax", booking.AddressFax, DbType.String, ParameterDirection.Input);
                    parameters.Add("AddressTel", booking.AddressTel, DbType.String, ParameterDirection.Input);
                    parameters.Add("AddressTo", booking.AddressTo, DbType.String, ParameterDirection.Input);
                    parameters.Add("CollectionYard", booking.CollectionYard, DbType.String, ParameterDirection.Input);
                    

                    string odbcQuery = "Update BookingConfirmation Set LoadPort = ?, DischargePort = ?, QuoteRefID = ?, CompanyName = ?, " +
                            "CutoffDate = ?, CROPickUpDate = ?, CutoffDateandTime = ?, ServiceMode = ?, GrossweightMeasurement = ?, " +
                            "Grossweight = ?, Commodity = ?, CommodityGroup = ?, RateType = ?, Temperature = ?, Humidity = ?, Ventilation = ?, Remark = ?, CRORemarks = ?, " +
                            "ModifiedBy = ?, PaymentTerms = ?, Volume = ?, AddressAttn = ?, AddressFax = ?, AddressTel = ?, AddressTo = ?, CollectionYard = ? " +
                            "Where BookingID = '" + booking.BookingID + "'";

                        int rowsAffected = conn.Execute(odbcQuery, parameters);
                        if (rowsAffected > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }
                    }
                }
                else
                {
                    errorLog.IsError = true;
                    errorLog.ErrorMessage = "Booking Request needs to be in Draft for Making any changes";
                    return errorLog;
                }
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }


        public static List<AgencyLocalCharges> GetImportChargesList(RateRequest rateRequest)
        {
            List<AgencyLocalCharges> agency = new List<AgencyLocalCharges>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("Location", rateRequest.DischargePort, DbType.String, ParameterDirection.Input);
                    parameters.Add("EquipmentType", rateRequest.EquipmentType, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<AgencyLocalCharges>("Select ID, ChargeDescription from AgencyLocalCharges where Location= ? and EquipmentType = ? and " +
                        "ImportOrExport= 'IMPORT'", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            agency.Add(new AgencyLocalCharges
                            {
                                ID = charge.ID,
                                ChargeDescription = charge.ChargeDescription,
                            });
                        }
                    }
                }
                return agency;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<AgencyLocalCharges> GetImportChargesList(UpdateRateRequest rateRequest)
        {
            List<AgencyLocalCharges> agency = new List<AgencyLocalCharges>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("Location", rateRequest.DischargePort, DbType.String, ParameterDirection.Input);
                    parameters.Add("EquipmentType", rateRequest.EquipmentType, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<AgencyLocalCharges>("Select ID, ChargeDescription from AgencyLocalCharges where Location= ? and EquipmentType = ? and " +
                        "ImportOrExport= 'IMPORT'", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            agency.Add(new AgencyLocalCharges
                            {
                                ID = charge.ID,
                                ChargeDescription = charge.ChargeDescription,
                            });
                        }
                    }
                }
                return agency;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<AgencyLocalCharges> GetExportChargesList(RateRequest rateRequest)
        {
            List<AgencyLocalCharges> agency = new List<AgencyLocalCharges>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("Location", rateRequest.LoadPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("EquipmentType", rateRequest.EquipmentType, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<AgencyLocalCharges>("Select ID, ChargeDescription from AgencyLocalCharges where Location= ? and EquipmentType = ? and " +
                        "ImportOrExport = 'EXPORT'", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            agency.Add(new AgencyLocalCharges
                            {
                                ID = charge.ID,
                                ChargeDescription = charge.ChargeDescription,
                            });
                        }
                    }
                }
                return agency;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<AgencyLocalCharges> GetExportChargesList(UpdateRateRequest rateRequest)
        {
            List<AgencyLocalCharges> agency = new List<AgencyLocalCharges>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("Location", rateRequest.LoadPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("EquipmentType", rateRequest.EquipmentType, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<AgencyLocalCharges>("Select ID, ChargeDescription from AgencyLocalCharges where Location= ? and EquipmentType = ? and " +
                        "ImportOrExport = 'EXPORT'", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            agency.Add(new AgencyLocalCharges
                            {
                                ID = charge.ID,
                                ChargeDescription = charge.ChargeDescription,
                            });
                        }
                    }
                }
                return agency;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<DQuoteSales> GetQuoteChargesList(string rateID)
        {
            List<DQuoteSales> dQuoteSales = new List<DQuoteSales>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("RateID", rateID, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<DQuoteSales>("Select ID, RateID, Description from DQuoteSales where RateID= ? ", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            dQuoteSales.Add(new DQuoteSales
                            {
                                ID = charge.ID,
                                RateID = charge.RateID,
                                Description = charge.Description,
                            }) ;
                        }
                    }
                }
                return dQuoteSales;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<DQuoteSales> GetRateChargesList(string rateID)
        {
            List<DQuoteSales> dQuoteSales = new List<DQuoteSales>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("RateID", rateID, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<DQuoteSales>("Select ID, RateID, Description from DQuoteCostLI where RateID= ? ", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            dQuoteSales.Add(new DQuoteSales
                            {
                                ID = charge.ID,
                                RateID = charge.RateID,
                                Description = charge.Description,
                            });
                        }
                    }
                }
                return dQuoteSales;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<AgencyLocalCharges> GetImportChargesList(QuoteRef quoteRef)
        {
            List<AgencyLocalCharges> agency = new List<AgencyLocalCharges>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("Location", quoteRef.DischargePort, DbType.String, ParameterDirection.Input);
                    parameters.Add("EquipmentType", quoteRef.EquipmentType, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<AgencyLocalCharges>("Select ID, ChargeDescription from AgencyLocalCharges where Location= ? and EquipmentType = ? and " +
                        "ImportOrExport= 'IMPORT'", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            agency.Add(new AgencyLocalCharges
                            {
                                ID = charge.ID,
                                ChargeDescription = charge.ChargeDescription,
                            });
                        }
                    }
                }
                return agency;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<QuoteRef> GetAllQuoteRefData(SearchParameters search)
        {
            //string status = statusdisplay.Status;
            string username = HttpContext.Current.User.Identity.Name;
            List<QuoteRef> quoteRefs = new List<QuoteRef>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    string query = "Select q.QuoteRefID, q.RateID, q.CompanyName, q.ShipmentTerm, q.LoadPort, q.TransshipmentPort, q.DischargePort, q.PlaceOfReceipt, q.PlaceOfDelivery, q.POLFreeDays, q.PODFreeDays, q.EffectiveDate,  " +
                            "q.Validity, q.StatusDIS, q.Quantity, q.EquipmentType, q.CargoType, r.Rate, r.RateCountered, r.AgentName From QuoteReference q INNER JOIN RateRequest r ON q.RateID = r.RateID INNER JOIN UserAccountManagement um ON r.IDCompany = um.IDCompany WHERE um.Name = '" + username + "' AND q.StatusDIS <> 'DRAFT' AND"; 
                    
                    if((search.QuoteRefID!=null)|| (search.RateID != null) || (search.Status != null) || (search.DischargePort != null) || (search.LoadPort != null) || (search.CompanyName != null))

                    {
                        if(search.QuoteRefID!=null)
                        {
                            search.QuoteRefID = search.QuoteRefID.ToUpperInvariant();
                            query += " q.QuoteRefID = '" + search.QuoteRefID + "' AND";
                        }
                        if (search.RateID != null)
                        {
                            search.RateID = search.RateID.ToUpperInvariant();
                            query += " q.RateID = '" + search.RateID + "' AND";
                        }
                        if (search.Status != null)
                        {
                            query += " q.StatusDIS = '" + search.Status + "' AND";
                        }
                        if (search.DischargePort != null)
                        {
                            query += " q.DischargePort = '" + search.DischargePort + "' AND";
                        }
                        if (search.LoadPort != null)
                        {
                            query += " q.LoadPort = '" + search.LoadPort + "' AND";
                        }
                        if (search.CompanyName != null)
                        {
                            query += " q.CompanyName = '" + search.CompanyName + "' AND";
                        }
                    }

                    if (query.LastIndexOf("AND") > 0)
                    {
                        query = query.Remove(query.LastIndexOf("AND"));
                    }

                    query += " Order By QuoteRefID DESC";// FETCH FIRST 10 ROWS ONLY";

                    var quotes = conn.Query<QuoteRef>(query);
                    if (quotes != null)
                    {
                        foreach (var quoteRef in quotes)
                        {
                            quoteRefs.Add(new QuoteRef
                            {
                                QuoteRefID = quoteRef.QuoteRefID,
                                RateID = quoteRef.RateID,
                                CompanyName = quoteRef.CompanyName,
                                ShipmentTerm = quoteRef.ShipmentTerm,
                                LoadPort = quoteRef.LoadPort,
                                TransshipmentPort = quoteRef.TransshipmentPort,
                                DischargePort = quoteRef.DischargePort,
                                PlaceOfReceipt = quoteRef.PlaceOfReceipt,
                                PlaceOfDelivery = quoteRef.PlaceOfDelivery,
                                POLFreeDays = quoteRef.POLFreeDays,
                                PODFreeDays = quoteRef.PODFreeDays,
                                EffectiveDate = quoteRef.EffectiveDate,
                                Validity = quoteRef.Validity,
                                StatusDIS = quoteRef.StatusDIS,
                                Quantity = quoteRef.Quantity,
                                EquipmentType = quoteRef.EquipmentType,
                                CargoType = quoteRef.CargoType,
                                Rate = quoteRef.Rate,
                                RateCountered = quoteRef.RateCountered,
                                AgentName = quoteRef.AgentName,
                                //ModifiedDate = quoteRef.ModifiedDate
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return quoteRefs;
        }

        public static QuoteRef GetQuoteRequestFromQuoteID(string QuoteRefID)
        {
            QuoteRef quoteRef = new QuoteRef();

            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("QuoteRefID", QuoteRefID, DbType.String, ParameterDirection.Input);

                    string query = "Select * from QuoteReference where QuoteRefID = ?";

                    var QuoteData = conn.Query<QuoteRef>(query, parameters);
                    foreach (var item in QuoteData)
                    {
                        quoteRef.AgentName = item.AgentName;
                        quoteRef.CargoType = item.CargoType;
                        quoteRef.UNNo = item.UNNo;
                        quoteRef.IMO = item.IMO;
                        quoteRef.CompanyName = item.CompanyName;
                        quoteRef.AgencyType = item.AgencyType;
                        quoteRef.TransshipmentType = item.TransshipmentType;
                        quoteRef.TransshipmentPort = item.TransshipmentPort;
                        quoteRef.RateType = item.RateType;
                        quoteRef.Temperature = item.Temperature;
                        quoteRef.Humidity = item.Humidity;
                        quoteRef.Ventilation = item.Ventilation;
                        quoteRef.DischargePort = item.DischargePort;
                        quoteRef.EffectiveDate = item.EffectiveDate;
                        quoteRef.EquipmentType = item.EquipmentType;
                        quoteRef.LoadPort = item.LoadPort;
                        quoteRef.GrossWt = item.GrossWt;
                        quoteRef.GrossWtUnit = item.GrossWtUnit;
                        quoteRef.ShipperName3 = item.ShipperName3;
                        quoteRef.PlaceOfDelivery = item.PlaceOfDelivery;
                        quoteRef.PlaceOfReceipt = item.PlaceOfReceipt;
                        quoteRef.PODFreeDays = item.PODFreeDays;
                        quoteRef.POLFreeDays = item.POLFreeDays;
                        quoteRef.Quantity = item.Quantity;
                        quoteRef.RateID = item.RateID;
                        UpdateRateRequest ratedetail = GetRateRequestFromRateID(quoteRef.RateID);
                        {
                            quoteRef.Rate = ratedetail.Rate;
                            quoteRef.RateCountered = ratedetail.RateCountered;
                            quoteRef.GTotalSalesCal = ratedetail.GTotalSalesCal;
                        }
                        quoteRef.Remark = item.Remark;
                        quoteRef.ShipmentTerm = item.ShipmentTerm;
                        quoteRef.StatusDIS = item.StatusDIS;
                        quoteRef.Validity = item.Validity;
                        quoteRef.QuoteLocalCharges = GetQuoteChargesList(quoteRef.RateID);
                    }
                }
            }

            catch (Exception)
            {

                throw;
            }
            return quoteRef;
        }

        public static Booking GetBookingfromID(string BookingID)
        {
            Booking bookref = new Booking();

            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("BookingID", BookingID, DbType.String, ParameterDirection.Input);

                    string query = "Select BookingID, BookingNo, VoyageNo, JobRef, BookingDate, Volume, LoadPort, DischargePort, QuoteRefID, CompanyName, CutoffDate, CutoffDateandTime, " +
                        "CROPickUpDate, RateType, Humidity, Temperature, ServiceMode, Grossweight, GrossweightMeasurement, Commodity, CommodityGroup, Ventilation, Remark, CRORemarks, BookingStatus, " +
                        "UniversalSerialNr, PaymentTerms, Volume, AddressAttn, AddressFax, AddressTel, AddressTo, CollectionYard, ContainerReleaseOrderNo " +
                        "from BookingConfirmation where BookingID = ?";

                    var booking = conn.Query<Booking>(query, parameters);
                    foreach (var item in booking)
                    {
                        bookref.BookingNo = item.BookingNo;
                        bookref.BookingID = item.BookingID;
                        bookref.BookingDate = item.BookingDate;
                        bookref.Volume = item.Volume;
                        bookref.JobRef = item.JobRef;
                        bookref.VoyageNo = item.VoyageNo;
                        //bookref.SOC = item.SOC;
                        bookref.LoadPort = item.LoadPort;
                        bookref.DischargePort = item.DischargePort;
                        bookref.QuoteRefID = item.QuoteRefID;
                        bookref.CompanyName = item.CompanyName;
                        //bookref.EffectiveDate = item.EffectiveDate;
                        //bookref.Validity = item.Validity;
                        //bookref.LoadAgent = item.LoadAgent;
                        //bookref.DischAgent = item.DischAgent;
                        //bookref.LoadTerminal = item.LoadTerminal;
                        //bookref.DischargeTerminal = item.DischargeTerminal;
                        bookref.CutoffDate = item.CutoffDate;
                        bookref.CutoffDateandTime = item.CutoffDateandTime;
                        bookref.CutoffDate = bookref.CutoffDate + bookref.CutoffDateandTime;
                        bookref.CROPickUpDate = item.CROPickUpDate;
                        //bookref.CutoffDateandTime = item.CutoffDateandTime;
                        bookref.ServiceMode = item.ServiceMode;
                        bookref.Grossweight = item.Grossweight;
                        bookref.GrossweightMeasurement = item.GrossweightMeasurement;
                        bookref.Commodity = item.Commodity;
                        bookref.CommodityGroup = item.CommodityGroup;
                        bookref.RateType = item.RateType;
                        bookref.Humidity = item.Humidity;
                        bookref.Temperature = item.Temperature;
                        bookref.Ventilation = item.Ventilation;
                        bookref.Remark = item.Remark;
                        bookref.CRORemarks = item.CRORemarks;
                        bookref.BookingStatus = item.BookingStatus;
                        bookref.UniversalSerialNr = item.UniversalSerialNr;
                        bookref.PaymentTerms = item.PaymentTerms;
                        bookref.Volume = item.Volume;
                        bookref.AddressAttn = item.AddressAttn;
                        bookref.AddressFax = item.AddressFax;
                        bookref.AddressTel = item.AddressTel;
                        bookref.AddressTo = item.AddressTo;
                        QuoteRef quotedetails = GetQuoteRequestFromQuoteID(bookref.QuoteRefID);
                        {
                            bookref.Rate = quotedetails.Rate;
                            bookref.RateCountered = quotedetails.RateCountered;
                            bookref.GTotalSalesCal = quotedetails.GTotalSalesCal;
                            bookref.EquipmentType = quotedetails.EquipmentType;
                            bookref.Quantity = quotedetails.Quantity;
                            bookref.PODFreeDays = quotedetails.PODFreeDays;
                            bookref.POLFreeDays = quotedetails.POLFreeDays;
                            bookref.CargoType = quotedetails.CargoType;
                            bookref.UNNo = quotedetails.UNNo;
                            bookref.IMO = quotedetails.IMO;
                            bookref.PlaceOfDelivery = quotedetails.PlaceOfDelivery;
                            bookref.PlaceOfReceipt = quotedetails.PlaceOfReceipt;
                            bookref.TransshipmentPort = quotedetails.TransshipmentPort;
                            bookref.TransshipmentType = quotedetails.TransshipmentType;

                        }

                        bookref.CollectionYard = item.CollectionYard;
                        bookref.ContainerReleaseOrderNo = item.ContainerReleaseOrderNo;
                        if(bookref.CollectionYard!=null)
                        {
                            Booking book = new Booking();
                            book = GetCollectionAddress(bookref.CollectionYard, bookref.LoadPort);
                            bookref.Address = book.Address;
                            bookref.Email = book.Email;
                            bookref.PhoneNo = book.PhoneNo;
                        }
                        bookref.Vessels = GetVesselsDetails(bookref.UniversalSerialNr);
                    }
                }
            }

            catch (Exception)
            {

                throw;
            }
            return bookref;
        }

        public static ShipmentBL GetShipmentFromJobRef(string JobRef)
        {
            ShipmentBL shipmentRef = new ShipmentBL();

            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string querysd = "Select JobRef, HBLHAWB, ShipmentTerm, MBLMAWB, CustomerRef, UniversalSerialNr from ShipmentDetailDry where JobRef = ?";

                    var ShipmentData = conn.Query<ShipmentDetails>(querysd, parametersd);
                    foreach (var item in ShipmentData)
                    {
                        shipmentRef.ShipmentDetailsModel.JobRef = item.JobRef;
                        shipmentRef.ShipmentDetailsModel.HBLHAWB = item.HBLHAWB;
                        shipmentRef.ShipmentDetailsModel.ShipmentTerm = item.ShipmentTerm;
                        shipmentRef.ShipmentDetailsModel.MBLMAWB = item.MBLMAWB;
                        shipmentRef.ShipmentDetailsModel.CustomerRef = item.CustomerRef;
                        shipmentRef.ShipmentDetailsModel.UniversalSerialNr = item.UniversalSerialNr;
                        
                    }

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string querybl = "Select * from BLDetail where UniversalSerialNr = ?";

                    var BLData = conn.Query<BLDetails>(querybl, parameterbl);
                    foreach (var item in BLData)
                    {
                        shipmentRef.BLDetailsModel.ShipperNameBL = item.ShipperNameBL;
                        shipmentRef.BLDetailsModel.ConsigneeNameBL = item.ConsigneeNameBL;
                        shipmentRef.BLDetailsModel.ShipperAddressBL = item.ShipperAddressBL;
                        shipmentRef.BLDetailsModel.ConsigneeAddressBL = item.ConsigneeAddressBL;
                        shipmentRef.BLDetailsModel.NotifyPartyName = item.NotifyPartyName;
                        shipmentRef.BLDetailsModel.DischAgentNameBL = item.DischAgentNameBL;
                        shipmentRef.BLDetailsModel.NotifyPartyAddress = item.NotifyPartyAddress;
                        shipmentRef.BLDetailsModel.DischAgentAddress = item.DischAgentAddress;
                        shipmentRef.BLDetailsModel.CountryPOL = item.CountryPOL;
                        shipmentRef.BLDetailsModel.PlaceofReceipt = item.PlaceofReceipt;
                        shipmentRef.BLDetailsModel.CountryPOD = item.CountryPOD;
                        shipmentRef.BLDetailsModel.PlaceofDelivery = item.PlaceofDelivery;
                        shipmentRef.BLDetailsModel.VesselDetails = item.VesselDetails;
                        shipmentRef.BLDetailsModel.MarksandNo = item.MarksandNo;
                        shipmentRef.BLDetailsModel.CargoDescription = item.CargoDescription;
                        shipmentRef.BLDetailsModel.PlaceofIssue = item.PlaceofIssue;
                        shipmentRef.BLDetailsModel.LadenOnBoard = item.LadenOnBoard;
                        shipmentRef.BLDetailsModel.HBLFreightPayment = item.HBLFreightPayment;
                        shipmentRef.BLDetailsModel.BLFinalisedDate = item.BLFinalisedDate;
                        shipmentRef.BLDetailsModel.DateofIssue = item.DateofIssue;
                        shipmentRef.BLDetailsModel.MBLFreightPayment = item.MBLFreightPayment;
                        shipmentRef.BLDetailsModel.NoofOriginalBLissued = item.NoofOriginalBLissued;

                    }

                    var parameterv = new DynamicParameters();
                    parameterv.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string queryv = "Select ETD, ETA from VesselScheduleDry where UniversalSerialNr = ?";

                    var VesselData = conn.Query<Vessel>(queryv, parameterv);
                    foreach (var item in VesselData)
                    {
                        shipmentRef.VesselModel.ETD = item.ETD;
                        shipmentRef.VesselModel.ETA = item.ETA;
                    }
                }
            }

            catch (Exception)
            {

                throw;
            }
            return shipmentRef;
        }

        public static List<Vessel> GetVesselsDetails(string UniversalSerialNr)
        {
            List<Vessel> vessels = new List<Vessel>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select ID, CarrierBookingRefNo, Carrier, UniversalSerialNr, VesselName, VoyNo, LoadPort, DischPort, ETD, ETA" +
                        " From VesselScheduleDry where UniversalSerialNr = ?";

                    var vesselData = conn.Query<Vessel>(query,parameters);
                    foreach (var item in vesselData)
                    {
                        vessels.Add(new Vessel
                        {
                            ID = item.ID,
                            CarrierBookingRefNo = item.CarrierBookingRefNo,
                            Carrier = item.Carrier,
                            //DateATA = item.DateATA,
                            //DateSOB = item.DateSOB,
                            DischPort = item.DischPort,
                            ETA = item.ETA,
                            ETD = item.ETD,
                            LoadPort = item.LoadPort,
                            UniversalSerialNr = item.UniversalSerialNr,
                            VesselName = item.VesselName,
                            VoyNo = item.VoyNo
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return vessels;
        }

        public static List<Booking> GetBookingDetails(SearchParameters search)
        {
            string IDCompany = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
            List<Booking> bookings = new List<Booking>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    string query = "Select BC.BookingID, BC.BookingNo, BC.QuoteRefID, BC.CompanyName, BC.DischargePort, BC.LoadPort, BC.UniversalSerialNr, BC.BookingStatus" +
                        " From BookingConfirmation BC INNER JOIN AgentAddressBook AB ON BC.CompanyName = AB.CompanyName WHERE AB.IDCompany = '" + IDCompany + "' AND";

                    if ((search.BookingID != null) || (search.BookingNo != null) || (search.Status != null) || (search.QuoteRefID != null) || (search.DischargePort != null) || (search.LoadPort != null) || (search.CompanyName != null))
                    {
                        if(search.BookingID!=null)
                        {
                            search.BookingID = search.BookingID.ToUpperInvariant();
                            query += " BC.BookingID = '" + search.BookingID + "' AND";
                        }

                        if (search.BookingNo != null)
                        {
                            search.BookingNo = search.BookingNo.ToUpperInvariant();
                            query += " BC.BookingNo = '" + search.BookingNo + "' AND";
                        }

                        if (search.Status != null)
                        {
                            query += " BC.BookingStatus = '" + search.Status + "' AND";
                        }

                        if (search.QuoteRefID != null)
                        {
                            search.QuoteRefID = search.QuoteRefID.ToUpperInvariant();
                            query += " BC.QuoteRefID = '" + search.QuoteRefID + "' AND";
                        }

                        if (search.DischargePort != null)
                        {
                            query += " BC.DischargePort = '" + search.DischargePort + "' AND";
                        }

                        if (search.LoadPort != null)
                        {
                            query += " BC.LoadPort = '" + search.LoadPort + "' AND";
                        }

                        if (search.CompanyName != null)
                        {
                            query += " BC.CompanyName = '" + search.CompanyName + "' AND";
                        }
                    }

                    if (query.LastIndexOf("AND") > 0)
                    {
                        query = query.Remove(query.LastIndexOf("AND"));
                    }

                    query += " Order By BC.BKID DESC";// FETCH FIRST 10 ROWS ONLY";

                    var bookingData = conn.Query<Booking>(query);
                    foreach (var item in bookingData)
                    {
                        bookings.Add(new Booking
                        {
                            BookingID = item.BookingID,
                            BookingNo = item.BookingNo,
                            QuoteRefID = item.QuoteRefID,
                            CompanyName = item.CompanyName,
                            DischargePort = item.DischargePort,
                            LoadPort = item.LoadPort,
                            UniversalSerialNr = item.UniversalSerialNr,
                            BookingStatus = item.BookingStatus
                        });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return bookings;
        }

        public static List<ShipmentDetails> GetShipmentDetails(SearchParameters search)
        {
            string IDCompany = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
            List<ShipmentDetails> shipments = new List<ShipmentDetails>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    string query = "Select SD.JobRef, BC.BookingNo, SD.pzChargeParty, SD.IDQuoteRef, SD.DischPort, SD.LoadPort, SD.UniversalSerialNr, SD.StatusShipment" +
                        " From ShipmentDetailDRY SD INNER JOIN BookingConfirmation BC ON SD.UniversalSerialNr = BC.UniversalSerialNr ";//INNER JOIN AgentAddressBook AB ON BC.CompanyName = AB.CompanyName "; +
                        //"WHERE AB.IDCompany = '" + IDCompany + "' AND";

                    if ((search.JobRef != null) || (search.pzChargeParty != null) || (search.BookingNo != null) || (search.Status != null) || (search.QuoteRefID != null) || (search.DischargePort != null) || (search.LoadPort != null))
                    {
                        query += "WHERE";
                        if (search.JobRef != null)
                        {
                            search.JobRef = search.JobRef.ToUpperInvariant();
                            query += " SD.JobRef = '" + search.JobRef + "' AND";
                        }
                        
                        if (search.pzChargeParty != null)
                        {
                            search.pzChargeParty = search.pzChargeParty.ToUpperInvariant();
                            query += " SD.pzChargeParty = '" + search.pzChargeParty + "' AND";
                        }

                        if (search.BookingNo != null)
                        {
                            search.BookingNo = search.BookingNo.ToUpperInvariant();
                            query += " BC.BookingNo = '" + search.BookingNo + "' AND";
                        }

                        if (search.Status != null)
                        {
                            query += " SD.StatusShipment = '" + search.Status + "' AND";
                        }

                        if (search.QuoteRefID != null)
                        {
                            search.QuoteRefID = search.QuoteRefID.ToUpperInvariant();
                            query += " SD.IDQuoteRef = '" + search.QuoteRefID + "' AND";
                        }

                        if (search.DischargePort != null)
                        {
                            query += " SD.DischPort = '" + search.DischargePort + "' AND";
                        }

                        if (search.LoadPort != null)
                        {
                            query += " SD.LoadPort = '" + search.LoadPort + "' AND";
                        }
                    }

                    if (query.LastIndexOf("AND") > 0)
                    {
                        query = query.Remove(query.LastIndexOf("AND"));
                    }

                    query += " Order By BC.BKID DESC";// FETCH FIRST 10 ROWS ONLY";

                    var shipmentData = conn.Query<ShipmentDetails>(query);
                    foreach (var item in shipmentData)
                    {
                        shipments.Add(new ShipmentDetails
                        {
                            JobRef = item.JobRef,
                            pzChargeParty = item.pzChargeParty,
                            BookingNo = item.BookingNo,
                            IDQuoteRef = item.IDQuoteRef,
                            DischPort = item.DischPort,
                            LoadPort = item.LoadPort,
                            StatusShipment = item.StatusShipment
                        });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return shipments;
        }

        public static ErrorLog CreateVessel(Vessel vessel)
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Carrier", vessel.Carrier, DbType.String, ParameterDirection.Input);
                    //parameters.Add("DateATA", vessel.DateATA, DbType.Date, ParameterDirection.Input); //Change field name to DischargePort
                    //parameters.Add("DateSOB", vessel.DateSOB, DbType.Date, ParameterDirection.Input);
                    parameters.Add("ETA", vessel.ETA, DbType.Date, ParameterDirection.Input);
                    parameters.Add("ETD", vessel.ETD, DbType.Date, ParameterDirection.Input);
                    parameters.Add("DischPort", vessel.DischPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("LoadPort", vessel.LoadPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("VesselName", vessel.VesselName, DbType.String, ParameterDirection.Input);
                    parameters.Add("VoyNo", vessel.VoyNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("CarrierBookingRefNo", vessel.CarrierBookingRefNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("UniversalSerialNr", vessel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string odbcQuery = "Insert Into VesselScheduleDry (Carrier, ETA, ETD,  DischPort, LoadPort, VesselName,  VoyNo, CarrierBookingRefNo, " +
                        "UniversalSerialNr) " +
                        //"ModifiedBy, ModificationTimestamp) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                        string query = "Select MAX (ID) as ID from VesselScheduleDry";

                        var vesselData = conn.Query<Vessel>(query, parameters);
                        foreach (var item in vesselData)
                        {
                            vessel.ID = item.ID;
                        }
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message;
            }
            return errorLog;
        }

        public static ErrorLog UpdateVessel(Vessel vessel)
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Carrier", vessel.Carrier, DbType.String, ParameterDirection.Input);
                    //parameters.Add("DateATA", vessel.DateATA, DbType.Date, ParameterDirection.Input); //Change field name to DischargePort
                    //parameters.Add("DateSOB", vessel.DateSOB, DbType.Date, ParameterDirection.Input);
                    parameters.Add("DischPort", vessel.DischPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("ETA", vessel.ETA, DbType.Date, ParameterDirection.Input);
                    parameters.Add("ETD", vessel.ETD, DbType.Date, ParameterDirection.Input);
                    parameters.Add("LoadPort", vessel.LoadPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("VesselName", vessel.VesselName, DbType.String, ParameterDirection.Input);
                    parameters.Add("VoyNo", vessel.VoyNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("CarrierBookingRefNo", vessel.CarrierBookingRefNo, DbType.String, ParameterDirection.Input);
                   // parameters.Add("BookingNo", vessel.BookingNo, DbType.Date, ParameterDirection.Input);
                    parameters.Add("UniversalSerialNr", vessel.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parameters.Add("ID", vessel.ID, DbType.String, ParameterDirection.Input);

                    string odbcQuery = "Update VesselScheduleDry Set Carrier = ?, DischPort = ?, ETA = ?, " +
                           "ETD = ?, LoadPort = ?, VesselName = ?, VoyNo = ?, CarrierBookingRefNo = ?, UniversalSerialNr = ? " +
                           "Where ID = ?";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message;
            }
            return errorLog;
        }

        public static ErrorLog DeleteVessel(string universalSerialNr, string vesselID)
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendTankLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);
                    parameters.Add("ID", vesselID, DbType.String, ParameterDirection.Input);
                    string query = "DELETE FROM VesselScheduleDry Where UniversalSerialNr = ? and ID = ?";
                    int rowsAffected = conn.Execute(query, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                }
            }
            catch (Exception ex)
            {
                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message;
            }
            return errorLog;
        }

        public static List<SelectListItem> ShipmentTerm()
        {
            List<SelectListItem> terms = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "CYCY",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "CYFO",
                    Value = "2"
                },
                new SelectListItem
                {
                    Text = "FIFO",
                    Value = "3"

                },
                new SelectListItem
                {
                    Text = "FICY",
                    Value = "4"
                },
                new SelectListItem
                {
                    Text = "FIHK",
                    Value = "5"
                },
                new SelectListItem
                {
                    Text = "HKFI",
                    Value = "6"
                },
                new SelectListItem
                {
                    Text = "CYHK",
                    Value = "7"
                },
                new SelectListItem
                {
                    Text = "HKCY",
                    Value = "8"
                }
            };

            return terms;
        }

        public static List<SelectListItem> EquipmentType()
        {
            List<SelectListItem> equip = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "20'GP",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "40'GP",
                    Value = "2"
                },
                new SelectListItem
                {
                    Text = "40'HC",
                    Value = "3"
                },
                new SelectListItem
                {
                    Text = "20'FR",
                    Value = "4"
                },
                new SelectListItem
                {
                    Text = "40'FR",
                    Value = "5"
                },
                new SelectListItem
                {
                    Text = "40'HR",
                    Value = "6"
                },
                new SelectListItem
                {
                    Text = "20'OT",
                    Value = "7"
                },
                new SelectListItem
                {
                    Text = "40'OT",
                    Value = "8"
                }
            };

            return equip;
        }

        public static List<SelectListItem> GetStatus()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "DRAFT",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "APPROVAL REQUIRED",
                    Value = "2"
                },
                new SelectListItem
                {
                    Text = "APPROVED",
                    Value = "3"
                },
                new SelectListItem
                {
                    Text = "COUNTER OFFER",
                    Value = "4"
                },
                new SelectListItem
                {
                    Text = "REJECTED",
                    Value = "5"
                },
                new SelectListItem
                {
                    Text = "EXPIRED",
                    Value = "6"
                }

            };

            return stat;
        }

        public static List<SelectListItem> GetQuoteStatus()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                //new SelectListItem
                //{
                //    Text = "DRAFT",
                //    Value = "1"
                //},
                new SelectListItem
                {
                    Text = "APPROVED",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "EXPIRED",
                    Value = "2"
                }
            };

            return stat;
        }

        public static List<SelectListItem> GetBookingStatus()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "DRAFT",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "CONFIRMED",
                    Value = "2"
                },
                new SelectListItem
                {
                    Text = "ISSUED",
                    Value = "3"
                },
                new SelectListItem
                {
                    Text = "EXPIRED",
                    Value = "4"
                }

            };

            return stat;
        }

        public static List<SelectListItem> GetShipmentStatus()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "DRAFT",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "CONFIRMED",
                    Value = "2"
                },
                new SelectListItem
                {
                    Text = "ISSUED",
                    Value = "3"
                },
                new SelectListItem
                {
                    Text = "EXPIRED",
                    Value = "4"
                }

            };

            return stat;
        }

        public static List<SelectListItem> GetAgencyType()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Branch Office",
                    Value = "Branch Office"
                },
                new SelectListItem
                {
                    Text = "Agent",
                    Value = "Agent"
                }
            };

            return stat;
        }

        private static string ConvertToString(List<string> charges)
        {
            string agentCostLi = string.Empty;
            if (charges.Any())
            {
                agentCostLi = string.Join(",", charges);
            }
            return agentCostLi;
        }

        private static List<string> ConvertToList(string agentCostLi)
        {
            List<string> selectedChargeList = new List<string>();
            if (!string.IsNullOrEmpty(agentCostLi))
            {
                selectedChargeList = agentCostLi.Split(',').ToList();
            }

            return selectedChargeList;
        }
    }
}