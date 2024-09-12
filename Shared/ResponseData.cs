using System.Formats.Asn1;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResponseData;

public enum ResponseType : ushort
{
    ConnectionRefused = 0,
    DataReceived = 1,
}
public class Response(ResponseType responseType, List<IDataInterface> foundData)
{
    public ResponseType ResponseType { get; set; } = responseType;
    public List<IDataInterface> FoundData { get; set; } = foundData;
    
    public Response() : this(ResponseType.DataReceived, new List<IDataInterface>())
    {
        
    }
}
public enum RequestType : ushort
{
    RequestData = 0,
    CloseConnection = 1,
}
public class Request(RequestType requestType, ushort clientID, string classType)
{
    public RequestType RequestType { get; set; } = requestType;
    public ushort ClientID { get; set; } = clientID;
    public string ClassType { get; set; } = classType.ToString();
}

public class IDataInterface
{
    public virtual string GetClassName()
    {
        return "";
    }

    public virtual string GetReadableData()
    {
        return "";
    }

    public object Data { get; set; }
}
[Serializable]
public class NoData() : IDataInterface
{
    private static string ClassName = "NoData";
    
    public override string GetClassName() => ClassName;
    
    public override string GetReadableData() => "No data found";
}
public class ImportantData : IDataInterface
{
    private static string ClassName { get; } = "ImportantData";

    public override string GetClassName()
    {
        return ClassName;
    }

    public override string GetReadableData()
    {
        return Data.ToString();
    }

    public ImportantData(string data)
    {
        Data = data;
    }
}

public class LessImportantData : IDataInterface
{
    private static string ClassName { get; } = "LessImportantData";

    public override string GetClassName()
    {
        return ClassName;
    }

    public override string GetReadableData()
    {
        return Data.ToString();
    }

    public LessImportantData(double data)
    {
        Data = data;
    }
    
}

public class EvenImportanterData : IDataInterface
{
    private static string ClassName { get; } = "EvenImportanterData";
    
    public override string GetClassName()
    {
        return ClassName;
    }

    public override string GetReadableData()
    {
        return Data.ToString();
    }
    public EvenImportanterData(int data)
    {
        Data = data;
    }
    
}
public class ResponseData
{
    private Dictionary<Type, List<IDataInterface>> Data { get; set; } = new();

    public ResponseData()
    {
        
    }
    public void PushData(IDataInterface data)
    {
        if (Data.ContainsKey(data.GetType()))
        {
            Data[data.GetType()].Add(data);
        }
        else
        {
            Data.Add(data.GetType(), [data]);
        }
    }

    public bool GetData(Type type, out List<IDataInterface> list)
    {
        bool found = Data.ContainsKey(type);
        list = found ? Data[type] : null;
        return found;
    }   
}