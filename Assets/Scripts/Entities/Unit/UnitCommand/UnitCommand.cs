using System.Collections;

namespace Game.UnitSystem.UnitCommand //本当はGame.Unit.UnitCommandにしてUnitたちもGame.Unitにしようかと思ったがクラス名とネームスペースが重複できないようで頓挫した
{

    public abstract class UnitCommand
    {
        protected Unit _executor;

        public UnitCommand(Unit executor)
        {
            _executor = executor;
        }

        public abstract IEnumerator ExcuteRoutine();
    }

    
}