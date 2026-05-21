using System.Collections;

namespace Game.UnitSystem.UnitCommand
{
    public class AttackCommand : UnitCommand
    {
        private Unit _target;
        
        public AttackCommand(Unit excutor, Unit target) : base(excutor)
        {
            _target = target;
        }

        public override IEnumerator ExcuteRoutine()
        {
            yield return _executor.StartCoroutine(_executor.AttackCoroutine(_target));
        }
    }
}