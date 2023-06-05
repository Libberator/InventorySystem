using UnityEngine;
using Utilities;

namespace TooltipSystem
{
    /// <summary>
    /// When a RichTextTooltipTrigger is triggered, it needs a way to find a Tooltip based off a LinkID.
    /// This class is used to be the <link="linkID">rich text</link> provider, a source in the LinkLookup.
    /// </summary>
    public class LinkProvider : MonoBehaviour
    {
        [SerializeField] private string _richTextLinkID;
        private IHaveTooltip _tooltipSource;

        protected virtual void Awake()
        {
            FindSource();
            LinkLookup.AddAsLinkSource(_tooltipSource, _richTextLinkID);
        }

        protected virtual void FindSource()
        {
            if (_tooltipSource == null && !this.TryGetComponentInHierarchy(out _tooltipSource))
                Debug.LogWarning("Could not find a Source with IHaveTooltip in the hierarchy.", this);
        }

        protected virtual void OnDestroy() =>
            LinkLookup.RemoveLinkSource(_tooltipSource, _richTextLinkID);
    }

    /// <summary>
    /// If you don't want to make a certain class implement IHaveTooltip,
    /// you can use this generic class to access internal details via "_provider". 
    /// </summary>
    public abstract class LinkProvider<T> : LinkProvider, IHaveTooltip
    {
        [SerializeField] protected T _provider;

        protected override void Awake()
        {
            base.Awake();
            FindProvider();
        }

        protected virtual void FindProvider()
        {
            if (_provider == null && !this.TryGetComponentInHierarchy(out _provider))
                Debug.LogWarning($"Could not find a Provider for [{typeof(T)}] in the hierarchy. Please assign one.", this);
        }

        public abstract Tooltip GetTooltip();

        protected virtual void OnValidate() => FindProvider();
    }
}
