using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UdonSharpEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using K13A.BehaviourEditor;
using Prism.BehaviourEditor;

namespace Prism.Setup
{
    [CustomEditor(typeof(PrismSetup))]
    public class PrismEditor : Editor
    {
        public PrismSetup origin;
        public Texture2D LOGO;
        private ReorderableList BehaviourList, CommandList;

        private void OnEnable()
        {
            #region ReorderableList Initialize

            {
                if(PrefabUtility.IsPartOfPrefabInstance(((PrismSetup)target).gameObject))
                    PrefabUtility.UnpackPrefabInstance(((PrismSetup)target).gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                
                var behaviourArray = serializedObject.FindProperty("events");
                BehaviourList = new ReorderableList(serializedObject, behaviourArray, true, true, true, true);

                BehaviourList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Evnet Invoke Target (UdonBehaviour)");

                BehaviourList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var behaviour = behaviourArray.GetArrayElementAtIndex(index);
                    rect.height -= 4;
                    rect.y += 2;
                    EditorGUI.PropertyField(rect, behaviour, new GUIContent(""));
                };
            }

            {
                var commandArray = serializedObject.FindProperty("endPointCommand");
                CommandList = new ReorderableList(serializedObject, commandArray, true, true, true, true);

                CommandList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Defined EndPoint Commands (URL/String)");

                CommandList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var behaviour = commandArray.GetArrayElementAtIndex(index);
                    rect.height -= 4;
                    rect.y += 2;
                    EditorGUI.PropertyField(rect, behaviour, new GUIContent(""));
                };
            }

            #endregion
        }

        public override void OnInspectorGUI()
        {
            origin = (PrismSetup)target;

            GUI.skin.label.richText = true;
            GUILayout.Space(20);
            var titleStyle = new GUIStyle();
            titleStyle.normal.background = null;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Box(LOGO, titleStyle, GUILayout.ExpandWidth(true), GUILayout.Height(100));
            GUILayout.Space(20);
            
            
            EditorUtil.MenuBox("Setup", () =>
            {
                EditorGUILayout.Space(20);
                
                EditorUtil.SubMenuBox("General", () =>
                {
                    ReSize(EditorGUILayout.FloatField("Size", origin.Size));

                    origin.InputField.SetActive(EditorGUILayout.Toggle("Use Input Field", origin.InputField.activeSelf));
                }, new ContentStyle(GUI.skin.font));
                
                EditorGUILayout.Space(20);
                
                EditorUtil.SubMenuBox("Network", () =>
                {
                    origin.StartUrl = EditorGUILayout.TextField("Start Url", origin.StartUrl);
                    origin.c_InputField.SetUrl(new VRCUrl(origin.StartUrl));
                    serializedObject.Update();
                        CommandList.DoLayoutList();
                        SetEndPoints(false);
                    serializedObject.ApplyModifiedProperties();
                }, new ContentStyle(GUI.skin.font));
                
            }, new ContentStyle(GUI.skin.font));

            EditorUtil.MenuBox("Event", () =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField($" ", GUI.skin.label);
                    BehaviourList.DoLayoutList();
                    SetEvents();
                serializedObject.ApplyModifiedProperties();
            }, new ContentStyle(GUI.skin.font));

            EditorGUILayout.Space(40);
            
            origin.CraditOpen = EditorUtil.FoldoutMenuBox("Cradit", origin.CraditOpen, () =>
            {
                EditorUtil.DrawSubTitle("System");
                EditorGUILayout.LabelField($"    <b>KIBA</b>", GUI.skin.label);
                EditorGUILayout.LabelField($"    <b>SieR</b>", GUI.skin.label);
                EditorGUILayout.Space(10);
                EditorUtil.DrawSubTitle("Deisgn");
                EditorGUILayout.LabelField($"    <b>KIBA</b>", GUI.skin.label);
                EditorGUILayout.Space(10);
            }, new ContentStyle(GUI.skin.font));
        }
        
        public void SetEndPoints(bool includeURL)
        {
            if (!origin.map)
            {
                origin.map = origin.gameObject.GetUdonSharpComponentInChildren<DataMap>();
            }
            
            if (!origin.StartUrl.EndsWith("/"))
            {
                origin.StartUrl += '/';
            }
            
            var baseURLMap = new List<VRCUrl>();
            
            void SetUrlsbyRange(char a, char b)
            {
                for (var k = a; k <= b; k++)
                {
                    baseURLMap.Add(new VRCUrl(origin.StartUrl + k));
                }
            }

            SetUrlsbyRange('A', 'Z');
            SetUrlsbyRange('a', 'z');
            SetUrlsbyRange('0', '9');
            
            baseURLMap.Add(new VRCUrl(origin.StartUrl + '+'));
            baseURLMap.Add(new VRCUrl(origin.StartUrl + '/'));
            baseURLMap.Add(new VRCUrl(origin.StartUrl + '='));

            BehaviourFieldUtil.SetVariableValue(origin.map, typeof(DataMap).GetField("endPointBaseMap"), baseURLMap.ToArray());


            var endPointCommand = new List<VRCUrl>();
            foreach (var command in origin.endPointCommand)
            {
                endPointCommand.Add(new VRCUrl((includeURL ? origin.StartUrl : "") + command));
            }
            BehaviourFieldUtil.SetVariableValue(origin.map, typeof(DataMap).GetField("endPointCommandMap"), endPointCommand.ToArray());
        }

        public void SetEvents()
        {
            for (var i = 0; i < origin.EventPool.transform.childCount; i++)
            {
                DestroyImmediate(origin.EventPool.transform.GetChild(i).gameObject);
            }
            for (var i = 0; i< origin.events.Length; i++)
            {
                GameObject eo = null;
                if (i >= origin.EventPool.transform.childCount)
                {
                    eo = new GameObject("Event");
                    eo.transform.parent = origin.EventPool.transform;
                }
                else
                {
                    eo = origin.EventPool.transform.GetChild(i).gameObject;
                    var bs = eo.GetComponents<UdonBehaviour>();
                    foreach (var b in bs)
                    {
                        DestroyImmediate(b);
                    }
                }

                eo.name = "Event_" + i;

                var udonEvent = eo.AddComponent<PrismEvent>();
                udonEvent.behaviour = origin.events[i];
                UdonSharpEditorUtility.ConvertToUdonBehaviours(new UdonSharp.UdonSharpBehaviour[] { udonEvent }, false);
            }
        }

        public void ReSize(float size)
        {
            origin.transform.localScale = new Vector3(size, size, size);
            origin.CaptureCam.orthographicSize = origin.OriginSize * size;
            origin.Size = size;
        }
    }
}
