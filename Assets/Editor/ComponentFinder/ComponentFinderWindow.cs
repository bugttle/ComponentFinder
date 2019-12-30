using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ComponentFinder
{
    public class ComponentFinderWindow : EditorWindow
    {
        enum Tab
        {
            Name,
            Missing,
        }
        Tab _tab;

        readonly ComponentFinder componentFinder = new ComponentFinder();

        string keyword = string.Empty;
        Vector2 scrollPosition;
        List<Component> components;
        List<Component> missings;
        float rootHeight = 0f;

        [MenuItem("Window/Component Finder")]
        static void Open()
        {
            GetWindow<ComponentFinderWindow>("CompFinder");
        }

        private static class Styles
        {
            private static GUIContent[] _tabToggles = null;
            public static GUIContent[] TabToggles
            {
                get
                {
                    if (_tabToggles == null)
                    {
                        _tabToggles = System.Enum.GetNames(typeof(Tab)).Select(x => new GUIContent(x)).ToArray();
                    }
                    return _tabToggles;
                }
            }

            public static readonly GUIStyle TabButtonStyle = "LargeButton";
            public static readonly GUI.ToolbarButtonSize TabButtonSize = GUI.ToolbarButtonSize.Fixed;
        }

        Tab OnGUI_DrawTab(Tab tab)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                // タブを描画する
                tab = (Tab)GUILayout.Toolbar((int)tab, Styles.TabToggles, Styles.TabButtonStyle, Styles.TabButtonSize);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            return tab;
        }

        void OnGUI_DrawName()
        {
            // Check user input
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Keyword: ", GUILayout.Width(58));
                keyword = EditorGUILayout.TextField(keyword, GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                components = componentFinder.Find(keyword.ToUpper());
            }
        }

        void OnGUI_DrawMissing()
        {
            if (GUILayout.Button("Missing"))
            {
                missings = componentFinder.FindMissing();
            }
        }

        void OnGUI_DrawResults(List<Component> items)
        {
            if (items == null)
            {
                return;
            }


            // Show results
            var itemRoot = EditorGUILayout.BeginVertical();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // 上部の描画が不要なエリアをスペースで埋める
            var lineHeight = 16;
            var startIndex = (int)(scrollPosition.y / lineHeight);
            GUILayout.Space(startIndex * lineHeight);

            var listCount = items.Count;
            var endIndex = listCount;
            if (0 < rootHeight)
            {
                 endIndex = startIndex + (int)(rootHeight / lineHeight);
                if (endIndex > listCount)
                {
                    endIndex = listCount;
                }
            }

            Debug.Log("startIndex * lineHeight:" + startIndex * lineHeight + ", endIndex:" + endIndex);

            for (int i = startIndex; i < endIndex; ++i)
            {
                var item = items[i];
                if (item != null)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(lineHeight));
                    {
                        EditorGUILayout.ObjectField(item, typeof(Component), false);
                        EditorGUILayout.TextField(ComponentFinder.GetHierarchyPath(item.transform));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            // 下部の描画が不要なエリアをスペースで埋める
            GUILayout.Space((listCount - endIndex) * lineHeight);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            // スクロールエリアの描画が完了したタイミングで更新
            if (itemRoot.height > 0f)
            {
                rootHeight = itemRoot.height;
            }
        }

        void OnGUI()
        {
            _tab = OnGUI_DrawTab(_tab);

            switch (_tab)
            {
                case Tab.Name:
                    OnGUI_DrawName();
                    OnGUI_DrawResults(components);
                    break;
                case Tab.Missing:
                    OnGUI_DrawMissing();
                    OnGUI_DrawResults(missings);
                    break;
            }

        }
    }
}
