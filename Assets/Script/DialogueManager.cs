using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class Choice
{
    public string text;
    public int nextIndex;
    public string action; // Action khi nhấn lựa chọn
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public string background;
    public string character;
    public List<Choice> choices;
    public string action; // Action riêng của câu thoại
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public Text speakerText;
    public Text dialogueText;
    public Image background;
    public Image character;

    [Header("Choice UI")]
    public GameObject choicePanel;
    public Button choiceButtonPrefab;

    [Header("Data")]
    public string jsonFileName = "dialogue";

    private List<DialogueLine> lines;
    private int index = 0;
    private string pendingAction = null;

    private void Start()
    {
        LoadDialogue();
        ShowLine();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (choicePanel.activeSelf) return;

            // Nếu đang chờ thực thi action sau đoạn thoại
            if (!string.IsNullOrEmpty(pendingAction))
            {
                PerformAction(pendingAction);
                pendingAction = null;
                return;
            }

            index++;
            if (index < lines.Count)
                ShowLine();
        }
    }

    void LoadDialogue()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName + ".json");

        if (!File.Exists(path))
        {
            Debug.LogError("Không tìm thấy file JSON tại: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        lines = JsonUtility.FromJson<DialogueWrapper>("{\"lines\":" + json + "}").lines;

        if (lines == null || lines.Count == 0)
        {
            Debug.LogError("File JSON không hợp lệ hoặc rỗng.");
        }
    }

    void ShowLine()
    {
        if (index >= lines.Count)
            return;

        var line = lines[index];
        speakerText.text = line.speaker;
        dialogueText.text = line.text;

        background.sprite = Resources.Load<Sprite>("Backgrounds/" + line.background);

        if (!string.IsNullOrEmpty(line.character))
        {
            character.sprite = Resources.Load<Sprite>("Characters/" + line.character);
            SetCharacterAlpha(1f);
        }
        else
        {
            character.sprite = null;
            SetCharacterAlpha(0f);
        }

        // Nếu có action, lưu để thực hiện sau khi click chuột
        pendingAction = string.IsNullOrEmpty(line.action) ? null : line.action;

        // Hiển thị lựa chọn nếu có
        if (line.choices != null && line.choices.Count > 0)
        {
            ShowChoices(line.choices);
        }
        else
        {
            choicePanel.SetActive(false);
        }
    }

    void ShowChoices(List<Choice> choices)
    {
        if (choicePanel == null || choiceButtonPrefab == null)
        {
            Debug.LogError("Thiếu gán UI ChoicePanel hoặc ButtonPrefab!");
            return;
        }

        choicePanel.SetActive(true);

        foreach (Transform child in choicePanel.transform)
            Destroy(child.gameObject);

        foreach (var choice in choices)
        {
            Choice capturedChoice = choice;
            Button btn = Instantiate(choiceButtonPrefab, choicePanel.transform);

            TextMeshProUGUI tmpText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
                tmpText.text = capturedChoice.text;

            btn.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(capturedChoice.action))
                    PerformAction(capturedChoice.action);

                index = capturedChoice.nextIndex;
                choicePanel.SetActive(false);
                ShowLine();
            });
        }
    }

    void SetCharacterAlpha(float alpha)
    {
        var color = character.color;
        color.a = alpha;
        character.color = color;
    }

    void PerformAction(string action)
    {
        switch (action)
        {
            case "Go":
                Debug.Log("Chuyển cảnh đến Lv1");
                SceneManager.LoadScene("Lv1");
                break;

            default:
                Debug.LogWarning("Không rõ hành động: " + action);
                break;
        }
    }

    [System.Serializable]
    private class DialogueWrapper
    {
        public List<DialogueLine> lines;
    }
}
