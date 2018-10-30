using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltraCombos
{
    [RequireComponent(typeof(GlobalMKHookInput))]
    public class GlobalMKHookInputModule : StandaloneInputModule
    {
        public bool forceProcessMouseEvent = true;

        protected override void Awake()
        {
            base.Awake();
            m_InputOverride = GetComponent<GlobalMKHookInput>();
        }

        public override bool IsModuleSupported()
        {
            return TUIOManager.Instance.IsReady();
        }

        public override void Process()
        {
            base.Process();
            if (forceProcessMouseEvent && input.touchCount > 0 == true && input.mousePresent)
                base.ProcessMouseEvent();
        }
    }
}

