using System.Threading.Tasks;

public class DbCommand
{
    public string Target { get; }
    public object Payload { get; }

    public DbCommand(string target, object payload)
    {
        Target = target;
        Payload = payload;
    }
}
