using UnityEngine;
using UnityEngine.UI;

public class TankTpsUı : MonoBehaviour
{
    [SerializeField] private RawImage compassBar;
    [SerializeField] private Text compassText;

    void Update()
    {
        compassBar.uvRect = new Rect(transform.localEulerAngles.y / 360, 0, 1, 1);
        float playerRotation = transform.rotation.eulerAngles.y;
        playerRotation = Mathf.RoundToInt(playerRotation);
        if (playerRotation < 0) { playerRotation += 360; }
        compassText.text = playerRotation.ToString();
    }
}
