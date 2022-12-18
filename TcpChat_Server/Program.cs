using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TcpChat_Server
{
    class Program
    {
        static string messageFromUser = "";

        static List<User> clientSockets = new List<User>();

        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.WriteLine("[SERVER]");

            Socket socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse("127.0.0.1");

            // создаём endpoint 127.0.0.1:7632
            IPEndPoint endPoint = new IPEndPoint(address, 7632);

            // привязываем сокет к endpoint
            socket.Bind(endPoint);

            socket.Listen(2); // переводим сокет в режим "слушать"

            Console.WriteLine("Ожидаем звонка от клиента...");

            while (true)
            {
                Socket socket_client = socket.Accept();  // ожидаем звонка от клиента

                User user = new User()
                {
                    Socket = socket_client,
                    Name = "Test"
                };

                clientSockets.Add(user);

                Console.WriteLine("Клиент на связи!");

                //создаём менеджера - приём сообщений
                Thread threadReceive = new Thread(ReceiveMessageForManager);
                threadReceive.Start(user);

                //создаём менеджера - отсылает сообщения
                Thread threadSend = new Thread(SendMessageForManager);
                threadSend.Start(user);
            }

            Console.ReadLine();
        }

        public static void SendMessage(Socket socket, string message)
        {
            byte[] bytes_answer = Encoding.Unicode.GetBytes(message);
            socket.Send(bytes_answer);
        }

        public static void SendMessageToAllUsers(string message)
        {
            foreach (var user in clientSockets)
            {
                SendMessage(user.Socket, message);
            }
        }

        public static void SendMessageForManager(object socketObj)
        {
            User user = (User)socketObj;
            while (true)
            {
                string sendMessage = Console.ReadLine();  
                //SendMessage(socket, sendMessage);
                SendMessageToAllUsers(user.Name + ", " + sendMessage);
            }
        }

        public static string ReceiveMessage(Socket socket)
        {
            byte[] bytes = new byte[1024];
            int num_bytes = socket.Receive(bytes);
            return Encoding.Unicode.GetString(bytes, 0, num_bytes);
        }

        public static void ReceiveMessageForManager(object socketObj)
        {
            User user = (User)socketObj;
                
            //получаем данные перед началом работы
            string name = ReceiveMessage(user.Socket);
            user.Name = name;

            //получаем сообщения от клиента
            while (true)
            {
                messageFromUser = ReceiveMessage(user.Socket);

                Console.WriteLine("[" + name + "]: " + messageFromUser);
            }
        }
    }

    public class User
    {
        public string Name { get; set; }
        public Socket Socket { get; set; }
    }
}
