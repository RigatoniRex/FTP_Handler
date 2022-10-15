using System.Net.Sockets;
using System.Security.Cryptography;
using System.Net;
using System.Text;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, I am Server!");

var listener = new TcpListener(IPAddress.Any, 5000);
listener.Start();
System.Console.WriteLine("Listening for client");

var tcpClient = listener.AcceptTcpClient();
tcpClient.ReceiveTimeout = 10000;
System.Console.WriteLine("Client Accepted");

string _username = null;
string dataIp = null;
int dataPort = 0;
TcpListener _passiveListener = null;

using (StreamReader reader = new StreamReader(tcpClient.GetStream(), Encoding.ASCII))
using (StreamWriter writer = new StreamWriter(tcpClient.GetStream(), Encoding.ASCII))
{
    writer.WriteLine("220 Ready!");
    writer.Flush();

    string line = null;
    System.Console.WriteLine("Waiting for data");
    while (!string.IsNullOrEmpty(line = reader.ReadLine()))
    {
        Console.WriteLine(line);

        string response = null;

        string[] command = line.Split(' ');

        string cmd = command[0].ToUpperInvariant();
        string arguments = command.Length > 1 ? line.Substring(command[0].Length + 1) : null;

        switch (cmd)
        {
            case "USER":
                response = User(arguments);
                break;
            case "PASS":
                response = Password(arguments);
                break;
            case "PWD":
                response = "257 \"/\" is current directory.";
                break;
            case "QUIT":
                response = "221 Service closing control connection";
                break;
            case "TYPE":
                response = "200 OK";
                break;
            case "PORT":
                response = Port(arguments);
                break;
            case "STOR":
                response = "200 OK";
                break;
            case "PASV":
                response = Pasv(tcpClient);
                break;
            default:
                response = "502 Command not implemented";
                break;
        }
        writer.WriteLine(response);
        writer.Flush();

        if (response.StartsWith("221"))
        {
            break;
        }

    }
}

string User(string username)
{
    _username = username;

    return "331 Username ok, need password";
}

string Password(string password)
{
    if (true)
    {
        return "230 User logged in";
    }
    else
    {
        return "530 Not logged in";
    }
}

string Port(string port)
{
    int[] IpAddress = new int[4];
    byte[] Port = new byte[2];
    if (BitConverter.IsLittleEndian)
        Array.Reverse(Port);
    dataIp = String.Join('.', IpAddress.Select(x => x.ToString()));
    dataPort = BitConverter.ToInt16(Port);
    return "200 OK";
}

string Pasv(TcpClient client)
{
    IPAddress localAddress = ((IPEndPoint)client.Client.LocalEndPoint).Address;

    _passiveListener = new TcpListener(localAddress, 0);
    _passiveListener.Start();

    IPEndPoint localEndpoint = ((IPEndPoint)_passiveListener.LocalEndpoint);

    byte[] address = localEndpoint.Address.GetAddressBytes();
    short port = (short)localEndpoint.Port;

    byte[] portArray = BitConverter.GetBytes(port);

    if (BitConverter.IsLittleEndian)
        Array.Reverse(portArray);

    return string.Format("227 Entering Passive Mode ({0},{1},{2},{3},{4},{5})",
                  address[0], address[1], address[2], address[3], portArray[0], portArray[1]);
}
