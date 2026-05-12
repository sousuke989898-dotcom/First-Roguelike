using Game.Effect;
using UnityEngine;

namespace Game.Manager //順次Managerクラスをここに移動していく
{
    public class DatabaseManager : MonoBehaviour
    {
        [SerializeField] private EffectDatabase _effectDatabase;

        public static EffectDatabase Effects => Instance._effectDatabase;

        public static DatabaseManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                enabled = false;
                Debug.LogError($"{this}が複数存在しています。");
            }
        }
    }

    
}