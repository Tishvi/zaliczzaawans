using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ResponseData;

namespace ClientServerExam;

public class Server
{
    private readonly string _serverIP = "127.0.0.1";
    private readonly int _serverPort = 2137;
    private ResponseData.ResponseData _serverData;

    private Dictionary<ushort, TcpClient> _clients = new Dictionary<ushort, TcpClient>();
    private ushort _userCount = 0;
    private ushort _maxUsers = 3;
    public Server(string serverIP, int serverPort)
    {
        _serverIP = serverIP;
        _serverPort = serverPort;
        _serverData = new ResponseData.ResponseData();

        ImportantData importantData = new ImportantData("Some data");
        ImportantData importantData2 = new ImportantData("Another data");
        LessImportantData lessImportantData1 = new LessImportantData(12.0f);
        LessImportantData lessImportantData2 = new LessImportantData(12.0f);
        EvenImportanterData evenImportanterData = new EvenImportanterData(1);
        EvenImportanterData evenImportanterData2 = new EvenImportanterData(2);
        _serverData.PushData(importantData);
        _serverData.PushData(importantData2);
        _serverData.PushData(lessImportantData1);
        _serverData.PushData(lessImportantData2);
        _serverData.PushData(evenImportanterData);
        _serverData.PushData(evenImportanterData2);
    }

    public void PrintServerInfo()
    {
        Console.WriteLine("Server IP: " + _serverIP);
        bool bDataFound = false;
        Console.WriteLine("Server data: " + _serverData);
    }
    public void Listen()
    {
        
        //---listen at the specified IP and port no.---
        IPAddress localAdd = IPAddress.Parse(_serverIP);
        TcpListener listener = new TcpListener(localAdd, _serverPort);
        Console.WriteLine("SERVER: Listening...");
        listener.Start();
        while (true)
        {
            //---incoming client connected---

            foreach (var connectedClient in _clients)
            {
               HandleClient(connectedClient.Value);
            }
            TcpClient client = listener.AcceptTcpClient();
            if (_userCount + 1 > _maxUsers)
            {
                Console.WriteLine("SERVER: Max users reached, refusing connection");
                var response = new Response(ResponseType.ConnectionRefused, []);
                string responseString = JsonSerializer.Serialize(response);
                client.GetStream().Write(System.Text.Encoding.ASCII.GetBytes(responseString), 0, System.Text.Encoding.ASCII.GetByteCount(responseString));
                continue;
            }
            else
            {
                Task.Run(async () => HandleClient(client));
                // HandleClient(client);
            }
            
        }
    }

    private void HandleClient(TcpClient client)
    {
        while (true)
        {
            if (!client.GetStream().DataAvailable)
            {
                continue;
            }
            byte[] buffer = new byte[client.ReceiveBufferSize];

            //---read incoming stream---
            int bytesRead = client.GetStream().Read(buffer, 0, client.ReceiveBufferSize);

            //---convert the data received into a string---
            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            if (dataReceived.Length <= 0)
                continue;
            Request parsedData = JsonSerializer.Deserialize<Request>(dataReceived);
            _clients.TryAdd(parsedData.ClientID, client);
            if (parsedData.RequestType == RequestType.CloseConnection)
            {
                _clients.Remove(parsedData.ClientID);
                Console.WriteLine("SERVER: Connection with client: " + parsedData.ClientID + " closed");
                client.Close();
                break;
            }
            Console.WriteLine("SERVER: Client ID: " + parsedData.ClientID + "Requested type: " + parsedData.ClassType);

            bool bDataFound = false;
            Response response = new Response(ResponseType.DataReceived, null);
            List<IDataInterface> FoundData = new List<IDataInterface>();
            bDataFound = _serverData.GetData(Type.GetType(parsedData.ClassType), out FoundData);
            response.FoundData = FoundData;
            if(!bDataFound)
                Console.WriteLine("SERVER: Couldn't find data for request: \nClientId: " + parsedData.ClientID + "Requested type: " + parsedData.ClassType);
            //---write back the text to the client---
            else
            {
                Console.WriteLine("SERVER: Data found, sending data");
            }
            List<string> data = new List<string>(){"some data", "some other data"};
            
            string jsonString = JsonSerializer.Serialize(response);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(jsonString);
            Console.Write("SERVER: Sending data: " + jsonString + " to " + parsedData.ClientID + "\n\n");
            client.GetStream().Write(jsonBytes, 0, jsonBytes.Length);
        }
    }
}