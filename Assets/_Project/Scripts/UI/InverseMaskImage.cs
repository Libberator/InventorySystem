using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI
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