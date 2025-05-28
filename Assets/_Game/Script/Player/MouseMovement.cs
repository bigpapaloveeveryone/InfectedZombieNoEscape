using UnityEngine;
using UnityEngine.EventSystems;

public class MouseMovement : MonoBehaviour
{
    public float swipeSensitivity = 0.2f;

    private float xRot;
    private float yRot;

    [SerializeField] private float topClamp = -90f;
    [SerializeField] private float botClamp = 90f;

    private Vector2 lastMousePos;
    private bool isDragging = false;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    private bool IsRightSideOfScreen(Vector2 touchPos)
    {
        return touchPos.x > Screen.width / 2;
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;

            // Nếu chạm vào nửa trái màn hình thì không xoay
            if (!IsRightSideOfScreen(lastMousePos)) return;

            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            float mouseX = delta.x * swipeSensitivity;
            float mouseY = delta.y * swipeSensitivity;

            xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, topClamp, botClamp);
            yRot += mouseX;

            transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void HandleTouchInput()
    {
        Touch? cameraTouch = null;

        // Duyệt qua tất cả ngón tay trên màn hình
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Nếu chạm vào nửa trái màn hình, bỏ qua
            if (!IsRightSideOfScreen(touch.position)) continue;

            cameraTouch = touch;
            break;
        }

        if (cameraTouch == null) return;

        Touch activeTouch = cameraTouch.Value;

        if (activeTouch.phase == TouchPhase.Began)
        {
            lastMousePos = activeTouch.position;
            isDragging = true;
        }
        else if (activeTouch.phase == TouchPhase.Moved && isDragging)
        {
            Vector2 delta = activeTouch.position - lastMousePos;
            lastMousePos = activeTouch.position;

            float mouseX = delta.x * swipeSensitivity;
            float mouseY = delta.y * swipeSensitivity;

            xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, topClamp, botClamp);
            yRot += mouseX;

            transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
        }
        else if (activeTouch.phase == TouchPhase.Ended)
        {
            isDragging = false;
        }
    }
}
