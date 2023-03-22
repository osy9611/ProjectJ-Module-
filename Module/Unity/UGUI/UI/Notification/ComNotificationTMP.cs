namespace Module.Unity.UGUI.Notification
{
    using Module.Core.Systems.Events;
    using TMPro;
    using UnityEngine;

    public class ComNotificationTMP : ComNotification
    {
        [SerializeField] TextMeshProUGUI text;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void OnSetData(IEventArgs args)
        {
            EventArgs<string>? val = args as EventArgs<string>?;

            if (text == null||!val.HasValue)
                return;

            text.text = val.Value.Arg1;

            Start();
        }

    }

}
