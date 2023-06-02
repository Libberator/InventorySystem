using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CursorSystem
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private CursorData[] _cursors;

        private CursorType _currentCursorType;
        private Coroutine _animation;
        private bool _isVisible = true;

        private void Awake() => ServiceLocator.Register(this);

        private void OnApplicationFocus(bool focus)
        {
            if (focus) Cursor.visible = _isVisible;
        }

        [SerializeField, HideInInspector] private Vector3 _defaultHotspot;
#if UNITY_EDITOR
        private void OnValidate() => _defaultHotspot = UnityEditor.PlayerSettings.cursorHotspot;
#endif

        #region Public Methods

        public CursorType CurrentCursorType => _currentCursorType;

        [Button]
        public void ChangeCursor(CursorType cursorType)
        {
            var cursorData = _cursors.FirstOrDefault(c => c.CursorType == cursorType);
            if (cursorData != null) SwitchTo(cursorData);
            else ResetCursor();
        }

        [ButtonGroup]
        public void ResetCursor()
        {
            if (_animation != null) StopCoroutine(_animation);
            Cursor.SetCursor(null, _defaultHotspot, CursorMode.Auto);
            _currentCursorType = CursorType.Default;

            //Texture2D blankCursor = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            //blankCursor.SetPixel(0, 0, Color.clear);
            //Cursor.SetCursor(blankCursor, Vector2.zero, CursorMode.ForceSoftware);
        }

        [ButtonGroup, HideIf("_isVisible")]
        public void ShowCursor() => _isVisible = Cursor.visible = true;

        [ButtonGroup, ShowIf("_isVisible")]
        public void HideCursor() => _isVisible = Cursor.visible = false;

        #endregion

        #region Private Methods

        private void SwitchTo(CursorData cursorData)
        {
            if (_animation != null) StopCoroutine(_animation);
            if (cursorData.IsAnimated) _animation = StartCoroutine(AnimateCursor(cursorData));
            else SetCursorTo(cursorData);
            _currentCursorType = cursorData.CursorType;
        }

        private IEnumerator AnimateCursor(CursorData cursor)
        {
            var waitTime = new WaitForSeconds(cursor.TimeBetweenFrames);
            int index = 0;
            while (true)
            {
                SetCursorTo(cursor, index++);
                if (index == cursor.Textures.Length) index = 0;
                yield return waitTime;
            }
        }

        private void SetCursorTo(CursorData cursor, int index = 0) => Cursor.SetCursor(cursor.Textures[index], cursor.Hotspot, CursorMode.Auto);

        #endregion
    }

    #region Data Structures

    public enum CursorType
    {
        Default,
        Arrow,
        Hand,
    }

    [Serializable]
    [InlineProperty]
    public class CursorData
    {
        [HorizontalGroup, HideLabel]
        public CursorType CursorType;

        [HorizontalGroup, HideLabel]
        public Vector2 Hotspot;

        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        public Texture2D[] Textures = new Texture2D[1];

        [ShowIf(nameof(IsAnimated)), Tooltip("Time between each cursor image for animating")]
        public float TimeBetweenFrames = 0.2f;

        public bool IsAnimated => Textures.Length > 1;
    }

    #endregion
}