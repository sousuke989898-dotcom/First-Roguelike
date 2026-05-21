using System.Collections;
using UnityEngine;

namespace Game.UnitSystem.UnitCommand
{
    public class MoveCommand : UnitCommand
    {
        private Vector2Int _targetPos;

        public MoveCommand(Unit excutor, Vector2Int targetPos) : base(excutor)
        {
            _targetPos = targetPos;
        }

        public override IEnumerator ExcuteRoutine()
        {
            yield return _executor.StartCoroutine(_executor.MoveCoroutine(_targetPos));
        }
    }
}