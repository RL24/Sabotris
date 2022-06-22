using System;
using UnityEngine.InputSystem;

namespace Sabotris.Util.Input
{
    public class RebindHelper
    {
        public static bool Rebinding { get; set; }
        public static event EventHandler OnRebindStart;
        public static event EventHandler OnRebindStop;

        public static void Rebind(InputActionReference action)
        {
            if (Rebinding)
                return;
            
            Rebinding = true;
            OnRebindStart?.Invoke(null, null);
            
            action.action.PerformInteractiveRebinding()
                .OnComplete(OnRebindComplete)
                .OnCancel(OnRebindComplete)
                .Start();
        }

        private static void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation rebindingOperation)
        {
            rebindingOperation.Dispose();
            Rebinding = false;
            OnRebindStop?.Invoke(null, null);
        }

        public static string BindToString(InputActionReference reference)
        {
            return InputControlPath.ToHumanReadableString(reference.action.bindings[reference.action.GetBindingIndexForControl(reference.action.controls[0])].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }
}