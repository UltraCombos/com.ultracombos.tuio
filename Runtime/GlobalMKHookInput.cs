using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltraCombos
{
    //[RequireComponent(typeof(BaseInputModule))]
    public class GlobalMKHookInput : BaseInput
    {
        public override int touchCount { get { return base.touchCount + TUIOManager.Instance.touches.Count; } }
        public override bool touchSupported { get { return true; } }

        public override Touch GetTouch(int index)
        {
            if (index < base.touchCount)
            {
                return base.GetTouch(index);
            }
            else
            {
                index -= base.touchCount;
                int[] keys = new int[TUIOManager.Instance.touches.Count];
                TUIOManager.Instance.touches.Keys.CopyTo(keys, 0);
                return TUIOManager.Instance.touches[keys[index]];
            }
        }
    }
}


