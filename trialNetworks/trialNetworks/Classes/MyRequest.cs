using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
namespace trialNetworks.Classes
{
    class MyRequest
    {
        private string hostName;
        private Socket socket;
        private string path;
        private string saveDir;
        private string response;
        private string contentType;
        private int contentLength;
        private string requestToSend;
        private string responseHeader;
        private bool FullDir;
        private string responseBody;
        private string httpType;
        private string status;
        private int port;
        List<byte> content;
        public MyRequest()
        {

        }
        public MyRequest(string url , string dir , bool fullDir)
        {
            FullDir = fullDir;
            saveDir = dir;
            hostName = getTheHostName(url);
            path = getThePath(url);
            httpType = "HTTP/1.1";
        }
        public void start()
        {

            try
            {
                sendGetRequest();

            }
            catch (Exception)
            {
                Console.WriteLine("error during sending request");
                return;
            }
            Console.WriteLine("Done sending request");
            try
            {
                recieveGetRequest();

            }
            catch (Exception)
            {
                Console.WriteLine("error during recieving responde");
                return;
            }
            
            if (connected())
                socket.Close();
        }
        public void connect(int p)
        {
            port = p;
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            try
            {
             
            socket.Connect(hostName, port);
            }
            catch (Exception)
            {
                Console.WriteLine("error during connecting to server ");
                return;
            }
                Console.WriteLine("connecting Done");
        }
        public void sendGetRequest()
        {
            if (socket.Connected != true)
            {
                Console.WriteLine("lost connection");
                return;

            }

            requestToSend = "GET "+path +" "+httpType+"\r\nHost: "+hostName+"\r\n\r\n";

            //Console.WriteLine(requestToSend);
            byte[] buffer = Encoding.ASCII.GetBytes(requestToSend);
            socket.Send(buffer);

        }
        
        public void recieveGetRequest()
        {
            content = new List<byte>();

            byte[] temp = new byte[1024];

            socket.ReceiveTimeout = 10000;

            int len = 0;

            Console.WriteLine("Downloading...");
            do
            {

                try
                {

                    len = socket.Receive(temp, temp.Length, 0);

                }
                catch
                {
                    break;
                }

                for (int i = 0; i < len; i++)
                    content.Add(temp[i]);
            } while (len != 0);

            //convert header from bytes to string
            responseHeader = getHeader();
            //get status code
            status = getResponseStatus();
            //check el status code
            if (status!="200")
            {
                if (status == "301")
                {
                    //need to try
                    string loc = "Location: ";
                    int locStart = find(responseHeader, 0, loc) + loc.Length;
                    int locEnd = find(responseHeader, locStart, "\r\n");
                    path = responseHeader.Substring(locStart, locEnd - locStart);
                    start();
                }
                else if (status=="400")
                {
                    //need to try

                    Console.WriteLine("400");

                }
                else if (status == "404")
                {

                    Console.WriteLine("404");
                    //need to try
                }
                return;
                //fe 7aga 3'lat
            }
            //get the content type
            contentType = getResponseContentType();

            //Console.WriteLine(responseHeader);
            int bodyStart = responseHeader.Length+4;

            //put the response body in to byte array to convert it
            List<byte> tem = new List<byte>();
            for (int i = bodyStart; i < content.Count; i++)
                tem.Add(content[i]);
            if (FullDir==false)
            {
                int fileNumber = 0;
                string fileName = @"\test";
                while(File.Exists(saveDir+fileName+'.'+contentType)==true)
                {
                    fileNumber++;
                    fileName = @"\test" + fileNumber.ToString();
                }
                saveDir += fileName + '.' + contentType ; 
            }
            File.WriteAllBytes(saveDir, tem.ToArray());
            Console.WriteLine("DONE");
        }
        public string getResponseStatus()
        {
            string tt = "";
            tt = responseHeader.Substring(find(responseHeader, 0, httpType) + httpType.Length+1 , 3);
            //Console.WriteLine(tt);
            return tt;
        }
        public string getResponseContentType()
        {
            string tt = "";
            string tofind = "Content-Type: ";

            int typeStart = find(responseHeader, 0, tofind) + tofind.Length;
            int extenStart = find(responseHeader, typeStart, "/") + 1;
            int extenLen = find(responseHeader, extenStart, "\r\n")-extenStart ;
            tt = responseHeader.Substring(extenStart, extenLen);
            //Console.WriteLine(tt);
            return tt;
        }
        public string getHeader()
        {
            string tt = "";
            for (int i = 0; i < content.Count; i++)
            {
                if (content[i] == (byte)'\r' && content[i + 1] == (byte)'\n' && content[i + 2] == (byte)'\r' && content[i + 3] == (byte)'\n')
                    return tt;
                tt += (char)content[i];
            }
            return tt;
        }
        public bool connected()
        {
            return socket.Connected;
        }

        public string getThePath(string url)
        {
            int hostNameEnd = find(url, 7, "/");
            return url.Substring(hostNameEnd);
        }
        public string getTheHostName(string url)
        {
            int hostNameEnd = find(url, 7, "/");
            return url.Substring(7, hostNameEnd - 7);
        }

        public int find(string st , int start , string st2)
        {
            for (int i = start; i < st.Count(); i++)
            {
                int c = 0;
                for (int j = 0; j < st2.Count(); j++)
                {

                    if (st[i + j] == st2[j])
                        c++;
                    else
                        break;
                }
                if (c == st2.Count())
                {
                    //Console.WriteLine(i);
                    return i;
                }
            }
            return -1;
        }
        public string HostName { get => hostName; set => hostName = value; }
        public string Path { get => path; set => path = value; }
        public string Response { get => response; set => response = value; }
        public string ContentType { get => contentType; set => contentType = value; }
        public int ContentLength { get => contentLength; set => contentLength = value; }
        public string RequestToSend { get => requestToSend; set => requestToSend = value; }
        public string Status { get => status; set => status = value; }
        public Socket Socket { get => socket; set => socket = value; }
        public int Port { get => port; set => port = value; }
        public string SaveDir { get => saveDir; set => saveDir = value; }
        public bool FullDir1 { get => FullDir; set => FullDir = value; }
    }
}
