using System.Collections.Generic;
using UnityEngine;

namespace UserSystem
{
    [CreateAssetMenu(fileName = "NewUser", menuName = "User System/User")]
    public class User : ScriptableObject
    {
        public string userName;
        public Sprite avatar;
        public FactionSelection selectedFaction;

        public Sprite[] availableAvatars;
        public List<FactionSelection> factionSelections;

        public void ClearData()
        {
            userName = string.Empty;
            avatar = null;
            selectedFaction = null;
        }

        public (string userName, Sprite avatar, FactionSelection selectedFaction) GetUserInfo()
        {
            return (userName, avatar, selectedFaction);
        }

        public void SetUserInfo(string newUserName, Sprite newAvatar, FactionSelection newFaction)
        {
            userName = newUserName;
            avatar = newAvatar;
            selectedFaction = newFaction;
        }

        public int GetAvatarIndex()
        {
            if (availableAvatars == null || availableAvatars.Length == 0 || avatar == null) return -1;

            for (int i = 0; i < availableAvatars.Length; i++)
            {
                if (availableAvatars[i] == avatar)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetFactionIndex()
        {
            if (factionSelections == null || factionSelections.Count == 0 || selectedFaction == null) return -1;

            for (int i = 0; i < factionSelections.Count; i++)
            {
                if (factionSelections[i].factionName == selectedFaction.factionName)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    [System.Serializable]
    public class FactionSelection
    {
        public string factionName;
        [TextArea(3, 10)] public string factionDescription;
        public Color factionColor;
        public Sprite factionLogo;
        public Material unitMaterial;

        public (string factionName, string factionDescription, Color factionColor, Sprite factionLogo, Material unitMaterial) GetFactionInfo()
        {
            return (factionName, factionDescription, factionColor, factionLogo, unitMaterial);
        }
    }
}
