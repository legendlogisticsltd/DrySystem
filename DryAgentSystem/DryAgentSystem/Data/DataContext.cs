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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string selectquery = "Select RR.RateID, RR.CompanyName, RR.ShipmentTerm, RR.LoadPort, RR.DischargePort, RR.PlaceOfReceipt, RR.TransshipmentPort, RR.PlaceOfDelivery, RR.POLFreeDays, RR.PODFreeDays, RR.EffectiveDate, " +
                         "RR.ValidityDate, RR.Status, RR.Quantity, RR.EquipmentType, RR.CargoType, RR.Rate, RR.RateCountered, RR.AgentName, RR.ModifiedDate From RateRequest RR INNER JOIN UserAccountManagement UM ON RR.IDCompany = UM.IDCompany WHERE UM.Name = '" + username + "' AND";

                    if ((filter.CompanyName != null) || (filter.Status != null) || (filter.DischargePort != null) || (filter.LoadPort != null) || (filter.RateID != null))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select Port, PortCode, Country from CountryPort Order By Port";// FETCH FIRST 10 ROWS ONLY";
                    var allports = conn.Query<CountryPort>(query);
                    if (allports != null)
                    {
                        foreach (var por in allports)
                        {
                            ports.Add(new CountryPort
                            {
                                Port = por.Port,
                                PortCode = por.PortCode,
                                Country = por.Country,
                                PortCountry = por.Port + ", " + por.Country
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

        public static string GetCountryPortFromPort(string Port)
        {
            CountryPort ports = new CountryPort();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Port", Port, DbType.String, ParameterDirection.Input);
                    string query = ("Select Port, PortCode, Country from CountryPort WHERE Port = ?");
                    var PortDetails = conn.Query<CountryPort>(query, parameters);
                    foreach (var item in PortDetails)
                    {
                        ports.Port = item.Port;
                        ports.PortCode = item.PortCode;
                        ports.Country = item.Country;
                        ports.PortCountry = ports.Port + ", " + ports.Country;
                    }

                }
                return ports.PortCountry;
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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

                    using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                    using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                    parameters.Add("Shipper", booking.Shipper, DbType.String, ParameterDirection.Input);

                    string odbcQuery = "Insert Into BookingConfirmation (LoadPort, DischargePort, QuoteRefID, CompanyName, " +
                        "CutoffDate, CROPickUpDate, CutoffDateandTime, ServiceMode, GrossweightMeasurement, Grossweight, Commodity, CommodityGroup, RateType, Temperature, Humidity, " +
                        "Ventilation, Remark, CRORemarks, CreatedBy, BookingStatus, PaymentTerms, Volume, AddressAttn, AddressFax, AddressTel, AddressTo, CollectionYard, Shipper) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

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
                    using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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

        public static List<InvoiceDetails> GetInvoiceChargesList(string UniversalSerialNr)
        {
            List<InvoiceDetails> invoicelineitems = new List<InvoiceDetails>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<InvoiceDetails>("Select ID, Description, Quantity, Currency, UnitRate, ExRate, AmountUSD from SalesInvoiceLineItemDRY where UniversalSerialNr = ? ", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            invoicelineitems.Add(new InvoiceDetails
                            {
                                ID = charge.ID,
                                Description = charge.Description,
                                Quantity = charge.Quantity,
                                Currency = charge.Currency,
                                UnitRate = charge.UnitRate,
                                ExRate = charge.ExRate,
                                AmountUSD = charge.AmountUSD,
                            });
                        }
                    }
                }
                return invoicelineitems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<InvoiceDetails> GetSalesChargesList(string UniversalSerialNr)
        {
            List<InvoiceDetails> invoicelineitems = new List<InvoiceDetails>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<InvoiceDetails>("Select ID, Description, Quantity, Currency, UnitRate, ExRate, AmountUSD, PaymentTerm from DRYShipmentSellLI where UniversalSerialNr = ? AND PaymentTerm = 'Prepaid' ", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            invoicelineitems.Add(new InvoiceDetails
                            {
                                ID = charge.ID,
                                Description = charge.Description,
                                Quantity = charge.Quantity,
                                Currency = charge.Currency,
                                UnitRate = charge.UnitRate,
                                ExRate = charge.ExRate,
                                AmountUSD = charge.AmountUSD,
                                PaymentTerm = charge.PaymentTerm,
                            });
                        }
                    }
                }
                return invoicelineitems;
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("RateID", rateID, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<DQuoteSales>("Select ID, RateID, Description, PayBy, PaymentTerm, PayMode, UnitRate, ExRate, Currency from DQuoteSales where RateID= ? ", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            dQuoteSales.Add(new DQuoteSales
                            {
                                ID = charge.ID,
                                RateID = charge.RateID,
                                Description = charge.Description,
                                PayBy = charge.PayBy,
                                PaymentTerm = charge.PaymentTerm,
                                PayMode = charge.PayMode,
                                UnitRate = charge.UnitRate,
                                Exrate = charge.Exrate,
                                Currency = charge.Currency,
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
        public static List<DQuoteSales> GetRateChargesList(string rateID)
        {
            List<DQuoteSales> dQuoteSales = new List<DQuoteSales>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("RateID", rateID, DbType.String, ParameterDirection.Input);
                    var allcharges = conn.Query<DQuoteSales>("Select ID, RateID, Description, PayBy, PaymentTerm, PayMode, UnitRate, ExRate, Currency from DQuoteCostLI where RateID= ? ", parameters);
                    if (allcharges != null)
                    {
                        foreach (var charge in allcharges)
                        {
                            dQuoteSales.Add(new DQuoteSales
                            {
                                ID = charge.ID,
                                RateID = charge.RateID,
                                Description = charge.Description,
                                PayBy = charge.PayBy,
                                PaymentTerm = charge.PaymentTerm,
                                PayMode = charge.PayMode,
                                UnitRate = charge.UnitRate,
                                Exrate = charge.Exrate,
                                Currency = charge.Currency,
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select q.QuoteRefID, q.RateID, q.CompanyName, q.ShipmentTerm, q.LoadPort, q.TransshipmentPort, q.DischargePort, q.PlaceOfReceipt, q.PlaceOfDelivery, q.POLFreeDays, q.PODFreeDays, q.EffectiveDate,  " +
                            "q.Validity, q.StatusDIS, q.Quantity, q.EquipmentType, q.CargoType, r.Rate, r.RateCountered, r.AgentName From QuoteReference q INNER JOIN RateRequest r ON q.RateID = r.RateID INNER JOIN UserAccountManagement um ON r.IDCompany = um.IDCompany WHERE um.Name = '" + username + "' AND q.StatusDIS <> 'DRAFT' AND";

                    if ((search.QuoteRefID != null) || (search.RateID != null) || (search.Status != null) || (search.DischargePort != null) || (search.LoadPort != null) || (search.CompanyName != null))

                    {
                        if (search.QuoteRefID != null)
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

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                            bookref.ShipmentTerm = quotedetails.ShipmentTerm;

                        }

                        bookref.CollectionYard = item.CollectionYard;
                        bookref.ContainerReleaseOrderNo = item.ContainerReleaseOrderNo;
                        if (bookref.CollectionYard != null)
                        {
                            Booking book = new Booking();
                            book = GetCollectionAddress(bookref.CollectionYard, bookref.LoadPort);
                            bookref.Address = book.Address;
                            bookref.Email = book.Email;
                            bookref.PhoneNo = book.PhoneNo;
                        }
                        bookref.Vessels = GetVesselsDetails(bookref.UniversalSerialNr);
                        bookref.hasshipment = CheckShipmentfromUSN(bookref.UniversalSerialNr);
                    }
                }
            }

            catch (Exception)
            {

                throw;
            }
            return bookref;
        }

        public static string GetContainerNofromID(string ID)
        {
            string ContainerNo = "";
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ID", ID, DbType.String, ParameterDirection.Input);

                    string query = "Select ContainerNo from AllocateEquipment where ID = ?";

                    ContainerNo = conn.QueryFirst<String>(query, parameters);

                }
            }

            catch (Exception)
            {
                throw;
            }
            return ContainerNo;
        }

        public static string GetRateTypeFromUSN(string UniversalSerialNr)
        {
            string rateType = string.Empty;
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select RateType from BookingConfirmation where UniversalSerialNr = ?";

                    rateType = conn.QueryFirst<String>(query, parameters);

                }
            }

            catch (Exception)
            {
                throw;
            }
            return rateType;
        }
        
        public static int GetCreditTermFromAddress(string CompanyName)
        {
            int CreditTerm = 0;
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyName", CompanyName.ToUpperInvariant(), DbType.String, ParameterDirection.Input);

                    string query = "Select CreditTerm from Address where CompanyName = ?";

                    CreditTerm = conn.QueryFirstOrDefault<int>(query, parameters);

                }
            }

            catch (Exception)
            {
                throw;
            }
            return CreditTerm;
        }


        public static bool CheckShipmentfromUSN(string UniversalSerialNr)
        {


            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select JobRef from ShipmentDetailDRY where UniversalSerialNr = ?";

                    var ShipmentData = conn.Query<ShipmentDetails>(query, parameters);

                    if (ShipmentData.Count() == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }

            catch (Exception)
            {
                throw;
            }

        }



        public static ShipmentBL GetShipmentFromJobRef(string JobRef)
        {
            ShipmentBL shipmentRef = new ShipmentBL();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string querysd = "Select JobRef, HBLHAWB, ClosingDate, ChargeParty, ClosingTime, EquipmentType, Remark, ShipmentTerm, Shipper, ShipperAddress, LoadPort, PlaceOfReceipt, DischPort, PlaceOfDelivery, Quantity, LDepotTerminal, DischAgentAddress, MBLMAWB, CustomerRef, UniversalSerialNr, IDQuoteRef from ShipmentDetailDry where JobRef = ?";

                    var ShipmentData = conn.Query<ShipmentDetails>(querysd, parametersd);
                    foreach (var item in ShipmentData)
                    {
                        shipmentRef.ShipmentDetailsModel.JobRef = item.JobRef;
                        shipmentRef.ShipmentDetailsModel.HBLHAWB = item.HBLHAWB;
                        shipmentRef.ShipmentDetailsModel.ClosingDate = item.ClosingDate;
                        shipmentRef.ShipmentDetailsModel.ClosingTime = item.ClosingTime;
                        shipmentRef.ShipmentDetailsModel.ClosingDate = shipmentRef.ShipmentDetailsModel.ClosingDate + shipmentRef.ShipmentDetailsModel.ClosingTime;
                        shipmentRef.ShipmentDetailsModel.ShipmentTerm = item.ShipmentTerm;
                        shipmentRef.ShipmentDetailsModel.Shipper = item.Shipper;
                        shipmentRef.ShipmentDetailsModel.ShipperAddress = item.ShipperAddress;
                        shipmentRef.ShipmentDetailsModel.DischAgentAddress = item.DischAgentAddress;
                        shipmentRef.ShipmentDetailsModel.LoadPort = item.LoadPort;
                        shipmentRef.ShipmentDetailsModel.PlaceOfReceipt = item.PlaceOfReceipt;
                        shipmentRef.ShipmentDetailsModel.DischPort = item.DischPort;
                        shipmentRef.ShipmentDetailsModel.PlaceOfDelivery = item.PlaceOfDelivery;
                        shipmentRef.ShipmentDetailsModel.MBLMAWB = item.MBLMAWB;
                        shipmentRef.ShipmentDetailsModel.CustomerRef = item.CustomerRef;
                        shipmentRef.ShipmentDetailsModel.UniversalSerialNr = item.UniversalSerialNr;
                        shipmentRef.ShipmentDetailsModel.IDQuoteRef = item.IDQuoteRef;
                        shipmentRef.ShipmentDetailsModel.Quantity = item.Quantity;
                        shipmentRef.ShipmentDetailsModel.LDepotTerminal = item.LDepotTerminal;
                        shipmentRef.ShipmentDetailsModel.ChargeParty = item.ChargeParty;
                        shipmentRef.ShipmentDetailsModel.Remark = item.Remark;
                        shipmentRef.ShipmentDetailsModel.EquipmentType = item.EquipmentType;
                        shipmentRef.ShipmentDetailsModel.ContainerList = GetContainerList(shipmentRef.ShipmentDetailsModel.LoadPort).Select(x => new SelectListItem { Text = x.ContainerNo, Value = x.ContainerNo });
                        if(String.IsNullOrEmpty(DataContext.GetInvoiceFromUSN(shipmentRef.ShipmentDetailsModel.UniversalSerialNr).InvoiceNo))
                        {
                            shipmentRef.ShipmentDetailsModel.InvoiceSave = false;
                        }
                        else
                        {
                            shipmentRef.ShipmentDetailsModel.InvoiceSave = true;
                        }
                    }

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);

                    string querybl = "Select * from BLDetail where JobRef = ?";

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
                        shipmentRef.BLDetailsModel.LoadPort = item.LoadPort;
                        shipmentRef.BLDetailsModel.PlaceofReceipt = item.PlaceofReceipt;
                        shipmentRef.BLDetailsModel.DischPort = item.DischPort;
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
                        shipmentRef.BLDetailsModel.BLTypes = item.BLTypes;
                        shipmentRef.BLDetailsModel.BLStatus = item.BLStatus;
                        shipmentRef.BLDetailsModel.PreCarriageBy = item.PreCarriageBy;
                        shipmentRef.BLDetailsModel.NoOfPkgs = item.NoOfPkgs;
                        shipmentRef.BLDetailsModel.PkgType = item.PkgType;
                        shipmentRef.BLDetailsModel.TotalGweight = item.TotalGweight;
                        shipmentRef.BLDetailsModel.GrossWeightUnit = item.GrossWeightUnit;
                        shipmentRef.BLDetailsModel.NetWtUnit = item.NetWtUnit;
                        shipmentRef.BLDetailsModel.MeasurementUnit = item.MeasurementUnit;
                        shipmentRef.BLDetailsModel.TotalContainerMeasurement = item.TotalContainerMeasurement;
                    }

                    var parameterv = new DynamicParameters();
                    parameterv.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string queryv = "Select ETD, ETA,VesselName,VoyNo from VesselScheduleDry where UniversalSerialNr = ?";

                    var VesselData = conn.Query<Vessel>(queryv, parameterv);
                    foreach (var item in VesselData)
                    {
                        shipmentRef.VesselModel.ETD = item.ETD;
                        shipmentRef.VesselModel.ETA = item.ETA;
                        shipmentRef.VesselModel.VesselName = item.VesselName;
                        shipmentRef.VesselModel.VoyNo = item.VoyNo;
                    }
                }
            }

            catch (Exception)
            {

                throw;
            }
            return shipmentRef;
        }

        public static Invoice GetInvoiceFromUSN(string UniversalSerialNr)
        {
            Invoice invoiceRef = new Invoice();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select UniversalSerialNr, InvoiceNo, AmountinUSDSUM, InvoiceDate, LoadPort, DischargePort, ETD, ETA , InvoiceMonth, CreditTerms, DueDate, BillingParty, BillingPartyAddress, Grossweight, GrossweightUnit, ContainerNo, Remarks, JobRefNo from SalesInvoiceDry where UniversalSerialNr = ?";

                    var InvoiceData = conn.Query<Invoice>(query, parameters);
                    foreach (var item in InvoiceData)
                    {
                        invoiceRef.InvoiceNo = item.InvoiceNo;
                        invoiceRef.InvoiceDate = item.InvoiceDate;
                        invoiceRef.InvoiceMonth = item.InvoiceMonth;
                        invoiceRef.DueDate = item.DueDate;
                        invoiceRef.BillingParty = item.BillingParty;
                        invoiceRef.BillingPartyAddress = item.BillingPartyAddress;
                        invoiceRef.CreditTerms = item.CreditTerms;
                        invoiceRef.Grossweight = item.Grossweight;
                        invoiceRef.GrossweightUnit = item.GrossweightUnit;
                        invoiceRef.Remarks = item.Remarks;
                        invoiceRef.UniversalSerialNr = item.UniversalSerialNr;
                        invoiceRef.LoadPort = item.LoadPort;
                        invoiceRef.DischargePort = item.DischargePort;
                        invoiceRef.ETD = item.ETD;
                        invoiceRef.ETA = item.ETA;
                        invoiceRef.JobRefNo = item.JobRefNo;
                        invoiceRef.ContainerNo = item.ContainerNo;
                        invoiceRef.AmountinUSDSUM = item.AmountinUSDSUM;
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }
            return invoiceRef;
        }

        public static ErrorLog SaveExportInvoice(Invoice invoice)
        {
            string username = HttpContext.Current.User.Identity.Name;
            //string status = "DRAFT";
            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);
            string RateType = GetRateTypeFromUSN(invoice.UniversalSerialNr);
            invoice.CreditTerms = GetCreditTermFromAddress(invoice.BillingParty);
            string InvoiceType = "EXPORT";
            invoice.InvoiceNo = GenerateInvoiceNumber(RateType, InvoiceType);
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("InvoiceNo", invoice.InvoiceNo, DbType.String, ParameterDirection.Input);
                    //parameters.Add("InvoiceDate", invoice.InvoiceDate, DbType.Date, ParameterDirection.Input);
                    parameters.Add("InvoiceMonth", invoice.InvoiceMonth, DbType.Int16, ParameterDirection.Input);
                    //parameters.Add("DueDate", invoice.DueDate, DbType.Date, ParameterDirection.Input);
                    parameters.Add("BillingParty", invoice.BillingParty, DbType.String, ParameterDirection.Input);
                    parameters.Add("BillingPartyAddress", invoice.BillingPartyAddress, DbType.String, ParameterDirection.Input);
                    parameters.Add("Grossweight", invoice.Grossweight, DbType.String, ParameterDirection.Input);
                    parameters.Add("GrossweightUnit", invoice.GrossweightUnit, DbType.String, ParameterDirection.Input);
                    parameters.Add("Remarks", invoice.Remarks, DbType.String, ParameterDirection.Input);
                    parameters.Add("UniversalSerialNr", invoice.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parameters.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("InvoiceType", InvoiceType, DbType.String, ParameterDirection.Input);
                    parameters.Add("JobRefNo", invoice.JobRefNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("CreditTerms", invoice.CreditTerms, DbType.Int32, ParameterDirection.Input);



                    string odbcQuery = "Insert Into SalesInvoiceDRY(InvoiceNo, InvoiceMonth, BillingParty, BillingPartyAddress, Grossweight, " +
                        "GrossweightUnit, Remarks, UniversalSerialNr, CreatedBy, InvoiceType, JobRefNo, CreditTerms) Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }


                    SaveInvoiceCharges(invoice);

                }


                errorLog.ErrorMessage = invoice.InvoiceNo;
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog SaveShipment(ShipmentBL shipmentRef)
        {
            string username = HttpContext.Current.User.Identity.Name;
            //string status = "DRAFT";
            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("ShipmentTerm", shipmentRef.ShipmentDetailsModel.ShipmentTerm, DbType.String, ParameterDirection.Input);
                    parametersd.Add("MBLMAWB", shipmentRef.ShipmentDetailsModel.MBLMAWB, DbType.String, ParameterDirection.Input);
                    //parametersd.Add("HBLHAWB", shipmentRef.ShipmentDetailsModel.HBLHAWB, DbType.String, ParameterDirection.Input);
                    parametersd.Add("CustomerRef", shipmentRef.ShipmentDetailsModel.CustomerRef, DbType.String, ParameterDirection.Input);
                    parametersd.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parametersd.Add("Shipper", shipmentRef.ShipmentDetailsModel.Shipper, DbType.String, ParameterDirection.Input);
                    parametersd.Add("ShipperAddress", shipmentRef.ShipmentDetailsModel.ShipperAddress, DbType.String, ParameterDirection.Input);
                    parametersd.Add("DischAgentAddress", shipmentRef.ShipmentDetailsModel.DischAgentAddress, DbType.String, ParameterDirection.Input);
                    parametersd.Add("LoadPort", shipmentRef.ShipmentDetailsModel.LoadPort, DbType.String, ParameterDirection.Input);
                    parametersd.Add("PlaceOfReceipt", shipmentRef.ShipmentDetailsModel.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                    parametersd.Add("DischPort", shipmentRef.ShipmentDetailsModel.DischPort, DbType.String, ParameterDirection.Input);
                    parametersd.Add("Quantity", shipmentRef.ShipmentDetailsModel.Quantity, DbType.String, ParameterDirection.Input);
                    parametersd.Add("PlaceOfDelivery", shipmentRef.ShipmentDetailsModel.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                    parametersd.Add("IDQuoteRef", shipmentRef.ShipmentDetailsModel.IDQuoteRef, DbType.String, ParameterDirection.Input);
                    parametersd.Add("LDepotTerminal", shipmentRef.ShipmentDetailsModel.LDepotTerminal, DbType.String, ParameterDirection.Input);
                    parametersd.Add("ClosingDate", shipmentRef.ShipmentDetailsModel.ClosingDate, DbType.Date, ParameterDirection.Input);
                    parametersd.Add("ClosingTime", shipmentRef.ShipmentDetailsModel.ClosingDate.TimeOfDay, DbType.Time, ParameterDirection.Input);
                    parametersd.Add("Remark", shipmentRef.ShipmentDetailsModel.Remark, DbType.String, ParameterDirection.Input);
                    parametersd.Add("EquipmentType", shipmentRef.ShipmentDetailsModel.EquipmentType, DbType.String, ParameterDirection.Input);
                    parametersd.Add("User", username, DbType.String, ParameterDirection.Input);


                    string odbcQuerysd = "Insert Into ShipmentDetailDRY(ShipmentTerm, MBLMAWB, CustomerRef, UniversalSerialNr, Shipper, ShipperAddress, DischAgentAddress, " +
                        "LoadPort, PlaceOfReceipt, DischPort, Quantity, PlaceOfDelivery, IDQuoteRef, LDepotTerminal, ClosingDate, ClosingTime, Remark, EquipmentType, \"User\") Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    int rowsAffected = conn.Execute(odbcQuerysd, parametersd);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                    string query = "SELECT JobRef FROM ShipmentDetailDRY WHERE \"User\" = '" + username + "' ORDER BY CreationTimestamp DESC FETCH FIRST 1 ROWS ONLY";

                    var shipmentreference = conn.Query<ShipmentDetails>(query);
                    foreach (var item in shipmentreference)
                    {
                        shipmentRef.ShipmentDetailsModel.JobRef = item.JobRef;
                    }


                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("ShipperNameBL", shipmentRef.ShipmentDetailsModel.Shipper, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("ConsigneeNameBL", shipmentRef.BLDetailsModel.ConsigneeNameBL, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("ShipperAddressBL", shipmentRef.ShipmentDetailsModel.ShipperAddress, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("ConsigneeAddressBL", shipmentRef.BLDetailsModel.ConsigneeAddressBL, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NotifyPartyName", shipmentRef.BLDetailsModel.NotifyPartyName, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischAgentNameBL", shipmentRef.BLDetailsModel.DischAgentNameBL, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NotifyPartyAddress", shipmentRef.BLDetailsModel.NotifyPartyAddress, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischAgentAddress", shipmentRef.ShipmentDetailsModel.DischAgentAddress, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("LoadPort", shipmentRef.ShipmentDetailsModel.LoadPort, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PlaceofReceipt", shipmentRef.ShipmentDetailsModel.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischPort", shipmentRef.ShipmentDetailsModel.DischPort, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PlaceofDelivery", shipmentRef.ShipmentDetailsModel.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                    //parameterbl.Add("VesselDetails", shipmentRef.BLDetailsModel.VesselDetails, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("MarksandNo", shipmentRef.BLDetailsModel.MarksandNo, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("CargoDescription", shipmentRef.BLDetailsModel.CargoDescription, DbType.String, ParameterDirection.Input);
                    //parameterbl.Add("PlaceofIssue", shipmentRef.BLDetailsModel.PlaceofIssue, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("LadenOnBoard", shipmentRef.BLDetailsModel.LadenOnBoard, DbType.Date, ParameterDirection.Input);
                    parameterbl.Add("HBLFreightPayment", shipmentRef.BLDetailsModel.HBLFreightPayment, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("BLFinalisedDate", shipmentRef.BLDetailsModel.BLFinalisedDate, DbType.Date, ParameterDirection.Input);
                    parameterbl.Add("DateofIssue", shipmentRef.BLDetailsModel.DateofIssue, DbType.Date, ParameterDirection.Input);
                    parameterbl.Add("MBLFreightPayment", shipmentRef.BLDetailsModel.MBLFreightPayment, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NoofOriginalBLissued", shipmentRef.BLDetailsModel.NoofOriginalBLissued, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NoOfPkgs", shipmentRef.BLDetailsModel.NoOfPkgs.ToString(), DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PkgType", shipmentRef.BLDetailsModel.PkgType, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("GrossWeightUnit", shipmentRef.BLDetailsModel.GrossWeightUnit, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NetWtUnit", shipmentRef.BLDetailsModel.NetWtUnit, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("MeasurementUnit", shipmentRef.BLDetailsModel.MeasurementUnit, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("JobRef", shipmentRef.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("JobRefFull", shipmentRef.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);

                    string odbcQuerybl = "Insert Into BLDetail (ShipperNameBL, ConsigneeNameBL, ShipperAddressBL, ConsigneeAddressBL, NotifyPartyName, DischAgentNameBL, NotifyPartyAddress, DischAgentAddress, " +
                        "LoadPort, PlaceofReceipt, DischPort, PlaceofDelivery, MarksandNo, CargoDescription, LadenOnBoard, HBLFreightPayment, BLFinalisedDate, DateofIssue, MBLFreightPayment, " +
                        "NoofOriginalBLissued, NoOfPkgs, PkgType, GrossWeightUnit, NetWtUnit, MeasurementUnit, UniversalSerialNr, JobRef, JobRefFull, CreatedBy) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    int rowsAffectedbl = conn.Execute(odbcQuerybl, parameterbl);
                    SendShipmentCharges(shipmentRef.ShipmentDetailsModel.UniversalSerialNr);
                    if (rowsAffectedbl > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }
                }


                errorLog.ErrorMessage = shipmentRef.ShipmentDetailsModel.JobRef;
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }
        public static ErrorLog UpdateShipment(ShipmentBL shipmentRef)
        {
            string username = HttpContext.Current.User.Identity.Name;
            //string status = "DRAFT";
            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("ShipmentTerm", shipmentRef.ShipmentDetailsModel.ShipmentTerm, DbType.String, ParameterDirection.Input);
                    parametersd.Add("MBLMAWB", shipmentRef.ShipmentDetailsModel.MBLMAWB, DbType.String, ParameterDirection.Input);
                    //parametersd.Add("HBLHAWB", shipmentRef.ShipmentDetailsModel.HBLHAWB, DbType.String, ParameterDirection.Input);
                    parametersd.Add("CustomerRef", shipmentRef.ShipmentDetailsModel.CustomerRef, DbType.String, ParameterDirection.Input);
                    parametersd.Add("Shipper", shipmentRef.ShipmentDetailsModel.Shipper, DbType.String, ParameterDirection.Input);
                    parametersd.Add("ShipperAddress", shipmentRef.ShipmentDetailsModel.ShipperAddress, DbType.String, ParameterDirection.Input);
                    parametersd.Add("DischAgentAddress", shipmentRef.ShipmentDetailsModel.DischAgentAddress, DbType.String, ParameterDirection.Input);
                    parametersd.Add("LoadPort", shipmentRef.ShipmentDetailsModel.LoadPort, DbType.String, ParameterDirection.Input);
                    parametersd.Add("PlaceOfReceipt", shipmentRef.ShipmentDetailsModel.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                    parametersd.Add("DischPort", shipmentRef.ShipmentDetailsModel.DischPort, DbType.String, ParameterDirection.Input);
                    parametersd.Add("PlaceOfDelivery", shipmentRef.ShipmentDetailsModel.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                    parametersd.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parametersd.Add("ClosingDate", shipmentRef.ShipmentDetailsModel.ClosingDate, DbType.Date, ParameterDirection.Input);
                    parametersd.Add("ClosingTime", shipmentRef.ShipmentDetailsModel.ClosingDate.TimeOfDay, DbType.Time, ParameterDirection.Input);
                    parametersd.Add("Remark", shipmentRef.ShipmentDetailsModel.Remark, DbType.String, ParameterDirection.Input);
                    parametersd.Add("UserModify", username, DbType.String, ParameterDirection.Input);
                    parametersd.Add("JobRef", shipmentRef.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);


                    string odbcQuerysd = "UPDATE ShipmentDetailDRY SET ShipmentTerm = ?, MBLMAWB = ?, CustomerRef = ?, Shipper = ?, ShipperAddress = ?, DischAgentAddress = ?, " +
                        "LoadPort = ?, PlaceOfReceipt = ?, DischPort = ?, PlaceOfDelivery = ?, UniversalSerialNr = ?, ClosingDate = ?, ClosingTime = ?, Remark = ?, UserModify = ? WHERE JobRef = ?";

                    int rowsAffected = conn.Execute(odbcQuerysd, parametersd);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }


                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("ShipperNameBL", shipmentRef.ShipmentDetailsModel.Shipper, DbType.String, ParameterDirection.Input); //Change field name to DischargePort
                    parameterbl.Add("ConsigneeNameBL", shipmentRef.BLDetailsModel.ConsigneeNameBL, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("ShipperAddressBL", shipmentRef.ShipmentDetailsModel.ShipperAddress, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("ConsigneeAddressBL", shipmentRef.BLDetailsModel.ConsigneeAddressBL, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NotifyPartyName", shipmentRef.BLDetailsModel.NotifyPartyName, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischAgentNameBL", shipmentRef.BLDetailsModel.DischAgentNameBL, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NotifyPartyAddress", shipmentRef.BLDetailsModel.NotifyPartyAddress, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischAgentAddress", shipmentRef.ShipmentDetailsModel.DischAgentAddress, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("LoadPort", shipmentRef.ShipmentDetailsModel.LoadPort, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PlaceofReceipt", shipmentRef.ShipmentDetailsModel.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischPort", shipmentRef.ShipmentDetailsModel.DischPort, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PlaceofDelivery", shipmentRef.ShipmentDetailsModel.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                    //parameterbl.Add("VesselDetails", shipmentRef.BLDetailsModel.VesselDetails, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("MarksandNo", shipmentRef.BLDetailsModel.MarksandNo, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("CargoDescription", shipmentRef.BLDetailsModel.CargoDescription, DbType.String, ParameterDirection.Input);
                    //parameterbl.Add("PlaceofIssue", shipmentRef.BLDetailsModel.PlaceofIssue, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("LadenOnBoard", shipmentRef.BLDetailsModel.LadenOnBoard, DbType.Date, ParameterDirection.Input);
                    parameterbl.Add("HBLFreightPayment", shipmentRef.BLDetailsModel.HBLFreightPayment, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("BLFinalisedDate", shipmentRef.BLDetailsModel.BLFinalisedDate, DbType.Date, ParameterDirection.Input);
                    parameterbl.Add("DateofIssue", shipmentRef.BLDetailsModel.DateofIssue, DbType.Date, ParameterDirection.Input);
                    parameterbl.Add("MBLFreightPayment", shipmentRef.BLDetailsModel.MBLFreightPayment, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NoofOriginalBLissued", shipmentRef.BLDetailsModel.NoofOriginalBLissued, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NoOfPkgs", shipmentRef.BLDetailsModel.NoOfPkgs.ToString(), DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PkgType", shipmentRef.BLDetailsModel.PkgType, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("GrossWeightUnit", shipmentRef.BLDetailsModel.GrossWeightUnit, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("NetWtUnit", shipmentRef.BLDetailsModel.NetWtUnit, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("MeasurementUnit", shipmentRef.BLDetailsModel.MeasurementUnit, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("JobRef", shipmentRef.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);

                    string odbcQuerybl = "Update BLDetail  SET ShipperNameBL = ?, ConsigneeNameBL = ?, ShipperAddressBL = ?, ConsigneeAddressBL = ?, NotifyPartyName = ?, DischAgentNameBL = ?, NotifyPartyAddress = ?, DischAgentAddress = ?, " +
                        "LoadPort = ?, PlaceofReceipt = ?, DischPort = ?, PlaceofDelivery = ?, MarksandNo = ?, CargoDescription = ?, LadenOnBoard = ?, HBLFreightPayment = ?, BLFinalisedDate = ?, DateofIssue = ?, MBLFreightPayment = ?, " +
                        "NoofOriginalBLissued = ?, NoOfPkgs = ?, PkgType = ?, GrossWeightUnit = ?, NetWtUnit = ?, MeasurementUnit = ?, ModifiedBy = ? WHERE JobRef = ? ";

                    int rowsAffectedbl = conn.Execute(odbcQuerybl, parameterbl);

                    if (rowsAffectedbl > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }


                }


                errorLog.ErrorMessage = shipmentRef.ShipmentDetailsModel.JobRef;
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }
        public static ErrorLog SendShipmentCharges(string UniversalSerialNr)
        {
            string username = HttpContext.Current.User.Identity.Name;
            //string status = "DRAFT";
            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);
            string RateID = "";
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select QR.RateID from BookingConfirmation BC INNER JOIN QuoteReference QR ON BC.QuoteRefID = QR.QuoteRefID where BC.UniversalSerialNr = ?";

                    RateID = conn.QueryFirst<string>(query, parameters);

                    var costcharges = GetRateChargesList(RateID);
                    var salescharges = GetQuoteChargesList(RateID);


                    string costQuery = "";
                    foreach (var item in costcharges)
                    {
                        var parameterc = new DynamicParameters();
                        parameterc.Add("Description", item.Description, DbType.String, ParameterDirection.Input);
                        parameterc.Add("UnitRate", item.UnitRate, DbType.Decimal, ParameterDirection.Input);
                        parameterc.Add("Currency", item.Currency, DbType.String, ParameterDirection.Input);
                        parameterc.Add("Exrate", item.Exrate, DbType.Decimal, ParameterDirection.Input);
                        parameterc.Add("PayBy", item.PayBy, DbType.String, ParameterDirection.Input);
                        parameterc.Add("PaymentTerm", item.PaymentTerm, DbType.String, ParameterDirection.Input);
                        parameterc.Add("PayMode", item.PayMode, DbType.String, ParameterDirection.Input);
                        parameterc.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);
                        parameterc.Add("User", username, DbType.String, ParameterDirection.Input);

                        costQuery = "Insert Into DryShipmentCostLI (Description, UnitRate, Currency, Exrate, PayBy, PaymentTerm, PayMode, UniversalSerialNr, \"User\") " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        int rowsAffected = conn.Execute(costQuery, parameterc);
                        if (rowsAffected > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }
                    }

                    string salesQuery = "";
                    foreach (var item in salescharges)
                    {
                        var parametersc = new DynamicParameters();
                        parametersc.Add("Description", item.Description, DbType.String, ParameterDirection.Input);
                        parametersc.Add("UnitRate", item.UnitRate, DbType.Decimal, ParameterDirection.Input);
                        parametersc.Add("Currency", item.Currency, DbType.String, ParameterDirection.Input);
                        parametersc.Add("Exrate", item.Exrate, DbType.Decimal, ParameterDirection.Input);
                        parametersc.Add("PayBy", item.PayBy, DbType.String, ParameterDirection.Input);
                        parametersc.Add("PaymentTerm", item.PaymentTerm, DbType.String, ParameterDirection.Input);
                        parametersc.Add("PayMode", item.PayMode, DbType.String, ParameterDirection.Input);
                        parametersc.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);
                        parametersc.Add("User", username, DbType.String, ParameterDirection.Input);

                        salesQuery = "Insert Into DryShipmentSellLI (Description, UnitRate, Currency, Exrate, PayBy, PaymentTerm, PayMode, UniversalSerialNr, \"User\") " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        int rowsAffected = conn.Execute(salesQuery, parametersc);
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


                //errorLog.ErrorMessage = shipmentRef.ShipmentDetailsModel.JobRef;
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog SaveInvoiceCharges(Invoice invoice)
        {
            string username = HttpContext.Current.User.Identity.Name;
            //string status = "DRAFT";
            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);

            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {


                    var salescharges = GetSalesChargesList(invoice.UniversalSerialNr);
                    string ContainerNo = ConvertToString(GetContainerNo(invoice.UniversalSerialNr));
                    string salesQuery = "";
                    foreach (var item in salescharges)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Description", item.Description, DbType.String, ParameterDirection.Input);
                        parameters.Add("UnitRate", item.UnitRate, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("Currency", item.Currency, DbType.String, ParameterDirection.Input);
                        parameters.Add("ExRate", item.ExRate, DbType.Decimal, ParameterDirection.Input);
                        parameters.Add("Quantity", item.Quantity, DbType.Int16, ParameterDirection.Input);
                        parameters.Add("PaymentTerm", item.PaymentTerm, DbType.String, ParameterDirection.Input);
                        parameters.Add("UniversalSerialNr", invoice.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                        parameters.Add("CompanyName", invoice.BillingParty, DbType.String, ParameterDirection.Input);
                        parameters.Add("InvoiceNo", invoice.InvoiceNo, DbType.String, ParameterDirection.Input);
                        parameters.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);
                        parameters.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);

                        salesQuery = "Insert Into SalesInvoiceLineItemDRY (Description, UnitRate, Currency, Exrate, Quantity, PaymentTerm, UniversalSerialNr, CompanyName, InvoiceNo, ContainerNo, CreatedBy) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        int rowsAffected = conn.Execute(salesQuery, parameters);
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

            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static List<Vessel> GetVesselsDetails(string UniversalSerialNr)
        {
            List<Vessel> vessels = new List<Vessel>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select ID, CarrierBookingRefNo, Carrier, UniversalSerialNr, VesselName, VoyNo, LoadPort, DischPort, ETD, ETA" +
                        " From VesselScheduleDry where UniversalSerialNr = ?";

                    var vesselData = conn.Query<Vessel>(query, parameters);
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
        public static List<AllocateEquipment> GetTanksDetails(string UniversalSerialNr)
        {
            List<AllocateEquipment> tanks = new List<AllocateEquipment>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select ID, ContainerNo, SealNo, GrossWeight, NettWeight, Measurement, UniversalSerialNr" +
                        " From AllocateEquipment where UniversalSerialNr = ?";

                    var tankData = conn.Query<AllocateEquipment>(query, parameters);
                    foreach (var item in tankData)
                    {
                        tanks.Add(new AllocateEquipment
                        {
                            ID = item.ID,
                            ContainerNo = item.ContainerNo,
                            SealNo = item.SealNo,
                            GrossWeight = item.GrossWeight,
                            NettWeight = item.NettWeight,
                            Measurement = item.Measurement,
                            UniversalSerialNr = item.UniversalSerialNr
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return tanks;
        }

        public static List<string> GetContainerNo(string UniversalSerialNr)
        {
            List<string> container = new List<string>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select ContainerNo" +
                        " From AllocateEquipment where UniversalSerialNr = ?";

                    var containerData = conn.Query<string>(query, parameters);
                    foreach (var item in containerData)
                    {
                        container.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return container;
        }

        public static ErrorLog CreateTank(ShipmentDetails shipment)
        {
            var containers = shipment.SelectedContainerList;
            try
            {
                string username = HttpContext.Current.User.Identity.Name;
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    foreach (var item in containers)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ContainerNo", item, DbType.String, ParameterDirection.Input);
                        parameters.Add("UniversalSerialNr", shipment.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                        parameters.Add("User", username, DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Insert Into AllocateEquipment (ContainerNo, UniversalSerialNr, \"User\") " +
                            "Values(?, ?, ?)";

                        int rowsAffected = conn.Execute(odbcQuery, parameters);
                        if (rowsAffected > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }

                        {
                            var parameters1 = new DynamicParameters();
                            parameters1.Add("ActiveDPort", shipment.DischPort, DbType.String, ParameterDirection.Input);
                            parameters1.Add("ActiveLDepotTerminal", shipment.LDepotTerminal, DbType.String, ParameterDirection.Input);
                            parameters1.Add("UserModify", username, DbType.String, ParameterDirection.Input);
                            parameters1.Add("ContainerNo", item, DbType.String, ParameterDirection.Input);

                            string invQuery1 = "UPDATE Inventory SET ActiveDPort = ?, ActiveLDepotTerminal = ?, ActiveDDepotTerminal = null, " +
                                "ActiveDDate = null, ERequestDate = null, ERequestLog = null, ERequester = null, PreUniversalSerialNr[2] = PreUniversalSerialNr[1], UserModify = ? WHERE ContainerNo = ? ";

                            int rowsAffectedq1 = conn.Execute(invQuery1, parameters1);
                            if (rowsAffectedq1 > 0)
                            {
                                errorLog.IsError = false;
                            }
                            else
                            {
                                errorLog.IsError = true;
                            }

                            string invQuery2 = "UPDATE Inventory SET PreUniversalSerialNr[1] = UniversalSerialNr WHERE ContainerNo = '" + item + "' ";

                            int rowsAffectedq2 = conn.Execute(invQuery2);
                            if (rowsAffectedq2 > 0)
                            {
                                errorLog.IsError = false;
                            }
                            else
                            {
                                errorLog.IsError = true;
                            }

                            string invQuery3 = "UPDATE Inventory SET UniversalSerialNr = '" + shipment.UniversalSerialNr + "' WHERE ContainerNo = '" + item + "' ";

                            int rowsAffectedq3 = conn.Execute(invQuery3);
                            if (rowsAffectedq3 > 0)
                            {
                                errorLog.IsError = false;
                            }
                            else
                            {
                                errorLog.IsError = true;
                            }
                        }


                        var parametersm = new DynamicParameters();
                        parametersm.Add("UniversalSerialNr", shipment.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                        parametersm.Add("LoadPort", shipment.LoadPort, DbType.String, ParameterDirection.Input);
                        parametersm.Add("DischargePort", shipment.DischPort, DbType.String, ParameterDirection.Input);
                        parametersm.Add("LDepotTerminal", shipment.LDepotTerminal, DbType.String, ParameterDirection.Input);
                        parametersm.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                        parametersm.Add("ContainerNo", item, DbType.String, ParameterDirection.Input);

                        string movQuery = "INSERT Into MovementRecord (UniversalSerialNr, LoadPort, DischargePort, LDepotTerminal, CreatedBy, ContainerNo) " +
                            "Values(?, ?, ?, ?, ?, ?)";

                        int rowsAffectedm = conn.Execute(movQuery, parametersm);
                        if (rowsAffectedm > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }
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
        public static ErrorLog UpdateTank(AllocateEquipment tank)
        {
            string username = HttpContext.Current.User.Identity.Name;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    //parameters.Add("ContainerNo", tank.ContainerNo, DbType.String, ParameterDirection.Input);
                    //parameters.Add("DateATA", vessel.DateATA, DbType.Date, ParameterDirection.Input); //Change field name to DischargePort
                    //parameters.Add("DateSOB", vessel.DateSOB, DbType.Date, ParameterDirection.Input);
                    parameters.Add("SealNo", tank.SealNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("GrossWeight", tank.GrossWeight, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("NettWeight", tank.NettWeight, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("Measurement", tank.Measurement, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("UniversalSerialNr", tank.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parameters.Add("ModifyUser", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("ID", tank.ID, DbType.String, ParameterDirection.Input);

                    string odbcQuery = "Update AllocateEquipment Set SealNo = ?, GrossWeight = ?, " +
                           "NettWeight = ?, Measurement = ?, UniversalSerialNr = ?, ModifyUser = ? " +
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

        public static ErrorLog DeleteTank(string ID, string UniversalSerialNr)
        {
            string username = HttpContext.Current.User.Identity.Name;
            try
            {
                string ContainerNo = GetContainerNofromID(ID);
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ID", ID, DbType.String, ParameterDirection.Input);
                    string query = "DELETE FROM AllocateEquipment Where ID = ?";
                    int rowsAffected = conn.Execute(query, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                    {
                        var parameters1 = new DynamicParameters();
                        parameters1.Add("UserModify", username, DbType.String, ParameterDirection.Input);
                        parameters1.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);

                        string invQuery1 = "UPDATE Inventory SET ActiveDPort = null, ActiveLDepotTerminal = null, ActiveDDepotTerminal = null, " +
                            "ActiveDDate = null, ERequestDate = null, ERequestLog = null, ERequester = null, UniversalSerialNr = PreUniversalSerialNr[1], UserModify = ? WHERE ContainerNo = ? ";

                        int rowsAffectedq1 = conn.Execute(invQuery1, parameters1);
                        if (rowsAffectedq1 > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }

                        string invQuery2 = "UPDATE Inventory SET PreUniversalSerialNr[1] = PreUniversalSerialNr[2] WHERE ContainerNo = '" + ContainerNo + "' ";

                        int rowsAffectedq2 = conn.Execute(invQuery2);
                        if (rowsAffectedq2 > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }
                    }

                    var parametersm = new DynamicParameters();
                    parametersm.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parametersm.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);
                    string movQuery = "DELETE FROM MovementRecord WHERE UniversalSerialNr = ? AND ContainerNo = ?";

                    int rowsAffectedm = conn.Execute(movQuery, parametersm);
                    if (rowsAffectedm > 0)
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
        public static List<Booking> GetBookingDetails(SearchParameters search)
        {
            string IDCompany = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
            List<Booking> bookings = new List<Booking>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select BC.BookingID, BC.BookingNo, BC.QuoteRefID, BC.CompanyName, BC.DischargePort, BC.LoadPort, BC.UniversalSerialNr, BC.BookingStatus" +
                        " From BookingConfirmation BC INNER JOIN AgentAddressBook AB ON BC.CompanyName = AB.CompanyName WHERE AB.IDCompany = '" + IDCompany + "' AND";

                    if ((search.BookingID != null) || (search.BookingNo != null) || (search.Status != null) || (search.QuoteRefID != null) || (search.DischargePort != null) || (search.LoadPort != null) || (search.CompanyName != null))
                    {
                        if (search.BookingID != null)
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select SD.JobRef, BC.BookingNo, SD.ChargeParty, SD.IDQuoteRef, SD.DischPort, SD.LoadPort, SD.UniversalSerialNr, SD.StatusShipment" +
                        " From ShipmentDetailDRY SD INNER JOIN BookingConfirmation BC ON SD.UniversalSerialNr = BC.UniversalSerialNr ";//INNER JOIN AgentAddressBook AB ON BC.CompanyName = AB.CompanyName "; +
                                                                                                                                       //"WHERE AB.IDCompany = '" + IDCompany + "' AND";

                    if ((search.JobRef != null) || (search.ChargeParty != null) || (search.BookingNo != null) || (search.Status != null) || (search.QuoteRefID != null) || (search.DischargePort != null) || (search.LoadPort != null))
                    {
                        query += "WHERE";
                        if (search.JobRef != null)
                        {
                            search.JobRef = search.JobRef.ToUpperInvariant();
                            query += " SD.JobRef = '" + search.JobRef + "' AND";
                        }

                        if (search.ChargeParty != null)
                        {
                            query += " SD.ChargeParty = '" + search.ChargeParty + "' AND";
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

                    query += " Order By SD.CreationTimestamp DESC";// FETCH FIRST 10 ROWS ONLY";

                    var shipmentData = conn.Query<ShipmentDetails>(query);
                    foreach (var item in shipmentData)
                    {
                        shipments.Add(new ShipmentDetails
                        {
                            JobRef = item.JobRef,
                            ChargeParty = item.ChargeParty,
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

        public static List<Invoice> GetInvoiceDetails(SearchParameters search)
        {
            string IDCompany = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
            List<Invoice> invoices = new List<Invoice>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select JobRefNo, InvoiceNo, BillingParty, Status, LoadPort, DischargePort" +
                        " From SalesInvoiceDRY ";//INNER JOIN AgentAddressBook AB ON BC.CompanyName = AB.CompanyName "; +
                                                 //"WHERE AB.IDCompany = '" + IDCompany + "' AND";

                    if ((search.JobRef != null) || (search.InvoiceNo != null) || (search.CompanyName != null) || (search.Status != null) || (search.LoadPort != null) || (search.DischargePort != null))
                    {
                        query += "WHERE";
                        if (search.JobRef != null)
                        {
                            search.JobRef = search.JobRef.ToUpperInvariant();
                            query += " JobRefNo = '" + search.JobRef + "' AND";
                        }

                        if (search.InvoiceNo != null)
                        {
                            search.InvoiceNo = search.InvoiceNo.ToUpperInvariant();
                            query += " InvoiceNo = '" + search.InvoiceNo + "' AND";
                        }

                        if (search.CompanyName != null)
                        {
                            //search.CompanyName = search.CompanyName.ToUpperInvariant();
                            query += " BillingParty = '" + search.CompanyName + "' AND";
                        }

                        if (search.Status != null)
                        {
                            query += " Status = '" + search.Status + "' AND";
                        }

                        if (search.DischargePort != null)
                        {
                            query += " DischargePort = '" + search.DischargePort + "' AND";
                        }

                        if (search.LoadPort != null)
                        {
                            query += " LoadPort = '" + search.LoadPort + "' AND";
                        }
                    }

                    if (query.LastIndexOf("AND") > 0)
                    {
                        query = query.Remove(query.LastIndexOf("AND"));
                    }

                    query += " Order By CreationTimestamp DESC";// FETCH FIRST 10 ROWS ONLY";

                    var invoicelist = conn.Query<Invoice>(query);
                    foreach (var item in invoicelist)
                    {
                        invoices.Add(new Invoice
                        {
                            JobRefNo = item.JobRefNo,
                            InvoiceNo = item.InvoiceNo,
                            BillingParty = item.BillingParty,
                            DischargePort = item.DischargePort,
                            LoadPort = item.LoadPort,
                            Status = item.Status
                        });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return invoices;
        }

        public static ErrorLog ChangeBLType(string BLTypes, string JobRef)
        {
            string username = HttpContext.Current.User.Identity.Name;

            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("BLTypes", BLTypes, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string odbcQuerybl = "Update BLDetail  SET BLTypes = ?, ModifiedBy = ?";

                    if (BLTypes == "ORIGINAL")
                    {
                        odbcQuerybl += ", BLStatus = 'ORIGINAL ISSUED'";
                    }

                    odbcQuerybl += " WHERE JobRef = ? ";


                    int rowsAffectedbl = conn.Execute(odbcQuerybl, parameterbl);

                    if (rowsAffectedbl > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }
                }

                errorLog.ErrorMessage = JobRef;
            }
            catch (Exception ex)
            {

                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }
        public static ErrorLog CreateVessel(Vessel vessel)
        {
            string username = HttpContext.Current.User.Identity.Name;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                    parameters.Add("User", username, DbType.String, ParameterDirection.Input);

                    string odbcQuery = "Insert Into VesselScheduleDry (Carrier, ETA, ETD,  DischPort, LoadPort, VesselName, VoyNo, CarrierBookingRefNo, " +
                        "UniversalSerialNr, \"User\") " +
                        //"ModifiedBy, ModificationTimestamp) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

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
            string username = HttpContext.Current.User.Identity.Name;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
                    parameters.Add("UserModify", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("ID", vessel.ID, DbType.String, ParameterDirection.Input);

                    string odbcQuery = "Update VesselScheduleDry Set Carrier = ?, DischPort = ?, ETA = ?, " +
                           "ETD = ?, LoadPort = ?, VesselName = ?, VoyNo = ?, CarrierBookingRefNo = ?, UniversalSerialNr = ?, UserModify = ? " +
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
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
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
        public static List<Inventory> GetContainerList(string loadPort)
        {
            List<Inventory> inventories = new List<Inventory>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ActiveLPort", loadPort, DbType.String, ParameterDirection.Input);

                    string query = "Select ContainerNo, UniversalSerialNr From Inventory where ActiveLPort = ? and StatusTrack IN ('NEW','AVAILABLE','POST AVAILABLE')";

                    var inventoryList = conn.Query<Inventory>(query, parameters);
                    foreach (var item in inventoryList)
                    {
                        inventories.Add(new Inventory
                        {
                            UniversalSerialNr = item.UniversalSerialNr,
                            ContainerNo = item.ContainerNo
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return inventories;
        }

        public static string GenerateInvoiceNumber(string RateType, string InvoiceType)
        {
            string invoiceNumber = string.Empty;
            string year = DateTime.Now.ToString("yy");
            string month = DateTime.Now.ToString("MM");
            int counter = 0;
            try
            {
                string rateTypeInitial;
                if (RateType.ToUpperInvariant() == Constant.Dry)
                {
                    rateTypeInitial = Constant.DryInitial;
                }
                else
                {
                    rateTypeInitial = Constant.ReeferInitial;
                }

                string invoiceTypeInitial;
                if (InvoiceType.ToUpperInvariant() == Constant.Export)
                {
                    invoiceTypeInitial = Constant.ExportInitial;
                }
                else
                {
                    invoiceTypeInitial = Constant.ImportInitial;
                }

                string invoicePrefix = string.Format("{0}{1}{2}{3}", rateTypeInitial, invoiceTypeInitial, year, month);

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "SELECT Max(InvoiceNo) AS InvoiceNo FROM SalesInvoiceDRY where InvoicePrefix = ?";
                    var parameters = new DynamicParameters();
                    parameters.Add("InvoicePrefix", invoicePrefix, DbType.String, ParameterDirection.Input);

                    invoiceNumber = conn.QueryFirst<string>(query, parameters);
                    if (string.IsNullOrEmpty(invoiceNumber))
                    {
                        counter += 1;
                        string counterNumber = counter.ToString("D4");
                        invoiceNumber = string.Format("{0}{1}", invoicePrefix, counterNumber);
                    }
                    else
                    {
                        counter = Convert.ToInt32(invoiceNumber.Substring(8));
                        counter += 1;
                        string counterNumber = counter.ToString("D4");
                        invoiceNumber = string.Format("{0}{1}", invoicePrefix, counterNumber);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return invoiceNumber;
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
        public static List<SelectListItem> GetInvoiceStatus()
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

        public static List<SelectListItem> GetWtUnit()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "KGS"
                },
                new SelectListItem
                {
                    Text = "MT"
                },
                new SelectListItem
                {
                    Text = "CBM"
                },
                new SelectListItem
                {
                    Text = "LTR"
                },
                new SelectListItem
                {
                    Text = "GRM"
                }
            };

            return stat;
        }

        public static List<SelectListItem> GetMUnit()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "CBM"
                },
                new SelectListItem
                {
                    Text = "M3"
                }
            };

            return stat;
        }

        public static List<SelectListItem> GetPackageType()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "SACKS"
                },
                new SelectListItem
                {
                    Text = "BARS"
                },
                new SelectListItem
                {
                    Text = "BUNDELS"
                },
                new SelectListItem
                {
                    Text = "BAGS"
                },
                new SelectListItem
                {
                    Text = "BULK"
                },
                new SelectListItem
                {
                    Text = "STEEL BLOCKS"
                },
                new SelectListItem
                {
                    Text = "BALES"
                },
                new SelectListItem
                {
                    Text = "BOXES"
                },
                new SelectListItem
                {
                    Text = "BARREL"
                },
                new SelectListItem
                {
                    Text = "STEEL BULKS"
                },
                new SelectListItem
                {
                    Text = "CANS"
                },
                new SelectListItem
                {
                    Text = "CASES"
                },
                new SelectListItem
                {
                    Text = "CARBOYS"
                },
                new SelectListItem
                {
                    Text = "CHEST"
                },
                new SelectListItem
                {
                    Text = "COILS"
                },
                new SelectListItem
                {
                    Text = "COLLIES"
                },
                new SelectListItem
                {
                    Text = "CONTAINER"
                },
                new SelectListItem
                {
                    Text = "TANKS"
                },
                new SelectListItem
                {
                    Text = "VAN"
                },
                new SelectListItem
                {
                    Text = "CRATES"
                },
                new SelectListItem
                {
                    Text = "WOODEN CASKS"
                },
                new SelectListItem
                {
                    Text = "CSKS"
                },
                new SelectListItem
                {
                    Text = "CARTONS"
                },
                new SelectListItem
                {
                    Text = "CYLINDER"
                },
                new SelectListItem
                {
                    Text = "DRUM"
                },
                new SelectListItem
                {
                    Text = "STEEL ENVELOPES"
                },
                new SelectListItem
                {
                    Text = "FLASK"
                },
                new SelectListItem
                {
                    Text = "FUTS"
                },
                new SelectListItem
                {
                    Text = "HABBUUK"
                },
                new SelectListItem
                {
                    Text = "JUMBLE BALE"
                },
                new SelectListItem
                {
                    Text = "JOTTA"
                },
                new SelectListItem
                {
                    Text = "KEGGS"
                },
                new SelectListItem
                {
                    Text = "LIFT"
                },
                new SelectListItem
                {
                    Text = "LIFT VAN"
                },
                new SelectListItem
                {
                    Text = "LOGS"
                },
                new SelectListItem
                {
                    Text = "LOT"
                },
                new SelectListItem
                {
                    Text = "LSES"
                },
                new SelectListItem
                {
                    Text = "M/T"
                },
                new SelectListItem
                {
                    Text = "INGOT"
                },
                new SelectListItem
                {
                    Text = "NUMBERS"
                },
                new SelectListItem
                {
                    Text = "PAIL"
                },
                new SelectListItem
                {
                    Text = "PALLS"
                },
                new SelectListItem
                {
                    Text = "PACKAGES"
                },
                new SelectListItem
                {
                    Text = "PACKS"
                },
                new SelectListItem
                {
                    Text = "PIECES"
                },
                new SelectListItem
                {
                    Text = "PKGS"
                },
                new SelectListItem
                {
                    Text = "SETS"
                },
                new SelectListItem
                {
                    Text = "PLS"
                },
                new SelectListItem
                {
                    Text = "PALLETS"
                },
                new SelectListItem
                {
                    Text = "QUADS"
                },
                new SelectListItem
                {
                    Text = "RCKS"
                },
                new SelectListItem
                {
                    Text = "REELS"
                },
                new SelectListItem
                {
                    Text = "ROLLS"
                },
                new SelectListItem
                {
                    Text = "SACK"
                },
                new SelectListItem
                {
                    Text = "SKIDS"
                },
                new SelectListItem
                {
                    Text = "SLABS"
                },
                new SelectListItem
                {
                    Text = "TABLE"
                },
                new SelectListItem
                {
                    Text = "TINS"
                },
                new SelectListItem
                {
                    Text = "TRUNK"
                },
                new SelectListItem
                {
                    Text = "UNITS"
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