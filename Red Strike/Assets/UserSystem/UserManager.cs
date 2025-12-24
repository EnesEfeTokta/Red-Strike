using UnityEngine;
using NetworkingSystem;

namespace UserSystem
{
    public class UserManager : MonoBehaviour
    {
        public User currentUser;

        public void LogIn(string userName)
        {
            if (currentUser == null) currentUser = new User();
            currentUser.userName = userName;
            if (GameBootstrap.Instance != null) GameBootstrap.Instance.LocalPlayerName = userName;
        }

        public void LogOut()
        {
            if (currentUser != null) currentUser.userName = string.Empty;
        }

        public void UpdateUserName(string newUserName)
        {
            if (currentUser != null) currentUser.userName = newUserName;
            if (GameBootstrap.Instance != null) GameBootstrap.Instance.LocalPlayerName = newUserName;
        }

        public void UpdateAvatar(int newAvatarIndex)
        {
            if (currentUser != null) currentUser.avatar = currentUser.availableAvatars[newAvatarIndex];
            if (GameBootstrap.Instance != null) GameBootstrap.Instance.LocalAvatarIndex = newAvatarIndex;
        }

        public bool IsLoggedIn()
        {
            return currentUser != null && !string.IsNullOrEmpty(currentUser.userName);
        }
    }
}