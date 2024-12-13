using UnityEngine.Events;

namespace DiceRoll.Battles
{
    public class StepProcessor
    {
        private int _step = 0;
        
        private UnityAction<int> _onStepComplete = _ => {};
        
        public void AddStepListener(UnityAction<int> onStepComplete)
        {
            _onStepComplete += onStepComplete;
        }
        
        public void RemoveStepListener(UnityAction<int> onStepComplete)
        {
            _onStepComplete -= onStepComplete;
        }
        
        public void ProcessStep()
        {
            _step++;
            _onStepComplete.Invoke(_step);
        }
    }
}