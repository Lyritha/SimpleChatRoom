using SpacetimeDB;
using SpacetimeDB.Types;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class UserSettingsMenu : NetworkedMonobehavior
{
    [SerializeField]
    private TMP_Text titleField;
    [SerializeField]
    private TMP_InputField usernameField;
    [SerializeField]
    private ColorPicker colorPicker;
    [SerializeField]
    private Image colorPreview;
    [SerializeField]
    private Slider connectClipsSlider;
    [SerializeField]
    private Slider disconnectClipsSlider;

    private void Start()
    {
        colorPicker.onColorChanged += color =>
        {
            // Defer until next frame
            StartCoroutine(ApplyColorNextFrame(color));
        };
    }
    private IEnumerator ApplyColorNextFrame(Color color)
    {
        yield return null; // wait one frame
        colorPreview.color = color;
    }

    protected override void OnConnectedToDB(DbConnection connection, Identity identity)
    {

    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!isConnected) return;

        connectClipsSlider.maxValue = AudioManager.Instance.AudioClipsConnected.Length - 1;
        disconnectClipsSlider.maxValue = AudioManager.Instance.AudioClipsDisconnected.Length - 1;

        UserTable user = connection.Db.User.Identity.Find(localIdentity);

        if (user != null)
        {
            usernameField.text = user.Name;
            connectClipsSlider.value = user.ConnectSoundId;
            disconnectClipsSlider.value = user.DisconnectSoundId;
            titleField.text = "User Settings";

            if (ColorUtility.TryParseHtmlString(user.Color, out Color color))
            {
                colorPicker.color = color;
            }
        }
        else
        {
            usernameField.text = "";
            connectClipsSlider.value = 0;
            disconnectClipsSlider.value = 0;
            titleField.text = "Welcome new user!";
        }
    }

    public void ApplyUserSettings()
    {
        string username = usernameField.text;
        string colorHex = colorPicker.color.ToHexString();
        colorHex = $"#{colorHex}";

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(colorHex))
        {
            Debug.Log("Invalid values");
            return;
        }

        // check if username already taken
        UserTable existing = connection.Db.User.Name.Find(username);
        if (existing != null && existing.Identity != localIdentity) return;

        // save settings
        UserSettings settings = new()
        {
            Name = username,
            Color = colorHex
        };

        // decide to either register new user or update existing one
        UserTable user = connection.Db.User.Identity.Find(localIdentity);
        string command = user == null ? "RegisterUser" : "UpdateUser";
        InputManager.Instance.EnqueueCommand(command, settings);

        gameObject.SetActive(false);
    }

    public void PreviewConnectClip()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioClipsConnected[(int)connectClipsSlider.value]);
    }

    public void PreviewDisconnectClip()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioClipsDisconnected[(int)disconnectClipsSlider.value]);
    }
}
