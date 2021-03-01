using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;

public class WaypointWindow : EditorWindow
{
    GUIStyle mystule = new GUIStyle();
    public GameObject gizmo;
    private ReorderableList m_itemList = null;
    private SerializedObject so = null;
    public GizmosDrawer gz;

    int count;
    string[] text = new string[] { "Load by name", "Load by file" };
    private string fileName;
    ArraySO so2;
    Object so2Old;
    GizmosDrawer displayObject;

    [MenuItem("Window/Waypoint Tool")]
    public static void Init()
    {
        WaypointWindow window = (WaypointWindow)EditorWindow.GetWindow(typeof(WaypointWindow));
        window.Show();
    }

    public static void ShowWindow(ArraySO itemDatabaseObject)
    {
        var window = GetWindow<WaypointWindow>("WaypointWindow");
        window.SelectDatabaseObject(itemDatabaseObject);
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
            m_itemList = new ReorderableList(itemDatabaseObject.WaypointList, typeof(ArraySO), true, true, true, true);
            
            m_itemList.onAddCallback += AddItem;
            m_itemList.onSelectCallback += SelectItem;
            m_itemList.onRemoveCallback += RemoveItem;
            m_itemList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Waypoints"); };
            m_itemList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), itemDatabaseObject.WaypointList[index].name);
                    EditorGUI.Vector3Field(new Rect(rect.x + 100, rect.y, 200, EditorGUIUtility.singleLineHeight), "", itemDatabaseObject.WaypointList[index].Coords);
                    if(GUI.Button(new Rect(rect.x + 310, rect.y, 50, EditorGUIUtility.singleLineHeight), "Edit"))
                    {
                        Debug.Log(itemDatabaseObject.WaypointList[index].Coords.ToString());
                        displayObject.sel(index);
                        //Selection.activeGameObject = gz.Spawned[index];
                    }
                };

            so = new SerializedObject(itemDatabaseObject);
            SerializedProperty t =  so.FindProperty("m_Name");
            t.stringValue = "test";
            //so.FindProperty("_name").stringValue = "r";
        }
        Repaint();
    }

    void OnGUI()
    {
        if (displayObject == null)
        {
            displayObject = FindObjectOfType<GizmosDrawer>().gameObject.GetComponent<GizmosDrawer>();
        }
        mystule.fontSize = 18;
        mystule.normal.textColor = Color.white;
        mystule.hover.textColor = Color.red;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Create new", mystule);
        fileName = EditorGUILayout.TextField("Name: ", fileName);
        GUILayout.Space(10);

        if (GUILayout.Button("Create"))
        {
            if (fileName != "")
            {
                GUILayout.Space(10);
                var myType = AssetDatabase.LoadAssetAtPath("Assets/WayPointSaves/" + fileName + ".asset", typeof(ArraySO)) as ArraySO;
                if (myType == null)
                {
                    Debug.Log("nothing");
                    this.ShowNotification(new GUIContent("New asset has been created"));
                    ArraySO asset = ScriptableObject.CreateInstance<ArraySO>();
                    if (Directory.Exists("Assets/WayPointSaves/"))
                    {
                        AssetDatabase.CreateAsset(asset, "Assets/WayPointSaves/" + fileName + ".asset");
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        Directory.CreateDirectory("Assets/WayPointSaves/");
                        AssetDatabase.CreateAsset(asset, "Assets/WayPointSaves/" + fileName + ".asset");
                        AssetDatabase.SaveAssets();
                    }
                    so2 = asset;
                    SelectDatabaseObject(so2 as ArraySO);
                }
                else
                {
                    this.ShowNotification(new GUIContent("Already existed, loaded asset"));
                    so2 = myType;
                    SelectDatabaseObject(so2 as ArraySO);
                }
            }
            else {
                this.ShowNotification(new GUIContent("File name can't be empty"));
            }
        }

        GUILayout.Space(10); 
        GUILayout.EndVertical();
        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Load existing", mystule);
        
        count = GUILayout.SelectionGrid(count, text, 2, EditorStyles.radioButton);
        
        if(count == 0)
        {
            fileName = EditorGUILayout.TextField("Name: ", fileName);
        }
        else
        {
            so2 = (ArraySO)EditorGUILayout.ObjectField("File", so2, typeof(ArraySO), true);
            
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Load"))
        {
            ArraySO myType = AssetDatabase.LoadAssetAtPath("Assets/WayPointSaves/" + fileName + ".asset", typeof(ArraySO)) as ArraySO;
            if (myType == null)
            {
                this.ShowNotification(new GUIContent("This object does not yet exist"));
                so2 = null;
            }
            else
            {
                so2 = myType;
                SelectDatabaseObject(so2 as ArraySO);
            }
            displayObject.hideFlags = HideFlags.None;

        }
        GUILayout.Space(10);
        GUILayout.EndVertical();
        if (so != null)
        {
            GUILayout.Label("Currently active: " + so2.name, mystule);
        }
        else
        {
            GUILayout.Label("Currently active: None" , mystule);
        }

        displayObject = (GizmosDrawer)EditorGUILayout.ObjectField("File", displayObject, typeof(GameObject), true);
        
        if (so != null && m_itemList != null)
        {
            so.Update();
            m_itemList.DoLayoutList();
            so.ApplyModifiedProperties();
        }
        if (so2Old != so2)
        {
            SelectDatabaseObject(so2);
            so2Old = so2;
        }

        if (so2 != null)
        {
            displayObject.loadedArray = so2;
            EditorUtility.SetDirty(so2);
        }
    }

    void AddItem(ReorderableList itemList)
    {
        itemList.list.Add(new ArraySO.Waypoint());
        itemList.index = itemList.count - 1;   
    }

    void RemoveItem(ReorderableList itemList)
    {
        // When we remove an item, clear the properties list:
        ReorderableList.defaultBehaviours.DoRemoveButton(itemList);
        Repaint();
    }

    void SelectItem(ReorderableList itemList)
    {
        displayObject.sel(itemList.index);
    }
}
