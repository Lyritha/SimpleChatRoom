using SpacetimeDB.Types;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class InputManager : UnitySingleton<InputManager>
{
    private readonly ConcurrentQueue<DbCommand> input_queue = new();

    private void Start()
    {
        NetworkManager.Instance.OnConnectedToDB += OnConnectedToDB;
    }

    private void OnConnectedToDB(DbConnection connection)
    {
        // You could spin up your process thread here if desired
        var cts = new CancellationTokenSource();
        var thread = new Thread(() => ProcessThread(connection, cts.Token));
        thread.Start();
    }

    private void ProcessThread(DbConnection conn, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                conn.FrameTick();
                ProcessCommands(conn.Reducers);
                Thread.Sleep(100);
            }
        }
        finally
        {
            conn.Disconnect();
        }
    }

    private void ProcessCommands(RemoteReducers reducers)
    {
        while (input_queue.TryDequeue(out var cmd))
        {
            var method = typeof(RemoteReducers).GetMethod(cmd.Target);
            if (method == null)
            {
                Debug.LogWarning($"Unknown reducer: {cmd.Target}");
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || !parameters[0].ParameterType.IsInstanceOfType(cmd.Payload))
            {
                Debug.LogError($"Reducer {cmd.Target} expects {parameters[0].ParameterType}, got {cmd.Payload?.GetType()}");
                continue;
            }

            method.Invoke(reducers, new object[] { cmd.Payload });
        }
    }

    public void EnqueueCommand<T>(string target, T payload)
    {
        input_queue.Enqueue(new DbCommand(target, payload!));
    }
}