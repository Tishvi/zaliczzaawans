// See https://aka.ms/new-console-template for more information

using ClientServerExam;

Console.WriteLine("Hello, World!");

Client c = new Client("127.0.0.1", 2137, 0);
c.Connect();