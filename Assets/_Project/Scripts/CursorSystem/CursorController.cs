using UnityEngine;

namespace InventorySystem
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private Cursor _defaultCursor;

        private void Start()
        {
            //Cursor.visible = false;
        }

        private void Update()
        {

        }

        //private void ChangeCursorTo()
        //{
        //    Cursor.SetCursor(_defaultCursor);
        //}

        private void OnEnable()
        {
            Debug.Log("SoftwareCursor: OnEnable()");
            //cursorVisible = Cursor.visible;
            if (true)//searchForCursorTexture())
            {
                Debug.Log("Found Cursor texture");
                Texture2D blankCursor = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                blankCursor.SetPixel(0, 0, Color.clear);
                Cursor.SetCursor(blankCursor, Vector2.zero, CursorMode.ForceSoftware);
            }
            else
            {
                Debug.LogError("Could not locate cursor texture!");
                enabled = false;
            }
        }

        public void ResetCursor()
        {
            //if (isCursorOverriden)
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            //isCursorOverriden = false;
        }
    }
}
