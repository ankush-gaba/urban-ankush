using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Messaging;

namespace TestQueueService
{
    public partial class QueueService : ServiceBase
    {

        public const string ReadQueuePath = "desktop-t6vk5ip\\private$\\ReadMessage";      //Read Message queue path
        bool IsQueueEmpty = false;      //Initialize IsQueueEmpty to false.
        public QueueService()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This function reads the message from the queue and writes the message of the queue to the log file.
        /// </summary>
        public void ReadQueue()
        {

            try
            {
                while (true)
                {
                    //Set the path to the message queue object. 
                    MessageQueue mqQueue = new MessageQueue(ReadQueuePath);
                    //Setting the message formatter as XmlMessageFormatter
                    mqQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                    //Message object for reading messages
                    Message mMessage = mqQueue.Receive(new TimeSpan(0));
                    IsQueueEmpty = false;
                    if (!IsQueueEmpty)
                    {
                        //Writes the message to the log file.
                        Library.WriteErrorLog("Message is:" + mMessage.Body.ToString());
                    }

                    //Checks after every 5 minutes.
                    Thread.Sleep(300000);
                }
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    IsQueueEmpty = true;
                }
                if (IsQueueEmpty)
                {
                    Library.WriteErrorLog("No message was in the queue");
                }
            }
        }
        /// <summary>
        /// This function writes the message in the specified queue
        /// </summary>
        public void SendQueue()
        {
            try
            {
                //Set the path to the message queue object
                MessageQueue mqQueue = new MessageQueue(ReadQueuePath);
                //Sends the message to the queue.
                mqQueue.Send("Have a nice day", "Label");
            }
            catch (Exception ex)
            {

                Library.WriteErrorLog("Exception:" + ex.Message);

            }
        }
        
        

        protected override void OnStart(string[] args)
        {

            Library.WriteErrorLog("Service Started");
            //Creates a tThread1 object and initialize it with the function SendQueue.
            Thread tThread1 = new Thread(new ThreadStart(SendQueue));
            //Starts the execution of the thread.
            tThread1.Start();
            //Creates a tThread2 object and initialize it with the function ReadQueue.
            Thread tThread2 = new Thread(new ThreadStart(ReadQueue));
            //Starts the execution of the thread.
            tThread2.Start();
        }

        protected override void OnStop()
        {
            Library.WriteErrorLog("Service Stopped");
        }
    }
}
