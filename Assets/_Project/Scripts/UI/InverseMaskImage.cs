using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace InventorySystem
{
    public class InverseMaskImage : Image
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