using System.Net;
using System.Net.Sockets;
using System.Text;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, I am Server!");

FtpServer server = new FtpServer();
server.Start(IPAddress.Any, 5000, 10000, true);