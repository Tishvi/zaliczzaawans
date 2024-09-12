using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ResponseData;

namespace ClientServerExam;


public class Client
{
    private ushort _id;
    private string _hostname;
    private int _port;

    private List<Response> _dataCollected = new List<Response>();

    private void PrintAllDataCollected()
    {
        Console.WriteLine("Data Collected by Client " + _id + " : ");
        foreach (var data in _dataCollected)
        {
            if (data.FoundData == null)
            {
                Console.WriteLine("Invalid data collected for client: " + _id);
                continue;
            }
            foreach (var dataItem in data.FoundData)
            {
                Console.WriteLine(dataItem.Data);
            }
        }
        Console.WriteLine("End of data collected");
    }

private Queue<Request> _pendingRequests = new Queue<Request>();
    public Client()
    {
        _hostname = "127.0.0.1";
        _port = 2137;
    }

    public Client(string hostname, int port, ushort id)
    {
        _hostname = hostname;
        _port = port;
        _id = id;
    }

    public void QueueRequest(RequestType requestType, Type requestedClass)
    {
        _pendingRequests.Enqueue(new Request(requestType, _id, requestedClass.AssemblyQualifiedName));
    }
    public async void Connect()
    {
        TcpClient client = new TcpClient(_hostname, _port);
        NetworkStream nwStream = client.GetStream();
        Console.WriteLine("CLIENT: Connected client ID: " + _id);
        if (!client.Connected)
        {
            Console.WriteLine("CLIENT: Cannot connect to server");
            return;
        }
        bool requestHandled = true;
        while (true)
        {
            Request request;
            if (_pendingRequests.Count > 0 && requestHandled)
            {
                request = _pendingRequests.Dequeue();
                // Console.WriteLine("CLIENT: Pending request found: " + request.RequestType + " " + request.ClassType);
                //---data to send to the server---
                string textToSend = JsonSerializer.Serialize(request);
                //---create a TCPClient object at the IP and port no.---
            
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

                //---send the text---
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                requestHandled = false;
            }

            if (client.Connected && client.GetStream().DataAvailable)
            {
                //---read back the text---
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                string jsonReceived = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                Console.WriteLine("CLIENT: Received : " + jsonReceived);
                Response response = JsonSerializer.Deserialize<Response>(jsonReceived);
                _dataCollected.Add(response);
                requestHandled = true;
            }

            if (requestHandled && _pendingRequests.Count <= 0)
            {
                request = new Request(RequestType.CloseConnection, _id, "");
                Console.WriteLine("CLIENT: Client id: " + _id + " ends connection");
                string textToSend = JsonSerializer.Serialize(request);
                //---create a TCPClient object at the IP and port no.---
            
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

                //---send the text---
                Console.WriteLine("CLIENT: Sending : " + textToSend);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                client.Close();
                PrintAllDataCollected();
                break;
            }
        }
    }
}