using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ComponentFinder
{
    public class ComponentFinder
    {
        readonly List<Component> components = new List<Component>();
        readonly List<Component> missings = new List<Component>();

        GameObject[] GetRootGameObjects()
        {
            return System.Array.FindAll(GameObject.FindObjectsOfType<GameObject>(), (item) => item.transform.parent == null);
        }

        public static string GetHierarchyPath(Transform t)
        {
            var builder = new StringBuilder();

            var parent = t.parent;
            while (parent != null)
            {
                builder.Insert(0, $"/{parent.name}");
                parent = parent.parent;
            }

            builder.Append($"/{t.gameObject.name}");
            return builder.ToString();
        }

        public List<Component> Find(string keyword)
        {
            Debug.Log(SceneManager.GetActiveScene().name);

            components.Clear();

            foreach (var go in GetRootGameObjects())
            {
                for (int i = 0; i < 200; ++i)
                {

                    FindComponent(go, keyword);
                }
            }

            return components;
        }

        public List<Component> FindMissing()
        {
            missings.Clear();

            foreach (var go in GetRootGameObjects())
            {
                FindMissing(go);
            }

            return missings;
        }

        void FindMissing(GameObject go)
        {
            var cs = go.GetComponents<Component>();

            foreach (var c in cs)
            {
                if (c == null)
                {
                    Debug.Log("Missing Scirpt!");
                }
                else
                {
                    var so = new SerializedObject(c);
                    var sp = so.GetIterator();

                    while (sp.NextVisible(true))
                    {
                        if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;
                        if (sp.objectReferenceValue != null) continue;
                        if (!sp.hasChildren) continue;
                        var fileId = sp.FindPropertyRelative("m_FileID");
                        if (fileId == null) continue;
                        if (fileId.intValue == 0) continue;

                        Debug.Log("Missing!");
                        missings.Add(c);
                        break;
                    }
                }
            }
        }

        void FindComponent(GameObject go, string keyword)
        {
            // Find component
            foreach (var component in go.GetComponents(typeof(Component)))
            {
                if (component != null)
                {
                    var componentName = component.GetType().ToString();
                    if (0 < componentName.ToUpper().IndexOf(keyword))
                    {
                        components.Add(component);
                    }
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
