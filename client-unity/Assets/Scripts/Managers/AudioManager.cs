using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class AudioManager : NetworkedSingleton<AudioManager>
{
    [SerializeField]
    private AudioSource source;

    public AudioClip[] AudioClipsConnected;
    public AudioClip[] AudioClipsDisconnected;

    protected override void OnConnectedToDB(DbConnection connection, Identity identity)
    {
        connection.Db.User.OnUpdate += UpdateItem;
    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {
        connection.Db.User.OnUpdate -= UpdateItem;
    }

    private void UpdateItem(EventContext ctx, UserTable oldValue, UserTable newValue)
    {
        // skip if the online status didn't change
        if (oldValue.Online == newValue.Online) return;

        int index = newValue.Online ? newValue.ConnectSoundId : newValue.DisconnectSoundId;

        RunOnMainThread(() =>
        {
            AudioClip audioClip = newValue.Online ? AudioClipsConnected[index] : AudioClipsDisconnected[index];
            PlayClip(audioClip);
        });
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
    }

    public void SetVolume(float volume)
    {
        if (source != null)
        {
            source.volume = volume;
        }
    }
}