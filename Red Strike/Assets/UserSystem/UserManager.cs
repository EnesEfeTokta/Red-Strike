using UnityEngine;

namespace UserSystem
{
    public class UserManager : MonoBehaviour
    {
        public User currentUser;

        public void LogIn(string userName)
        {
            if (currentUser != null)
            {
                currentUser.userName = userName;
            }
        }

        public void LogOut()
        {
            if (currentUser != null)
            {
                currentUser.userName = string.Empty;
            }
        }

        public void UpdateProfile(string newUserName)
        {
            if (currentUser != null)
            {
                currentUser.userName = newUserName;
            }
        }
    }
}
