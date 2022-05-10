using JetBrains.Annotations;
using UnityEngine;

namespace K13A.BehaviourEditor
{
    public class ContentStyle
    {
        public ContentStyle(Font font, bool UpperBorder)
        {
            this.font = font;
            this.UpperBorder = UpperBorder;
        }
        
        public ContentStyle(Font font) => this.font = font;
        
        public ContentStyle(bool UpperBorder) => this.UpperBorder = UpperBorder;
        
        public ContentStyle() {}
        
        [CanBeNull] public Font font;
        public bool UpperBorder = true;
    }
}