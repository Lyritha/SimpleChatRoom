using TMPro;
using UnityEngine;

public class VersionWarning : MonoBehaviour
{
    [SerializeField]
    private TMP_Text versionText;


    public void SetVersionWarning(int currentClientVersion, int clientVersion, string reason)
    {
        versionText.text = $"Client Version {currentClientVersion} is not supported. \nPlease update to version {clientVersion}.\n\nReason for update:\n{reason}";
    }
}
