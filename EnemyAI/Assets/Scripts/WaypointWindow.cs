using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine.UIElements;

public class WaypointWindow : EditorWindow
{
    GUIStyle mystule = new GUIStyle();
    private ReorderableList m_itemList = null;
    private SerializedObject so = null;
    public GizmosDrawer gz;
    public GameObject gizmoDrawPref;
    bool loop;
    bool follow;
    int countName;
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

    void OnEnable()
    {
        SceneView.duringSceneGui += SceneGUI;
    }

    void SceneGUI(SceneView sceneView)
    {
        try
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.P)
            {
                e.Use();
                DrawSphereGizmo currentDrawGizmo = Selection.activeGameObject.GetComponent<DrawSphereGizmo>();
                currentDrawGizmo.followMouse = !currentDrawGizmo.followMouse;
                Debug.Log("Right Click");
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.N)
            {
                e.Use();
                AddItem(m_itemList);
                displayObject.Redo();
                SelectItemNumber(m_itemList.index);
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.D)
            {
                e.Use();
                if (Selection.activeGameObject.GetComponent<DrawSphereGizmo>())
                {
                    DubItem(m_itemList, Selection.activeGameObject.transform.position);
                    displayObject.Redo();
                    SelectItemNumber(m_itemList.index);
                }
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
            {
                e.Use();
                if (Selection.activeGameObject.GetComponent<DrawSphereGizmo>())
                {
                    DubItem(m_itemList, Selection.activeGameObject.transform.position);
                    displayObject.Redo();
                    SelectItemNumber(m_itemList.index);
                }
            }
        }
        catch
        {
            Debug.Log("test");
        } 
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
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), "name: ");
                    try
                    {
                        itemDatabaseObject.WaypointList[index].name = EditorGUI.TextField(new Rect(rect.x + 45, rect.y, 100, EditorGUIUtility.singleLineHeight), itemDatabaseObject.WaypointList[index].name);
                        displayObject.Spawned[index].gameObject.transform.position = EditorGUI.Vector3Field(new Rect(rect.x + 160, rect.y, 200, EditorGUIUtility.singleLineHeight), "", itemDatabaseObject.WaypointList[index].Coords);
                        
                    }
                    catch { }
                };

            so = new SerializedObject(itemDatabaseObject);
        }
        Repaint();
    }
    Vector2 scrollPos;
    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos,
                                                      false,
                                                      false);
        GUILayout.BeginVertical();
        if (displayObject == null)
        {
            try
            {
                displayObject = FindObjectOfType<GizmosDrawer>().gameObject.GetComponent<GizmosDrawer>();
            }
            catch
            {
                Instantiate(gizmoDrawPref);
            }
        }

        mystule.fontSize = 18;
        mystule.normal.textColor = Color.white;
        mystule.hover.textColor = Color.red;

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
        //GUILayout.EndVertical();
        //GUILayout.BeginVertical("box");
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
        //GUILayout.EndVertical();
        if (so2 != null)
        {
            GUILayout.Label("Currently active: " + so2.name, mystule);
            loop = GUILayout.Toggle(loop, "Loop waypoints");
            displayObject.loop = loop;
            displayObject.follow = follow;
        }
        else
        {
            GUILayout.Label("Currently active: None" , mystule);
        }

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
        else
        {
            if (displayObject != null)
            {
                displayObject.loadedArray = null;
            }
        }

        GUILayout.EndVertical();

        EditorGUILayout.EndScrollView();

    }

    void AddItem(ReorderableList itemList)// as ArraySO)
    {
        countName = 0;
        ArraySO.Waypoint t = new ArraySO.Waypoint();
        t.name = "Waypoint" + (countName);

        if (so2.WaypointList.Any(innerList => innerList.name.Contains(t.name)))
        {
            while (so2.WaypointList.Any(innerList => innerList.name.Contains(t.name)))
            {
                t.name = "Waypoint" + countName;
                countName++;
            }
        }

        itemList.list.Add(t);
        itemList.index = itemList.count - 1;
        GUILayout.EndScrollView();
        Repaint();
    }

    void DubItem(ReorderableList itemList, Vector3 pos)// as ArraySO)
    {
        countName = 0;
        ArraySO.Waypoint t = new ArraySO.Waypoint();
        t.name = "Waypoint" + (countName);
        t.Coords = pos;

        if (so2.WaypointList.Any(innerList => innerList.name.Contains(t.name)))
        {
            while (so2.WaypointList.Any(innerList => innerList.name.Contains(t.name)))
            {
                t.name = "Waypoint" + countName;
                countName++;
            }
        }

        itemList.list.Add(t);
        itemList.index = itemList.count - 1;
        GUILayout.EndScrollView();
        Repaint();
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
        Repaint();
    }

    void SelectItemNumber(int itemList)
    {
        displayObject.sel(itemList);
        Repaint();
    }
}
