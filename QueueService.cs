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
