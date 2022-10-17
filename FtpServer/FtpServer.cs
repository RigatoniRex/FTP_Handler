using System.Net.Sockets;
using System.Security.Cryptography;
using System.Net;
using System.Text;

public class FtpServer
{
    public TcpListener? listener { get; private set; }
    private List<FtpConnection> connections = new List<FtpConnection>();
    public FtpConnection GetConnection(int index)
    {
        return connections[index];
    }
    public FtpConnection[] GetConnections(){
        return connections.ToArray();
    }
    public FtpServer()
    {
    }
    public bool Start(IPAddress address, int port, uint timeout, bool keepAlive)
    {
        if (listener == null)
        {
            try
            {
                listener = new TcpListener(address, port);
                listener.Start();
                System.Console.WriteLine("Ftp Control Server Initialized");
                var result = listener.BeginAcceptTcpClient(HandleAcceptTcpClient, listener);
                System.Console.WriteLine("Waiting for client...");
                if (!result.AsyncWaitHandle.WaitOne((int)timeout))
                {
                    //Timeout
                    listener.EndAcceptTcpClient(result);//stop accepting clients.
                    this.Stop();
                    System.Console.WriteLine("Ftp Control Server Listener timed out... No Client Request");
                    if (keepAlive)
                    {
                        System.Console.WriteLine("Keep-Alive Active, Restarting...");
                        Start(address, port, timeout, keepAlive);
                    }
                    return false;
                }
            }
            catch (ArgumentNullException nullEx)// Can be thrown by TcpListener Constructor
            {
                System.Console.WriteLine("Aborting...");
                this.Stop();
                throw nullEx;
            }
            catch (ArgumentOutOfRangeException outOfRngEx)// Can be thrown by TcpListener Constructor
            {
                System.Console.WriteLine("Aborting...");
                this.Stop();
                throw outOfRngEx;
            }
            catch (Exception ex)
            {
                this.Stop();
                System.Console.WriteLine("Unexpected Exception: " + ex.Message);
                if (!string.IsNullOrEmpty(ex.InnerException?.Message))
                    System.Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                if (keepAlive)
                {
                    System.Console.WriteLine("Keep-Alive Active, Restarting...");
                    Start(address, port, timeout, keepAlive);
                }
                else
                {
                    System.Console.WriteLine("Aborting...");
                    return false;
                }
            }
        }
        else
            throw new ApplicationException("Ftp Control Server already started");
        System.Console.WriteLine("Ftp Control Server Started Successfully");
        return true;
    }
    public void Stop()
    {
        if (listener != null)
        {
            listener.Stop();
            listener = null;
        }
    }
    private void HandleAcceptTcpClient(IAsyncResult result)
    {
        var conn = new FtpConnection(listener.EndAcceptTcpClient(result));
        System.Console.WriteLine("Client connected!");
    }
}