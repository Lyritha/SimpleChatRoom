using SpacetimeDB.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for MonoBehaviours that need to react to SpacetimeDB connection events.
/// Automatically subscribes to the NetworkManager and tracks connection state.
/// Provides a main-thread queue so derived classes can safely schedule Unity API calls from background threads.
/// </summary>
public abstract class NetworkedMonobehavior : MonoBehaviour
{
    protected bool isConnected = false;

    // Queue for actions that must run on Unity's main thread
    private readonly Queue<Action> _mainThreadQueue = new Queue<Action>();

    protected virtual void OnEnable()
    {
        // Subscribe when enabled
        NetworkManager.Instance.OnConnectedToDB += OnConnected;
        NetworkManager.Instance.OnDisconnectedToDB += OnDisconnected;

        // If already connected, catch up immediately
        if (NetworkManager.Instance.IsConnected && !isConnected)
        {
            OnConnected(NetworkManager.Instance.Connection);
        }
    }

    protected virtual void OnDisable()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnConnectedToDB -= OnConnected;
            NetworkManager.Instance.OnDisconnectedToDB -= OnDisconnected;
        }
    }

    private void OnConnected(DbConnection connection)
    {
        isConnected = true;
        OnConnectedToDB(connection);
    }

    private void OnDisconnected(DbConnection connection)
    {
        isConnected = false;
        OnDisconnectedToDB(connection);
    }

    /// <summary>
    /// Schedule an action to run on Unity's main thread.
    /// </summary>
    /// <param name="action">The action to execute on the main thread.</param>
    protected void RunOnMainThread(Action action)
    {
        if (action == null) return;

        lock (_mainThreadQueue)
        {
            _mainThreadQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        // Execute all queued actions on the main thread
        lock (_mainThreadQueue)
        {
            while (_mainThreadQueue.Count > 0)
            {
                _mainThreadQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// Called when a connection to SpacetimeDB is established.
    /// Override this in your subclass to perform initialization that depends on the DB.
    /// </summary>
    /// <param name="connection">The active DbConnection.</param>
    protected abstract void OnConnectedToDB(DbConnection connection);

    /// <summary>
    /// Called when the connection to SpacetimeDB is lost or closed.
    /// Override this in your subclass to handle cleanup or UI updates.
    /// </summary>
    /// <param name="connection">The connection that was lost.</param>
    protected abstract void OnDisconnectedToDB(DbConnection connection);
}
