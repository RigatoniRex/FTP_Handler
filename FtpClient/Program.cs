using System.Net.Sockets;
using System.Security.Cryptography;
using System.Net;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, I am Client!");

FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1:5000/testFile.txt");
System.Console.WriteLine("Request made!");

request.Method = WebRequestMethods.Ftp.UploadFile;
// using(Stream fileStream = File.OpenRead(@"D:\CodingProjects\Sandbox\C#\Ftp\testFile.txt"))
// using(Stream ftpStream = request.GetRequestStream()){
//     fileStream.CopyTo(ftpStream);
// }

request.UsePassive = true;
//request.Timeout = 10000;
var response = request.GetResponse();