using UnityEngine;

public record DbCommand
{
    public string Target;
    public object Payload;

    public DbCommand(string target, object payload)
    {
        Target = target; 
        Payload = payload;
    }
}