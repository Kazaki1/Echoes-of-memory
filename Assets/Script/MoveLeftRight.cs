using UnityEngine;

public class MoveLeftRight : MonoBehaviour
{
    public float moveDistance = 2f;      // Khoảng cách di chuyển trái/phải
    public float speed = 1f;             // Tốc độ di chuyển
    public float delayTime = 2f;         // Thời gian chờ trước khi bắt đầu di chuyển
    public float pauseTime = 2f;         // Thời gian dừng sau khi di chuyển trái
    public float rightPauseTime = 2f;    // Thời gian dừng sau khi di chuyển phải
    public bool startGoingRight = true;  // Tick = bắt đầu đi phải, không tick = bắt đầu đi trái

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer = 0f;
    private bool isMoving = false;
    private bool isPausing = false;
    private bool isRightPausing = false;
    private float moveProgress = 0f;
    private int direction; // 1: phải, -1: trái
    private bool hasStarted = false;

    void Start()
    {
        startPosition = transform.position;
        direction = startGoingRight ? 1 : -1;
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
                transform.position = targetPosition; // đảm bảo đúng vị trí
                isMoving = false;
                moveProgress = 0f;
                startPosition = targetPosition;

                if (direction == 1)
                {
                    isRightPausing = true;
                    timer = 0f;
                }
                else
                {
                    isPausing = true;
                    timer = 0f;
                }
            }
        }
        else if (isRightPausing)
        {
            timer += Time.deltaTime;
            if (timer >= rightPauseTime)
            {
                isRightPausing = false;
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
        return startPosition + new Vector3(moveDistance * dir, 0, 0);
    }
}