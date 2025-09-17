using SpacetimeDB;
using SpacetimeDB.Types;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Connect : NetworkedMonobehavior
{
    [SerializeField]
    private RectTransform newUserPanel;


    protected override void OnConnectedToDB(DbConnection connection, Identity identity)
    {
        UserTable userTable = connection.Db.User.Identity.Find(identity);

        RunOnMainThread(() =>
        {
            // new user, show setup panel
            if (userTable == null)
            {
                newUserPanel.gameObject.SetActive(true);
                StartCoroutine(WaitUntilSetupClosed());
                return;
            }

            SceneManager.LoadScene(1);
        });
    }

    private IEnumerator WaitUntilSetupClosed()
    {
        yield return new WaitUntil(() => !newUserPanel.gameObject.activeSelf);
        SceneManager.LoadScene(1);
    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {
    }
}
