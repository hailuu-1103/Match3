using UnityEngine;
using System;
using System.Collections.Generic;

namespace Scripts.Board
{
    using UnityEditor;

    [CreateAssetMenu(fileName = "ItemSkin", menuName = "Configs/ItemSkin")]
    public class ItemSkin : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string ResourcePath = "ItemSkin";

        public IReadOnlyDictionary<NormalItem.eNormalType, string> NormalMap => this.normalTypePools;
        public IReadOnlyDictionary<BonusItem.eBonusType, string>   BonusMap  => this.bonusTypePools;

        private readonly Dictionary<NormalItem.eNormalType, string> normalTypePools = new Dictionary<NormalItem.eNormalType, string>();
        private readonly Dictionary<BonusItem.eBonusType, string>   bonusTypePools  = new Dictionary<BonusItem.eBonusType, string>();

        [SerializeField] private List<NormalItem.eNormalType> normalKeys = new List<NormalItem.eNormalType>();
        [SerializeField] private List<string>                 normalVals = new List<string>();
        [SerializeField] private List<BonusItem.eBonusType>   bonusKeys  = new List<BonusItem.eBonusType>();
        [SerializeField] private List<string>                 bonusVals  = new List<string>();

        public void AddOrUpdateNormal(NormalItem.eNormalType key, string prefab)
        {
            if (string.IsNullOrWhiteSpace(prefab)) return;
            this.normalTypePools[key] = prefab;
            EditorUtility.SetDirty(this);
        }

        public void AddOrUpdateBonus(BonusItem.eBonusType key, string prefab)
        {
            if (string.IsNullOrWhiteSpace(prefab)) return;
            this.bonusTypePools[key] = prefab;
            EditorUtility.SetDirty(this);
        }

        public void RemoveNormal(NormalItem.eNormalType key)
        {
            if (this.normalTypePools.Remove(key)) EditorUtility.SetDirty(this);
        }

        public void RemoveBonus(BonusItem.eBonusType key)
        {
            if (this.bonusTypePools.Remove(key)) EditorUtility.SetDirty(this);
        }

        public string GetPrefabName(string type)
        {
            if (Enum.TryParse(type, out NormalItem.eNormalType n) && this.normalTypePools.TryGetValue(n, out var a)) return a;
            if (Enum.TryParse(type, out BonusItem.eBonusType b) && this.bonusTypePools.TryGetValue(b, out var c)) return c;

            return null;
        }

        public void OnBeforeSerialize()
        {
            this.normalKeys.Clear();
            this.normalVals.Clear();

            foreach (var kv in this.normalTypePools)
            {
                this.normalKeys.Add(kv.Key);
                this.normalVals.Add(kv.Value);
            }

            this.bonusKeys.Clear();
            this.bonusVals.Clear();

            foreach (var kv in this.bonusTypePools)
            {
                this.bonusKeys.Add(kv.Key);
                this.bonusVals.Add(kv.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.normalTypePools.Clear();
            for (int i = 0, m = Math.Min(this.normalKeys.Count, this.normalVals.Count); i < m; i++) this.normalTypePools[this.normalKeys[i]] = this.normalVals[i];

            this.bonusTypePools.Clear();
            for (int i = 0, m = Math.Min(this.bonusKeys.Count, this.bonusVals.Count); i < m; i++) this.bonusTypePools[this.bonusKeys[i]] = this.bonusVals[i];
        }
    }
}