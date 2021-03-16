using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine.UIElements;

public class WaypointWindow : EditorWindow
{
    GUIStyle mystule = new GUIStyle();
    Vector2 scrollPos;
    private ReorderableList m_itemList = null;
    private SerializedObject so = null;
    private string fileName;
    ArraySO so2;
    ArraySO so2Old;
    GizmosDrawer displayObject;
    int countName;
    int count;
    bool loop, goldenpath;
    public GameObject gizmoDrawPref;

    string[] text = new string[] { "Load by name", "Load by file" };

    [MenuItem("Window/Waypoint Tool")]
    private static void Init()
    {
        WaypointWindow window = (WaypointWindow)EditorWindow.GetWindow(typeof(WaypointWindow));
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += SceneGUI;
        SelectDatabaseObject(so2 as ArraySO);
        mystule.fontSize = 18;
        mystule.normal.textColor = Color.white;
        
    }

    private static void ShowWindow(ArraySO itemDatabaseObject)
    {
        var window = GetWindow<WaypointWindow>("WaypointWindow");
        window.SelectDatabaseObject(itemDatabaseObject);
    }
    
    void SelectDatabaseObject(ArraySO itemDatabaseObject)
    {
        try
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
                        displayObject.Spawned[index].gameObject.transform.position = EditorGUI.Vector3Field(new Rect(rect.x + 160, rect.y, 200, EditorGUIUtility.singleLineHeight), "", displayObject.Spawned[index].gameObject.transform.position);

                    }
                    catch { }
                };

            so = new SerializedObject(itemDatabaseObject);
            Repaint();
        }
        catch
        {
           // Repaint();
        }
    }
  
    void OnGUI()
    {
        
        if (!AssetDatabase.IsValidFolder("Assets/WayPointSaves")) {
            {
                so = null;
                so2 = null;
                so2Old = null;
            } 
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
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

        GUILayout.Label("Create new", mystule);
        fileName = EditorGUILayout.TextField("Name: ", fileName);
        GUILayout.Space(10);

        //Create Button here
        if (GUILayout.Button("Create"))
        {
            //Checks if textbox is empty
            if (fileName != "")
            {
                GUILayout.Space(10);
                var myType = AssetDatabase.LoadAssetAtPath("Assets/WayPointSaves/" + fileName + ".asset", typeof(ArraySO)) as ArraySO;
                if (myType == null)
                {
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

        //Load here
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
        }

        GUILayout.Space(10);

        if (so2 != null)
        {
            GUILayout.Label("Shortcuts");
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.HelpBox("Make sure the sceneview is selected for the shortcuts to work", MessageType.Warning);
            EditorGUILayout.EndHorizontal();
            EditorStyles.helpBox.fontSize = 13;
            EditorStyles.helpBox.fixedWidth = 120;
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.HelpBox("NumberKey 6  ", MessageType.Info);
            GUILayout.Box("If waypoint selected follow the mouse position. If active and pressed again it will stop following");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.HelpBox("NumberKey 7  ", MessageType.Info);
            GUILayout.Box("If waypoint selected dupplicate it. Recommended: press 6 after 7");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.HelpBox("NumberKey 8  ", MessageType.Info);
            GUILayout.Box("Tries to spawn waypoint at mouseposition, if it isn't able to place a waypoint or its the first one it will spawn at (0,0,0)");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.HelpBox("NumberKey 9  ", MessageType.Info);
            GUILayout.Box("Delete selected waypoint");
            EditorGUILayout.EndHorizontal();

            EditorStyles.helpBox.fontSize = 13;
            EditorStyles.helpBox.fixedWidth = 0;

            GUILayout.Space(10);
            GUILayout.Space(10);

            loop = GUILayout.Toggle(loop, "Loop waypoints");
            GUILayout.Space(10);
            
            goldenpath = GUILayout.Toggle(goldenpath, "Show Navmesh Path");
            GUILayout.Label("Fir this option you need to have a navmesh baked");
            displayObject.loop = loop;
            displayObject.navmesh = goldenpath;

            GUILayout.Space(10);
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.HelpBox("The AI will always loop, it's up to the programmer to do whatever they want with the array", MessageType.Info);
            EditorGUILayout.EndHorizontal();
            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Stop live AI preview"))
                {
                    EditorApplication.ExitPlaymode();
                }
            }
            else
            {
                if (GUILayout.Button("Start live AI preview"))
                {
                    EditorApplication.EnterPlaymode();
                }
            }
            

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.HelpBox("To change the name or coordinates in the editor make sure no object is selected in the sceneview", MessageType.Warning);

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Click here to make sure you have nothing selected"))
            {
                Selection.activeObject = null;
            }
            GUILayout.Space(20);
            GUILayout.Label("Currently active: " + so2.name, mystule);
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

    void AddItemCustom(ReorderableList itemList, Vector3 coords)// as ArraySO)
    {
        countName = 0;
        ArraySO.Waypoint t = new ArraySO.Waypoint();
        t.name = "Waypoint" + (countName);

        if (so2.WaypointList.Any(innerList => innerList.name.Contains(t.name)))
        {
            while (so2.WaypointList.Any(innerList => innerList.name.Contains(t.name)))
            {
                t.name = "Waypoint" + countName;
                t.Coords = coords;
                countName++;
            }
        }

        itemList.list.Add(t);
        itemList.index = itemList.count - 1;
        GUILayout.EndScrollView();
        Repaint();
        displayObject.Redo();
        displayObject.sel(itemList.count - 1);
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
        displayObject.Redo();
        displayObject.sel(itemList.count - 1);
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


    void SceneGUI(SceneView sceneView)
    {
        var e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Alpha6)
        {
            e.Use();
            if (m_itemList == null)
            {
                this.ShowNotification(new GUIContent("No array has been loaded shortcuts won't work"));
                Repaint();
            }
            else
            {
                if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<DrawSphereGizmo>() == null)
                {
                    this.ShowNotification(new GUIContent("Make sure to select a waypoint to move it, if non exist try shortcut(8)"));
                    Repaint();
                    return;
                }else if(Selection.activeGameObject.GetComponent<DrawSphereGizmo>() != null)
                {
                    DrawSphereGizmo currentDrawGizmo = Selection.activeGameObject.GetComponent<DrawSphereGizmo>();
                    currentDrawGizmo.followMouse = !currentDrawGizmo.followMouse;
                } 
            }
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Alpha7)
        {
            e.Use();
            if (m_itemList == null)
            {
                this.ShowNotification(new GUIContent("No array has been loaded shortcuts won't work"));
                Repaint();
            }
            else
            {
                if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<DrawSphereGizmo>() == null)
                {
                    this.ShowNotification(new GUIContent("Make sure to select a waypoint to move it, if non exist try shortcut(8)"));
                    Repaint();
                    return;
                }
                else if(Selection.activeGameObject.GetComponent<DrawSphereGizmo>() != null)
                {
                    DubItem(m_itemList, Selection.activeGameObject.transform.position);
                    displayObject.Redo();
                    SelectItemNumber(m_itemList.index);
                }
            }
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Alpha8)
        {
            if (m_itemList == null)
            {
                this.ShowNotification(new GUIContent("No array has been loaded shortcuts won't work"));
                Repaint();
            }
            else
            {
                e.Use();
                Ray mousepos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                Ray pRay = mousepos;
                if (Physics.Raycast(pRay, out hit))
                {
                    if (hit.collider)
                    {
                        AddItemCustom(m_itemList, hit.point);
                        //this.transform.position = hit.point;
                    }
                }
                else
                {
                    AddItemCustom(m_itemList, new Vector3(0, 0, 0));
                }
                //AddItem(m_itemList);
                displayObject.Redo();
                SelectItemNumber(m_itemList.index);
            }
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Alpha9)
        {
            e.Use();
            if (m_itemList == null)
            {
                this.ShowNotification(new GUIContent("No array has been loaded shortcuts won't work"));
                Repaint();
            }
            else
            {
                if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<DrawSphereGizmo>() == null)
                {
                    this.ShowNotification(new GUIContent("Make sure to select a waypoint to move it, if non exist try shortcut(8)"));
                    Repaint();
                    return;
                }
                else if (Selection.activeGameObject.GetComponent<DrawSphereGizmo>() != null)
                {
                    RemoveItem(m_itemList);
                    displayObject.Redo();
                    try
                    {
                        SelectItemNumber(m_itemList.index);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    this.ShowNotification(new GUIContent("Make sure to select a waypoint"));
                    Repaint();
                }
            }
        }
    }
}
