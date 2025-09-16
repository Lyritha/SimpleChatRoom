using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Connect : NetworkedMonobehavior
{
    [SerializeField]
    private RectTransform versionWarning;
    [SerializeField]


    protected override void OnConnectedToDB(DbConnection connection)
    {
        SceneManager.LoadScene(1);
    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {
    }

}
