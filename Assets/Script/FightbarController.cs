using UnityEngine;
using UnityEngine.UI;

public class FightbarController : MonoBehaviour
{
    public Slider fightSlider;
    public float fillSpeed = 80f;

    private bool isFilling = false;
    private bool isForward = true;
    private float currentValue = 0f;

    void OnEnable()
    {
        currentValue = 0f;
        fightSlider.value = 0f;
        isFilling = true;
        isForward = true;
    }

    void Update()
    {
        if (!isFilling) return;

        float delta = fillSpeed * Time.deltaTime;

        if (isForward)
        {
            currentValue += delta;

            if (currentValue >= fightSlider.maxValue)
            {
                currentValue = fightSlider.maxValue;
                isForward = false; 
            }
        }
        else
        {
            currentValue -= delta;

            if (currentValue <= fightSlider.minValue)
            {
                currentValue = fightSlider.minValue;
                isFilling = false; 
                Debug.Log("Tự động dừng tại giá trị thấp nhất (0)");
                return;
            }
        }

        fightSlider.value = currentValue;

        // Người chơi dừng thủ công
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            isFilling = false;
            Debug.Log("Người chơi dừng tại: " + fightSlider.value);
            // TODO: Gọi xử lý chém/damage tại đây
        }
    }
}
