using UnityEngine;
using UnityEngine.UI;

public class FightbarController : MonoBehaviour
{
    public Slider fightSlider;
    public float fillSpeed = 80f;

    private bool isFilling = false;
    private bool isForward = true;
    private float currentValue = 0f;

    // Event để gửi damage multiplier về BattleManager
    public System.Action<float> OnPlayerStopFilling;

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

                // Gửi damage multiplier = 0% về BattleManager
                float damageMultiplier = 0f;
                OnPlayerStopFilling?.Invoke(damageMultiplier);
                return;
            }
        }

        fightSlider.value = currentValue;

        // Người chơi dừng thủ công
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFilling = false;

            // Tính damage multiplier dựa trên vị trí dừng
            float damageMultiplier = fightSlider.value / fightSlider.maxValue;

            Debug.Log($"Người chơi dừng tại: {fightSlider.value}/{fightSlider.maxValue} - Damage Multiplier: {damageMultiplier:P}");

            // Gửi damage multiplier về BattleManager
            OnPlayerStopFilling?.Invoke(damageMultiplier);
        }
    }
}