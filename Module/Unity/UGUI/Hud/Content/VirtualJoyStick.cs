namespace Module.Unity.UGUI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    public class VirtualJoyStick : UI_Base
    {
        [SerializeField] private RectTransform lever;
        [SerializeField] private RectTransform leverBack;
        [SerializeField] private RectTransform leverArea;

        [SerializeField, Range(5f, 50f)]
        private float leverRange;

        private Vector2 pointerDownPos;
        [SerializeField] private Vector2 dir;

        public override void Init()
        {
            BindEvent(this.gameObject, OnDrag, Define.UIEvent.Drag);
            BindEvent(this.gameObject, OnPointerUp, Define.UIEvent.Up);
        }

        public void OnDrag(PointerEventData eventData,System.Action<Vector2> callback)
        {
            if (eventData == null)
                throw new System.ArgumentException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), eventData.position, 
                eventData.pressEventCamera, out Vector2 position);
            Vector2 delta = position - pointerDownPos;
            delta = Vector2.ClampMagnitude(delta, leverRange);
            lever.anchoredPosition = pointerDownPos + delta;

            Vector2 newPos = new Vector2(delta.x / leverRange, delta.y / leverRange);
            dir = newPos;
            callback?.Invoke(newPos);
        }

        public void OnPointerUp(PointerEventData eventData, System.Action<Vector2> callback)
        {
            if (eventData == null)
                throw new System.ArgumentException(nameof(eventData));

            dir = Vector2.zero;
            lever.anchoredPosition = Vector2.zero;
            leverBack.anchoredPosition = Vector2.zero;
            callback?.Invoke(Vector2.zero);
        }
    }

}

