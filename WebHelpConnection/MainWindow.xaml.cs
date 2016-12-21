using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;




// avancement :
// le serveur ouvre la page et la charge dan le browser
// mais il y a des erreurs javascript et surtout ca ne lit plus une fois que ca a envoye la page
// on dirait que le client tcp est arrete apres le flush

    // a voir

namespace WebHelpConnection
{


   


    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
            private StreamWriter SwSender;
            private StreamReader SrReciever;
            private Thread thrMessaging;
            private delegate void UpdateLogCallBack(string strMessage);

            private StreamWriter txt_Log;
            private IPAddress IPAddress;

            private TcpClient _Client;

        private string lapage;

            public MainWindow()
            {
                InitializeComponent();
                lapage = System.IO.File.ReadAllText(@"D:\essais\WebHelpConnection\WebHelpConnection\test.html");
            

        }


        private Boolean _isRunning;
        private TcpListener _server;
        private Thread tlisten;

        private void btn_Go_Click(object sender, RoutedEventArgs e)
        { LeBrowser.Navigate("http://127.0.0.1:33333");  // apres ca le serveur a demarre
            //SendClient(_Client); // On envoie une page


        }


    private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            tlisten = new Thread(new ThreadStart(listen));
            tlisten.Start();

        }


        // thread lancant le listener. tant que pas de page -> ca bloque. donc ca ne marchera que quand l'app aura charge une page
        // avec lebrowser.navigate(127.0.0.1:3333)
        private void listen()
        {try
            {
                IPAddress IP = IPAddress.Parse("127.0.0.1");

                _server = new TcpListener(IP, 33333);
            _server.Start();

            _isRunning = true;

        }
   
            catch (ArgumentNullException exec)
            {
                Console.WriteLine("ArgumentNullException caught!!!");
                Console.WriteLine("Source : " + exec.Source);
                Console.WriteLine("Message : " + exec.Message);
            }

            catch (FormatException exec)
            {
                Console.WriteLine("FormatException caught!!!");
                Console.WriteLine("Source : " + exec.Source);
                Console.WriteLine("Message : " + exec.Message);
            }

            /*catch (Exception exec)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + exec.Source);
                Console.WriteLine("Message : " + exec.Message);
            }

            if (!_server.Pending())
            {
                MessageBox.Show("pas pret", "aie");
                
                
                return;
            }*/

            _Client = _server.AcceptTcpClient();

            // client found.
            // create a thread to handle communication
            Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
            t.Start(_Client);
        
            }


        // envoi de qqch au client
        // normalement on envoie une jolie page avec son script dedans pour renvoyer plein de messages au server
        public void SendClient(TcpClient client)
        {
            // retrieve client from parameter passed to thread
         if (client == null) // ca demarre pas au quart de seconde -> faire +sieurs essais
                return; 

            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            sWriter.WriteLine(lapage);
            sWriter.Flush();

        }


        // thread pour traiter ce qui arrive du client

        public void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;

            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested

            Boolean bClientConnected = true;
            String sData = null;


            // to write something back.
             sWriter.WriteLine(lapage);
             sWriter.Flush();

            while (bClientConnected)
            {
                // reads from stream
                sData = sReader.ReadLine();

                // shows content on the console.
                if (sData == null)
                    Console.WriteLine("Client &gt; " + sData);
                else
                    Console.WriteLine("+" );

                // to write something back.
                // sWriter.WriteLine("Meaningfull things here");
                // sWriter.Flush();
            }
        }

            private void UpdateLog(string strMessage)
            {
                //txt_Log.AppendText(strMessage);
            }
        }

    }

