using UnityEngine;
using NetworkingSystem; // Eğer bu namespace yoksa kaldırabilirsiniz
using System.Linq;

namespace UserSystem
{
    public class UserManager : MonoBehaviour
    {
        public User currentUser;

        public void LogIn(string userName)
        {
            if (currentUser == null) return;
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
            if (currentUser == null || currentUser.availableAvatars == null || currentUser.availableAvatars.Length == 0) return;
            
            if(newAvatarIndex >= 0 && newAvatarIndex < currentUser.availableAvatars.Length)
            {
                currentUser.avatar = currentUser.availableAvatars[newAvatarIndex];
                if (GameBootstrap.Instance != null) GameBootstrap.Instance.LocalAvatarIndex = newAvatarIndex;
            }
        }

        public void UpdateFactionSelection(int newFactionIndex)
        {
            if (currentUser == null || currentUser.factionSelections == null || currentUser.factionSelections.Count == 0) return;

            if (newFactionIndex >= 0 && newFactionIndex < currentUser.factionSelections.Count)
            {
                currentUser.selectedFaction = currentUser.factionSelections[newFactionIndex];
            }
        }

        public int GetCurrentFactionIndex()
        {
            if (currentUser == null || currentUser.selectedFaction == null || currentUser.factionSelections == null) return 0;
            return currentUser.factionSelections.IndexOf(currentUser.selectedFaction);
        }

        public bool IsLoggedIn()
        {
            return currentUser != null && !string.IsNullOrEmpty(currentUser.userName);
        }
    }
}