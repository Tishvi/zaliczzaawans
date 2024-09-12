// See https://aka.ms/new-console-template for more information

using ClientServerExam;

Console.WriteLine("Hello, World!");

var server = new Server("127.0.0.1", 2137);
server.Listen();