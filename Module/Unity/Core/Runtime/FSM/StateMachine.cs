namespace Module.Unity.Core
{
    public class StateMachine<T> where T : class
    {
        private T owner;
        private State<T> currentState;
        private State<T> previousState;

        public void Init(T owner, State<T> state)
        {
            this.owner = owner;
            currentState = null;
            previousState = null;

            ChangeState(state);
        }

        public void ChangeState(State<T> state)
        {
            if (state == null)
                return;

            if(currentState != null)
            {
                previousState= currentState;
                previousState.Exit(owner);
            }

            currentState = state;
            currentState.Enter(owner);
        }

        public void Execute()
        {
            if (currentState != null)
                currentState.Execute(owner);
        }

        public State<T> GetCurrentState()
        {
            return currentState;
        }

        public State<T> GetPrevState()
        {
            return previousState;
        }


        public void RevertToPreviousState()
        {
            ChangeState(previousState);
        }
    }

}
