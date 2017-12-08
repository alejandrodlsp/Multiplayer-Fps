
using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobbyManager : MonoBehaviour {

    public Text usernameText;

    private void Start()
    {
        if (UserAccountManager.isLoggedIn)
        {
            usernameText.text = UserAccountManager.playerUsername;
        }
    }

    public void LogOut() {
        if (UserAccountManager.isLoggedIn)
        {
            UserAccountManager.instance.LogOut();
        }
    }
}
