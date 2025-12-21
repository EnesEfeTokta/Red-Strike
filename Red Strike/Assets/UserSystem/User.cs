using UnityEngine;

namespace UserSystem
{
    [CreateAssetMenu(fileName = "NewUser", menuName = "User System/User")]
    public class User : ScriptableObject
    {
        public string userName;
    }
}
