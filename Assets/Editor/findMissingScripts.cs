using UnityEditor;
using UnityEngine;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScripts));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in Scene"))
        {
            FindInScene();
        }

        if (GUILayout.Button("Find Missing Scripts in Prefabs"))
        {
            FindInPrefabs();
        }
    }

    private static void FindInScene()
    {
        GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
        int missingScriptCount = 0;

        foreach (GameObject g in goArray)
        {
            Component[] components = g.GetComponents<Component>();

            foreach (Component c in components)
            {
                if (c == null)
                {
                    Debug.LogError("Missing script found in GameObject: " + g.name, g);
                    missingScriptCount++;
                }
            }
        }

        Debug.Log("Finished searching. Found " + missingScriptCount + " GameObjects with missing scripts.");
    }

    private static void FindInPrefabs()
    {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths();
        int missingScriptCount = 0;

        foreach (string path in prefabPaths)
        {
            if (path.EndsWith(".prefab"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    Component[] components = prefab.GetComponentsInChildren<Component>(true);

                    foreach (Component c in components)
                    {
                        if (c == null)
                        {
                            Debug.LogError("Missing script found in prefab: " + path, prefab);
                            missingScriptCount++;
                        }
                    }
                }
            }
        }

        Debug.Log("Finished searching. Found " + missingScriptCount + " prefabs with missing scripts.");
    }
}
