using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ComponentFinder
{
    public class ComponentFinderWindow : EditorWindow
    {
        string userKeyword = string.Empty;
        Vector2 scrollPosition;
        List<Component> resultComponents = new List<Component>();

        [MenuItem("Window/Component Finder")]
        static void Open()
        {
            ComponentFinderWindow.GetWindow<ComponentFinderWindow>("CompFinder");
        }

        void OnGUI()
        {
            // Check user input
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Keyword: ", GUILayout.Width(58));
            userKeyword = EditorGUILayout.TextField(userKeyword, GUILayout.Width(Screen.width - 70));
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                resultComponents.Clear();
                FindComponent(userKeyword.ToUpper());
            }

            // Show results
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.BeginVertical();
            foreach (var component in resultComponents)
            {
                if (component != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(component, typeof(Component), false);
                    EditorGUILayout.LabelField(component.GetType().ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        GameObject[] GetRootGameObjects()
        {
            return System.Array.FindAll(GameObject.FindObjectsOfType<GameObject>(), (item) => item.transform.parent == null);
        }

        void FindComponent(string keyword)
        {
            foreach (var go in GetRootGameObjects())
            {
                FindComponent(go, keyword);
            }
        }

        void FindComponent(GameObject go, string keyword)
        {
            // Find component
            foreach (var component in go.GetComponents(typeof(Component)))
            {
                var componentName = component.GetType().ToString().ToUpper();
                if (0 < componentName.ToUpper().IndexOf(keyword))
                {
                    resultComponents.Add(component);
                }
            }
            // Find child component
            for (int i = 0, count = go.transform.childCount; i < count; ++i)
            {
                FindComponent(go.transform.GetChild(i).gameObject, keyword);
            }
        }
    }
}
