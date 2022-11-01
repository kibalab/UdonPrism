using System;
using System.Collections.Generic;

namespace K13A.BehaviourEditor
{
    public static class TitleOptionsBuilder
    {
        public static List<TitleOption> Build(TitleOption[] options)
        {
            var actionList = new List<TitleOption>();
            foreach (var option in options)
            {
                actionList.Add(option);
            }

            return actionList;
        }
    }
}