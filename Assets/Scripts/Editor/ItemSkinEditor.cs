#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Scripts.Board
{
    [CustomEditor(typeof(ItemSkin))]
    public class ItemSkinEditor : Editor
    {
        // temp input state
        private NormalItem.eNormalType _normalKey;
        private string                 _normalPrefab = "";

        private BonusItem.eBonusType _bonusKey;
        private string               _bonusPrefab = "";

        public override void OnInspectorGUI()
        {
            var skin = (ItemSkin)this.target;

            EditorGUILayout.LabelField("Dictionary Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            this.DrawNormalInputs(skin);
            EditorGUILayout.Space(8);
            this.DrawBonusInputs(skin);

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Normal Map", EditorStyles.boldLabel);
            this.DrawNormalList(skin);

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Bonus Map", EditorStyles.boldLabel);
            this.DrawBonusList(skin);
            
            if (GUILayout.Button("Save"))
                AssetDatabase.SaveAssets();
        }

        private void DrawNormalInputs(ItemSkin skin)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                this._normalKey = (NormalItem.eNormalType)EditorGUILayout.EnumPopup("Normal Type", this._normalKey);
                this._normalPrefab = EditorGUILayout.TextField("Prefab Name", this._normalPrefab);

                using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(this._normalPrefab)))
                {
                    if (GUILayout.Button("Add / Update Normal Entry"))
                    {
                        Undo.RecordObject(skin, "Add/Update Normal Entry");
                        skin.AddOrUpdateNormal(this._normalKey, this._normalPrefab.Trim());
                        EditorUtility.SetDirty(skin);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }

        private void DrawBonusInputs(ItemSkin skin)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                this._bonusKey = (BonusItem.eBonusType)EditorGUILayout.EnumPopup("Bonus Type", this._bonusKey);
                this._bonusPrefab = EditorGUILayout.TextField("Prefab Name", this._bonusPrefab);

                using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(this._bonusPrefab)))
                {
                    if (GUILayout.Button("Add / Update Bonus Entry"))
                    {
                        Undo.RecordObject(skin, "Add/Update Bonus Entry");
                        skin.AddOrUpdateBonus(this._bonusKey, this._bonusPrefab.Trim());
                        EditorUtility.SetDirty(skin);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }

        private void DrawNormalList(ItemSkin skin)
        {
            if (skin.NormalMap == null || skin.NormalMap.Count == 0)
            {
                EditorGUILayout.HelpBox("No Normal entries yet.", MessageType.Info);
                return;
            }

            foreach (var kv in skin.NormalMap.ToList()) 
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(kv.Key.ToString(), GUILayout.MaxWidth(140));
                    string newVal = EditorGUILayout.TextField(kv.Value);

                    if (newVal != kv.Value)
                    {
                        Undo.RecordObject(skin, "Edit Normal Prefab");
                        skin.AddOrUpdateNormal(kv.Key, newVal);
                        EditorUtility.SetDirty(skin);
                    }

                    if (GUILayout.Button("X", GUILayout.Width(24)))
                    {
                        Undo.RecordObject(skin, "Remove Normal Entry");
                        skin.RemoveNormal(kv.Key);
                        EditorUtility.SetDirty(skin);
                        GUI.FocusControl(null);
                        break; 
                    }
                }
            }
        }

        private void DrawBonusList(ItemSkin skin)
        {
            if (skin.BonusMap == null || skin.BonusMap.Count == 0)
            {
                EditorGUILayout.HelpBox("No Bonus entries yet.", MessageType.Info);
                return;
            }

            foreach (var kv in skin.BonusMap.ToList())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(kv.Key.ToString(), GUILayout.MaxWidth(140));
                    string newVal = EditorGUILayout.TextField(kv.Value);

                    if (newVal != kv.Value)
                    {
                        Undo.RecordObject(skin, "Edit Bonus Prefab");
                        skin.AddOrUpdateBonus(kv.Key, newVal);
                        EditorUtility.SetDirty(skin);
                    }

                    if (GUILayout.Button("X", GUILayout.Width(24)))
                    {
                        Undo.RecordObject(skin, "Remove Bonus Entry");
                        skin.RemoveBonus(kv.Key);
                        EditorUtility.SetDirty(skin);
                        GUI.FocusControl(null);
                        break;
                    }
                }
            }
        }
    }
}
#endif
