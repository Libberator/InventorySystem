using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public class InverseMaskText : TextMeshProUGUI
    {
        public override Material materialForRendering
        {
            get
            {
                var mat = new Material(base.materialForRendering);
                mat.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
                return mat;
            }
        }
    }
}