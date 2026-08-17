using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 2Dゲームのカメラ移動・ズームを管理する。
/// マウスホイールでズーム。
/// 左クリックドラッグでカメラを移動。
/// マップ外は映さない。
/// </summary>
public class CameraController : MonoBehaviour
{
    // =====================================================
    // ズーム
    // =====================================================

    [Header("ズーム設定")]

    [SerializeField]
    private float zoomSpeed = 2f;

    [SerializeField]
    private float minZoom = 3f;

    [SerializeField]
    private float maxZoom = 8f;


    // =====================================================
    // カメラ移動
    // =====================================================

    [Header("カメラ移動")]

    [SerializeField]
    private float dragSpeed = 1f;


    // =====================================================
    // マップ範囲
    // =====================================================

    [Header("マップ範囲")]

    [Tooltip("マップの左端")]
    [SerializeField]
    private float mapMinX = -20f;

    [Tooltip("マップの右端")]
    [SerializeField]
    private float mapMaxX = 20f;

    [Tooltip("マップの下端")]
    [SerializeField]
    private float mapMinY = -15f;

    [Tooltip("マップの上端")]
    [SerializeField]
    private float mapMaxY = 15f;


    // =====================================================
    // 内部変数
    // =====================================================

    private Camera cam;

    private Vector3 dragStartWorldPosition;

    private bool isDragging;


    // =====================================================
    // 初期化
    // =====================================================

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError(
                "CameraController: Cameraコンポーネントがありません。",
                this
            );
        }
    }


    // =====================================================
    // 更新
    // =====================================================

    private void Update()
    {
        if (cam == null)
            return;


        // -------------------------------------------------
        // UI操作中
        // -------------------------------------------------

        if (TowerBuildUI.IsUIOpen)
        {
            isDragging = false;
            return;
        }


        // -------------------------------------------------
        // UIの上にマウスがある場合
        // -------------------------------------------------

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            isDragging = false;

            HandleZoom();

            return;
        }


        // -------------------------------------------------
        // カメラ操作
        // -------------------------------------------------

        HandleZoom();

        HandleDrag();

        ClampCameraPosition();
    }


    // =====================================================
    // ズーム
    // =====================================================

    /// <summary>
    /// マウスホイールによるズーム。
    /// </summary>
    private void HandleZoom()
    {
        float scroll =
            Input.mouseScrollDelta.y;


        if (Mathf.Abs(scroll) < 0.01f)
            return;


        float newZoom =
            cam.orthographicSize
            - scroll * zoomSpeed;


        newZoom =
            Mathf.Clamp(
                newZoom,
                minZoom,
                maxZoom
            );


        cam.orthographicSize =
            newZoom;


        ClampCameraPosition();
    }


    // =====================================================
    // ドラッグ
    // =====================================================

    /// <summary>
    /// 左クリックによるカメラ移動。
    /// </summary>
    private void HandleDrag()
    {
        // -------------------------------------------------
        // 左クリックを押した瞬間
        // -------------------------------------------------

        if (Input.GetMouseButtonDown(0))
        {
            dragStartWorldPosition =
                GetMouseWorldPosition();

            isDragging = true;
        }


        // -------------------------------------------------
        // 左クリックを押している間
        // -------------------------------------------------

        if (Input.GetMouseButton(0) &&
            isDragging)
        {
            Vector3 currentMouseWorldPosition =
                GetMouseWorldPosition();


            Vector3 difference =
                dragStartWorldPosition
                - currentMouseWorldPosition;


            transform.position +=
                difference * dragSpeed;
        }


        // -------------------------------------------------
        // 左クリックを離した
        // -------------------------------------------------

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }


    // =====================================================
    // マウス座標
    // =====================================================

    /// <summary>
    /// マウス位置をワールド座標に変換する。
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition =
            Input.mousePosition;


        mousePosition.z =
            Mathf.Abs(
                transform.position.z
            );


        return cam.ScreenToWorldPoint(
            mousePosition
        );
    }


    // =====================================================
    // カメラ範囲制限
    // =====================================================

    /// <summary>
    /// カメラがマップ外を映さないようにする。
    /// </summary>
    private void ClampCameraPosition()
    {
        // -------------------------------------------------
        // カメラが現在映している範囲
        // -------------------------------------------------

        float cameraHeight =
            cam.orthographicSize;


        float cameraWidth =
            cameraHeight * cam.aspect;


        // -------------------------------------------------
        // X方向
        // -------------------------------------------------

        float minX;
        float maxX;


        float mapWidth =
            mapMaxX - mapMinX;


        if (mapWidth <= cameraWidth * 2f)
        {
            // マップがカメラより小さい場合
            float centerX =
                (mapMinX + mapMaxX) / 2f;

            minX = centerX;
            maxX = centerX;
        }
        else
        {
            minX =
                mapMinX + cameraWidth;

            maxX =
                mapMaxX - cameraWidth;
        }


        // -------------------------------------------------
        // Y方向
        // -------------------------------------------------

        float minY;
        float maxY;


        float mapHeight =
            mapMaxY - mapMinY;


        if (mapHeight <= cameraHeight * 2f)
        {
            // マップがカメラより小さい場合
            float centerY =
                (mapMinY + mapMaxY) / 2f;

            minY = centerY;
            maxY = centerY;
        }
        else
        {
            minY =
                mapMinY + cameraHeight;

            maxY =
                mapMaxY - cameraHeight;
        }


        // -------------------------------------------------
        // カメラ位置を制限
        // -------------------------------------------------

        Vector3 position =
            transform.position;


        position.x =
            Mathf.Clamp(
                position.x,
                minX,
                maxX
            );


        position.y =
            Mathf.Clamp(
                position.y,
                minY,
                maxY
            );


        transform.position =
            position;
    }
}