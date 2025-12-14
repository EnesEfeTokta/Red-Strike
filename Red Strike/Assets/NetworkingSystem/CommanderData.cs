using UnityEngine;
using Fusion;

namespace NetworkingSystem
{
    public class CommanderData : NetworkBehaviour
    {
        [Networked] public int PlayerMoney { get; set; }
        [Networked] public int PlayerTeamID { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Debug.Log($"Benim Komutan objem yüklendi! Takımım: {PlayerTeamID}");

                if (InputController.InputController.Instance != null)
                {
                    InputController.InputController.Instance.teamId = PlayerTeamID;

                    Debug.Log("InputController Takım ID güncellendi: " + PlayerTeamID);
                }
                else
                {
                    Debug.LogError("Sahne InputController bulunamadı!");
                }

                if (Runner.IsServer) PlayerMoney = 2000;
            }
            else
            {
                Debug.Log($"Rakibin Komutan objesi (Takım: {PlayerTeamID}) yüklendi.");
            }
        }
    }
}
