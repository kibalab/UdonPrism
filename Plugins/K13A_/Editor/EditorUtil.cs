using System;
using UnityEditor;
using UnityEngine;

namespace K13A.BehaviourEditor
{
    public class DetachedEditor : EditorWindow
    {
        public Action Render;
        void OnGUI () {
            if(Render == null) this.Close();
            
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical();
            Render.Invoke();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
    public static class EditorUtil
    {

        public static void DrawOptions(TitleOption[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                options[i].OnGUI(i);
            }
        }

        public static void DrawTitle(string Title, Action contant, ContentStyle style)
        {
            EditorGUILayout.LabelField("", GUI.skin.window, GUILayout.Height(25));
            EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), $"<size=15>    <b>{Title}</b></size>", GUI.skin.label);
            
            var options = TitleOptionsBuilder.Build( new TitleOption[]
            {
                new TitleOption(new GUIContent("Test1"), () =>
                {
                    var winbow = EditorWindow.GetWindow(typeof(DetachedEditor)) as DetachedEditor;
                    winbow.Render = contant;
                    winbow.titleContent = new GUIContent(Title);
                })
            });
            
            DrawOptions(options.ToArray());
        }
        
        public static void DrawSubTitle(string Title)
        {
            EditorGUILayout.LabelField("", GUI.skin.window, GUILayout.Height(20), GUILayout.MaxWidth(GUI.skin.window.CalcSize(new GUIContent($"    {Title}    ")).x));
            EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), $"    <b>{Title}</b>", GUI.skin.label);
            GUI.Box(new Rect(
                GUILayoutUtility.GetLastRect().x + GUILayoutUtility.GetLastRect().width + 20, 
                GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height / 2, 
                 EditorGUIUtility.currentViewWidth - GUILayoutUtility.GetLastRect().x - GUILayoutUtility.GetLastRect().width - 40, 
                2), 
                "");
            GUILayout.Space(5);
        }
        
        public static bool DrawFoldoutTitle(bool b, string Title)
        {
            EditorStyles.foldout.richText = true;
            if (GUILayout.Button("", GUI.skin.window, GUILayout.Height(25)))
                b = !b;
            EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), $"<size=15>    <b>{Title}</b></size>", GUI.skin.label);
            return b;
        }
        
        public static void MenuBox(string Title, Action contant, ContentStyle style)
        {
            var origin_font = GUI.skin.label.font;
            
            EditorGUILayout.BeginVertical();
                DrawTitle(Title, contant, style);
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    GUI.skin.label.font = style.font;
                    if(style.UpperBorder) EditorGUILayout.Space(10);
                    EditorGUILayout.BeginVertical();
                        EditorGUILayout.Space(5);
                        contant.Invoke();
                        GUILayout.Space(5);
                    EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
            
            GUI.skin.label.font = origin_font;
        }
        
        public static void SubMenuBox(string Title, Action contant, ContentStyle style)
        {
            var origin_font = GUI.skin.label.font;
            DrawSubTitle(Title);
            GUI.skin.label.font = style.font;
            
            EditorGUILayout.BeginVertical();
            contant.Invoke();
            EditorGUILayout.EndVertical();
            
            GUI.skin.label.font = origin_font;
        }
        
        public static bool FoldoutMenuBox(string Title, bool b, Action contant, ContentStyle style)
        {
            var origin_font = GUI.skin.label.font;
            b = DrawFoldoutTitle(b, Title);

            if (!b)
            {
                GUILayout.Space(5);
                return b;
            }
            
            GUI.skin.label.font = style.font;
            
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if(style.UpperBorder) EditorGUILayout.Space(10);
                EditorGUILayout.BeginVertical();
                    EditorGUILayout.Space(5);
                    contant.Invoke();
                    GUILayout.Space(5);
                EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            GUI.skin.label.font = origin_font;
            
            EditorGUILayout.Space(5);
            
            return b;
        }

        public static float Slider(float value)
        {
            return 0;
        }
    }
}