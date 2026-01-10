using UnityEngine;

namespace InputController
{
    [RequireComponent(typeof(Outline.Scripts.Outline))]
    public class SelectionHighlighter : MonoBehaviour
    {
        private Outline.Scripts.Outline outline;

        private void Awake()
        {
            outline = GetComponent<Outline.Scripts.Outline>();
            if(outline != null) outline.enabled = false;
        }

        public void EnableHighlight(int viewerTeamId, int objectTeamId)
        {
            if (outline == null) return;

            outline.enabled = true;

            if (viewerTeamId == objectTeamId)
            {
                outline.OutlineColor = Color.blue;
            }
            else
            {
                outline.OutlineColor = Color.red;
            }
        }

        public void DisableHighlight()
        {
            if (outline != null) outline.enabled = false;
        }
    }
}