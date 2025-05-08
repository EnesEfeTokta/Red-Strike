using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private RawImage compassBar;

    private bool isTPS = false;

    private bool IsTPS
    {
        get { return isTPS; }
        set { isTPS = value; }
    }

    void Update()
    {
        if (isTPS)
        {
            compassBar.uvRect = new Rect(player.localEulerAngles.y / 360, 0, 1, 1);

            float playerRotation = player.rotation.eulerAngles.y;

            playerRotation = Mathf.RoundToInt(playerRotation);

            if (playerRotation < 0) { playerRotation += 360; }
        }
    }
}