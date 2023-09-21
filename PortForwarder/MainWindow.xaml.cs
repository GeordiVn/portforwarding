using PortMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
namespace PortForwarder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        
        public MainWindow()
        {
            InitializeComponent();
            

            readIpPortListAndPutInTextBox();

            //bijvoorbeeld 127.0.0.1:8080 & 192.168.0.110:9090;

            buttonStart.Click += ButtonStart_Click;
            //(s, e) => 
            // {
            // startListening();
            // };
            buttonAddRule.Click += ButtonAddRule_Click;

        }

        private void ButtonAddRule_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("clicked");
            textBoxInfo.Text += "clicked!";
            if (addLocalIpTextBox.Text.Length > 0 && addLocalPortTextBox.Text.Length > 0 && addRemoteIpTextBox.Text.Length > 0 && addRemotePortTextBox.Text.Length > 0)
            {
                string lineToAddToFile = addLocalIpTextBox.Text + ":" + addLocalPortTextBox.Text + "&" + addRemoteIpTextBox.Text + ":" + addRemotePortTextBox.Text + ";";

                File.AppendAllText(Environment.CurrentDirectory + "/IpPortList.txt", "\n" + lineToAddToFile);
                textBoxInfo.Text += "added ??";
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            string[] ipPortListLineSplit = textBoxListIpRemote.Text.Split(';');


            foreach (string line in ipPortListLineSplit)
            {
                if (line.Contains(":"))
                {
                    string[] ipPortSplit = line.Split('&');
                    string[] ipPortLocal = ipPortSplit[0].Split(":");
                    string[] ipPortRemote = ipPortSplit[1].Split(":");
                    try
                    {

                        Thread thread = new Thread(new ThreadStart(() => {

                            try
                            {
                                Forward forward = new Forward(textBoxInfo, Application.Current);

                                forward.Start(new System.Net.IPEndPoint(IPAddress.Parse(ipPortLocal[0].Trim()), Int32.Parse(ipPortLocal[1].Trim())),
                                              new System.Net.IPEndPoint(IPAddress.Parse(ipPortRemote[0].Trim()), Int32.Parse(ipPortRemote[1].Trim())));
                            }
                            catch (Exception ex) { Application.Current.Dispatcher.Invoke(new Action(() => { textBoxInfo.AppendText(ex.Message + "\n" + "Ip local: " + ipPortLocal[0] + " Port local: " + ipPortLocal[1] + " Ip remote: " + ipPortRemote[0] + " Port remote: " + ipPortRemote[1]); })); }



                        }));
                        thread.Start();
                    }
                    catch (Exception ex) { textBoxInfo.AppendText(ex.Message + "\n"); }


                }
            }
        }

      

        public void readIpPortListAndPutInTextBox() 
        {
            string[] ipPortListFile = File.ReadAllLines(Environment.CurrentDirectory+"/IpPortList.txt");

            foreach (string ipPort in ipPortListFile) 
            {
                if (ipPort.Length > 5) 
                {
                    textBoxListIpRemote.AppendText(ipPort+"\n");
                }
            }
        }
    }
}
