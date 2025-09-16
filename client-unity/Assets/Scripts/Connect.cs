using SpacetimeDB;
using SpacetimeDB.Types;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Connect : NetworkedMonobehavior
{
    [Header("Client version"),SerializeField]
    private int currentClientVersion = 1;

    [Header("Popups"),SerializeField]
    private RectTransform versionWarning;
    [SerializeField]
    private TMP_Text versionText;
    [SerializeField]
    private RectTransform connectDisallowedWarning;
    [SerializeField]
    private RectTransform newUserPanel;


    protected override void OnConnectedToDB(DbConnection connection, Identity identity)
    {
        ClientUpdatesTable clientUpdate = connection.Db.ClientUpdates.Iter().OrderByDescending(u => u.ClientVersion).FirstOrDefault();
        UserTable userTable = connection.Db.User.Identity.Find(identity);
        int userAuthorityLevel = userTable == null ? 0 : (int)userTable.AuthorityLevel;

        RunOnMainThread(() =>
        {
            // client version not supported, show warning
            if (currentClientVersion != clientUpdate.ClientVersion)
            {
                versionWarning.gameObject.SetActive(true);
                versionText.text = $"Client Version {currentClientVersion} is not supported. \nPlease update to version {clientUpdate.ClientVersion}.\n\nReason for update:\n{clientUpdate.Reason}";
                connection.Disconnect();
                return;
            }

            // user not allowed to connect right, likely because maintenance mode is on
            if (userAuthorityLevel < (int)clientUpdate.MinimumConnectionLevel)
            {
                connectDisallowedWarning.gameObject.SetActive(true);
                connection.Disconnect();
                return;
            }

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
