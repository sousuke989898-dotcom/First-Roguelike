using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using System.Diagnostics;

public class CameraAndInputController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Tilemap walkStyleTilemap;  

    [SerializeField] private float clickThresholdDistance = 10f; // このピクセル分マウスが動いたらドラッグとみなす

    private Vector3 _touchStartMousePos;
    private Vector3 _lastMousePos;
    private bool _isDraggingMode = false;
    private bool _isFollowingPlayer = true; // プレイヤーを追従中かどうか

    void Start()
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = transform;
            //virtualCamera.LookAt = transform;
        }
    }

    void LateUpdate()
    {
        if (GameManager.Player == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            _touchStartMousePos = Input.mousePosition;
            _lastMousePos = Input.mousePosition;
            _isDraggingMode = false;
        }

        if (Input.GetMouseButton(0))
        {
            float travelDistance = Vector3.Distance(_touchStartMousePos, Input.mousePosition);
            
            if (!_isDraggingMode && travelDistance > clickThresholdDistance)
            {
                _isDraggingMode = true;
                _isFollowingPlayer = false;
            }

            if (_isDraggingMode)
            {
                DraggingCamera();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!_isDraggingMode )
            {
                TriggerAutoMove(_touchStartMousePos);
            }
        }

        if (_isFollowingPlayer)
        {
            transform.position = GameManager.Player.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCameraToPlayer();
        }
    }

    private void TriggerAutoMove(Vector3 screenPosition)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start(); // 計測開始


        if (walkStyleTilemap == null || GameManager.Player == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector3Int cellPos = walkStyleTilemap.WorldToCell(worldPos);
        Vector2Int targetGridPos = new(cellPos.x, cellPos.y);

        _isFollowingPlayer = true;

        GameManager.Player.UnitMovement.SetPath(targetGridPos);


        sw.Stop(); // 計測終了

        UnityEngine.Debug.Log($"処理時間: {sw.ElapsedMilliseconds} ms");
        UnityEngine.Debug.Log($"処理時間(詳細): {sw.ElapsedTicks} ticks");
    }

    /// <summary>
    /// カーソルの移動量に合わせてカメラを移動させる
    /// </summary>
    private void DraggingCamera()
    {
        float worldSizePerPixel = Camera.main.orthographicSize * 2f / Screen.height;
        Vector3 difference = _lastMousePos - Input.mousePosition;
        Vector3 move = new Vector3(difference.x, difference.y, 0) * worldSizePerPixel;

        transform.position += move;
        _lastMousePos = Input.mousePosition;

        // Vector3 difference = _lastMousePos - Input.mousePosition;
        // Vector3 move = dragSpeed * Time.deltaTime * new Vector3(difference.x, difference.y, 0);
        // transform.position += move;
        // _lastMousePos = Input.mousePosition;
    }

    public void ResetCameraToPlayer()
    {
        _isFollowingPlayer = true;
        UnityEngine.Debug.Log("カメラをプレイヤーに戻しました。");
    }
}