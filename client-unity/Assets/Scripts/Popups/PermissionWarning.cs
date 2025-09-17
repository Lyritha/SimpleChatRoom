using SpacetimeDB.Types;
using TMPro;
using UnityEngine;

public class PermissionWarning : MonoBehaviour
{
    [SerializeField]
    private TMP_Text permissionText;

    public void SetPermissionWarning(AuthorityLevel user, AuthorityLevel server)
    {
        string userAutority = user.ToString();
        string serverAutority = server.ToString();

        permissionText.text = $"Current user does not have required\nautority to connect to the server right now.\n\nUser authority: {userAutority}\nRequired authority: {serverAutority}";
    }
}
