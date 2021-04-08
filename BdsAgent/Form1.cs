﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace BdsAgent
{

    public partial class Form1 : Form
    {
        private static HubConnection connection = null;
        private static bool isConnected = false;
        static string hostName = "";
        static string myIp = "";
        static string userWindows = "";
        /*private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);*/
        public Form1()
        {
            InitializeComponent();
            StartService();
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        static void StartService()
        {

            // Load configuration
            /*var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config")); */
            //var a = GetDbConnection();
            var dataParam = new ParamFinger();
            var dataOutParam = new OutParam();
            Console.WriteLine("SignalR Client Application Started");
            /*log.Info("SignalR Client Application Started");*/

            myIp = "";
            userWindows = Environment.UserName.ToString().ToLower();


            Console.WriteLine("username : " + userWindows);
            ////Connect without Authentication

            SimpleConnection(userWindows, myIp);

            ////Connect through JWT Token (Required [Authorize] Attribute at the IntegrationHub Broker)
            ////JWTConnection();
            ///
            #region ~ Listeners ~
            connection.On<ParamFinger>("SendToAgent", (par) =>
            {

                dataParam = par;

                var ip = $"{par.ip}";

                Console.WriteLine("Agent client get response from " + par.userDomain);
                /*log.Info("Agent client get response from " + par.userDomain);*/

                RunUtils(par.userDomain, par.ip, par.operation, par.fingerTemplate, par.nik, par.wsid, "", par.hashedTemplate);

            });

            connection.On<string, string>("ReqConnDevice", (userDomain, operation) =>
            {
                Console.WriteLine("Agent client get request connection from " + userDomain);
                /*log.Info("Agent client get request connection from " + userDomain);*/
                RunUtils(userDomain, "", operation, "", "", "", "", "");

            });

            connection.On<string, string, string>("ReqFingerScanNasabah", (userDomain, operation, nik) =>
            {
                Console.WriteLine("Agent client get request verify finger nasabah from " + userDomain);
                /*log.Info("Agent client get request verify finger nasabah from " + userDomain);*/
                RunUtils(userDomain, "", operation, "", nik, "", "", "");

            });

            connection.On<string, string>("ReqPutEktp", (userDomain, operation) =>
            {
                Console.WriteLine("Agent client get request Put EKTP from " + userDomain);
                /*log.Info("Angular send request to put EKTP from " + userDomain);*/
                RunUtils(userDomain, "", operation, "1234567890123456", "", "", "", "");
            });

            connection.On<ParamFinger>("ReqDataDemography", (par) =>
            {
                Console.WriteLine("Agent client get request Data Demography from " + par.userDomain);
                /*log.Info("Angular send request to Get Data Demography from " + par.userDomain); */
                RunUtils(par.userDomain, "", par.operation, "", "", "", "", "");
            });

            connection.On<string, string, string>("ReqInputPinCard", (userDomain, operation, cardNumber) =>
            {
                Console.WriteLine("Agent client get request Put EKTP from " + userDomain);
                /*log.Info("Angular send request to put EKTP from " + userDomain);*/
                RunUtils(userDomain, "", operation, "", "", "", cardNumber, "");
            });

            connection.On<string, string>("ReqSwapCard", (userDomain, operation) =>
            {
                Console.WriteLine("Agent client get request Put EKTP from " + userDomain);
                /*log.Info("Angular send request to put EKTP from " + userDomain);*/
                RunUtils(userDomain, "", operation, "", "", "", "", "");
            });
            #endregion

            try
            {
                //await TryConnectAsync();
                TryConnectAsync();
            }
            catch (Exception ex)
            {
                /*log.Error(ex.ToString());*/
            }

            connection.Closed += Connection_Closed;


        }

        private async static Task Connection_Closed(Exception arg)
        {
            Console.WriteLine("Client Disconnected...");
            isConnected = false;
            await TryConnectAsync();
        }

        public async static Task TryConnectAsync()
        {
            while (!isConnected)
            {
                await connection.StartAsync().ContinueWith((task) =>
                {
                    isConnected = true;
                    if (task.IsCompleted)
                        Console.WriteLine("Client Connected...");
                    else
                        Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                });
            }
        }

        private static void SimpleConnection(string userDomain, string ip)
        {
            // Load configuration
            /* var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
             XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config")); */
            try
            {
                /*log.Info("Create Connection to Websocket Server");*/



                /*connection = new HubConnectionBuilder()
             .WithUrl("http://192.168.1.21:8585/signalHub?userdomain=" + userDomain.Trim().ToLower().ToString() + "&ip=" + ip.Trim().ToLower().ToString()).Build();*/

                connection = new HubConnectionBuilder()
                    .WithUrl("http://139.99.92.36:8585/signalHub?userdomain=" + userDomain.Trim().ToLower().ToString() + "&ip=" + ip.Trim().ToLower().ToString(), (opts) =>
                    {
                        opts.HttpMessageHandlerFactory = (message) =>
                        {
                            if (message is HttpClientHandler clientHandler)
                                // bypass SSL certificate
                                clientHandler.ServerCertificateCustomValidationCallback +=
                                    (sender, certificate, chain, sslPolicyErrors) => { return true; };
                            return message;
                        };
                    })
                    .Build();


                /*connection = new HubConnectionBuilder()
                 .WithUrl("http://139.99.92.36:8585/signalHub?userdomain=" + userDomain.Trim().ToString() + "&ip=" + ip.Trim().ToLower().ToString()).Build();*/
                /* connection = new HubConnectionBuilder()
                                 .WithUrl("http://10.5.176.93:5001/signalHub?userdomain=" + userDomain.Trim().ToString() + "&ip=" + ip.Trim().ToLower().ToString()).Build();

                   connection = new HubConnectionBuilder()
                    .WithUrl("http://192.168.1.21:8585/signalHub?userdomain=" + userDomain.Trim().ToLower().ToString() + "&ip=" + ip.Trim().ToLower().ToString()).Build();*/
                /* koneksi ke mas Jan
                connection = new HubConnectionBuilder()
                  .WithUrl("http://10.5.176.109:5001/signalHub?userdomain=" + userDomain.Trim().ToString() + "&ip=" + ip.Trim().ToLower().ToString()).Build();*/

            }
            catch (Exception err)
            {
                /*log.Error("Error!");
                log.Error(err); */
            }

        }

        static void RunUtils(string userDomain, string ip, string operation, string fingerTemplate, string nik, string wsid, string cardNumber, string hashIso)
        {
            // Load configuration
            /*var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));*/


            var dataOutParam = new OutParam();
            dataOutParam.userDomain = userDomain;
            dataOutParam.ip = ip;
            dataOutParam.operation = operation;
            Utils ut = new Utils(operation);

            /*log.Info("Check Type of Operation");*/
            if (operation == "1")
            {
                /*log.Info("Operation Type 1: Login Finger Print");*/

                var ektpDeviceStatus = ut.DeviceOpen();
                var ektpDevice = ut.EktpDevice(fingerTemplate, nik, wsid, hashIso);

                dataOutParam.errCode = "0";//ektpDevice.Error_code.ToString();
                dataOutParam.errMessage = ektpDevice.Err_message;

                // call signalR method
                /*log.Info("Invoke Websocket method ReceiveFromAgent");
                log.Info("To Deliver Data to Angular Client"); */
                ut.DeviceClose();
                connection.InvokeAsync("ReceiveFromAgent", dataOutParam);

            }
            else if (operation == "12")
            {
                var ektpDevice = ut.DeviceOpen();

                dataOutParam.errCode = "0";// ektpDevice.Error_code.ToString();//"0";
                dataOutParam.errMessage = ektpDevice.Err_message;

                ut.DeviceClose();
                // call signalR method
                connection.InvokeAsync("InfoConnDevice", dataOutParam);



            }
            else if (operation == "13")
            {
                var ektpDeviceStatus = ut.DeviceOpen();
                var putEktp = ut.PutEKTP();
                dataOutParam.errCode = "0";//putEktp.Error_code.ToString();//"0";
                dataOutParam.errMessage = putEktp.Err_message;
                ut.DeviceClose();
                // call signalR method
                connection.InvokeAsync("SendResultPutEktp", dataOutParam);
            }
            else if (operation == "14")
            {
                var ektpDeviceStatus = ut.DeviceOpen();
                var getDataDemo = ut.GetDataDemography(wsid, nik, "idAuth", "nikAuth", 20);
                dataOutParam.errCode = getDataDemo.Error_code.ToString();
                dataOutParam.errMessage = getDataDemo.Err_message;
                ut.DeviceClose();
                // call signalR method
                connection.InvokeAsync("SendResultDataDemography", dataOutParam);
            }
            else if (operation == "15")
            {
                var ektpDeviceStatus = ut.DeviceOpen();

                var verifyFingerNasabah = ut.VerifyFinger("2R", userDomain, nik);
                dataOutParam.errCode = "0";// verifyFingerNasabah.Error_code.ToString();//"0";
                                           //dataOutParam.errMessage = verifyFingerNasabah.Err_message;
                /*if (verifyFingerNasabah.Error_code == 0)
                {
                    var getDataDemo = ut.GetDataDemography(wsid, nik, "idAuth", "nikAuth", 20);
                    dataOutParam.errCode = getDataDemo.Error_code.ToString();
                    dataOutParam.errMessage = getDataDemo.Err_message;
                } */
                Demography demo = new Demography();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                demo.Nik = "1234567890";
                demo.Name = "Nama Dummy";
                demo.Address = "Alamat Dummy";

                demo.PlaceOfBirth = "";
                demo.DateOfBirth = "";
                demo.Gender = "";

                demo.RT = "";
                demo.RW = "";
                demo.Village = "";

                demo.SubDistrict = "";
                demo.City = "";
                demo.Province = "";

                demo.BloodType = "";
                demo.Religion = "";
                demo.MaritalStatus = "";
                demo.Occupation = "";
                demo.Nationality = "";
                demo.PlaceOfIssue = "";
                dataOutParam.errMessage = JsonSerializer.Serialize(demo, options);
                Console.WriteLine(dataOutParam.errMessage);
                ut.DeviceClose();
                // call signalR method
                connection.InvokeAsync("SendResultVerifyNasabah", dataOutParam);
            }
            else if (operation == "2")
            {
                var ektpDeviceStatus = ut.PinpadOpen();
                // EDC Swipe Card
                var cardSwipeResult = ut.SwipeCard("Please swipe card");
                dataOutParam.errCode = "0";//cardSwipeResult.Error_code.ToString();//"0";
                dataOutParam.errMessage = cardSwipeResult.Err_message;//"1234567890";

                ut.closeLibraryPinpad();
                // nanti call signalR method
                connection.InvokeAsync("SendResultSwapCard", dataOutParam);
            }
            else if (operation == "21")
            {
                var ektpDeviceStatus = ut.PinpadOpen();
                // EDC Verify Pin
                var verifyPinResult = ut.GetPin("Input Pin:", cardNumber);
                dataOutParam.errCode = "0"; verifyPinResult.Error_code.ToString();//"0";
                dataOutParam.errMessage = verifyPinResult.Err_message;
                ut.closeLibraryPinpad();
                // nanti call signalR method
                connection.InvokeAsync("SendResultInputPin", dataOutParam);
            }


        }
    }
}
