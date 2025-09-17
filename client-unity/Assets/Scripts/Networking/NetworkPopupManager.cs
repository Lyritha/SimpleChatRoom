using SpacetimeDB.Types;
using UnityEngine;

public class NetworkPopupManager : UnitySingleton<NetworkPopupManager>
{
    [Header("Network popups"), SerializeField]
    private VersionWarning versionWarning;
    [SerializeField]
    private PermissionWarning connectionPermissionWarning;
    [SerializeField]
    private DisconnectWarning disconnectedWarning;

    private VersionWarning instancedVersionWarning;
    private PermissionWarning instancedConnectionPermissionWarning;
    private DisconnectWarning instancedDisconnectedWarning;

    public void ShowDisconnectPopup()
    {
        // clear other popups
        if (instancedConnectionPermissionWarning != null) Destroy(instancedConnectionPermissionWarning);
        if (instancedVersionWarning != null) Destroy(instancedVersionWarning);

        Transform popupTarget = FindFirstObjectByType<PopupContainerTarget>().transform;
        if (popupTarget != null)
        {
            if (instancedDisconnectedWarning == null)
            {
                instancedDisconnectedWarning = Instantiate(disconnectedWarning, popupTarget);
            }
            else
            {
                instancedDisconnectedWarning.gameObject.SetActive(true);
                instancedDisconnectedWarning.transform.SetParent(popupTarget, false);
            }

            instancedDisconnectedWarning.SetDisconnectWarning();
        }
    }
    public void ShowAuthorityPopup(AuthorityLevel user, AuthorityLevel server)
    {
        if (instancedDisconnectedWarning != null) Destroy(instancedDisconnectedWarning);
        if (instancedVersionWarning != null) Destroy(instancedVersionWarning);

        Transform popupTarget = FindFirstObjectByType<PopupContainerTarget>().transform;
        if (popupTarget != null)
        {
            if (instancedConnectionPermissionWarning == null)
            {
                instancedConnectionPermissionWarning = Instantiate(connectionPermissionWarning, popupTarget);
            }
            else
            {
                instancedConnectionPermissionWarning.gameObject.SetActive(true);
                instancedConnectionPermissionWarning.transform.SetParent(popupTarget, false);
            }

            instancedConnectionPermissionWarning.SetPermissionWarning(user, server);
        }
    }
    public void ShowVersionPopup(int currentClientVersion, int clientVersion, string reason)
    {
        if (instancedDisconnectedWarning != null) Destroy(instancedDisconnectedWarning);
        if (instancedConnectionPermissionWarning != null) Destroy(instancedConnectionPermissionWarning);

        Transform popupTarget = FindFirstObjectByType<PopupContainerTarget>().transform;
        if (popupTarget != null)
        {
            if (instancedVersionWarning == null)
            {
                instancedVersionWarning = Instantiate(versionWarning, popupTarget);
            }
            else
            {
                instancedVersionWarning.gameObject.SetActive(true);
                instancedVersionWarning.transform.SetParent(popupTarget, false);
            }

            instancedVersionWarning.SetVersionWarning(currentClientVersion, clientVersion, reason);

        }
    }
}
