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
        }

        public void LogOut()
        {
            if (currentUser != null) currentUser.userName = string.Empty;
        }

        public void UpdateUserName(string newUserName)
        {
            if (currentUser != null) currentUser.userName = newUserName;
        }

        public string GetUserName()
        {
            if (currentUser != null)
            {
                return currentUser.userName;
            }
            return string.Empty;
        }

        public bool IsLoggedIn()
        {
            return currentUser != null && !string.IsNullOrEmpty(currentUser.userName);
        }
    }
}