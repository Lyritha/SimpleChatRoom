using SpacetimeDB.Types;
using UnityEngine.SceneManagement;

public class Connect : NetworkedMonobehavior
{
    protected override void OnConnectedToDB(DbConnection connection)
    {
        SceneManager.LoadScene(1);
    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {
    }

}
