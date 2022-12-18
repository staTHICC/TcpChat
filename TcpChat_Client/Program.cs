using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;    //для кодирования
using System.Threading.Tasks;

namespace TcpChat_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            Console.WriteLine("[CLIENT]");

            Socket socket_sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse("127.0.0.1");   //server ip
            IPEndPoint endRemotePoint = new IPEndPoint(address, 7632);   //server port

            Console.WriteLine("Нажмите Enter для подключения");
            Console.ReadLine();

            // подключаемся к удалённой точке
            socket_sender.Connect(endRemotePoint);

            //работа с именем клиента
            Console.Write("Пожалуйста, введите Ваше имя: ");
            string name = Console.ReadLine();
            SendMessage(socket_sender, name);


            Action<Socket> taskSendMessage = SendMessageFortask;
            IAsyncResult res = taskSendMessage.BeginInvoke(socket_sender, null, null);

            Action<Socket> taskReceiveMessage = ReceiveMessageForTask;
            IAsyncResult resReceive = taskReceiveMessage.BeginInvoke(socket_sender, null, null);


            taskSendMessage.EndInvoke(res);
            taskReceiveMessage.EndInvoke(resReceive);

            Console.ReadLine();
        }

        public static void SendMessageFortask(Socket socket)
        {
            string message = Console.ReadLine();
            SendMessage(socket, message);
        }

        public static void ReceiveMessageForTask(Socket socket)
        {
            while (true)
            {
                string answer = ReceiveMessage(socket);
                Console.WriteLine(answer);
            }
        }

        public static void SendMessage(Socket socket, string message)
        {
            byte[] bytes_answer = Encoding.Unicode.GetBytes(message);
            socket.Send(bytes_answer);
        }

        public static string ReceiveMessage(Socket socket)
        {
            byte[] bytes = new byte[1024];
            int num_bytes = socket.Receive(bytes);
            return Encoding.Unicode.GetString(bytes, 0, num_bytes);
        }

    }
}
