using TMPro;
using UnityEngine;

public class ScreenLogger : UnitySingleton<ScreenLogger>
{
    [SerializeField]
    private TMP_Text text;

    public void Log(string message)
    {
        text.text += $"\n{message}";
        Debug.Log(message);
    }

}
