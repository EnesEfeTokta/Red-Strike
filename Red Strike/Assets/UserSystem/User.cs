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
    }

    [System.Serializable]
    public class FactionSelection
    {
        public string factionName;
        [TextArea(3, 10)] public string factionDescription;
        public Color factionColor;
        public Sprite factionLogo;

        public (string factionName, string factionDescription, Color factionColor, Sprite factionLogo) GetFactionInfo()
        {
            return (factionName, factionDescription, factionColor, factionLogo);
        }
    }
}
