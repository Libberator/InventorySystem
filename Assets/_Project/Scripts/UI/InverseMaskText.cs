using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

namespace InventorySystem
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