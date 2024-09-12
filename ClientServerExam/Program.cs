// See https://aka.ms/new-console-template for more information
using ClientServerExam;
using ResponseData;

Random rnd = new Random();
var server = new Server("127.0.0.1", 2137);
server.PrintServerInfo();
Thread serverThread = new Thread(server.Listen);
serverThread.Start();
var client1 = new Client("127.0.0.1", 2137, 0);
client1.QueueRequest(RequestType.RequestData, typeof(ImportantData));
client1.QueueRequest(RequestType.RequestData, typeof(LessImportantData));
Thread clientThread1 = new Thread(client1.Connect);
clientThread1.Start();
var client2 = new Client("127.0.0.1", 2137, 1);
client2.QueueRequest(RequestType.RequestData, typeof(ImportantData));
client2.QueueRequest(RequestType.RequestData, typeof(ImportantData));
client2.QueueRequest(RequestType.RequestData, typeof(LessImportantData));
Thread clientThread2 = new Thread(client2.Connect);
clientThread2.Start();
var client3 = new Client("127.0.0.1", 2137, 2);
client3.QueueRequest(RequestType.RequestData, typeof(ImportantData));
client3.QueueRequest(RequestType.RequestData, typeof(EvenImportanterData));
client3.QueueRequest(RequestType.RequestData, typeof(RequestType));
Thread clientThread3 = new Thread(client3.Connect);
clientThread3.Start();
var client4 = new Client("127.0.0.1", 2137, 3);
client4.QueueRequest(RequestType.RequestData, typeof(EvenImportanterData));
client4.QueueRequest(RequestType.RequestData, typeof(LessImportantData));
Thread clientThread4 = new Thread(client4.Connect);
clientThread4.Start();

