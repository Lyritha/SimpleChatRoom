using UnityEngine;

public class DisconnectWarning : MonoBehaviour
{
    public void SetDisconnectWarning()
    {

    }

    public void RetryConnect()
    {
        NetworkManager.Instance.InitializeConnectionAsync();
    }
}
