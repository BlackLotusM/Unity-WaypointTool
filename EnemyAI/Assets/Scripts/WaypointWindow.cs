using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;

public class WaypointWindow : EditorWindow
{
    private ReorderableList m_itemList = null;
    private ReorderableList m_propertiesList = null;
    private SerializedObject so = null;


    [MenuItem("Window/ItemObjectDatabase Editor")]
    public static void OpenWindow()
    {
        ShowWindow(Selection.activeObject as ArraySO);
    }

    public static void ShowWindow(ArraySO itemDatabaseObject)
    {
        var window = GetWindow<WaypointWindow>("InventoryEditor");
        window.SelectDatabaseObject(itemDatabaseObject);
    }

    void OnSelectionChange()
    {
        var itemDatabaseObject = Selection.activeObject as ArraySO;
        if (itemDatabaseObject != null) SelectDatabaseObject(itemDatabaseObject);
    }

    void SelectDatabaseObject(ArraySO itemDatabaseObject)
    {
        if (itemDatabaseObject == null)
        {
            m_itemList = null;
            so = null;
        }
        else
        {
            m_itemList = new ReorderableList(itemDatabaseObject.itemList, typeof(ArraySO.Item), true, true, true, true);
            m_itemList.onAddCallback += AddItem;
            m_itemList.onRemoveCallback += RemoveItem;
            m_itemList.onSelectCallback += SelectItem;
            m_itemList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Items"); };
            so = new SerializedObject(itemDatabaseObject);
        }
        m_propertiesList = null;
        Repaint();
    }

    void OnGUI()
    {
        if (so != null && m_itemList != null)
        {
            so.Update();
            m_itemList.DoLayoutList();
            if (m_propertiesList != null)
            {
                m_propertiesList.DoLayoutList();
            }
            so.ApplyModifiedProperties();
        }
    }

    void AddItem(ReorderableList itemList)
    {
        // When we add an item, select that item:
        itemList.list.Add(new ArraySO.Item());
        itemList.index = itemList.count - 1;
        SelectItem(itemList);
    }

    void RemoveItem(ReorderableList itemList)
    {
        // When we remove an item, clear the properties list:
        ReorderableList.defaultBehaviours.DoRemoveButton(itemList);
        m_propertiesList = null;
        Repaint();
    }

    void SelectItem(ReorderableList itemList)
    {
        // We when select an item, init the properties list for that item:
        if (0 <= itemList.index && itemList.index < itemList.count)
        {
            var item = itemList.list[itemList.index] as ArraySO.Item;
            if (item != null)
            {
                m_propertiesList = new ReorderableList(item.properties, typeof(string), true, true, true, true);
                m_propertiesList.drawElementCallback = DrawProperty;
            }
            Repaint();
        }
    }

    void DrawProperty(Rect rect, int index, bool isActive, bool isFocused)
    {
        // Added tons of debugging to help if you have issues:
        var itemListSerializedProperty = so.FindProperty("itemList");
        if (itemListSerializedProperty == null) { Debug.Log("itemList is null!"); return; }
        if (!itemListSerializedProperty.isArray) { Debug.Log("itemList is not an array!"); return; }
        if (!(0 <= m_itemList.index && m_itemList.index < itemListSerializedProperty.arraySize)) { Debug.Log("itemList[" + m_itemList.index + "] is outside array bounds!"); return; }
        if (0 <= m_itemList.index && m_itemList.index < itemListSerializedProperty.arraySize)
        {
            var itemSerializedProperty = itemListSerializedProperty.GetArrayElementAtIndex(m_itemList.index);
            if (itemSerializedProperty == null) { Debug.Log("itemSerializedProperty[" + m_itemList.index + "] is null!"); return; }

            var propertiesListSerializedProperty = itemSerializedProperty.FindPropertyRelative("properties");
            if (propertiesListSerializedProperty == null) { Debug.Log("propertiesListSerializedProperty is null!"); return; }
            if (!propertiesListSerializedProperty.isArray) { Debug.Log("propertiesListSerializedProperty is not an array!"); return; }

            if (0 <= index && index < propertiesListSerializedProperty.arraySize)
            {
                var propertySerializedProperty = propertiesListSerializedProperty.GetArrayElementAtIndex(index);
                if (propertySerializedProperty == null) { Debug.Log("propertySerializedProperty[" + index + "] is null!"); return; }

                // If you have a custom property drawer, you can use PropertyField:
                //---EditorGUI.PropertyField(rect, propertySerializedProperty);

                // I didn't bother with one, so I just use TextField:
                propertySerializedProperty.FindPropertyRelative("name").stringValue =
                    EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width / 2, rect.height),
                    propertySerializedProperty.FindPropertyRelative("name").stringValue);
                propertySerializedProperty.FindPropertyRelative("value").stringValue =
                    EditorGUI.TextField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height),
                    propertySerializedProperty.FindPropertyRelative("value").stringValue);
            }
        }
    }

    [MenuItem("Assets/Create/ItemObjectDatabase")]
    public static void CreateAsset()
    {
        CreateAsset<ArraySO>();
    }

    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}