using UnityEngine;

public class AutoMoveUpDown : MonoBehaviour
{
    public float moveDistance = 3f;   // Khoảng cách lên/xuống
    public float speed = 10f;         // Tốc độ di chuyển
    public float delayTime = 0.2f;   // Thời gian chờ trước khi di chuyển
    public float pauseTime = 0.2f;   // Dừng sau khi đi xuống
    public float upPauseTime = 0.2f;  // Dừng sau khi đi lên
    public bool startGoingUp = true; // Tick = lên, Bỏ tick = xuống

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer = 0f;
    private bool isMoving = false;
    private bool isPausing = false;
    private bool isUpPausing = false;
    private float moveProgress = 0f;
    private int direction = 1; // 1: lên, -1: xuống
    private bool hasStarted = false;

    void Start()
    {
        startPosition = transform.position;
        direction = startGoingUp ? 1 : -1;
        targetPosition = GetTargetPosition(direction);
    }

    void Update()
    {
        if (!hasStarted)
        {
            timer += Time.deltaTime;
            if (timer >= delayTime)
            {
                isMoving = true;
                hasStarted = true;
                timer = 0f;
                moveProgress = 0f;
            }
            return;
        }

        if (isMoving)
        {
            moveProgress += Time.deltaTime * speed / moveDistance;
            transform.position = Vector3.Lerp(startPosition, targetPosition, moveProgress);

            if (moveProgress >= 1f)
            {
                transform.position = targetPosition; // Đảm bảo vị trí chính xác
                isMoving = false;
                moveProgress = 0f;
                startPosition = targetPosition;

                if (direction == 1)
                {
                    isUpPausing = true;
                    timer = 0f;
                }
                else
                {
                    isPausing = true;
                    timer = 0f;
                }
            }
        }
        else if (isUpPausing)
        {
            timer += Time.deltaTime;
            if (timer >= upPauseTime)
            {
                isUpPausing = false;
                direction = -1;
                targetPosition = GetTargetPosition(direction);
                isMoving = true;
            }
        }
        else if (isPausing)
        {
            timer += Time.deltaTime;
            if (timer >= pauseTime)
            {
                isPausing = false;
                direction = 1;
                targetPosition = GetTargetPosition(direction);
                isMoving = true;
            }
        }
    }

    Vector3 GetTargetPosition(int dir)
    {
        return startPosition + new Vector3(0, moveDistance * dir, 0);
    }
}