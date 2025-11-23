using UnityEngine;
using System.Collections.Generic;

namespace InputController
{
    [RequireComponent(typeof(Outline.Scripts.Outline))]
    public class SelectionHighlighter : MonoBehaviour
    {
        private Renderer[] renderersToHighlight;
        private List<Material> materials = new List<Material>();
        private List<Color> originalColors = new List<Color>();

        private Outline.Scripts.Outline outline;

        public Color selectionColor;

        private void Awake()
        {
            outline = gameObject.GetComponent<Outline.Scripts.Outline>();

            if (renderersToHighlight == null || renderersToHighlight.Length == 0)
                renderersToHighlight = GetComponentsInChildren<Renderer>();

            foreach (var rend in renderersToHighlight)
            {
                foreach (var mat in rend.materials)
                {
                    materials.Add(mat);
                    if (mat.HasProperty("_Color")) originalColors.Add(mat.color);
                    else originalColors.Add(Color.white);
                }
            }
        }

        public void EnableHighlight()
        {
            outline.enabled = true;
            outline.OutlineColor = selectionColor;
        }

        public void DisableHighlight()
        {
            if (outline != null) outline.enabled = false;
        }
    }
}
