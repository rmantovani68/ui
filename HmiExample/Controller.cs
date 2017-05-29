#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;

using OMS.Core.Communication;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Threading;
using HmiExample.Properties;
using System.Threading;
using System.Windows;
using System.Collections.ObjectModel;
#endregion

namespace HmiExample
{
    public class Controller
    {
        #region Singleton
        private static Controller _instance;
        public static Controller Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new Controller();
                }
                return _instance;
            }
        }
        #endregion Singleton

        #region Public Properties

        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MDSClient mdsClient {get; private set;}
        
        public string ApplicationName {get; private set;}
        
        public string ManagerApplicationName {get; private set;}
        
        public Model model { get; private set; }

        #endregion Public Properties
        
        #region Private Properties

        private DispatcherTimer timer { get; set; }
        private short LoopTime { get; set; }
        
        #endregion Private Properties

        #region Constructor
        private Controller()
        {
            // Name of this application: HMIClient
            ApplicationName = "HMIClient";
            // Name of the plc server application: PLCServer
            ManagerApplicationName = "Manager";
            
            model = new Model();

            timer = new DispatcherTimer();

            // Create MDSClient object to connect to DotNetMQ
            mdsClient = new MDSClient(ApplicationName);

            // Connect to DotNetMQ server
            try
            {
                mdsClient.Connect();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            // Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += hmi_MessageReceived;

            // timer 
            timer.Interval = TimeSpan.FromMilliseconds(LoopTime);
            timer.Tick += timer_Tick;
            timer.IsEnabled = true;

            // connessione al manager
            ManagerConnect();
        }
        #endregion

        
        #region Public Methods

        public bool SubscribeProperty(PropertyItem prop)
        {
            bool RetValue = true;

            // se già presente non lo aggiungo
            if (model.ListProperties.Contains(prop))
            {
                Logger.InfoFormat("Property {0} già presente", prop.Path);
                return false;
            }


            //Create a DotNetMQ Message to send 
            var message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = ManagerApplicationName;

            //Create a message
            var MsgData = new PropertyData
            {
                MsgCode = MsgCodes.SubscribeProperty,
                Prop = new Property() { ObjPath = prop.Path }
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
#if ELIMINATO
                var responseMessage = message.SendAndGetResponse();

                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;

                //Acknowledge received message
                responseMessage.Acknowledge();
#endif
            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }

            // da spostare dove arriva la conerma della addproperty
            if(RetValue)
            {
                Logger.InfoFormat("Aggiunta {0}", prop.Path);

                model.ListProperties.Add(prop);
            }

            return RetValue;
        }

        public bool RemoveProperty(PropertyItem prop)
        {
            bool RetValue = true;

            //Create a DotNetMQ Message to send 
            var message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = ManagerApplicationName;

            //Create a message
            var MsgData = new PropertyData
            {
                MsgCode = MsgCodes.RemoveProperty,
                Prop = new Property() { ObjPath = prop.Path }
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
#if ELIMINATO
                var responseMessage = message.SendAndGetResponse();

                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;

                //Acknowledge received message
                responseMessage.Acknowledge();
#endif

            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }
            // spostare dove arriverà messaggio di rimozione property
            if (RetValue)
            {
                model.ListProperties.Remove(prop);
            }
            return RetValue;
        }

        private bool ManagerConnect()
        {
            bool RetValue = true;

            //Create a DotNetMQ Message to send 
            var message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = ManagerApplicationName;

            //Create a message
            var MsgData = new PropertyData
            {
                MsgCode = MsgCodes.ConnectSubscriber,
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
#if ELIMINATO
                var responseMessage = message.SendAndGetResponse();

                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;

                //Acknowledge received message
                responseMessage.Acknowledge();
#endif
            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }

            return RetValue;
        }

        private bool ManagerDisconnect()
        {
            bool RetValue = true;

            //Create a DotNetMQ Message to send 
            var message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = ManagerApplicationName;

            //Create a message
            var MsgData = new PropertyData
            {
                MsgCode = MsgCodes.DisconnectSubscriber,
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
#if ELIMINATO
                var responseMessage = message.SendAndGetResponse();

                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;

                //Acknowledge received message
                responseMessage.Acknowledge();
#endif
            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }

            return RetValue;
        }

        public void Close()
        {
            ManagerDisconnect();

            mdsClient.Disconnect();
        }

        #endregion Public Methods

        #region Private Methods

        private void timer_Tick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message parameters</param>
        private void hmi_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                // Get message 
                var Message = e.Message;
                // Get message data
                var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as MsgData;

                switch (MsgData.MsgCode)
                {
                    case MsgCodes.PropertiesChanged:
                        /* gestione da fare */
                        break;
                    case MsgCodes.PropertyChanged:
                        PropertyChanged(Message);
                        break;

                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            // Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }

        private bool PropertyChanged(Property prop)
        {
            bool RetValue = true;

            var property = model.ListProperties.FirstOrDefault(item => item.Path == prop.ObjPath);
            if (property != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    property.Value = prop.Value;
                }));
            }
            else
            {
                RetValue = false;
            }

            return RetValue;
        }

        private bool PropertyChanged(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PropertyData;
            var prop = MsgData.Prop;

            Logger.InfoFormat("Ricevuto Messaggio {1}:{2} da {0}", Message.SourceApplicationName, prop.ObjPath, prop.Value);

            RetValue = PropertyChanged(prop);

            if(RetValue==false)
            {
                Logger.InfoFormat("Property {0} non trovata", prop.ObjPath);
            }

            return RetValue;
        }


        #endregion Private Methods

    }
}
