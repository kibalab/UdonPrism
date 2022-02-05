using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UdonSharpEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Prism.Setup
{
    [CustomEditor(typeof(PrismSetup))]
    public class PrismEditor : Editor
    {
        public PrismSetup origin;
        private ReorderableList BehaviourList, CommandList;

        private void OnEnable()
        {
            {
                var behaviourArray = serializedObject.FindProperty("events");
                BehaviourList = new ReorderableList(serializedObject, behaviourArray, true, true, true, true);

                BehaviourList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Event Invoke Behaviours");

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

                CommandList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Server EndPoint Commands");

                CommandList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var behaviour = commandArray.GetArrayElementAtIndex(index);
                    rect.height -= 4;
                    rect.y += 2;
                    EditorGUI.PropertyField(rect, behaviour, new GUIContent(""));
                };
            }
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            EditorGUILayout.Space(20);

            origin = (PrismSetup)target;

            ReSize(EditorGUILayout.FloatField("Size", origin.Size));

            EditorGUILayout.Space(10);

            origin.InputField.SetActive(EditorGUILayout.Toggle("Use Input Field", origin.InputField.activeSelf));

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginVertical("Box");

            EditorGUI.BeginDisabledGroup(origin.UrlSetted);
            EditorGUILayout.BeginHorizontal();

            origin.StartUrl = EditorGUILayout.TextField("Start Url", origin.StartUrl);

            if (!origin.UrlSetted)
            {
                if (GUILayout.Button("SET"))
                {
                    new AskPopup(() => {
                        origin.UrlSetted = true;


                        if (!origin.StartUrl.EndsWith("/"))
                        {
                            origin.StartUrl += '/';
                        }

                        var baseURLMap = new VRCUrl[65];
                        var i = 0;

                        for (var k = 'A'; k <= 'Z'; k++)
                        {
                            baseURLMap[i] = new VRCUrl(origin.StartUrl + k);
                            i++;
                        }
                        for (var k = 'a'; k <= 'z'; k++)
                        {
                            baseURLMap[i] = new VRCUrl(origin.StartUrl + k);
                            i++;
                        }
                        for (var k = '0'; k <= '9'; k++)
                        {
                            baseURLMap[i] = new VRCUrl(origin.StartUrl + k);
                            i++;
                        }
                        baseURLMap[62] = new VRCUrl(origin.StartUrl + '+');
                        baseURLMap[63] = new VRCUrl(origin.StartUrl + '/');
                        baseURLMap[64] = new VRCUrl(origin.StartUrl + '=');

                        origin.map.endPointBaseMap = baseURLMap;

                        SetCommand(true);

                        UdonSharpEditorUtility.ConvertToUdonBehaviours(new UdonSharp.UdonSharpBehaviour[] { origin.map, origin.map.GetComponentInChildren<PrismEncoder>() }, true);
                    });
                }
            }
            EditorGUILayout.EndHorizontal();


            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            CommandList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                SetCommand(false);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();

            origin.c_InputField.SetUrl(new VRCUrl(origin.StartUrl));

            EditorGUILayout.Space(20);


            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            BehaviourList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                SetEvents();
            }
        }

        public void SetCommand(bool includeURL)
        {
            var i = 0;
            origin.map.endPointCommandMap = new VRCUrl[origin.endPointCommand.Length + 2];
            foreach (var command in origin.endPointCommand)
            {
                origin.map.endPointCommandMap[i] = new VRCUrl((includeURL ? origin.StartUrl : "") + command);
                i++;
            }
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

    public delegate void Func();

    public class AskPopup : EditorWindow
    {
        public Func func;
        public AskPopup(Func func)
        {
            this.func = func;
            var window = (AskPopup)GetWindow(typeof(AskPopup), utility: true, title: "ASK", focus: true);
            window.Show();
            window.minSize = new Vector2(209, 100);
            window.maxSize = window.minSize;

            window.position = new Rect(Screen.currentResolution.width / 2 - window.minSize.x / 2, Screen.currentResolution.height / 2 - window.minSize.y / 2, 0, 0);
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("After that, you can't edit it.\nWould you like to go on ? ", GUILayout.Height(50));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel", GUILayout.Width(100), GUILayout.Height(50)))
            {
                this.Close();
            }
            if (GUILayout.Button("OK", GUILayout.Width(100), GUILayout.Height(50)))
            {
                func.Invoke();
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
