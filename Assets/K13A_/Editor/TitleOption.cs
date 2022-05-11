using System;
using UnityEditor;
using UnityEngine;

namespace K13A.BehaviourEditor
{
    
    public class TitleOption
    {
        #region Fields

        public GUIContent Content;
        public Action action;
        public GUIStyle Style;

        #endregion

        #region Properties

        public TitleOption(GUIContent Content, Action action)
        {
            this.Content = Content;
            this.action = action;
            this.Style = new GUIStyle(GUI.skin.window);
            //this.Style.normal.background * 10;
        }
        
        #endregion

        #region Functions

        public void OnGUI(int ID)
        {   
            var b = GUI.Button(new Rect(
                GUILayoutUtility.GetLastRect().x + GUILayoutUtility.GetLastRect().width - (ID+1) * 25, 
                GUILayoutUtility.GetLastRect().y + 3, 
                20, 
                18), "", Style);

            if (b)
            {
                action.Invoke();;
            }
        }

        #endregion
    }
}