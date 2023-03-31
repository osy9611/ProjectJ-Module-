namespace Module.Unity.Input
{
    using System;
    using UnityEngine.InputSystem;

    [Flags]
    public enum InputEvnetType
    {
        None = 0,
        Start = 1 << 0,
        Performed = 1 << 1,
        Cancel = 1 << 2
    }

    public class InputManager
    {
        private PlayerInput playerInput;

        public void Init()
        {
        }

        public void RegisterInput<T>(T comPlayerActor, string schemeType) where T : UnityEngine.Component
        {
            this.playerInput = comPlayerActor.GetComponent<PlayerInput>();

            playerInput.defaultControlScheme = schemeType;
        }

        public string GetNowContorolScheme()
        {
            return playerInput.defaultControlScheme;
        }

        public void AddEvent(string eventName, System.Action<InputAction.CallbackContext> callback, InputEvnetType eventType)
        {
            if (callback == null)
                return;

            if (eventType.HasFlag(InputEvnetType.Start))
                playerInput.actions[eventName].started += callback;
            if (eventType.HasFlag(InputEvnetType.Performed))
                playerInput.actions[eventName].performed += callback;
            if (eventType.HasFlag(InputEvnetType.Cancel))
                playerInput.actions[eventName].canceled += callback;
        }

        public void RemoveEvent(string eventName, System.Action<InputAction.CallbackContext> callback)
        {
            if (callback == null)
                return;
            playerInput.actions[eventName].performed -= callback;
        }


    }
}



