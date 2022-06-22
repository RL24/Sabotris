using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace Sabotris.Util.Input
{
    public static class GamepadUtil
    {
        private static readonly Dictionary<string, GamepadButton> ButtonMap = new Dictionary<string, GamepadButton>()
        {
            {"up", GamepadButton.DpadUp},
            {"down", GamepadButton.DpadDown},
            {"left", GamepadButton.DpadLeft},
            {"right", GamepadButton.DpadRight},
            {"buttonNorth", GamepadButton.North},
            {"buttonEast", GamepadButton.East},
            {"buttonSouth", GamepadButton.South},
            {"buttonWest", GamepadButton.West},
            {"leftStickPress", GamepadButton.LeftStick},
            {"rightStickPress", GamepadButton.RightStick},
            {"leftShoulder", GamepadButton.LeftShoulder},
            {"rightShoulder", GamepadButton.RightShoulder},
            {"start", GamepadButton.Start},
            {"select", GamepadButton.Select},
            {"leftTrigger", GamepadButton.LeftTrigger},
            {"rightTrigger", GamepadButton.RightTrigger},
        };

        public static GamepadButton? GetGamepadButton(ButtonControl control)
        {
            if (!ButtonMap.TryGetValue(control.name, out var button))
                return null;
            return button;
        }
    }
}