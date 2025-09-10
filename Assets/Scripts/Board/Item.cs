#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Scripts.Board;
using Object = UnityEngine.Object;

[Serializable]
public abstract class Item
{
    private static readonly Dictionary<string, GameObject> s_prefabCache = new Dictionary<string, GameObject>(16);

    private class Pool
    {
        private readonly Stack<GameObject> _stack = new Stack<GameObject>(16);
        public GameObject Get(GameObject prefab, Transform parent)
        {
            if (_stack.Count > 0)
            {
                var go = _stack.Pop();
                go.transform.SetParent(parent, false);
                go.SetActive(true);
                return go;
            }
            return Object.Instantiate(prefab, parent, false);
        }
        public void Release(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(null, false);
            _stack.Push(go);
        }
    }
    private static readonly Dictionary<string, Pool> s_pools = new Dictionary<string, Pool>(16);

    public  Cell           Cell { get; private set; }
    public  Transform      View { get; private set; }
    private SpriteRenderer _sprite;              
    private string         _prefabKey;
    private ItemSkin?      itemSkin;

    public abstract string Type { get; }
    
    public virtual void SetView()
    {
        this.itemSkin ??= Resources.Load<ItemSkin>(ItemSkin.ResourcePath);

        var prefabName = this.itemSkin.GetPrefabName(this.Type);
        if (string.IsNullOrEmpty(prefabName)) return;


        if (!s_prefabCache.TryGetValue(prefabName, out var prefab) || prefab == null)
        {
            prefab = Resources.Load<GameObject>(prefabName);
            if (prefab == null) return;
            s_prefabCache[prefabName] = prefab;
        }

        if (!s_pools.TryGetValue(prefabName, out var pool))
        {
            pool = new Pool();
            s_pools[prefabName] = pool;
        }

        var go = pool.Get(prefab, null);
        View = go.transform;

        _sprite = View.GetComponent<SpriteRenderer>();

        View.DOKill(true); 
    }


    public virtual void SetCell(Cell cell)
    {
        Cell = cell;
    }

    internal void AnimationMoveToPosition()
    {
        if (View == null || Cell == null) return;

        DOTween.Kill(View, complete: false);

        bool useLocal = View.parent != null;
        Vector3 target = useLocal ? View.parent.InverseTransformPoint(Cell.transform.position)
                                  : Cell.transform.position;

        if (useLocal)
        {
            View.DOLocalMove(target, 0.2f)
                .SetEase(Ease.OutQuad)
                .SetRecyclable(true)
                .SetLink(View.gameObject, LinkBehaviour.KillOnDisable);
        }
        else
        {
            View.DOMove(target, 0.2f)
                .SetEase(Ease.OutQuad)
                .SetRecyclable(true)
                .SetLink(View.gameObject, LinkBehaviour.KillOnDisable);
        }
    }

    public void SetViewPosition(Vector3 pos)
    {
        if (View == null) return;
        if (View.parent != null)
            View.localPosition = View.parent.InverseTransformPoint(pos);
        else
            View.position = pos;
    }

    public void SetViewRoot(Transform root)
    {
        if (View == null) return;
        View.SetParent(root, false);
    }

    public void SetSortingLayerHigher()
    {
        if (_sprite != null) _sprite.sortingOrder = 1;
    }

    public void SetSortingLayerLower()
    {
        if (_sprite != null) _sprite.sortingOrder = 0;
    }

    internal void ShowAppearAnimation()
    {
        if (View == null) return;

        DOTween.Kill(View, complete:false); 
        Vector3 target = View.localScale;
        View.localScale = Vector3.one * 0.1f;

        View.DOScale(target, 0.1f)
            .SetRecyclable(true)
            .SetLink(View.gameObject, LinkBehaviour.KillOnDisable);
    }

    internal virtual bool IsSameType(Item other)
    {
        return false;
    }

    internal virtual void ExplodeView()
    {
        if (View == null) return;

        DOTween.Kill(View, complete:false);

        View.DOScale(0.1f, 0.1f)
            .OnComplete(RecycleToPool)           
            .SetRecyclable(true)
            .SetLink(View.gameObject, LinkBehaviour.KillOnDisable);
    }

    private void RecycleToPool()
    {
        if (View == null) return;
        var go = View.gameObject;

        View.localScale = Vector3.one;
        if (_sprite != null) _sprite.sortingOrder = 0;

        if (!string.IsNullOrEmpty(_prefabKey) && s_pools.TryGetValue(_prefabKey, out var pool))
        {
            pool.Release(go);
        }
        else
        {
            Object.Destroy(go);
        }

        View = null;
        _sprite = null;
    }

    internal void AnimateForHint()
    {
        if (View == null) return;

        DOTween.Kill(View, complete:false);

        float punch = 0.1f;
        Vector3 target = View.localScale * (1f + punch);

        View.DOScale(target, 0.12f)
            .SetLoops(-1)
            .SetRecyclable(true)
            .SetLink(View.gameObject, LinkBehaviour.KillOnDisable);
    }

    internal void StopAnimateForHint()
    {
        if (View == null) return;

        DOTween.Kill(View, complete:false);
        View.localScale = Vector3.one;
    }

    internal void Clear()
    {
        Cell = null;

        if (View != null)
        {
            DOTween.Kill(View, complete:false);
            RecycleToPool();
        }
    }
}
