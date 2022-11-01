
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Prism
{
    public class EventManager : UdonSharpBehaviour
    {
        [SerializeField] public PrismEvent[] prismEvents;

        private void Start()
        {
            prismEvents = GetComponentsInChildren<PrismEvent>();
        }

        public void Invoke(string eventName, string data)
        {
            foreach (var prismEvent in prismEvents)
            {
                if (prismEvent != null)
                {
                    prismEvent.behaviour.SendCustomEvent(eventName);
                    prismEvent.behaviour.SetProgramVariable("Prism_Data", data);
                }
            }
        }
    }
}