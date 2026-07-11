using UnityEngine;

namespace Game.GridMap
{
    public enum DoorState { Open, Closed, Locked};

    [RequireComponent(typeof(SpriteRenderer))]
    public class DoorTile : Entity
    {
        [Header("設定")]
        [SerializeField] private DoorState initialState = DoorState.Closed;

        [Header("グラフィック（Spriteを指定）")]
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private Sprite lockedSprite;

        public DoorState currentState;
        private SpriteRenderer _spriteRenderer;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            // 初期状態をセットし、見た目を反映する
            currentState = initialState;
            UpdateGraphics();
        }

        /// <summary>
        /// ドアの状態を変更し、見た目も自動で更新するメソッド
        /// </summary>
        public void SetState(DoorState newState)
        {
            currentState = newState;
            UpdateGraphics();
        }

        private void UpdateGraphics()
        {
            if (_spriteRenderer == null) return;

            switch (currentState)
            {
                case DoorState.Open:
                    _spriteRenderer.sprite = openSprite;
                    break;
                case DoorState.Closed:
                    _spriteRenderer.sprite = closedSprite;
                    break;
                case DoorState.Locked:
                    _spriteRenderer.sprite = lockedSprite;
                    break;
            }
        }

    }

}