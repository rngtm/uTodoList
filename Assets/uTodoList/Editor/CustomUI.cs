﻿///-----------------------------------
/// uTodoList
/// @ 2017 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace uTodoList
{
    using UnityEngine;
    using UnityEditor;
    using System.Linq;

    /// <summary>
    /// UI定義クラス
    /// </summary>
    public static class CustomUI
    {
        static GUIStyle CreateButtonStyle(Color color)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = color;
            return style;
        }
        
        /// <summary>
        /// 色つきボタンの表示
        /// </summary>
        public static bool Button(string text, Color textColor, Color bgColor, params GUILayoutOption[] options)
        {
            var style = CreateButtonStyle(textColor);
            var defautlColor = GUI.color;
            GUI.backgroundColor = bgColor;
            bool click = GUILayout.Button(text, style);
            GUI.backgroundColor = defautlColor;
            
            return click;
        }

        /// <summary>
        /// 折り畳みをかっこよく表示
        /// </summary>
        public static bool Foldout(string title, bool display)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }

    }
}
