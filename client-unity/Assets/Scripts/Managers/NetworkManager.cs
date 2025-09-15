using SpacetimeDB;
using SpacetimeDB.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

public class NetworkManager : UnitySingleton<NetworkManager>
{
    private const string HOST = "wss://maincloud.spacetimedb.com";
    private const string DB_NAME = "unity-chat-room";

    // Auth0 OIDC info
    private const string AUTH0_DOMAIN = "dev-oiaipmk58n86ow7t.us.auth0.com";
    private const string AUTH0_CLIENT_ID = "OeYtJn4B3yFwA701SuKJzllqFmL6QZ9u";
    private const string AUTH0_REDIRECT_URI = "http://localhost:5000/callback/";
    private const string AUTH0_SCOPE = "openid profile email";

    public DbConnection Connection { get; private set; }
    public bool IsConnected { get; private set; } = false;
    public Identity LocalIdentity { get; private set; }

    public event Action<DbConnection> OnConnectedToDB;
    public event Action<DbConnection> OnDisconnectedToDB;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Start async initialization without blocking main thread
        InitializeConnectionAsync();
    }

    private async void InitializeConnectionAsync()
    {
        try
        {
            string token = await GetValidTokenAsync();

            Connection = DbConnection.Builder()
                .WithUri(HOST)
                .WithModuleName(DB_NAME)
                .WithToken(token)
                .OnConnect(OnConnected)
                .OnConnectError(OnConnectError)
                .OnDisconnect(OnDisconnected)
                .Build();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize connection: {ex}");
        }
    }


    private async Task<string> GetValidTokenAsync()
    {
        string savedToken = PlayerPrefs.GetString("SpacetimeDB_IdToken", "");
        if (!string.IsNullOrEmpty(savedToken))
        {
            // Decode JWT expiration
            var parts = savedToken.Split('.');
            if (parts.Length == 3)
            {
                string payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1].PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=')));
                var payload = JsonUtility.FromJson<JwtPayload>(payloadJson);
                if (payload.exp > DateTimeOffset.UtcNow.ToUnixTimeSeconds()) return savedToken;
            }

            Debug.Log("Old Token, grab new one...");
        }

        // Otherwise fetch a new token
        string token = await GetOidcTokenAsync();
        PlayerPrefs.SetString("SpacetimeDB_IdToken", token);
        return token;
    }


    /// <summary>
    /// Get OpenID token from auth0 by opening an website
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<string> GetOidcTokenAsync()
    {
        // Step 1: Open Auth0 login page in default browser
        Process.Start(new ProcessStartInfo
        {
            FileName = $"https://{AUTH0_DOMAIN}/authorize?response_type=code&client_id={AUTH0_CLIENT_ID}&" + 
            $"redirect_uri={Uri.EscapeDataString(AUTH0_REDIRECT_URI)}&scope={Uri.EscapeDataString(AUTH0_SCOPE)}",
            UseShellExecute = true
        });

        // Step 2: Listen locally for the redirect with the auth code
        using var listener = new HttpListener();
        listener.Prefixes.Add(AUTH0_REDIRECT_URI);
        listener.Start();

        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString["code"];

        // Respond to browser
        var responseString = @"
        <html>
          <body>
            <p>Login successful! This window will close automatically.</p>
            <script>
              setTimeout(() => window.close(), 1000); // Close after 1 second
            </script>
          </body>
        </html>";
        var buffer = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
        listener.Stop();

        if (code == null) throw new Exception("Failed to receive OAuth code.");

        // Step 3: Exchange code for Auth0 ID token
        var tokenRequest = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", AUTH0_CLIENT_ID },
            { "code", code },
            { "redirect_uri", AUTH0_REDIRECT_URI }
        };

        using var client = new HttpClient();
        var res = await client.PostAsync($"https://{AUTH0_DOMAIN}/oauth/token", new FormUrlEncodedContent(tokenRequest));
        res.EnsureSuccessStatusCode();
        var token = JsonConvert.DeserializeObject<TokenResponse>(await res.Content.ReadAsStringAsync());
        return token.id_token ?? throw new Exception("Failed to get ID token");
    }

    private void OnConnected(DbConnection conn, Identity identity, string authToken)
    {
        Debug.Log("Connected to server");

        LocalIdentity = identity;

        conn.SubscriptionBuilder()
            .OnApplied(ctx => {
                OnConnectedToDB?.Invoke(conn);
                IsConnected = true;
            })
            .SubscribeToAllTables();
    }
    private void OnDisconnected(DbConnection conn, Exception? e)
    {
        if (e != null) Debug.Log($"Disconnected abnormally: {e}");
        else Debug.Log("Disconnected normally.");

        OnDisconnectedToDB?.Invoke(conn);
        IsConnected = false;
    }

    private void OnConnectError(Exception e)
        => Debug.Log($"Error while connecting: {e}");

}