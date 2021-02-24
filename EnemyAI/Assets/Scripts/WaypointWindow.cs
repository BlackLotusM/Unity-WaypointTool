using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;

public class WaypointWindow : EditorWindow
{
    GUIStyle mystule = new GUIStyle();

    private ReorderableList m_itemList = null;
    private ReorderableList m_propertiesList = null;
    private SerializedObject so = null;

    int count;
    string[] text = new string[] { "Load by name", "Load by file" };
    private string fileName;
    Object so2;

    [MenuItem("Window/Waypoint Tool")]
    public static void OpenWindow()
    {
        ShowWindow(Selection.activeObject as ArraySO);
    }

    public static void ShowWindow(ArraySO itemDatabaseObject)
    {
        var window = GetWindow<WaypointWindow>("WaypointWindow");
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
            m_itemList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Waypoints"); };
            so = new SerializedObject(itemDatabaseObject);
        }
        m_propertiesList = null;
        Repaint();
    }
    
    private void Update()
    {
        
    }
    void OnGUI()
    {
        mystule.fontSize = 18;
        mystule.normal.textColor = Color.white;
        mystule.hover.textColor = Color.red;
        

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Create new", mystule);
        fileName = EditorGUILayout.TextField("Name: ", fileName);

        if (GUILayout.Button("Create"))
        {
            if(fileName != "")
            {
                GUILayout.Space(30);
                var myType = AssetDatabase.LoadAssetAtPath("Assets/WayPointSaves/"+fileName+".asset", typeof(ArraySO)) as ArraySO;
                if (myType == null) 
                { 
                    Debug.Log("nothing");
                    this.ShowNotification(new GUIContent("New asset has been created"));
                    ArraySO asset = ScriptableObject.CreateInstance<ArraySO>();
                    if (Directory.Exists("Assets/WayPointSaves/"))
                    {
                        AssetDatabase.CreateAsset(asset, "Assets/WayPointSaves/" + fileName + ".asset");
                        AssetDatabase.SaveAssets();
                        Selection.activeObject = asset;
                    }
                    else
                    {
                        Directory.CreateDirectory("Assets/WayPointSaves/");
                        AssetDatabase.CreateAsset(asset, "Assets/WayPointSaves/" + fileName + ".asset");
                        AssetDatabase.SaveAssets();
                        Selection.activeObject = asset;
                    }
                }
                else 
                {
                    this.ShowNotification(new GUIContent("Already existed, loaded asset"));
                    Selection.activeObject = myType;
                }  
            }
        }
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
        GUILayout.Space(20); 
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("Load existing", mystule);
        
        count = GUILayout.SelectionGrid(count, text, 2, EditorStyles.radioButton);
        GUILayout.EndHorizontal();

        if(count == 0)
        {
            fileName = EditorGUILayout.TextField("Name: ", fileName);
        }
        else
        {
            so2 = EditorGUILayout.ObjectField(so2, typeof(ArraySO), true);
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
}