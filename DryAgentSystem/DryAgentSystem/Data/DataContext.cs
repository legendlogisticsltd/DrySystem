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

            catch (Exception ex)
            {
                throw ex;
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

                    selectquery += " Order By RR.ModifiedDate DESC";//FETCH FIRST 10 ROWS ONLY";


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
        public static List<CountryPort> GetCountryPortsByASC()
        {
            List<CountryPort> ports = new List<CountryPort>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select DISTINCT Country from CountryPort Order By Country ASC, Port";
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
        public static List<CountryPort> GetCountryPorts()
        {
            List<CountryPort> ports = new List<CountryPort>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select Port, PortCode, Country from CountryPort Order By Port";//FETCH FIRST 10 ROWS ONLY"; //WHERE Port IN('CHENNAI','JEBEL ALI') 
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
        public static List<SelectListItem> GetPortAliasFromPort(string Port)
        {
            CountryPort ports = new CountryPort();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                { 
                    for(int i=1;i<=10;i++)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Port", Port, DbType.String, ParameterDirection.Input);
                        string query = ("Select PortNameAlias["+i+"] from CountryPort WHERE Port = ?");
                        var PortDetails = conn.Query<String>(query, parameters);

                        foreach (var item in PortDetails)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                ports.PortNameAlias.Add(new SelectListItem
                                {
                                    Text = item
                                });
                            }
                        }

                    }
                    

                }
                return ports.PortNameAlias;
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
                    parameters.Add("Port", "%"+Port+"%", DbType.String, ParameterDirection.Input);
                    //parameters.Add("IDDepot", IDDepot, DbType.String, ParameterDirection.Input);



                    //string query = "Select DepotName from DepotDetails WHERE Port = ? AND IDDepot = ? Order By DepotName";
                    string query = "SELECT CompanyName FROM Address WHERE(TOBPort LIKE ?) AND (TOB LIKE '%DEPOT TERMINAL%') Order By CompanyName";
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
                    parameters.Add("Port", "%" + Port + "%", DbType.String, ParameterDirection.Input);
                    //parameters.Add("IDDepot", IDDepot, DbType.String, ParameterDirection.Input);
                    parameters.Add("CompanyName", DepotName, DbType.String, ParameterDirection.Input);



                    //string query = "Select Address,Email,PhoneNo from DepotDetails WHERE Port = ? AND IDDepot = ? AND DepotName = ?";
                    string query = "Select LabelAddress AS Address from Address WHERE (TOBPort LIKE ?) AND (TOB LIKE '%DEPOT TERMINAL%') AND CompanyName = ?";
                    var DepoDetails = conn.Query<Booking>(query, parameters);
                    foreach (var item in DepoDetails)
                    {
                        booking.Address = item.Address;
                        
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
                    string query = "Select AGB.CompanyName, AGB.CustomAddress AS Address from AgentAddressBook AGB INNER JOIN UserAccountManagement UM ON " +
                        "UM.IDCompany = AGB.IDCompany Where UM.Name = ? Order By CompanyName";

                    var allcompanies = conn.Query<CompanyInfo>(query, parameters);
                    if (allcompanies != null)
                    {
                        foreach (var com in allcompanies)
                        {
                            companyname.Add(new SelectListItem
                            {
                                Text = com.CompanyName,
                                Value = com.Address
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
                        rateRequest.GTotalSalesCal = Math.Round(item.GTotalSalesCal, 2);
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
            catch (Exception ex)
            {
                throw ex;
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
                    parameters.Add("GrossWt", rateRequest.GrossWt, DbType.Double, ParameterDirection.Input);
                    parameters.Add("GrossWtUnit", rateRequest.GrossWtUnit, DbType.String, ParameterDirection.Input);
                    parameters.Add("DischargePort", rateRequest.DischargePort, DbType.String, ParameterDirection.Input);
                    parameters.Add("EquipmentType", rateRequest.EquipmentType, DbType.String, ParameterDirection.Input);
                    parameters.Add("LoadPort", rateRequest.LoadPort, DbType.String, ParameterDirection.Input);
                    parameters.Add("PlaceOfDelivery", rateRequest.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                    parameters.Add("PlaceOfReceipt", rateRequest.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                    parameters.Add("Quantity", rateRequest.Quantity, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("Rate", rateRequest.Rate, DbType.Double, ParameterDirection.Input);
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

                    //SendCostLICharges(rateRequest.SelectedExportLocalCharges, rateRequest.SelectedImportLocalCharges, rateRequest.RateID);

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
                        parameters.Add("GrossWt", rateRequest.GrossWt, DbType.Double, ParameterDirection.Input);
                        parameters.Add("GrossWtUnit", rateRequest.GrossWtUnit, DbType.String, ParameterDirection.Input);
                        parameters.Add("DischargePort", rateRequest.DischargePort, DbType.String, ParameterDirection.Input);
                        parameters.Add("EquipmentType", rateRequest.EquipmentType, DbType.String, ParameterDirection.Input);
                        parameters.Add("LoadPort", rateRequest.LoadPort, DbType.String, ParameterDirection.Input);
                        parameters.Add("PlaceOfDelivery", rateRequest.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                        parameters.Add("PlaceOfReceipt", rateRequest.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                        parameters.Add("Quantity", rateRequest.Quantity, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("Rate", rateRequest.Rate, DbType.Double, ParameterDirection.Input);
                        parameters.Add("RateCountered", rateRequest.RateCountered, DbType.Double, ParameterDirection.Input);
                        parameters.Add("ShipmentTerm", rateRequest.ShipmentTerm, DbType.String, ParameterDirection.Input);
                        parameters.Add("POLFreeDays", rateRequest.POLFreeDays, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("PODFreeDays", rateRequest.PODFreeDays, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("EffectiveDate", rateRequest.EffectiveDate, DbType.Date, ParameterDirection.Input);
                        parameters.Add("ValidityDate", rateRequest.ValidityDate, DbType.Date, ParameterDirection.Input);
                        //parameters.Add("QuoteType", quoteref.QuoteType, DbType.String, ParameterDirection.Input);
                        //parameters.Add("Status", status, DbType.String, ParameterDirection.Input);
                        parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                        parameters.Add("Remark", rateRequest.Remark, DbType.String, ParameterDirection.Input);
                        parameters.Add("AgentCostLI", agentCostLi, DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Update RateRequest Set CargoType = ?, UNNo = ?, IMO = ?, CompanyName = ?, AgencyType = ?, TransshipmentType = ?, TransshipmentPort = ?, RateType = ?, " +
                            "Temperature = ?, Humidity = ?, Ventilation = ?, ShipperName = ?, GrossWt = ?, GrossWtUnit = ?, DischargePort = ?, EquipmentType = ?, LoadPort = ? ," +
                            "PlaceOfDelivery = ?, PlaceOfReceipt = ?, Quantity = ?, Rate = ?, RateCountered = ?, ShipmentTerm = ? ,POLFreeDays = ?,PODFreeDays = ?, " +
                            "EffectiveDate = ?, ValidityDate = ?, ModifiedBy = ?, Remark = ?, AgentCostLI = ? " +
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
                        parameters.Add("RateCountered", rateRequest.RateCountered, DbType.Double, ParameterDirection.Input);
                        parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Update RateRequest Set RateCountered = ?, ModifiedBy = ?, Status = 'APPROVAL REQUIRED' Where RateID = '" + rateRequest.RateID + "'";

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
            string username = HttpContext.Current.User.Identity.Name;
            string status = Constant.ApprovalRequired;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();


                    parameters.Add("Status", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("RateID", rateID, DbType.String, ParameterDirection.Input);


                    string odbcQuery = "Update RateRequest Set Status = ?, ModifiedBy = ? Where RateID = ?";

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
            string username = HttpContext.Current.User.Identity.Name;
            string status = Constant.Issued;

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();


                    parameters.Add("BookingStatus", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("BookingID", BookingID, DbType.String, ParameterDirection.Input);


                    string odbcQuery = "Update BookingConfirmation Set BookingStatus = ?, ModifiedBy = ? Where BookingID = ?";

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
            string username = HttpContext.Current.User.Identity.Name;
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
                    parameters1.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameters1.Add("BookingID", BookingID, DbType.String, ParameterDirection.Input);

                    string Query = "Update BookingConfirmation Set BookingDate = ?, ModifiedBy = ? Where BookingID = ?";

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
            string username = HttpContext.Current.User.Identity.Name;
            string status = Constant.Draft;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();


                    parameters.Add("BookingStatus", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("BookingDate", null, DbType.Date, ParameterDirection.Input);
                    parameters.Add("AccMonth", string.Empty, DbType.String, ParameterDirection.Input);
                    parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("BookingID", BookingID, DbType.String, ParameterDirection.Input);


                    string odbcQuery = "Update BookingConfirmation Set BookingStatus = ?, BookingDate = ?, AccMonth = ?, ModifiedBy = ? Where BookingID = ?";

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
                    parameters.Add("Grossweight", booking.Grossweight, DbType.Double, ParameterDirection.Input);
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
                                ExRate = Math.Round(charge.ExRate,2),
                                AmountUSD = Math.Round(charge.AmountUSD,2),
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

                    query += " Order By q.usermodifydate DESC";

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
                        quoteRef.QuantityLifted = GetQuantityLiftedFromQuoteRef(QuoteRefID);
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

            catch (Exception ex)
            {

                throw ex;
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
                            bookref.Shipper = quotedetails.ShipperName3;

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

            catch (Exception ex)
            {

                throw ex;
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

            catch (Exception ex)
            {
                throw ex;
            }
            return ContainerNo;
        }

        public static string GetContainerStatusfromNo(string ContainerNo)
        {
            string StatusTrack = "";
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);

                    string query = "Select StatusTrack from Inventory where ContainerNo = ?";

                    StatusTrack = conn.QueryFirst<String>(query, parameters);

                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return StatusTrack;
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

            catch (Exception ex)
            {
                throw ex;
            }
            return rateType;
        }

        public static int GetQuantityLiftedFromQuoteRef(string IDQuoteRef)
        {
            int QuantityLifted;
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("IDQuoteRef", IDQuoteRef, DbType.String, ParameterDirection.Input);

                    string query = "Select COUNT(ContainerNo) AS QuantityLifted FROM AllocateEquipment WHERE IDQuoteRef = ?";

                    QuantityLifted = conn.QueryFirst<int>(query, parameters);

                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return QuantityLifted;
        }


        public static string GetJobRefFromUSN(string UniversalSerialNr)
        {
            string JobRef = string.Empty;
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select JobRef from BookingConfirmation where UniversalSerialNr = ?";

                    JobRef = conn.QueryFirstOrDefault<String>(query, parameters);

                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return JobRef;
        }
        
        public static int GetCreditTermFromAddress(string CompanyName)
        {
            int CreditTerm = 0;
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    if (CompanyName != null)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyName", CompanyName.ToUpperInvariant(), DbType.String, ParameterDirection.Input);

                        string query = "Select CreditTerm from Address where CompanyName = ?";

                        if(conn.QueryFirstOrDefault<int?>(query, parameters)!=null)
                        {
                            CreditTerm = conn.QueryFirstOrDefault<int>(query, parameters);
                        }
                        else
                        {
                            CreditTerm = 0;
                        }
                        
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
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

            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        public static bool CheckAllocationStatus(string UniversalSerialNr)
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "SELECT StatusTrack FROM Inventory WHERE (StatusTrack = 'booked') AND UniversalSerialNr = ? ";

                    var bookedData = conn.Query<ShipmentDetails>(query, parameters);

                    if (bookedData.Count() == 0)
                    {
                        query = "SELECT StatusTrack FROM Inventory WHERE (StatusTrack = 'allocated') AND UniversalSerialNr = ? ";

                        var allocateData = conn.Query<ShipmentDetails>(query, parameters);
                        if(allocateData.Count()>0)
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

            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static bool CheckShipmentfromBL(string UniversalSerialNr)
        {


            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select JobRef from BLDetail where UniversalSerialNr = ?";

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

            catch (Exception ex)
            {
                throw ex;
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

                    string querysd = "Select JobRef, HBLHAWB, ClosingDate, ChargeParty, ClosingTime, EquipmentType, PrintApproval, Remark, ShipmentTerm, Shipper, ShipperAddress, LoadPort, PlaceOfReceipt, DischPort, PlaceOfDelivery, Quantity, QuantityLifting, LDepotTerminal, DischAgentAddress, MBLMAWB, CustomerRef, UniversalSerialNr, IDQuoteRef from ShipmentDetailDry where JobRef = ?";

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
                        shipmentRef.ShipmentDetailsModel.QuantityLifting = item.QuantityLifting;
                        shipmentRef.ShipmentDetailsModel.LDepotTerminal = item.LDepotTerminal;
                        shipmentRef.ShipmentDetailsModel.ChargeParty = item.ChargeParty;
                        shipmentRef.ShipmentDetailsModel.Remark = item.Remark;
                        shipmentRef.ShipmentDetailsModel.PrintApproval = item.PrintApproval;
                        shipmentRef.ShipmentDetailsModel.EquipmentType = item.EquipmentType;
                        shipmentRef.ShipmentDetailsModel.AllocateStatus = CheckAllocationStatus(shipmentRef.ShipmentDetailsModel.UniversalSerialNr);
                        shipmentRef.ShipmentDetailsModel.ContainerList = GetContainerList(shipmentRef.ShipmentDetailsModel.LoadPort).Select(x => new SelectListItem { Text = x.ContainerNo, Value = x.ContainerNo });
                        if(String.IsNullOrEmpty(GetInvoiceFromUSN(shipmentRef.ShipmentDetailsModel.UniversalSerialNr).ProformaInvoiceNo))
                        {
                            shipmentRef.ShipmentDetailsModel.InvoiceSave = false;
                        }
                        else
                        {
                            shipmentRef.ShipmentDetailsModel.InvoiceSave = true;
                        }
                    }

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string querybl = "Select * from BLDetail where JobRef = ?";

                    var BLData = conn.Query<BLDetails>(querybl, parameterbl);
                    foreach (var item in BLData)
                    {
                        shipmentRef.BLDetailsModel.ShipperNameBL = item.ShipperNameBL;
                        shipmentRef.BLDetailsModel.ConsigneeNameBL = item.ConsigneeNameBL;
                        shipmentRef.BLDetailsModel.ShipperAddressBL = item.ShipperAddressBL;
                        shipmentRef.BLDetailsModel.ConsigneeAddressBL = item.ConsigneeAddressBL;
                        shipmentRef.BLDetailsModel.ShipperNameSI = item.ShipperNameSI;
                        shipmentRef.BLDetailsModel.ConsigneeNameSI = item.ConsigneeNameSI;
                        shipmentRef.BLDetailsModel.ShipperAddressSI = item.ShipperAddressSI;
                        shipmentRef.BLDetailsModel.ConsigneeAddressSI = item.ConsigneeAddressSI;
                        shipmentRef.BLDetailsModel.NotifyPartyName = item.NotifyPartyName;
                        shipmentRef.BLDetailsModel.DischAgentNameBL = item.DischAgentNameBL;
                        shipmentRef.BLDetailsModel.NotifyPartyAddress = item.NotifyPartyAddress;
                        shipmentRef.BLDetailsModel.DischAgentAddress = item.DischAgentAddress;
                        shipmentRef.BLDetailsModel.LoadPort = item.LoadPort;
                        shipmentRef.BLDetailsModel.LoadPortAlias = item.LoadPortAlias;
                        shipmentRef.BLDetailsModel.PlaceofReceipt = item.PlaceofReceipt;
                        shipmentRef.BLDetailsModel.DischPort = item.DischPort;
                        shipmentRef.BLDetailsModel.DischPortAlias = item.DischPortAlias;
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
                        shipmentRef.BLDetailsModel.BLSeawayStatus = item.BLSeawayStatus;
                        shipmentRef.BLDetailsModel.PreCarriageBy = item.PreCarriageBy;
                        shipmentRef.BLDetailsModel.NoOfPkgs = item.NoOfPkgs;
                        shipmentRef.BLDetailsModel.PortAlias = item.PortAlias;
                        shipmentRef.BLDetailsModel.PkgType = item.PkgType;
                        shipmentRef.BLDetailsModel.TotalGweight = item.TotalGweight;
                        shipmentRef.BLDetailsModel.GrossWeightUnit = item.GrossWeightUnit;
                        shipmentRef.BLDetailsModel.NetWtUnit = item.NetWtUnit;
                        shipmentRef.BLDetailsModel.MeasurementUnit = item.MeasurementUnit;
                        shipmentRef.BLDetailsModel.TotalContainerMeasurement = item.TotalContainerMeasurement;
                    }

                    shipmentRef.TransVessel = GetTransVessel(shipmentRef);

                    var parameterQuote = new DynamicParameters();
                    parameterQuote.Add("IDQuoteRef", shipmentRef.ShipmentDetailsModel.IDQuoteRef, DbType.String, ParameterDirection.Input);

                    string queryQuote = "Select TransshipmentType from QuoteReference where QuoteRefID = ?";

                    var QuoteData = conn.Query<QuoteRef>(queryQuote, parameterQuote);
                    foreach (var item in QuoteData)
                    {
                        shipmentRef.QuoteRefModel.TransshipmentType = item.TransshipmentType;
                    }
                }
            }

            catch (Exception ex)
            {

                throw ex;
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

                    string query = "Select UniversalSerialNr, InvoiceNo, ProformaInvoiceNo, InvoiceType, AmountinUSDSUM, AmountinUSDSUMWTax, Amountinwords, TaxAmountSum, InvoiceDate, LoadPort, DischargePort, ETD, ETA , InvoiceMonth, CreditTerms, DueDate, BillingParty, BillingPartyAddress, Grossweight, GrossweightUnit, ContainerNo, Remarks, JobRefNo, VesselName, VoyageNo from SalesInvoiceDry where UniversalSerialNr = ? AND InvoiceType = 'EXPORT'";

                    var InvoiceData = conn.Query<Invoice>(query, parameters);
                    foreach (var item in InvoiceData)
                    {
                        invoiceRef.InvoiceNo = item.InvoiceNo;
                        invoiceRef.ProformaInvoiceNo = item.ProformaInvoiceNo;
                        invoiceRef.InvoiceType = item.InvoiceType;
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
                        invoiceRef.AmountinUSDSUM = Math.Round(item.AmountinUSDSUM,2);
                        invoiceRef.AmountinUSDSUMWTax = item.AmountinUSDSUMWTax;
                        invoiceRef.TaxAmountSum = item.TaxAmountSum;
                        invoiceRef.Amountinwords = item.Amountinwords;
                        invoiceRef.VesselName = item.VesselName;
                        invoiceRef.VoyageNo = item.VoyageNo;
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
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
            string AccMonth = DateTime.Today.ToString("MMMyy");
            invoice.ProformaInvoiceNo = GenerateInvoiceNumber(RateType, InvoiceType);
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("ProformaInvoiceNo", invoice.ProformaInvoiceNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("AcctMonth", AccMonth, DbType.String, ParameterDirection.Input);
                    parameters.Add("InvoiceMonth", DateTime.Today.Month, DbType.Int16, ParameterDirection.Input);
                    //parameters.Add("DueDate", invoice.DueDate, DbType.Date, ParameterDirection.Input);
                    parameters.Add("BillingParty", invoice.BillingParty, DbType.String, ParameterDirection.Input);
                    //parameters.Add("BillingPartyAddress", invoice.BillingPartyAddress, DbType.String, ParameterDirection.Input);
                    parameters.Add("Grossweight", invoice.Grossweight, DbType.Double, ParameterDirection.Input);
                    parameters.Add("GrossweightUnit", invoice.GrossweightUnit, DbType.String, ParameterDirection.Input);
                    parameters.Add("Remarks", invoice.Remarks, DbType.String, ParameterDirection.Input);
                    parameters.Add("UniversalSerialNr", invoice.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parameters.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("InvoiceType", InvoiceType, DbType.String, ParameterDirection.Input);
                    parameters.Add("JobRefNo", invoice.JobRefNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("CreditTerms", invoice.CreditTerms, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("Status", Constant.Issued, DbType.String, ParameterDirection.Input);
                    parameters.Add("VesselName", invoice.VesselName, DbType.String, ParameterDirection.Input);
                    parameters.Add("VoyageNo", invoice.VoyageNo, DbType.String, ParameterDirection.Input);



                    string odbcQuery = "Insert Into SalesInvoiceDRY(ProformaInvoiceNo, AcctMonth, InvoiceMonth, BillingParty, Grossweight, " +
                        "GrossweightUnit, Remarks, UniversalSerialNr, CreatedBy, InvoiceType, JobRefNo, CreditTerms, Status, VesselName, VoyageNo) Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);
                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                    string query = "SELECT ID FROM SalesInvoiceDRY WHERE ProformaInvoiceNo = '" + invoice.ProformaInvoiceNo+ "'";

                    var invoicereference = conn.Query<Invoice>(query);
                    foreach (var item in invoicereference)
                    {
                        invoice.ID = item.ID;
                    }

                    SaveInvoiceCharges(invoice);

                }


                errorLog.ErrorMessage = invoice.ProformaInvoiceNo;
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
                    parameterbl.Add("LoadPortAlias", shipmentRef.BLDetailsModel.LoadPortAlias, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PlaceofReceipt", shipmentRef.ShipmentDetailsModel.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischPort", shipmentRef.ShipmentDetailsModel.DischPort, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischPortAlias", shipmentRef.BLDetailsModel.DischPortAlias, DbType.String, ParameterDirection.Input);
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
                    parameterbl.Add("PortAlias", shipmentRef.BLDetailsModel.PortAlias, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PkgType", shipmentRef.BLDetailsModel.PkgType, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PreCarriageBy", shipmentRef.BLDetailsModel.PreCarriageBy, DbType.String, ParameterDirection.Input);
                    //parameterbl.Add("NetWtUnit", shipmentRef.BLDetailsModel.NetWtUnit, DbType.String, ParameterDirection.Input);
                    //parameterbl.Add("MeasurementUnit", shipmentRef.BLDetailsModel.MeasurementUnit, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("JobRefFull", shipmentRef.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("JobRef", shipmentRef.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);

                    string odbcQuerybl = "";
                    if (CheckShipmentfromBL(shipmentRef.ShipmentDetailsModel.UniversalSerialNr))
                    {
                        odbcQuerybl = "Update BLDetail  SET ShipperNameBL = ?, ConsigneeNameBL = ?, ShipperAddressBL = ?, ConsigneeAddressBL = ?, NotifyPartyName = ?, DischAgentNameBL = ?, NotifyPartyAddress = ?, DischAgentAddress = ?, " +
                        "LoadPort = ?, LoadPortAlias = ?, PlaceofReceipt = ?, DischPort = ?, DischPortAlias = ?, PlaceofDelivery = ?, MarksandNo = ?, CargoDescription = ?, LadenOnBoard = ?, HBLFreightPayment = ?, BLFinalisedDate = ?, DateofIssue = ?, MBLFreightPayment = ?, " +
                        "NoofOriginalBLissued = ?, NoOfPkgs = ?, PortAlias = ?, PkgType = ?, PreCarriageBy = ?, UniversalSerialNr = ?, JobRefFull = ?, ModifiedBy = ? WHERE JobRef = ? ";
                    }
                    else
                    {
                        odbcQuerybl = "Insert Into BLDetail (ShipperNameBL, ConsigneeNameBL, ShipperAddressBL, ConsigneeAddressBL, NotifyPartyName, DischAgentNameBL, NotifyPartyAddress, DischAgentAddress, " +
                        "LoadPort, LoadPortAlias, PlaceofReceipt, DischPort, DischPortAlias, PlaceofDelivery, MarksandNo, CargoDescription, LadenOnBoard, HBLFreightPayment, BLFinalisedDate, DateofIssue, MBLFreightPayment, " +
                        "NoofOriginalBLissued, NoOfPkgs, PortAlias, PkgType, PreCarriageBy, UniversalSerialNr, JobRefFull, CreatedBy, JobRef) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                    }
                    
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
                    parameterbl.Add("LoadPortAlias", shipmentRef.BLDetailsModel.LoadPortAlias, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PlaceofReceipt", shipmentRef.ShipmentDetailsModel.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischPort", shipmentRef.ShipmentDetailsModel.DischPort, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("DischPortAlias", shipmentRef.BLDetailsModel.DischPortAlias, DbType.String, ParameterDirection.Input);
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
                    parameterbl.Add("PortAlias", shipmentRef.BLDetailsModel.PortAlias, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PkgType", shipmentRef.BLDetailsModel.PkgType, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PreCarriageBy", shipmentRef.BLDetailsModel.PreCarriageBy, DbType.String, ParameterDirection.Input);
                    //parameterbl.Add("NetWtUnit", shipmentRef.BLDetailsModel.NetWtUnit, DbType.String, ParameterDirection.Input);
                    //parameterbl.Add("MeasurementUnit", shipmentRef.BLDetailsModel.MeasurementUnit, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("JobRef", shipmentRef.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);

                    string odbcQuerybl = "Update BLDetail  SET ShipperNameBL = ?, ConsigneeNameBL = ?, ShipperAddressBL = ?, ConsigneeAddressBL = ?, NotifyPartyName = ?, DischAgentNameBL = ?, NotifyPartyAddress = ?, DischAgentAddress = ?, " +
                        "LoadPort = ?, LoadPortAlias = ?, PlaceofReceipt = ?, DischPort = ?, DischPortAlias = ?, PlaceofDelivery = ?, MarksandNo = ?, CargoDescription = ?, LadenOnBoard = ?, HBLFreightPayment = ?, BLFinalisedDate = ?, DateofIssue = ?, MBLFreightPayment = ?, " +
                        "NoofOriginalBLissued = ?, NoOfPkgs = ?, PortAlias = ?, PkgType = ?, PreCarriageBy = ?, ModifiedBy = ? WHERE JobRef = ? ";

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
                        parameterc.Add("UnitRate", item.UnitRate, DbType.Double, ParameterDirection.Input);
                        parameterc.Add("Currency", item.Currency, DbType.String, ParameterDirection.Input);
                        parameterc.Add("Exrate", item.Exrate, DbType.Double, ParameterDirection.Input);
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
                        parametersc.Add("UnitRate", item.UnitRate, DbType.Double, ParameterDirection.Input);
                        parametersc.Add("Currency", item.Currency, DbType.String, ParameterDirection.Input);
                        parametersc.Add("Exrate", item.Exrate, DbType.Double, ParameterDirection.Input);
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
        
        public static ErrorLog SendCostLICharges(List<string> SelectedExportLocalCharges, List<string> SelectedImportLocalCharges, string RateID)
        {
            string username = HttpContext.Current.User.Identity.Name;
            
            try
            {

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    foreach(var item in SelectedExportLocalCharges)
                    {
                        DQuoteSales costcharges = new DQuoteSales();
                        var parameters = new DynamicParameters();
                        parameters.Add("ID", item, DbType.String, ParameterDirection.Input);
                        string Query = "Select ChargeDescription AS Description, Cost AS UnitRate, Currency, ID FROM AgencyLocalCharges WHERE ID = ?";
                        var chargedetails = conn.Query<DQuoteSales>(Query, parameters);
                        foreach(var itemdetails in chargedetails)
                        {
                            costcharges.Description = itemdetails.Description;
                            costcharges.UnitRate = itemdetails.UnitRate;
                            costcharges.Currency = itemdetails.Currency;
                            costcharges.ID = itemdetails.ID;
                            costcharges.PayBy = "SH";
                            costcharges.PaymentTerm = "PREPAID";
                        }

                        string query = "Select Rate FROM ExchangeRate WHERE currency = '" + costcharges.Currency + "' AND status = 'current'";

                        costcharges.Exrate = conn.QueryFirst<Double>(query);

                        var parameterc = new DynamicParameters();
                        parameterc.Add("Description", costcharges.Description, DbType.String, ParameterDirection.Input);
                        parameterc.Add("UnitRate", costcharges.UnitRate, DbType.Double, ParameterDirection.Input);
                        parameterc.Add("Currency", costcharges.Currency, DbType.String, ParameterDirection.Input);
                        parameterc.Add("CostTypeID", costcharges.ID, DbType.String, ParameterDirection.Input);
                        parameterc.Add("Exrate", costcharges.Exrate, DbType.Double, ParameterDirection.Input);
                        parameterc.Add("PayBy", costcharges.PayBy, DbType.String, ParameterDirection.Input);
                        parameterc.Add("PaymentTerm", costcharges.PaymentTerm, DbType.String, ParameterDirection.Input);
                        parameterc.Add("RateID", RateID, DbType.String, ParameterDirection.Input);
                        parameterc.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);

                        string costliQuery = "Insert Into DQuoteCostLI (Description, UnitRate, Currency, CostTypeID, Exrate, PayBy, PaymentTerm, RateID, CreatedBy) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        int rowsAffected = conn.Execute(costliQuery, parameterc);
                        if (rowsAffected > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }
                    }

                    foreach (var item in SelectedImportLocalCharges)
                    {
                        DQuoteSales costcharges = new DQuoteSales();
                        var parameters = new DynamicParameters();
                        parameters.Add("ID", item, DbType.String, ParameterDirection.Input);
                        string Query = "Select ChargeDescription AS Description, Cost AS UnitRate, Currency, ID FROM AgencyLocalCharges WHERE ID = ?";
                        var chargedetails = conn.Query<DQuoteSales>(Query, parameters);
                        foreach (var itemdetails in chargedetails)
                        {
                            costcharges.Description = itemdetails.Description;
                            costcharges.UnitRate = itemdetails.UnitRate;
                            costcharges.Currency = itemdetails.Currency;
                            costcharges.ID = itemdetails.ID;
                            costcharges.PayBy = "CN";
                            costcharges.PaymentTerm = "COLLECT";
                        }

                        string query = "Select Rate FROM ExchangeRate WHERE currency = '" + costcharges.Currency + "' AND status = 'current'";

                        costcharges.Exrate = conn.QueryFirst<Double>(query);

                        var parameterc = new DynamicParameters();
                        parameterc.Add("Description", costcharges.Description, DbType.String, ParameterDirection.Input);
                        parameterc.Add("UnitRate", costcharges.UnitRate, DbType.Double, ParameterDirection.Input);
                        parameterc.Add("Currency", costcharges.Currency, DbType.String, ParameterDirection.Input);
                        parameterc.Add("CostTypeID", costcharges.ID, DbType.String, ParameterDirection.Input);
                        parameterc.Add("Exrate", costcharges.Exrate, DbType.Double, ParameterDirection.Input);
                        parameterc.Add("PayBy", costcharges.PayBy, DbType.String, ParameterDirection.Input);
                        parameterc.Add("PaymentTerm", costcharges.PaymentTerm, DbType.String, ParameterDirection.Input);
                        parameterc.Add("RateID", RateID, DbType.String, ParameterDirection.Input);
                        parameterc.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);

                        string costliQuery = "Insert Into DQuoteCostLI (Description, UnitRate, Currency, CostTypeID, Exrate, PayBy, PaymentTerm, RateID, CreatedBy) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        int rowsAffected = conn.Execute(costliQuery, parameterc);
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
                        parameters.Add("UnitRate", item.UnitRate, DbType.Double, ParameterDirection.Input);
                        parameters.Add("Currency", item.Currency, DbType.String, ParameterDirection.Input);
                        parameters.Add("ExRate", item.ExRate, DbType.Double, ParameterDirection.Input);
                        parameters.Add("Quantity", item.Quantity, DbType.Int16, ParameterDirection.Input);
                        parameters.Add("PaymentTerm", item.PaymentTerm, DbType.String, ParameterDirection.Input);
                        parameters.Add("UniversalSerialNr", invoice.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                        parameters.Add("CompanyName", invoice.BillingParty, DbType.String, ParameterDirection.Input);
                        parameters.Add("ProformaInvoiceNo", invoice.ProformaInvoiceNo, DbType.String, ParameterDirection.Input);
                        parameters.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);
                        parameters.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                        parameters.Add("IDInvoiceNr", invoice.ID, DbType.String, ParameterDirection.Input);

                        salesQuery = "Insert Into SalesInvoiceLineItemDRY (Description, UnitRate, Currency, Exrate, Quantity, PaymentTerm, UniversalSerialNr, CompanyName, ProformaInvoiceNo, ContainerNo, CreatedBy, IDInvoiceNr) " +
                        "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

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
        public static List<AllocateEquipment> GetAllocateEquipmentDetails(string UniversalSerialNr)
        {
            List<AllocateEquipment> allocateEquipment = new List<AllocateEquipment>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select ID, ContainerNo, SealNo, GrossWeight, GrossWeightUnit, NettWeight, NetWeightUnit, Measurement, MeasurementUnit, UniversalSerialNr" +
                        " From AllocateEquipment where UniversalSerialNr = ?";

                    var allocateEquipmentData = conn.Query<AllocateEquipment>(query, parameters);
                    foreach (var item in allocateEquipmentData)
                    {
                        allocateEquipment.Add(new AllocateEquipment
                        {
                            ID = item.ID,
                            ContainerNo = item.ContainerNo,
                            SealNo = item.SealNo,
                            GrossWeight = item.GrossWeight,
                            GrossWeightUnit = item.GrossWeightUnit,
                            NettWeight = item.NettWeight,
                            NetWeightUnit = item.NetWeightUnit,
                            Measurement = item.Measurement,
                            MeasurementUnit = item.MeasurementUnit,
                            UniversalSerialNr = item.UniversalSerialNr
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return allocateEquipment;
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

                    string query = "Select ContainerNo From AllocateEquipment where UniversalSerialNr = ?";

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
                        string StatusTrack = GetContainerStatusfromNo(item);
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
                            string invQuery1 = "";
                            if (StatusTrack == "new")
                            {

                                parameters1.Add("ActiveLPort", shipment.LoadPort, DbType.String, ParameterDirection.Input);
                                parameters1.Add("ActiveDPort", shipment.DischPort, DbType.String, ParameterDirection.Input);
                                parameters1.Add("ActiveLDepotTerminal", shipment.LDepotTerminal, DbType.String, ParameterDirection.Input);
                                parameters1.Add("ContainerNo", item, DbType.String, ParameterDirection.Input);

                                invQuery1 = "UPDATE Inventory SET ActiveLPort = ?, ActiveDPort = ?, ActiveLDepotTerminal = ?, ActiveDDepotTerminal = null, ActiveLDate = ActiveDDate, " +
                                    "ActiveDDate = null, ERequestDate = null, ERequestLog = null, ERequester = null, PreUniversalSerialNr[2] = PreUniversalSerialNr[1] WHERE ContainerNo = ? ";

                            }

                            else
                            {
                                parameters1.Add("ActiveDPort", shipment.DischPort, DbType.String, ParameterDirection.Input);
                                parameters1.Add("ActiveLDepotTerminal", shipment.LDepotTerminal, DbType.String, ParameterDirection.Input);
                                parameters1.Add("ContainerNo", item, DbType.String, ParameterDirection.Input);

                                invQuery1 = "UPDATE Inventory SET ActiveLPort = ActiveDPort, ActiveDPort = ?, ActiveLDepotTerminal = ActiveDDepotTerminal, ActiveDDepotTerminal = ?, " +
                                    "ActiveDDate = null, ERequestDate = null, ERequestLog = null, ERequester = null, PreUniversalSerialNr[2] = PreUniversalSerialNr[1] WHERE ContainerNo = ? ";

                            }
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

                            string invQuery3 = "UPDATE Inventory SET UniversalSerialNr = '" + shipment.UniversalSerialNr + "', UserModify = '" + username + "' WHERE ContainerNo = '" + item + "' ";

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

                        if (StatusTrack != "new")
                        {
                            var parametermr = new DynamicParameters();
                            parametermr.Add("ContainerNo", item, DbType.String, ParameterDirection.Input);
                            string querymr = "Select DGateInDate FROM MovementRecord WHERE ContainerNo = ? ORDER BY CreateDate DESC FETCH FIRST 1 ROWS ONLY";

                            MovementRecord movement = new MovementRecord();

                            var movementData = conn.Query<MovementRecord>(querymr, parametermr);
                            foreach (var movementitem in movementData)
                            { 
                                movement.LGateInDate = movementitem.DGateInDate;
                            }

                            var parametersm = new DynamicParameters();
                            parametersm.Add("UniversalSerialNr", shipment.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                            parametersm.Add("LoadPort", shipment.LoadPort, DbType.String, ParameterDirection.Input);
                            parametersm.Add("LGateInDate", movement.LGateInDate, DbType.Date, ParameterDirection.Input);
                            parametersm.Add("DischargePort", shipment.DischPort, DbType.String, ParameterDirection.Input);
                            parametersm.Add("LDepotTerminal", shipment.LDepotTerminal, DbType.String, ParameterDirection.Input);
                            parametersm.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                            parametersm.Add("ContainerNo", item, DbType.String, ParameterDirection.Input);

                            string movQuery = "INSERT Into MovementRecord (UniversalSerialNr, LoadPort, LGateInDate, DischargePort, LDepotTerminal, CreatedBy, ContainerNo) " +
                                "Values(?, ?, ?, ?, ?, ?, ?)";

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
                        else
                        {
                            var parametersm = new DynamicParameters();
                            parametersm.Add("UniversalSerialNr", shipment.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                            parametersm.Add("LoadPort", shipment.LoadPort, DbType.String, ParameterDirection.Input);
                            //parametersm.Add("LGateInDate", movement.LGateInDate, DbType.String, ParameterDirection.Input);
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
            }
            catch (Exception ex)
            {
                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message;
            }
            return errorLog;
        }
        public static ErrorLog UpdateAllocateEquipment(AllocateEquipment allocateEquipment)
        {
            string username = HttpContext.Current.User.Identity.Name;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("SealNo", allocateEquipment.SealNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("GrossWeight", allocateEquipment.GrossWeight, DbType.Double, ParameterDirection.Input);
                    parameters.Add("GrossWeightUnit", allocateEquipment.GrossWeightUnit, DbType.String, ParameterDirection.Input);
                    parameters.Add("NettWeight", allocateEquipment.NettWeight, DbType.Double, ParameterDirection.Input);
                    parameters.Add("NetWeightUnit", allocateEquipment.NetWeightUnit, DbType.String, ParameterDirection.Input);
                    parameters.Add("Measurement", allocateEquipment.Measurement, DbType.Double, ParameterDirection.Input);
                    parameters.Add("MeasurementUnit", allocateEquipment.MeasurementUnit, DbType.String, ParameterDirection.Input);
                    parameters.Add("UniversalSerialNr", allocateEquipment.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parameters.Add("ModifyUser", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("ID", allocateEquipment.ID, DbType.String, ParameterDirection.Input);

                    string odbcQuery = "Update AllocateEquipment Set SealNo = ?, GrossWeight = ?,  GrossWeightUnit = ?, " +
                           "NettWeight = ?, NetWeightUnit = ?, Measurement = ?, MeasurementUnit = ?, UniversalSerialNr = ?, ModifyUser = ? " +
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

        public static ErrorLog DeleteAllocateEquipment(string ID, string UniversalSerialNr)
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
                        parameters1.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);

                        string invQuery1 = "UPDATE Inventory SET ActiveDPort = null, ActiveLDepotTerminal = null, ActiveDDepotTerminal = null, " +
                            "ActiveDDate = null, ERequestDate = null, ERequestLog = null, ERequester = null, UniversalSerialNr = PreUniversalSerialNr[1] WHERE ContainerNo = ? ";

                        int rowsAffectedq1 = conn.Execute(invQuery1, parameters1);
                        if (rowsAffectedq1 > 0)
                        {
                            errorLog.IsError = false;
                        }
                        else
                        {
                            errorLog.IsError = true;
                        }

                        string invQuery2 = "UPDATE Inventory SET PreUniversalSerialNr[1] = PreUniversalSerialNr[2], UserModify = '" + username + "'WHERE ContainerNo = '" + ContainerNo + "' ";

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

                    var parametersd = new DynamicParameters();
                    parametersd.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);

                    string queryd = "Select UniversalSerialNr from Inventory where ContainerNo = ?";

                    UniversalSerialNr = conn.QueryFirstOrDefault<String>(queryd, parametersd);

                    if (UniversalSerialNr != null)
                    {
                        var parametermr = new DynamicParameters();
                        parametermr.Add("UniversalSerialNr", UniversalSerialNr, DbType.String, ParameterDirection.Input);
                        parametermr.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);
                        string querymr = "Select DischargePort, LoadPort, LDepotTerminal, DDepotTerminal, DGateInDate, LGateOutDate FROM MovementRecord WHERE UniversalSerialNr = ? AND ContainerNo = ?";

                        MovementRecord movement = new MovementRecord();

                        var movementData = conn.Query<MovementRecord>(querymr, parametermr);
                        foreach (var item in movementData)
                        {

                            movement.DischargePort = item.DischargePort;
                            movement.LoadPort = item.LoadPort;
                            movement.LDepotTerminal = item.LDepotTerminal;
                            movement.DDepotTerminal = item.DDepotTerminal;
                            movement.DGateInDate = item.DGateInDate;
                            movement.LGateOutDate = item.LGateOutDate;
                        }

                        var parameter = new DynamicParameters();
                        parameter.Add("ActiveDPort", movement.DischargePort, DbType.String, ParameterDirection.Input);
                        parameter.Add("ActiveLPort", movement.LoadPort, DbType.String, ParameterDirection.Input);
                        parameter.Add("ActiveLDepotTerminal", movement.LDepotTerminal, DbType.String, ParameterDirection.Input);
                        parameter.Add("ActiveDDepotTerminal", movement.DDepotTerminal, DbType.String, ParameterDirection.Input);
                        parameter.Add("ActiveDDate", movement.DGateInDate, DbType.Date, ParameterDirection.Input);
                        parameter.Add("ActiveLDate", movement.LGateOutDate, DbType.Date, ParameterDirection.Input);
                        parameter.Add("UserModify", username, DbType.String, ParameterDirection.Input);
                        parameter.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);

                        string invQuery1 = "UPDATE Inventory SET ActiveDPort = ?, ActiveLPort = ?, ActiveLDepotTerminal = ?, ActiveDDepotTerminal = ?, ActiveDDate = ?, " +
                            "ActiveLDate = ?, UserModify = ? WHERE ContainerNo = ? ";

                        int rowsAffectedq1 = conn.Execute(invQuery1, parameter);
                        if (rowsAffectedq1 > 0)
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
        public static List<Booking> GetBookingDetails(SearchParameters search)
        {
            string IDCompany = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
            List<Booking> bookings = new List<Booking>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select BC.BookingID, BC.BookingNo, BC.QuoteRefID, BC.CompanyName, BC.DischargePort, BC.LoadPort, BC.UniversalSerialNr, BC.BookingStatus" +
                        " From BookingConfirmation BC INNER JOIN AgentAddressBook AB ON UPPER(BC.CompanyName) = UPPER(AB.CompanyName) WHERE AB.IDCompany = '" + IDCompany + "' AND";

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

                    query += " Order By BC.ModificationTimestamp DESC";

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
            catch (Exception ex)
            {

                throw ex;
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
                    string query = "Select SD.JobRef, BC.BookingNo, SD.ChargeParty, SD.IDQuoteRef, SD.DischPort, SD.LoadPort, SD.UniversalSerialNr, BL.BLTypes" +
                        " From ShipmentDetailDRY SD INNER JOIN BookingConfirmation BC ON SD.UniversalSerialNr = BC.UniversalSerialNr " +
                        "INNER JOIN BLDetail BL ON SD.UniversalSerialNr = BL.UniversalSerialNr " +
                        "INNER JOIN AgentAddressBook AB ON UPPER(BC.CompanyName) = UPPER(AB.CompanyName) AND AB.IDCompany = '" + IDCompany + "' ";

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
                            query += " BL.BLTypes = '" + search.Status + "' AND";
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

                    query += " Order By SD.ModifyDate DESC";

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
                            BLTypes = item.BLTypes
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
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
                        " From SalesInvoiceDRY ";//INNER JOIN AgentAddressBook AB ON UPPER(BC.CompanyName) = UPPER(AB.CompanyName) "; +
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

                    query += " Order By ModificationTimestamp DESC";

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
            catch (Exception ex)
            {

                throw ex;
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
                    
                    if (BLTypes == "SEAWAY")
                    {
                        odbcQuerybl += ", BLSeawayStatus = 'SEAWAY ISSUED'";
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
        
        public static ErrorLog RePrintRequest(string JobRef)
        {
            string username = HttpContext.Current.User.Identity.Name;

            //string agentCostLi = ConvertToString(booking.SelectedExportLocalCharges) + "," + ConvertToString(booking.SelectedImportLocalCharges);

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("UserModify", username, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string odbcQuerybl = "Update ShipmentDetailDRY SET PrintApproval = 'Approval Requested', UserModify = ? WHERE JobRef = ? ";


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

        public static ErrorLog SaveShippingInstruction(BLDetails bLDetails)
        {
            string username = HttpContext.Current.User.Identity.Name;
            string JobRef = GetJobRefFromUSN(bLDetails.UniversalSerialNr);

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    if (CheckShipmentfromBL(bLDetails.UniversalSerialNr))
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ShipperNameSI", bLDetails.ShipperNameSI, DbType.String, ParameterDirection.Input);
                        parameters.Add("ShipperAddressSI", bLDetails.ShipperAddressSI, DbType.String, ParameterDirection.Input);
                        parameters.Add("ConsigneeNameSI", bLDetails.ConsigneeNameSI, DbType.String, ParameterDirection.Input);
                        parameters.Add("ConsigneeAddressSI", bLDetails.ConsigneeAddressSI, DbType.String, ParameterDirection.Input);
                        parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                        parameters.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Update BLDetail SET ShipperNameSI = ?, ShipperAddressSI = ?, ConsigneeNameSI = ?, ConsigneeAddressSI = ?, ModifiedBy = ? WHERE JobRef = ?";

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

                    else
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ShipperNameSI", bLDetails.ShipperNameSI, DbType.String, ParameterDirection.Input);
                        parameters.Add("ShipperAddressSI", bLDetails.ShipperAddressSI, DbType.String, ParameterDirection.Input);
                        parameters.Add("ConsigneeNameSI", bLDetails.ConsigneeNameSI, DbType.String, ParameterDirection.Input);
                        parameters.Add("ConsigneeAddressSI", bLDetails.ConsigneeAddressSI, DbType.String, ParameterDirection.Input);
                        parameters.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);
                        parameters.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                        parameters.Add("UniversalSerialNr", bLDetails.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Insert INTO BLDetail(ShipperNameSI, ShipperAddressSI, ConsigneeNameSI, ConsigneeAddressSI, JobRef, CreatedBy, UniversalSerialNr) Values(?, ?, ?, ?, ?, ?, ?)";

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
        public static ErrorLog DeleteVessel(string vesselID)
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    //parameters.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);
                    parameters.Add("ID", vesselID, DbType.String, ParameterDirection.Input);
                    string query = "DELETE FROM VesselScheduleDry Where ID = ?";
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
                    parameters.Add("ActiveDPort", loadPort, DbType.String, ParameterDirection.Input);

                    string query = "SELECT ContainerNo FROM Inventory WHERE(ActiveLPort = ?) AND (StatusTrack = 'new') OR (StatusTrack IN('available', 'post available')) AND (ActiveDPort = ?)";

                    var inventoryList = conn.Query<Inventory>(query, parameters);
                    foreach (var item in inventoryList)
                    {
                        inventories.Add(new Inventory
                        {
                            //UniversalSerialNr = item.UniversalSerialNr,
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
        
        public static List<Vessel> GetTransVessel(ShipmentBL shipmentRef)
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameterv = new DynamicParameters();
                    parameterv.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string queryv = "Select Carrier,ETD, ETA,VesselName,VoyNo, LoadPort,  DischPort from VesselScheduleDry where UniversalSerialNr = ?";

                    var VesselData = conn.Query<Vessel>(queryv, parameterv);

                    foreach (var item in VesselData)
                    {
                        shipmentRef.TransVessel.Add(new Vessel
                        {
                            Carrier = item.Carrier,
                            ETD = item.ETD,
                            ETA = item.ETA,
                            VesselName = item.VesselName,
                            VoyNo = item.VoyNo,
                            LoadPort = item.LoadPort,
                            DischPort = item.DischPort
                        });
                    }
                    
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return shipmentRef.TransVessel;
        }

        public static string GenerateInvoiceNumber(string RateType, string InvoiceType)
        {
            string ProformaInvoiceNo = string.Empty;
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

                string ProformaInvPrefix = string.Format("{0}{1}{2}{3}{4}", Constant.ProformaInitial, rateTypeInitial, invoiceTypeInitial, year, month);

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "SELECT Max(ProformaInvoiceNo) AS ProformaInvoiceNo FROM SalesInvoiceDRY where ProformaInvPrefix = ?";
                    var parameters = new DynamicParameters();
                    parameters.Add("ProformaInvPrefix", ProformaInvPrefix, DbType.String, ParameterDirection.Input);

                    ProformaInvoiceNo = conn.QueryFirst<string>(query, parameters);
                    if (string.IsNullOrEmpty(ProformaInvoiceNo))
                    {
                        counter += 1;
                        string counterNumber = counter.ToString("D4");
                        ProformaInvoiceNo = string.Format("{0}{1}", ProformaInvPrefix, counterNumber);
                    }
                    else
                    {
                        counter = Convert.ToInt32(ProformaInvoiceNo.Substring(10));
                        counter += 1;
                        string counterNumber = counter.ToString("D4");
                        ProformaInvoiceNo = string.Format("{0}{1}", ProformaInvPrefix, counterNumber);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ProformaInvoiceNo;
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

        public static List<string> ConvertToList(string agentCostLi)
        {
            
            List<string> selectedChargeList = new List<string>();
            if (!string.IsNullOrEmpty(agentCostLi))
            {
                selectedChargeList = agentCostLi.Split(',').ToList();
                
                selectedChargeList.RemoveAll(s => string.IsNullOrEmpty(s));
                
            }

            

            return selectedChargeList;
        }

        //Added by Shrutika
        public static List<SelectListItem> GetDischargePlanStatus()
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
                }
            };

            return stat;
        }

        public static List<DischargePlan> GetDischargePlanList(SearchParameters search)
        {
            string IDCompany = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
            List<DischargePlan> discharge = new List<DischargePlan>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    //string query = "Select SD.ClosingDate, SD.Country, SD.Quantity, SD.QuantityLifting, SD.QuoteType, SD.\"User\", SD.UniversalSerialNr, SD.ModifyDate, SD.PortPair, SD.PlaceOfReceipt, SD.PlaceOfDelivery, SD.LoadPort, SD.LoadFT, SD.JobRef, SD.LoadAgentAddress, SD.ETA, SD.ETD, SD.IDAgentDisch, SD.IDAgentLoad, SD.LoadAgent, BL.DischAgentNameBL, BL.ShipperNameBL, SD.CreateDate, SD.DischAgentAddress, SD.DischFT, SD.DischPort" +
                    //    " from ShipmentDetailDry SD INNER JOIN BLDetail BL ON SD.UniversalSerialNr = BL.UniversalSerialNr ";
                    string query = "Select DP.JobRef, DP.ChargeParty, DP.IDNo, DP.LoadPort, DP.DischPort, DP.Quantity, DP.DischargePlanStatus, " +
                        "DP.ETD, DP.ETA, DP.ATA, DP.\"User\" from DischargePlan DP INNER JOIN AgentAddressBook AB ON UPPER(DP.ChargeParty) = UPPER(AB.CompanyName) " +
                        "AND AB.IDCompany = '" + IDCompany + "' AND";

                    if ((search.Status != null || search.JobRef != null || (search.CompanyName!=null) || search.DischargePort != null) || (search.LoadPort != null))
                    {
                        if (search.Status != null)
                        {
                            query += " DP.DischargePlanStatus = '" + search.Status + "' AND";
                        }

                        if (search.JobRef != null)
                        {
                            query += " DP.JobRef = '" + search.JobRef + "' AND";
                        }

                        if (search.CompanyName != null)
                        {
                            query += " DP.ChargeParty = '" + search.CompanyName + "' AND";
                        }

                        if (search.DischargePort != null)
                        {
                            query += " DP.DischPort = '" + search.DischargePort + "' AND";
                        }

                        if (search.LoadPort != null)
                        {
                            query += " DP.LoadPort = '" + search.LoadPort + "' AND";
                        }
                    }

                    if (query.LastIndexOf("AND") > 0)
                    {
                        query = query.Remove(query.LastIndexOf("AND"));
                    }

                    query += "Order By DP.ModificationTimestamp DESC";
                    var dischargeData = conn.Query<DischargePlan>(query);
                    foreach (var item in dischargeData)
                    {
                        discharge.Add(new DischargePlan
                        {
                            IDNo = item.IDNo,
                            ChargeParty = item.ChargeParty,
                            DischPort = item.DischPort,
                            DischargePlanStatus = item.DischargePlanStatus,
                            ETA = item.ETA,
                            ATA = item.ATA,
                            ETD = item.ETD,                            
                            JobRef = item.JobRef,
                            LoadPort = item.LoadPort,
                            Quantity = item.Quantity,
                            UniversalSerialNr = item.UniversalSerialNr,
                            User = item.User
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return discharge;
        }
        public static ShipmentDetails GetShipmentDetailsFromJobRef(string JobRef)
        {
            ShipmentDetails shipmentDetails = new ShipmentDetails();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string query = "Select SD.ClosingDate, SD.Country, SD.Quantity, SD.QuantityLifting, SD.QuoteType, SD.\"User\", SD.UniversalSerialNr, SD.ModifyDate, SD.PortPair, SD.PlaceOfReceipt, SD.PlaceOfDelivery, SD.LoadPort, SD.LoadFT, SD.JobRef, SD.LoadAgentAddress, SD.ETA, SD.ETD, SD.IDAgentDisch, SD.IDAgentLoad, SD.LoadAgent, BL.DischAgentNameBL, BL.ShipperNameBL, SD.CreateDate, SD.DischAgentAddress, SD.DischFT, SD.DischPort" +
                        " from ShipmentDetailDry SD INNER JOIN BLDetail BL ON SD.UniversalSerialNr = BL.UniversalSerialNr where SD.JobRef = ?";

                    var shipmentData = conn.Query<ShipmentDetails>(query, parametersd);

                    if (shipmentData.Any())
                    {
                        foreach (var item in shipmentData)
                        {
                            shipmentDetails = new ShipmentDetails
                            {
                                ClosingDate = item.ClosingDate,
                                Country = item.Country,
                                CreateDate = item.CreateDate,
                                DischAgentAddress = item.DischAgentAddress,
                                DischFT = item.DischFT,
                                DischPort = item.DischPort,
                                ETA = item.ETA,
                                LoadAgentAddress = item.LoadAgentAddress,
                                ETD = item.ETD,
                                IDAgentDisch = item.IDAgentDisch,
                                IDAgentLoad = item.IDAgentLoad,
                                JobRef = item.JobRef,
                                LoadFT = item.LoadFT,
                                LoadPort = item.LoadPort,
                                ModifyDate = item.ModifyDate,
                                PlaceOfDelivery = item.PlaceOfDelivery,
                                PlaceOfReceipt = item.PlaceOfReceipt,
                                PortPair = item.PortPair,
                                DischAgentNameBL = item.DischAgentNameBL,
                                LoadAgent = item.LoadAgent,
                                ShipperNameBL = item.ShipperNameBL,
                                Quantity = item.Quantity,
                                QuantityLifting = item.QuantityLifting,
                                QuoteType = item.QuoteType,
                                UniversalSerialNr = item.UniversalSerialNr,
                                User = item.User
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return shipmentDetails;
        }
        public static ErrorLog SaveDischargePlan(ShipmentDetails shipmentDetails)
        {
            string username = HttpContext.Current.User.Identity.Name;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("JobRef", shipmentDetails.JobRef, DbType.String, ParameterDirection.Input);

                    string query = "Select IDNo from DischargePlan where JobRef = ?";

                    var dischargePlanExists = conn.Query<ShipmentDetails>(query, parametersd);
                    if (dischargePlanExists.Count() == 0)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ClosingDate", shipmentDetails.ClosingDate, DbType.Date, ParameterDirection.Input);
                        //parameters.Add("Country", shipmentDetails.Country, DbType.String, ParameterDirection.Input);
                        parameters.Add("CreateDate", shipmentDetails.CreateDate, DbType.DateTime, ParameterDirection.Input);
                        //parameters.Add("DischAgentAddress", shipmentDetails.DischAgentAddress, DbType.String, ParameterDirection.Input);
                        parameters.Add("DischFT", shipmentDetails.DischFT, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("DischPort", shipmentDetails.DischPort, DbType.String, ParameterDirection.Input);
                        parameters.Add("ATA", shipmentDetails.ATA, DbType.Date, ParameterDirection.Input);
                        //parameters.Add("ETA", shipmentDetails.ETD, DbType.Date, ParameterDirection.Input);
                        //parameters.Add("ETD", shipmentDetails.ETD, DbType.Date, ParameterDirection.Input);
                        parameters.Add("IDAgentDisch", shipmentDetails.IDAgentDisch, DbType.String, ParameterDirection.Input);
                        parameters.Add("IDAgentLoad", shipmentDetails.IDAgentLoad, DbType.String, ParameterDirection.Input);
                        parameters.Add("JobRef", shipmentDetails.JobRef, DbType.String, ParameterDirection.Input);
                        //parameters.Add("LoadAgentAddress", shipmentDetails.LoadAgentAddress, DbType.String, ParameterDirection.Input);
                        parameters.Add("LoadFT", shipmentDetails.LoadFT, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("LoadPort", shipmentDetails.LoadPort, DbType.String, ParameterDirection.Input);
                        parameters.Add("ModifyDate", shipmentDetails.ModifyDate, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("PlaceOfDelivery", shipmentDetails.PlaceOfDelivery, DbType.String, ParameterDirection.Input);
                        parameters.Add("PlaceOfReceipt", shipmentDetails.PlaceOfReceipt, DbType.String, ParameterDirection.Input);
                        parameters.Add("PortPair", shipmentDetails.PortPair, DbType.String, ParameterDirection.Input);
                        //parameters.Add("DischAgentNameBL", shipmentDetails.DischAgentNameBL, DbType.String, ParameterDirection.Input);
                        //parameters.Add("LoadAgent", shipmentDetails.LoadAgent, DbType.String, ParameterDirection.Input);
                        //parameters.Add("ShipperNameBL", shipmentDetails.ShipperNameBL, DbType.String, ParameterDirection.Input);
                        parameters.Add("Quantity", shipmentDetails.Quantity, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("Remark", shipmentDetails.Remark, DbType.String, ParameterDirection.Input);
                        //parameters.Add("QuantityLifting", shipmentDetails.QuantityLifting, DbType.Int32, ParameterDirection.Input);
                        //parameters.Add("QuoteType", shipmentDetails.QuoteType, DbType.String, ParameterDirection.Input);
                        parameters.Add("UniversalSerialNr", shipmentDetails.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                        parameters.Add("User", username, DbType.String, ParameterDirection.Input);
                        parameters.Add("DischargePlanStatus", shipmentDetails.DischargePlanStatus, DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Insert Into DischargePlan(ClosingDate,CreateDate,DischFT,DischPort,ATA,IDAgentDisch,IDAgentLoad,JobRef,LoadFT,LoadPort,ModifyDate,PlaceOfDelivery,PlaceOfReceipt,PortPair,Quantity,Remark,UniversalSerialNr, \"User\", DischargePlanStatus)" +
                            "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        int rowsAffected = conn.Execute(odbcQuery, parameters);

                        if (rowsAffected > 0)
                        {
                            errorLog.IsError = false;
                            errorLog.ErrorMessage = shipmentDetails.JobRef;
                        }
                        else
                        {
                            errorLog.IsError = true;
                            errorLog.ErrorMessage = shipmentDetails.JobRef;
                        }

                        string query2 = "Select MAX (IDNo) as IDNo from DischargePlan where UniversalSerialNr= '" + shipmentDetails.UniversalSerialNr + "'";

                        var dischargeData = conn.Query<ShipmentDetails>(query2, parameters);
                        foreach (var item in dischargeData)
                        {
                            shipmentDetails.IDNo = item.IDNo;
                        }
                    }
                    else
                    {
                        errorLog.IsError = true;
                        errorLog.ErrorMessage = "Discharge Plan is already created";
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

        public static DischargePlan GetDischargePlanFromIDNo(string JobRef)
        {
            DischargePlan dischargePlan = new DischargePlan();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);
                                        
                    string query = "Select * from DischargePlan where JobRef = ?";

                    var dischargePlanData = conn.Query<DischargePlan>(query, parameters);
                    if (dischargePlanData.Any())
                    {
                        //var parameter1 = new DynamicParameters();
                        //parameter1.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);
                        //string query1 = "Select Status from SalesInvoiceDRY WHERE JobRefNo = ?";
                        //var invoiceStatus = conn.Query<DischargePlan>(query1, parameter1);

                        //foreach (var item in invoiceStatus)
                        //{
                        //    dischargePlan.Status = item.Status;
                        //}

                        foreach (var item in dischargePlanData)
                        {
                            dischargePlan.IDNo = item.IDNo;
                            dischargePlan.ClosingDate = item.ClosingDate;
                            dischargePlan.Country = item.Country;
                            dischargePlan.CreateDate = item.CreateDate;
                            dischargePlan.DischAgentAddress = item.DischAgentAddress;
                            dischargePlan.DischFT = item.DischFT;
                            dischargePlan.DischPort = item.DischPort;
                            dischargePlan.ETA = item.ETA;
                            dischargePlan.ETD = item.ETD;
                            dischargePlan.ATA = item.ATA;
                            dischargePlan.LoadAgentAddress = item.LoadAgentAddress;
                            dischargePlan.IDAgentDisch = item.IDAgentDisch;
                            dischargePlan.IDAgentLoad = item.IDAgentLoad;
                            dischargePlan.JobRef = item.JobRef;
                            dischargePlan.LoadFT = item.LoadFT;
                            dischargePlan.LoadPort = item.LoadPort;
                            dischargePlan.ModifyDate = item.ModifyDate;
                            dischargePlan.PlaceOfDelivery = item.PlaceOfDelivery;
                            dischargePlan.PlaceOfReceipt = item.PlaceOfReceipt;
                            dischargePlan.PortPair = item.PortPair;
                            dischargePlan.DischAgentNameBL = item.DischAgentNameBL;
                            dischargePlan.LoadAgent = item.LoadAgent;
                            dischargePlan.ShipperNameBL = item.ShipperNameBL;
                            dischargePlan.Remark = item.Remark;
                            dischargePlan.Quantity = item.Quantity;
                            dischargePlan.QuantityLifting = item.QuantityLifting;
                            dischargePlan.QuoteType = item.QuoteType;
                            dischargePlan.UniversalSerialNr = item.UniversalSerialNr;
                            dischargePlan.User = item.User;
                            dischargePlan.DischargePlanStatus = item.DischargePlanStatus;
                            dischargePlan.VesselName = item.VesselName;
                            dischargePlan.DishTo = item.DishTo;
                            dischargePlan.DishAttn = item.DishAttn;
                        }
                    }
                }
            }
            catch (Exception ex) 
            { 
                throw ex; 
            }
            return dischargePlan;
        }

        public static ErrorLog UpdateDischargePlan(DischargePlan dischargePlan)
        {
            string username = HttpContext.Current.User.Identity.Name;
            try
            {
                if (dischargePlan.DischargePlanStatus == Constant.Draft)
                {
                    using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("ATA", dischargePlan.ATA, DbType.Date, ParameterDirection.Input);
                        parameters.Add("ModifyDate", dischargePlan.ModifyDate, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                        parameters.Add("Remark", dischargePlan.Remark, DbType.String, ParameterDirection.Input);
                        parameters.Add("IDNo", dischargePlan.IDNo, DbType.Int32, ParameterDirection.Input);

                        string odbcQuery = "Update DischargePlan Set ATA = ? , ModifyDate = ?, ModifiedBy = ?, Remark = ? WHERE IDNo = ?";

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
                    errorLog.ErrorMessage = "Discharge plan needs to be in Draft for Making any changes";
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

        public static ErrorLog ConfirmDischargePlan(int idNo)
        {
            string username = HttpContext.Current.User.Identity.Name;
            string status = Constant.Confirmed;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("DischargePlanStatus", status, DbType.String, ParameterDirection.Input);
                    parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                    parameters.Add("IDNo", idNo, DbType.Int32, ParameterDirection.Input);

                    string odbcQuery = "Update DischargePlan Set DischargePlanStatus = ?, ModifiedBy = ? Where IDNo = ?";

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

        public static List<DischargePlan> GetConfirmedDischargePlanList(SearchParameters search)
        {
            string IDCompany = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
            List<DischargePlan> discharge = new List<DischargePlan>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select DP.ChargeParty, DP.IDNo, DP.JobRef, DP.ATA, DP.ETA, DP.ETD, DP.LoadPort, DP.DischPort, DP.DischargePlanStatus, DP.UniversalSerialNr " +
                        "from DischargePlan DP INNER JOIN SalesInvoiceDRY SID ON DP.UniversalSerialNr = SID.UniversalSerialNr " +
                        "INNER JOIN AgentAddressBook AB ON UPPER(DP.ChargeParty) = UPPER(AB.CompanyName) AND AB.IDCompany = '" + IDCompany + "' " +
                        "WHERE SID.InvoiceType = 'IMPORT' AND DP.DischargePlanStatus = '" + Constant.Confirmed + "' AND SID.Status = '" + Constant.Issued + "' AND";

                    if ((search.JobRef != null || (search.CompanyName != null) || search.DischargePort != null) || (search.LoadPort != null))
                    {
                        if (search.JobRef != null)
                        {
                            query += " DP.JobRef = '" + search.JobRef + "' AND";
                        }

                        if (search.CompanyName != null)
                        {
                            query += " DP.ChargeParty = '" + search.CompanyName + "' AND";
                        }

                        if (search.DischargePort != null)
                        {
                            query += " DP.DischPort = '" + search.DischargePort + "' AND";
                        }

                        if (search.LoadPort != null)
                        {
                            query += " DP.LoadPort = '" + search.LoadPort + "' AND";
                        }
                    }

                    if (query.LastIndexOf("AND") > 0)
                    {
                        query = query.Remove(query.LastIndexOf("AND"));
                    }

                    query += " Order By DP.ModificationTimestamp DESC";
                    var dischargeData = conn.Query<DischargePlan>(query);
                    foreach (var item in dischargeData)
                    {
                        discharge.Add(new DischargePlan
                        {
                            IDNo = item.IDNo,
                            ChargeParty = item.ChargeParty,
                            JobRef = item.JobRef,
                            ATA = item.ATA,
                            ETA = item.ETA,
                            ETD = item.ETD,
                            LoadPort = item.LoadPort,
                            DischPort = item.DischPort,
                            DischargePlanStatus = item.DischargePlanStatus,
                            UniversalSerialNr = item.UniversalSerialNr
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return discharge;
        }

        public static List<MovementRecord> FetchDeliveryContainenrsFromJobRef(string universalSerialNr)
        {
            List<MovementRecord> discharge = new List<MovementRecord>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);

                    string query = "Select DP.ETA, DP.ATA, DP.JobRef, DP.VesselName, AE.UniversalSerialNr, AE.ID, AE.ContainerNo, AE.SealNo, AE.DONumber, AE.DOIssueDate" +
                        " FROM DischargePlan DP INNER JOIN AllocateEquipment AE ON AE.UniversalSerialNr = DP.UniversalSerialNr WHERE AE.UniversalSerialNr = ?";

                    query += " Order By AE.JobRefSerialNo DESC";
                    var dischargeData = conn.Query<MovementRecord>(query, parameters);
                    foreach (var item in dischargeData)
                    {
                        discharge.Add(new MovementRecord
                        {
                            ID = item.ID,
                            ETA = item.ETA,
                            VesselName = item.VesselName,
                            ATA = item.ATA,
                            JobRefSerialNo = item.JobRefSerialNo,
                            UniversalSerialNr = item.UniversalSerialNr,
                            ContainerNo = item.ContainerNo,
                            SealNo = item.SealNo,
                            DONumber = item.DONumber,
                            DOIssueDate = item.DOIssueDate
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return discharge;
        }

        public static object UpdateContainersDetails(List<DevAllocateEquipment> allocateEquipment, string JobRef, string DishTo, string DishAttn)
        {
            int rowsAffected;
            string username = HttpContext.Current.User.Identity.Name;
            object doData = new object();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string query = "Select UniversalSerialNr from DischargePlan where JobRef = ?";

                    string UniversalSerialNo = conn.QueryFirst<string>(query, parameters);
                    string deliveryOrderNumber = GenerateDONumber(JobRef, UniversalSerialNo);

                    for (int i = 0; i < allocateEquipment.Count(); i++)
                    {
                        var AllocateEqParameter = new DynamicParameters();
                        AllocateEqParameter.Add("DONumber", deliveryOrderNumber, DbType.String, ParameterDirection.Input);
                        AllocateEqParameter.Add("DOIssueDate", DateTime.Now, DbType.Date, ParameterDirection.Input);
                        AllocateEqParameter.Add("ModifyUser", username, DbType.String, ParameterDirection.Input);
                        AllocateEqParameter.Add("ContainerNo", allocateEquipment[i].ContainerNo, DbType.String, ParameterDirection.Input);
                        AllocateEqParameter.Add("UniversalSerialNr", UniversalSerialNo, DbType.String, ParameterDirection.Input);


                        string odbcQuery = "Update AllocateEquipment Set DONumber = ?, DOIssueDate = ?, ModifyUser = ? Where ContainerNo = ? AND UniversalSerialNr = ?";

                        rowsAffected = conn.Execute(odbcQuery, AllocateEqParameter);
                        if (rowsAffected > 0)
                        {
                            var DPParameter = new DynamicParameters();
                            DPParameter.Add("DishTo", DishTo, DbType.String, ParameterDirection.Input);
                            DPParameter.Add("DishAttn", DishAttn, DbType.String, ParameterDirection.Input);
                            DPParameter.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);
                            DPParameter.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                            string query1 = "Update DischargePlan Set DishTo = ? , DishAttn = ?, ModifiedBy = ? Where JobRef = ?";

                            int rowsAffect = conn.Execute(query1, DPParameter);
                            if (rowsAffect > 0)
                            {
                                doData = new
                                {
                                    UniversalSerialNr = UniversalSerialNo,
                                    DONumber = deliveryOrderNumber
                                };
                            }
                            else
                            {
                                doData = new
                                {
                                    UniversalSerialNr = UniversalSerialNo,
                                    DONumber = deliveryOrderNumber
                                };
                            }
                        }
                        else
                        {
                            doData = new
                            {
                                UniversalSerialNr = UniversalSerialNo,
                                DONumber = deliveryOrderNumber
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return doData;
        }
        public static string GenerateDONumber(string JobRef, string universalSerialNr)
        {
            string deliveryOrderNumber = string.Empty;
            string doPrefix = "LEGIMP";
            string part1 = JobRef.Substring(JobRef.Length - 5);
            int printCounter = 0;

            try
            {
                string DONumberPrefix = string.Format("{0}{1}", doPrefix, part1);

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "SELECT Max(DONumber) AS DONumber FROM AllocateEquipment where UniversalSerialNr = ?";

                    var parameters = new DynamicParameters();
                    parameters.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);

                    deliveryOrderNumber = conn.QueryFirst<string>(query, parameters);
                    if (string.IsNullOrEmpty(deliveryOrderNumber))
                    {
                        printCounter += 1;
                        string counterNumber = printCounter.ToString("D3");
                        deliveryOrderNumber = string.Format("{0}-{1}", DONumberPrefix, counterNumber);
                    }
                    else
                    {
                        printCounter = Convert.ToInt32(deliveryOrderNumber.Substring(12));
                        printCounter += 1;
                        string counterNumber = printCounter.ToString("D3");
                        deliveryOrderNumber = string.Format("{0}-{1}", DONumberPrefix, counterNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return deliveryOrderNumber;
        }

        public static DeliveryDetails GetDeliveryOrderDetailsFromJobRef(string universalSerialNr, string doNumber)
        {
            DeliveryDetails DeliveryRef = new DeliveryDetails();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);
                    parametersd.Add("DONumber", doNumber, DbType.String, ParameterDirection.Input);

                    string querysd = "Select JobRefSerialNo, DOIssueDate, DONumber, " +
                        " Measurement, GrossWeight, GrossWeightUnit,MeasurementUnit, " +
                        "  ContainerNo, SealNo, " +
                        " UniversalSerialNr from AllocateEquipment where UniversalSerialNr = ? and DONumber = ?";

                    var AllEquipData = conn.Query<DevAllocateEquipment>(querysd, parametersd);
                    foreach (var item in AllEquipData)
                    {
                        DeliveryRef.DevAllocateEquipmentModel.Add(new DevAllocateEquipment
                        {
                            JobRefSerialNo = item.JobRefSerialNo,
                            DOIssueDate = item.DOIssueDate,
                            Measurement = item.Measurement,
                            MeasurementUnit = item.MeasurementUnit,
                            GrossWeight = item.GrossWeight,
                            GrossWeightUnit = item.GrossWeightUnit,
                            ContainerNo = item.ContainerNo,
                            SealNo = item.SealNo,
                            DONumber = item.DONumber,
                            UniversalSerialNr = item.UniversalSerialNr
                        });
                    }

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);

                    string querybl = "Select JobRef, LoadPort, VesselName, " +
                                " Voyage, ETA, ATA, DishTo, DishAttn" +
                                " from DischargePlan where UniversalSerialNr = ?";

                    var dischragePlanData = conn.Query<DischargePlan>(querybl, parameterbl);
                    foreach (var item in dischragePlanData)
                    {
                        DeliveryRef.DischargePlanModel.JobRef = item.JobRef;
                        DeliveryRef.DischargePlanModel.LoadPort = item.LoadPort;
                        DeliveryRef.DischargePlanModel.VesselName = item.VesselName;
                        DeliveryRef.DischargePlanModel.Voyage = item.Voyage;
                        DeliveryRef.DischargePlanModel.ETA = item.ETA;
                        DeliveryRef.DischargePlanModel.ATA = item.ATA;
                        DeliveryRef.DischargePlanModel.DishTo = item.DishTo;
                        DeliveryRef.DischargePlanModel.DishAttn = item.DishAttn;
                    }

                    var parameterquote = new DynamicParameters();
                    parameterquote.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);

                    string queryquote = "select MBLMAWB from ShipmentDetailDry where UniversalSerialNr = ?";

                    var quotedata = conn.Query<ShipmentDetails>(queryquote, parameterquote);
                    foreach (var item in quotedata)
                    {
                        DeliveryRef.ShipmentDetailsModel.MBLMAWB = item.MBLMAWB;
                    }

                    var parameterBL = new DynamicParameters();
                    parameterBL.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);

                    string queryBL = "select NoOfPkgs from BLDetail where UniversalSerialNr = ?";

                    var quoteBLdata = conn.Query<BLDetails>(queryBL, parameterBL);
                    foreach (var item in quoteBLdata)
                    {
                        DeliveryRef.BLDetailsModel.NoOfPkgs = item.NoOfPkgs;
                    }

                    var parameterQ = new DynamicParameters();
                    parameterQ.Add("UniversalSerialNr", universalSerialNr, DbType.String, ParameterDirection.Input);

                    string queryQ = "Select EquipmentType from ShipmentDetailDry where UniversalSerialNr = ?";

                    var queryQData = conn.Query<QuoteRef>(queryQ, parameterQ);
                    foreach (var item in queryQData)
                    {
                        DeliveryRef.QuoteRefModel.EquipmentType = item.EquipmentType;
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return DeliveryRef;
        }

        //Added By Varsha
        public static List<BLDetails> GetBLDetails(SearchParameters search)
        {
            string IDCompany = GetUserDetailsForUser(HttpContext.Current.User.Identity.Name).IDCompany;
            List<BLDetails> BLDet = new List<BLDetails>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select BL.JobRef, BL.BLTypes, BL.ShipperNameBL, BL.ConsigneeNameBL, BL.CountryPOL, BL.DischPort, BL.LoadPort, BL.UniversalSerialNr From BLDetail BL " +
                        "INNER JOIN AgentAddressBook AB ON UPPER(BL.ConsigneeNameBL) = UPPER(AB.CompanyName) AND AB.IDCompany = '" + IDCompany + "' " +
                        "WHERE BL.BLTypes <> 'DRAFT' AND BL.JobRef NOT IN(select Jobref from DischargePlan) AND";

                    if ((search.Status != null) || (search.JobRef != null) || (search.CompanyName != null) || (search.DischargePort != null) || (search.LoadPort != null))
                    {
                        if (search.Status != null)
                        {
                            query += " BL.BLTypes = '" + search.Status + "' AND";
                        }
                        if (search.JobRef != null)
                        {
                            query += " BL.JobRef = '" + search.JobRef + "' AND";
                        }
                        
                        if (search.CompanyName != null)
                        {
                            query += " BL.ConsigneeNameBL = '" + search.CompanyName + "' AND";
                        }

                        if (search.DischargePort != null)
                        {
                            query += " BL.DischPort = '" + search.DischargePort + "' AND";
                        }

                        if (search.LoadPort != null)
                        {
                            query += " BL.LoadPort = '" + search.LoadPort + "' AND";
                        }
                    }

                    if (query.LastIndexOf("AND") > 0)
                    {
                        query = query.Remove(query.LastIndexOf("AND"));
                    }

                    query += " ORDER BY BL.ModificationTimestamp DESC";

                    var BLData = conn.Query<BLDetails>(query);
                    foreach (var item in BLData)
                    {
                        BLDet.Add(new BLDetails
                        {
                            JobRef = item.JobRef,
                            BLTypes = item.BLTypes,
                            ShipperNameBL = item.ShipperNameBL,
                            ConsigneeNameBL = item.ConsigneeNameBL,
                            DischPort = item.DischPort,
                            LoadPort = item.LoadPort,
                            CountryPOL = item.CountryPOL
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return BLDet;
        }

        public static List<SelectListItem> GetImportDocStatus()
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
                    Text = "SURRENDER",
                    Value = "2"
                },
                new SelectListItem
                {
                    Text = "NON-NEGOTIABLE",
                    Value = "3"
                },
                new SelectListItem
                {
                    Text = "ORIGINAL",
                    Value = "4"
                },
                new SelectListItem
                {
                    Text = "SEAWAY",
                    Value = "5"
                }
            };

            return stat;
        }
        
        public static List<SelectListItem> GetServiceModes()
        {
            List<SelectListItem> stat = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "FCL/FCL",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "FCL/LCL",
                    Value = "2"
                },
                new SelectListItem
                {
                    Text = "LCL/FCL",
                    Value = "3"
                },
                new SelectListItem
                {
                    Text = "LCL/LCL",
                    Value = "4"
                }
            };

            return stat;
        }

        public static ShipmentBL GetCRADetailsJobRef(string JobRef)
        {
            ShipmentBL shipmentRef = new ShipmentBL();
            List<DevAllocateEquipment> EquipLi = new List<DevAllocateEquipment>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string querysd = "Select JobRef, ShipmentTerm, ETA, UniversalSerialNr, IDQuoteRef from ShipmentDetailDry where JobRef = ?";

                    var ShipmentData = conn.Query<ShipmentDetails>(querysd, parametersd);
                    foreach (var item in ShipmentData)
                    {
                        shipmentRef.ShipmentDetailsModel.JobRef = item.JobRef;
                        shipmentRef.ShipmentDetailsModel.ShipmentTerm = item.ShipmentTerm;
                        shipmentRef.ShipmentDetailsModel.ETA = item.ETA;
                        shipmentRef.ShipmentDetailsModel.UniversalSerialNr = item.UniversalSerialNr;
                        shipmentRef.ShipmentDetailsModel.IDQuoteRef = item.IDQuoteRef;
                    }

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string querybl = "Select JobRef, ShipperNameBL,ConsigneeNameBL,CargoDescription, ShipperAddressBL,ConsigneeAddressBL, " +
                                " LoadPort, DischPort, TotalGweight, PlaceofReceipt, CountryPOD, NoOfPkgs, PkgType " +
                                " from BLDetail where UniversalSerialNr = ?";

                    var BLData = conn.Query<BLDetails>(querybl, parameterbl);
                    foreach (var item in BLData)
                    {
                        shipmentRef.BLDetailsModel.JobRef = item.JobRef;
                        shipmentRef.BLDetailsModel.ShipperNameBL = item.ShipperNameBL;
                        shipmentRef.BLDetailsModel.ConsigneeNameBL = item.ConsigneeNameBL;
                        shipmentRef.BLDetailsModel.CargoDescription = item.CargoDescription;
                        shipmentRef.BLDetailsModel.ShipperAddressBL = item.ShipperAddressBL;
                        shipmentRef.BLDetailsModel.ConsigneeAddressBL = item.ConsigneeAddressBL;

                        shipmentRef.BLDetailsModel.LoadPort = item.LoadPort;
                        shipmentRef.BLDetailsModel.DischPort = item.DischPort;
                        shipmentRef.BLDetailsModel.TotalGweight = item.TotalGweight;

                        shipmentRef.BLDetailsModel.PlaceofReceipt = item.PlaceofReceipt;
                        shipmentRef.BLDetailsModel.CountryPOD = item.CountryPOD;

                        shipmentRef.BLDetailsModel.NoOfPkgs = item.NoOfPkgs;
                        shipmentRef.BLDetailsModel.PkgType = item.PkgType;
                    }

                    var parameterv = new DynamicParameters();
                    parameterv.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    shipmentRef.TransVessel = GetTransVessel(shipmentRef);

                    var parameterEquip = new DynamicParameters();
                    parameterEquip.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string queryEquip = "Select ContainerNo, SealNo, Measurement, MeasurementUnit, GrossWeight, GrossWeightUnit from AllocateEquipment where UniversalSerialNr = ?";

                    var EquipmentData = conn.Query<DevAllocateEquipment>(queryEquip, parameterEquip);
                    foreach (var item in EquipmentData)
                    {
                        EquipLi.Add(new DevAllocateEquipment
                        {
                            ContainerNo = item.ContainerNo,
                            SealNo = item.SealNo,
                            Measurement = item.Measurement,
                            MeasurementUnit = item.MeasurementUnit,
                            GrossWeight = item.GrossWeight,
                            GrossWeightUnit = item.GrossWeightUnit
                        });
                    }
                    shipmentRef.DevAllocateEquipmentModel = EquipLi;

                    var parameterQuote = new DynamicParameters();
                    parameterQuote.Add("IDQuoteRef", shipmentRef.ShipmentDetailsModel.IDQuoteRef, DbType.String, ParameterDirection.Input);

                    string queryQuote = "Select EquipmentType from QuoteReference where QuoteRefID = ?";

                    var QuoteData = conn.Query<QuoteRef>(queryQuote, parameterQuote);
                    foreach (var item in QuoteData)
                    {
                        shipmentRef.QuoteRefModel.EquipmentType = item.EquipmentType;
                    }

                    var parameterRemark = new DynamicParameters();
                    parameterRemark.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string queryRemark = "Select Remark from DischargePlan where JobRef = ?";

                    var QuoteRemark = conn.Query<ShipmentDetails>(queryRemark, parameterRemark);
                    foreach (var item in QuoteRemark)
                    {
                        shipmentRef.ShipmentDetailsModel.Remark = item.Remark;
                    }

                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
            return shipmentRef;
        }

        public static ShipmentBL GetShipmentDetailsFromJobRefForImportInvoice(string JobRef)
        {
            ShipmentBL shipmentRef = new ShipmentBL();
            List<DryShipmentSellLi> sellLi = new List<DryShipmentSellLi>();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("JobRef", JobRef, DbType.String, ParameterDirection.Input);

                    string querysd = "Select JobRef, POL, POD, HBLHAWB, MBLMAWB, ETA, ETD, Remark, UniversalSerialNr from ShipmentDetailDry where JobRef = ?";

                    var ShipmentData = conn.Query<ShipmentDetails>(querysd, parametersd);
                    foreach (var item in ShipmentData)
                    {
                        shipmentRef.ShipmentDetailsModel.JobRef = item.JobRef;
                        shipmentRef.ShipmentDetailsModel.POL = item.POL;
                        shipmentRef.ShipmentDetailsModel.POD = item.POD;
                        shipmentRef.ShipmentDetailsModel.HBLHAWB = item.HBLHAWB;
                        shipmentRef.ShipmentDetailsModel.MBLMAWB = item.MBLMAWB;
                        shipmentRef.ShipmentDetailsModel.ETA = item.ETA;
                        shipmentRef.ShipmentDetailsModel.ETD = item.ETD;
                        shipmentRef.ShipmentDetailsModel.Remark = item.Remark;
                        shipmentRef.ShipmentDetailsModel.UniversalSerialNr = item.UniversalSerialNr;
                    }

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    string querybl = "Select * from DryShipmentSellLi where UniversalSerialNr = ?";

                    var BLData = conn.Query<DryShipmentSellLi>(querybl, parameterbl);
                    foreach (var item in BLData)
                    {
                        if (item.PaymentTerm == "Collect")
                        {
                            sellLi.Add(new DryShipmentSellLi
                            {
                                Quantity = item.Quantity,
                                CompanyAddress = item.CompanyAddress,
                                CompanyName = item.CompanyName,
                                UnitRate = item.UnitRate,
                                Currency = item.Currency,
                                ExRate = item.ExRate,
                                AmountUSD = item.AmountUSD,
                                PaymentTerm = item.PaymentTerm,
                                User = item.User,
                                Description = item.Description,
                                PayMode = item.PayMode,
                                IDCompany = item.IDCompany,
                                UnitRateUSD = item.UnitRateUSD,
                                UniversalSerialNr = item.UniversalSerialNr
                            });
                        }
                    }

                    shipmentRef.DryShipmentSellLiModel = sellLi;

                    shipmentRef.TransVessel = GetTransVessel(shipmentRef);

                    var parameterv = new DynamicParameters();
                    parameterv.Add("UniversalSerialNr", shipmentRef.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    

                    string queryGWeight = "Select TotalGweight, ConsigneeAddressBL,GrossWeightUnit, ConsigneeNameBL from BLDetail where UniversalSerialNr = ?";

                    var BLDetailData = conn.Query<BLDetails>(queryGWeight, parameterv);
                    foreach (var item3 in BLDetailData)
                    {
                        shipmentRef.BLDetailsModel.TotalGweight = item3.TotalGweight;
                        shipmentRef.BLDetailsModel.ConsigneeAddressBL = item3.ConsigneeAddressBL;
                        shipmentRef.BLDetailsModel.ConsigneeNameBL = item3.ConsigneeNameBL;
                        shipmentRef.BLDetailsModel.GrossWeightUnit = item3.GrossWeightUnit;
                    }
                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
            return shipmentRef;
        }

        public static ErrorLog SaveSalesInvoiceData(ShipmentBL DetShipBL)
        {
            string username = HttpContext.Current.User.Identity.Name;
            ImportInvoiceDetails InvoiceRef = new ImportInvoiceDetails();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters2 = new DynamicParameters();
                    parameters2.Add("JobRefNo", DetShipBL.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);
                    parameters2.Add("InvoiceType", Constant.Import, DbType.String, ParameterDirection.Input);
                    string queryStatus = "Select InvoiceType,Status from SalesInvoiceDry where JobRefNo = ? AND InvoiceType = ?";

                    var SalesInvoiceDryData = conn.Query<SalesInvoiceDry>(queryStatus, parameters2);
                    if (SalesInvoiceDryData.Any())
                    {


                        foreach (var item in SalesInvoiceDryData)
                        {
                            InvoiceRef.SalesInvoiceDryModel.Status = item.Status;
                        }

                        errorLog.IsError = false;
                        errorLog.ErrorMessage = InvoiceRef.SalesInvoiceDryModel.Status;

                    }
                    else
                    {
                        string RateType = GetRateTypeFromUSN(DetShipBL.ShipmentDetailsModel.UniversalSerialNr);
                        string InvoiceType = Constant.Import;
                        string ProformaInvoiceNo = GenerateInvoiceNumber(RateType, InvoiceType);
                        int creditTerms = GetCreditTermFromAddress(DetShipBL.BLDetailsModel.ConsigneeNameBL);
                        string ContainerNo = ConvertToString(GetContainerNo(DetShipBL.ShipmentDetailsModel.UniversalSerialNr));
                        string GetID = "";
                        string AccMonth = DateTime.Today.ToString("MMMyy");

                        int rowsAffectedInvoiceDry;
                        // SqlTransaction trans = conn.BeginTransaction();
                        var parameters = new DynamicParameters();
                        parameters.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);
                        parameters.Add("ProformaInvoiceNo", ProformaInvoiceNo, DbType.String, ParameterDirection.Input);
                        parameters.Add("InvoiceType", InvoiceType, DbType.String, ParameterDirection.Input);
                        parameters.Add("GrossWeightUnit", DetShipBL.BLDetailsModel.GrossWeightUnit, DbType.String, ParameterDirection.Input);
                        // parameters.Add("BillingPartyAddress", DetShipBL.BLDetailsModel.ConsigneeAddressBL, DbType.String, ParameterDirection.Input);
                        parameters.Add("AcctMonth", AccMonth, DbType.String, ParameterDirection.Input);
                        parameters.Add("BillingParty", DetShipBL.BLDetailsModel.ConsigneeNameBL, DbType.String, ParameterDirection.Input);
                        // parameters.Add("InvoiceDate", DateTime.Now, DbType.Date, ParameterDirection.Input);
                        parameters.Add("JobRefNo", DetShipBL.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);

                        parameters.Add("CreditTerms", creditTerms, DbType.Int32, ParameterDirection.Input);

                        // parameters.Add("DueDate", DueDate, DbType.Date, ParameterDirection.Input);
                        // parameters.Add("MBLHAWB", DetShipBL.ShipmentDetailsModel.MBLMAWB, DbType.String, ParameterDirection.Input);

                        //parameters.Add("HBLHAWB", DetShipBL.ShipmentDetailsModel.HBLHAWB, DbType.String, ParameterDirection.Input);
                        parameters.Add("VesselName", DetShipBL.TransVessel[0].VesselName, DbType.String, ParameterDirection.Input);
                        parameters.Add("VoyageNo", DetShipBL.TransVessel[0].VoyNo, DbType.String, ParameterDirection.Input);

                        //parameters.Add("LoadPort", DetShipBL.ShipmentDetailsModel.POL, DbType.String, ParameterDirection.Input);
                        //parameters.Add("DischargePort", DetShipBL.ShipmentDetailsModel.POD, DbType.String, ParameterDirection.Input);
                        // parameters.Add("ETD", DetShipBL.ShipmentDetailsModel.ETD, DbType.Date, ParameterDirection.Input);
                        // parameters.Add("ETA", DetShipBL.ShipmentDetailsModel.ETA, DbType.Date, ParameterDirection.Input);

                        parameters.Add("Grossweight", DetShipBL.BLDetailsModel.TotalGweight, DbType.Double, ParameterDirection.Input);
                        //parameters.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);
                        //parameters.Add("AmountinUSDSUM", agentaddressbook.AmountinUSDSUM, DbType.String, ParameterDirection.Input);
                        parameters.Add("Remarks", DetShipBL.ShipmentDetailsModel.Remark, DbType.String, ParameterDirection.Input);
                        //parameters.Add("PostDate", DateTime.Now, DbType.Date, ParameterDirection.Input);

                        parameters.Add("Status", Constant.Issued, DbType.String, ParameterDirection.Input);
                        parameters.Add("UniversalSerialNr", DetShipBL.ShipmentDetailsModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                        string odbcQuery = "Insert Into SalesInvoiceDry(CreatedBy, ProformaInvoiceNo,InvoiceType,GrossWeightUnit, AcctMonth, BillingParty,JobRefNo,CreditTerms,VesselName,VoyageNo,Grossweight,Remarks,Status,UniversalSerialNr)" +
                            "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                        rowsAffectedInvoiceDry = conn.Execute(odbcQuery, parameters);

                        if (rowsAffectedInvoiceDry > 0)
                        {
                            var parameters1 = new DynamicParameters();
                            parameters1.Add("JobRefNo", DetShipBL.ShipmentDetailsModel.JobRef, DbType.String, ParameterDirection.Input);
                            parameters1.Add("InvoiceType", Constant.Import, DbType.String, ParameterDirection.Input);

                            string query = "Select ID from SalesInvoiceDry where JobRefNo = ? AND InvoiceType = ? ";
                            var GetIDData = conn.Query<SalesInvoiceDry>(query, parameters1);
                            foreach (var item in GetIDData)
                            {
                                GetID = item.ID;
                            }
                        }

                        var SalesLine = DetShipBL.DryShipmentSellLiModel;
                        int rowsAffected;

                        for (int i = 0; i < SalesLine.Count(); i++)
                        {
                            var SalesLineparameter = new DynamicParameters();
                            SalesLineparameter.Add("Quantity", DetShipBL.DryShipmentSellLiModel[i].Quantity, DbType.Int32, ParameterDirection.Input);
                            SalesLineparameter.Add("Description", DetShipBL.DryShipmentSellLiModel[i].Description, DbType.String, ParameterDirection.Input);

                            SalesLineparameter.Add("UnitRate", DetShipBL.DryShipmentSellLiModel[i].UnitRate, DbType.Double, ParameterDirection.Input);

                            //parameters.Add("AmountUSD", DetShipBL.DryShipmentSellLiModel[i].AmountUSD, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("Currency", DetShipBL.DryShipmentSellLiModel[i].Currency, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("ExRate", DetShipBL.DryShipmentSellLiModel[i].ExRate, DbType.Double, ParameterDirection.Input);

                            SalesLineparameter.Add("CompanyName", DetShipBL.DryShipmentSellLiModel[i].CompanyName, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("IDCompany", DetShipBL.DryShipmentSellLiModel[i].IDCompany, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("ProformaInvoiceNo", ProformaInvoiceNo, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("ContainerNo", ContainerNo, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("PaymentTerm", DetShipBL.DryShipmentSellLiModel[i].PaymentTerm, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("UniversalSerialNr", DetShipBL.DryShipmentSellLiModel[i].UniversalSerialNr, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("IDInvoiceNr", GetID, DbType.String, ParameterDirection.Input);
                            SalesLineparameter.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);

                            string processquery = "Insert Into SalesInvoiceLineItemDry(Quantity, Description, UnitRate, Currency, ExRate, CompanyName, IDcompany, ProformaInvoiceNo, ContainerNo,PaymentTerm, UniversalSerialNr, IDInvoiceNr, CreatedBy)" +
                            " values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                            rowsAffected = conn.Execute(processquery, SalesLineparameter);
                        }

                        if (rowsAffectedInvoiceDry > 0)
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
                throw ex;
            }

            return errorLog;
        }

        public static ImportInvoiceDetails GetImportInvoiceDetailsJobRef(string JobRef)
        {
            ImportInvoiceDetails InvoiceRef = new ImportInvoiceDetails();
            ShipmentBL shipmentRef = new ShipmentBL();
            List<SalesInvoiceLineItemDry> salesLine = new List<SalesInvoiceLineItemDry>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parametersd = new DynamicParameters();
                    parametersd.Add("JobRefNo", JobRef, DbType.String, ParameterDirection.Input);
                    parametersd.Add("InvoiceType", Constant.Import, DbType.String, ParameterDirection.Input);

                    string querysd = "Select JobRefNo, BillingPartyAddress, BillingParty, InvoiceNo, ProformaInvoiceNo, InvoiceDate, CreditTerms, " +
                        " DueDate, MBLHAWB, HBLHAWB, VesselName, VoyageNo, LoadPort, DischargePort, ETD, ETA, " +
                        "  Grossweight, GrossWeightUnit, ContainerNo, AmountinUSDSUM, Remarks, AmountInWords, AmountinUSDSUMWTax, TaxAmountSum, " +
                        " UniversalSerialNr from SalesInvoiceDry where JobRefNo = ? AND InvoiceType = ? ";

                    var ShipmentData = conn.Query<SalesInvoiceDry>(querysd, parametersd);
                    foreach (var item in ShipmentData)
                    {
                        InvoiceRef.SalesInvoiceDryModel.JobRefNo = item.JobRefNo;
                        InvoiceRef.SalesInvoiceDryModel.BillingPartyAddress = item.BillingPartyAddress;
                        InvoiceRef.SalesInvoiceDryModel.BillingParty = item.BillingParty;
                        InvoiceRef.SalesInvoiceDryModel.InvoiceNo = item.InvoiceNo;
                        InvoiceRef.SalesInvoiceDryModel.ProformaInvoiceNo = item.ProformaInvoiceNo;
                        InvoiceRef.SalesInvoiceDryModel.InvoiceDate = item.InvoiceDate;
                        InvoiceRef.SalesInvoiceDryModel.CreditTerms = item.CreditTerms;

                        InvoiceRef.SalesInvoiceDryModel.DueDate = item.DueDate;
                        InvoiceRef.SalesInvoiceDryModel.MBLHAWB = item.MBLHAWB;
                        InvoiceRef.SalesInvoiceDryModel.HBLHAWB = item.HBLHAWB;
                        InvoiceRef.SalesInvoiceDryModel.VesselName = item.VesselName;
                        InvoiceRef.SalesInvoiceDryModel.VoyageNo = item.VoyageNo;

                        InvoiceRef.SalesInvoiceDryModel.LoadPort = item.LoadPort;
                        InvoiceRef.SalesInvoiceDryModel.DischargePort = item.DischargePort;
                        InvoiceRef.SalesInvoiceDryModel.ETD = item.ETD;
                        InvoiceRef.SalesInvoiceDryModel.ETA = item.ETA;

                        InvoiceRef.SalesInvoiceDryModel.Remarks = item.Remarks;
                        InvoiceRef.SalesInvoiceDryModel.Grossweight = item.Grossweight;
                        InvoiceRef.SalesInvoiceDryModel.GrossWeightUnit = item.GrossWeightUnit;
                        InvoiceRef.SalesInvoiceDryModel.ContainerNo = item.ContainerNo;
                        InvoiceRef.SalesInvoiceDryModel.AmountinUSDSUM = item.AmountinUSDSUM;
                        InvoiceRef.SalesInvoiceDryModel.AmountInWords = item.AmountInWords;

                        InvoiceRef.SalesInvoiceDryModel.AmountinUSDSUMWTax = item.AmountinUSDSUMWTax;
                        InvoiceRef.SalesInvoiceDryModel.TaxAmountSum = item.TaxAmountSum;

                        InvoiceRef.SalesInvoiceDryModel.UniversalSerialNr = item.UniversalSerialNr;
                    }

                    var parameterbl = new DynamicParameters();
                    parameterbl.Add("UniversalSerialNr", InvoiceRef.SalesInvoiceDryModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);
                    parameterbl.Add("PaymentTerm", "Collect", DbType.String, ParameterDirection.Input);

                    string querybl = "Select Quantity, Description, UnitRate, Currency, ExRate, " +
                                     " AmountUSD, TaxAmount, TaxPercent, ContainerNo, PaymentTerm" +
                                     " from SalesInvoiceLineItemDry where UniversalSerialNr = ? AND PaymentTerm = ?";

                    var BLData = conn.Query<SalesInvoiceLineItemDry>(querybl, parameterbl);
                    foreach (var item in BLData)
                    {
                        if (item.PaymentTerm == "Collect")
                        {
                            salesLine.Add(new SalesInvoiceLineItemDry
                            {
                                Quantity = item.Quantity,
                                Description = item.Description,
                                UnitRate = item.UnitRate,
                                Currency = item.Currency,
                                ExRate = item.ExRate,
                                AmountUSD = item.AmountUSD,
                                TaxAmount = item.TaxAmount,
                                TaxPercent = item.TaxPercent,
                                ContainerNo = item.ContainerNo
                            });
                        }
                    }

                    InvoiceRef.salesInvoiceLineItemDryModel = salesLine;

                    var parameterquote = new DynamicParameters();
                    parameterquote.Add("UniversalSerialNr", InvoiceRef.SalesInvoiceDryModel.UniversalSerialNr, DbType.String, ParameterDirection.Input);

                    string queryquote = "select EquipmentType, QuantityLifting from ShipmentDetailDry where UniversalSerialNr = ?";

                    var quotedata = conn.Query<ShipmentDetails>(queryquote, parameterquote);
                    foreach (var item in quotedata)
                    {
                        InvoiceRef.ShipmentDetailsModel.EquipmentType = item.EquipmentType;
                        InvoiceRef.ShipmentDetailsModel.QuantityLifting = item.QuantityLifting;
                    }
                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
            return InvoiceRef;
        }

        //Added By Sonali

        public static List<AgentAddressBook> GetAllAgentAddressBookData(SearchParameters filter)
        {
            string username = HttpContext.Current.User.Identity.Name;
            List<AgentAddressBook> addressBooks = new List<AgentAddressBook>();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string selectquery = "Select AB.ID, AB.Email, AB.PhoneNo, " +
                         "AB.IDCompany, AB.CompanyName From AgentAddressBook AB INNER JOIN UserAccountManagement UM ON AB.IDCompany = UM.IDCompany WHERE UM.Name = '" + username + "' AND";

                    if ((filter.CompanyName != null) || (filter.CompanyID != null))
                    {
                        if (filter.CompanyName != null)
                        {
                            selectquery += " AB.CompanyName = '" + filter.CompanyName + "' AND";
                        }
                        if (filter.CompanyID != null)
                        {
                            selectquery += " AB.ID = '" + filter.CompanyID + "' AND";
                        }
                    }

                    if (selectquery.LastIndexOf("AND") > 0)
                    {
                        selectquery = selectquery.Remove(selectquery.LastIndexOf("AND"));
                    }

                    selectquery += " Order By AB.ID DESC";

                    var AddressBookData = conn.Query<AgentAddressBook>(selectquery);
                    if (AddressBookData != null)
                    {
                        foreach (var AddressBook in AddressBookData)
                        {
                            addressBooks.Add(new AgentAddressBook
                            {
                                ID = AddressBook.ID,
                                //Owner = AddressBook.Owner,
                                Email = AddressBook.Email,
                                PhoneNo = AddressBook.PhoneNo,
                                IDCompany = AddressBook.IDCompany,
                                CompanyName = AddressBook.CompanyName,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return addressBooks;
        }

        public static AgentAddressBook GetAgentAddressBookfromID(string ID)
        {
            AgentAddressBook addressbookref = new AgentAddressBook();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ID", ID, DbType.String, ParameterDirection.Input);

                    string query = "Select ID, Email, PhoneNo, IDCompany, CompanyName " +
                        "from AgentAddressBook where ID = ?";

                    var agentaddressbook = conn.Query<AgentAddressBook>(query, parameters);
                    foreach (var item in agentaddressbook)
                    {
                        addressbookref.ID = item.ID;
                        addressbookref.Email = item.Email;
                        addressbookref.PhoneNo = item.PhoneNo;
                        addressbookref.IDCompany = item.IDCompany;
                        addressbookref.CompanyName = item.CompanyName;
                    }
                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
            return addressbookref;
        }

        public static ErrorLog SaveAddressBook(AgentAddressBook agentaddressbook)
        {
            string username = HttpContext.Current.User.Identity.Name;

            UserDetails userdetail = new UserDetails();
            try
            {
                var userRoleParam = new DynamicParameters();
                userRoleParam.Add("Name", username, DbType.String, ParameterDirection.Input);

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query = "Select IDCompany From UserAccountManagement  Where Name = ?";

                    var userdetails = conn.Query<UserDetails>(query, userRoleParam);
                    foreach (var item in userdetails)
                    {
                        userdetail.IDCompany = item.IDCompany;
                    }
                    var parameters = new DynamicParameters();
                    parameters.Add("IDCompany", userdetail.IDCompany, DbType.String, ParameterDirection.Input);

                    parameters.Add("CompanyName", agentaddressbook.CompanyName.ToUpperInvariant(), DbType.String, ParameterDirection.Input);
                    parameters.Add("Address", agentaddressbook.Address, DbType.String, ParameterDirection.Input);
                    parameters.Add("Country", agentaddressbook.Country, DbType.String, ParameterDirection.Input);

                    //parameters.Add("Owner", agentaddressbook.Owner, DbType.String, ParameterDirection.Input);
                    parameters.Add("Email", agentaddressbook.Email, DbType.String, ParameterDirection.Input);
                    parameters.Add("PhoneNo", agentaddressbook.PhoneNo, DbType.String, ParameterDirection.Input);

                    //parameters.Add("PortOfBusiness", agentaddressbook.PortOfBusiness, DbType.String, ParameterDirection.Input);
                    //parameters.Add("TypeOfBusiness", agentaddressbook.TypeOfBusiness, DbType.String, ParameterDirection.Input);
                    parameters.Add("Website", agentaddressbook.Website, DbType.String, ParameterDirection.Input);

                    //parameters.Add("Depot", agentaddressbook.Depot, DbType.String, ParameterDirection.Input);
                    parameters.Add("Remark", agentaddressbook.Remark, DbType.String, ParameterDirection.Input);
                    parameters.Add("Zipcode", agentaddressbook.Zipcode, DbType.String, ParameterDirection.Input);

                    //parameters.Add("CreditLimit", agentaddressbook.CreditLimit, DbType.String, ParameterDirection.Input);
                    //parameters.Add("CreditTerm", agentaddressbook.CreditTerm, DbType.String, ParameterDirection.Input);

                    parameters.Add("CreatedBy", username, DbType.String, ParameterDirection.Input);

                    parameters.Add("CustomAddress", agentaddressbook.CustomAddress, DbType.String, ParameterDirection.Input);

                    //string odbcQuery = "Insert Into AgentAddressBook(IDCompany,CompanyName,Address,Country,Owner,Email,PhoneNo,PortOfBusiness,TypeOfBusiness,Website,Depot,Remark,Zipcode,CreditLimit,CreditTerm,CreatedBy,CustomAddress)" +
                    //    "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    string odbcQuery = "Insert Into AgentAddressBook(IDCompany,CompanyName,Address,Country,Email,PhoneNo,Website,Remark,Zipcode,CreatedBy,CustomAddress)" +
                    "Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    int rowsAffected = conn.Execute(odbcQuery, parameters);

                    if (rowsAffected > 0)
                    {
                        errorLog.IsError = false;
                    }
                    else
                    {
                        errorLog.IsError = true;
                    }

                    string query1 = "Select MAX (ID) as ID from AgentAddressBook where createdBy= '" + username + "'";

                    var addressbookData = conn.Query<AgentAddressBook>(query1, parameters);
                    foreach (var item in addressbookData)
                    {
                        agentaddressbook.ID = item.ID;
                    }
                }
                errorLog.IsError = false;
                errorLog.ErrorMessage = agentaddressbook.ID;
            }
            catch (Exception ex)
            {
                errorLog.IsError = true;
                errorLog.ErrorMessage = ex.Message.ToString();
            }
            return errorLog;
        }

        public static ErrorLog UpdateAgentAddressDetails(AgentAddressBook agentadd)
        {
            string username = HttpContext.Current.User.Identity.Name;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("CompanyName", agentadd.CompanyName, DbType.String, ParameterDirection.Input);
                    // parameters.Add("Owner", agentadd.Owner, DbType.String, ParameterDirection.Input);
                    parameters.Add("Address", agentadd.Address, DbType.String, ParameterDirection.Input);
                    parameters.Add("Country", agentadd.Country, DbType.String, ParameterDirection.Input);
                    parameters.Add("Zipcode", agentadd.Zipcode, DbType.String, ParameterDirection.Input);
                    parameters.Add("Email", agentadd.Email, DbType.String, ParameterDirection.Input);
                    parameters.Add("PhoneNo", agentadd.PhoneNo, DbType.String, ParameterDirection.Input);
                    parameters.Add("Website", agentadd.Website, DbType.String, ParameterDirection.Input);
                    parameters.Add("CustomAddress", agentadd.CustomAddress, DbType.String, ParameterDirection.Input);
                    parameters.Add("Remark", agentadd.Remark, DbType.String, ParameterDirection.Input);
                    parameters.Add("ModifiedBy", username, DbType.String, ParameterDirection.Input);


                    string odbcQuery = "Update AgentAddressBook Set CompanyName = ?, Address = ?, Country = ?, Zipcode = ?, Email = ?, PhoneNo =?, Website = ?, CustomAddress = ?, Remark = ?, ModifiedBy = ? " +

                        "Where ID = '" + agentadd.ID + "'";

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

        public static ErrorLog DeleteAgentAddress(string ID)
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("ID", ID, DbType.String, ParameterDirection.Input);
                    string query = "DELETE FROM AgentAddressBook Where  ID = ?";
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

        public static AgentAddressBook GetAddressBookFromID(string ID)
        {
            AgentAddressBook addressBook = new AgentAddressBook();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ID", ID, DbType.String, ParameterDirection.Input);

                    string query = "Select * from AgentAddressBook where ID = ?";

                    var rateRequestData = conn.Query<AgentAddressBook>(query, parameters);
                    foreach (var item in rateRequestData)
                    {
                        addressBook.ID = item.ID;
                        addressBook.IDCompany = item.IDCompany;
                        addressBook.CompanyName = item.CompanyName;
                        addressBook.Address = item.Address;
                        addressBook.Country = item.Country;
                        addressBook.Email = item.Email;
                        addressBook.PhoneNo = item.PhoneNo;
                        addressBook.Remark = item.Remark;
                        addressBook.Website = item.Website;
                        addressBook.Zipcode = item.Zipcode;
                        addressBook.CustomAddress = item.CustomAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return addressBook;
        }

        public static bool GetCompanyNames(string companyName)
        {
            string username = HttpContext.Current.User.Identity.Name;
            UserDetails userdetail = new UserDetails();
            try
            {
                var userRoleParam = new DynamicParameters();
                userRoleParam.Add("Name", username, DbType.String, ParameterDirection.Input);

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query1 = "Select IDCompany From UserAccountManagement  Where Name = ?";

                    var userdetails = conn.Query<UserDetails>(query1, userRoleParam);
                    foreach (var item in userdetails)
                    {
                        userdetail.IDCompany = item.IDCompany;
                    }

                    var parameters1 = new DynamicParameters();
                    parameters1.Add("IDCompany", userdetail.IDCompany, DbType.String, ParameterDirection.Input);

                    string query = "Select CompanyName from AgentAddressBook where IDCompany =? ";

                    var CompanyNameData = conn.Query<AgentAddressBook>(query, parameters1);
                    foreach (var item in CompanyNameData)
                    {
                        if (companyName.ToUpper() == item.CompanyName.ToUpper())
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }               

        public static bool GetCompanyNamesForUpdate(string CompName, string ID)
        {
            string username = HttpContext.Current.User.Identity.Name;
            UserDetails userdetail = new UserDetails();
            string SelectID = "";
            try
            {
                var userRoleParam = new DynamicParameters();
                userRoleParam.Add("Name", username, DbType.String, ParameterDirection.Input);

                using (OdbcConnection conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["LegendDryLogistics"].ConnectionString))
                {
                    string query1 = "Select IDCompany From UserAccountManagement  Where Name = ?";

                    var userdetails = conn.Query<UserDetails>(query1, userRoleParam);
                    foreach (var item in userdetails)
                    {
                        userdetail.IDCompany = item.IDCompany;
                    }

                    var parameters1 = new DynamicParameters();
                    parameters1.Add("IDCompany", userdetail.IDCompany, DbType.String, ParameterDirection.Input);

                    string query = "Select CompanyName from AgentAddressBook where IDCompany =? ";

                    var ComanyNameData = conn.Query<AgentAddressBook>(query, parameters1);
                    foreach (var item in ComanyNameData)
                    {
                        if (CompName.ToUpper() == item.CompanyName.ToUpper())
                        {
                            var parametersCom = new DynamicParameters();
                            parametersCom.Add("CompanyName", item.CompanyName, DbType.String, ParameterDirection.Input);
                            parametersCom.Add("IDCompany", userdetail.IDCompany, DbType.String, ParameterDirection.Input);

                            string queryCom = "Select ID from AgentAddressBook where CompanyName =? and IDCompany =? ";

                            var companyData = conn.Query<AgentAddressBook>(queryCom, parametersCom);
                            foreach (var item1 in companyData)
                            {
                                SelectID = item1.ID;
                            }
                            var count = 1;
                            if (SelectID.ToUpper() == ID.ToUpper())
                            {
                                count++;
                            }
                            if (count > 1)
                            { return false; }
                            else { return true; }
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}