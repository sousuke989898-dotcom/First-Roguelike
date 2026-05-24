using UnityEngine;
using Cinemachine; // ★これを忘れずにインポート！

public class CameraZoomController : MonoBehaviour
{
    // インスペクターからCinemachineのVirtual Cameraをセットする
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private float minSize = 3f;  // 最大ズームイン時のサイズ
    [SerializeField] private float maxSize = 10f; // 最大ズームアウト時のサイズ
    [SerializeField] private float zoomSpeed = 5f; // ズームの速度

    private float _targetSize;

    void Start()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
        
        // 初期状態のサイズを現在のターゲットに設定
        _targetSize = virtualCamera.m_Lens.OrthographicSize;
    }

    void Update()
    {
        // 例：マウスのスクロールホイールの入力を取得
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0)
        {
            _targetSize -= scroll * zoomSpeed;
            
            _targetSize = Mathf.Clamp(_targetSize, minSize, maxSize);
        }

        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
            virtualCamera.m_Lens.OrthographicSize, 
            _targetSize, 
            Time.deltaTime * zoomSpeed
        );
    }
}