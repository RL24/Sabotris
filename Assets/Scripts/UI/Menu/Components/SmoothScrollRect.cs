using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sabotris.UI.Menu
{
    public class SmoothScrollRect : ScrollRect
    {
        public override void OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;

            velocity = -data.scrollDelta * scrollSensitivity;
        }
    }
}