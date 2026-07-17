//Author Luka Brännlund
//
//Everything that has to do with main menu type actions goes here (Settings, user customization, etc.)
//
//
//

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Singleton
    public static MainMenu Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    [SerializeField] private TMP_InputField sessionCodeField;//Text field where the user inserts the session code.
    [SerializeField] private TMP_InputField usernameField;//Text field where the user inserts their user name.
    [SerializeField] GameObject uiBase;

    public void HostButtonClicked() //Called when host button is pressed
    {
        if (RelayManager.Instance.isConnecting) return;
        RelayManager.Instance.AttemptHosting();
    }

    public void JoinButtonClicked()//Called when the join button is presses. Checks the sessionCodeField for a session code.
    {
        if (RelayManager.Instance.isConnecting) return;
        RelayManager.Instance.AttemptJoining(sessionCodeField.text);
    }

    public string GetLocalUserName()
    {
        return usernameField.text;
    }

    public void CloseUI()
    {
        uiBase.SetActive(false);
    }
}
