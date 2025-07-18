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
    public string action;
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public string background;
    public string character;
    public List<Choice> choices;
    public string action;
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

            if (!string.IsNullOrEmpty(pendingAction))
            {
                PerformAction(pendingAction);
                pendingAction = null;
                return;
            }

            index++;
            if (index < lines.Count)
            {
                ShowLine();
            }
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


        if (!string.IsNullOrEmpty(line.background))
        {
            Sprite bgSprite = Resources.Load<Sprite>("Backgrounds/" + line.background);
            if (bgSprite != null)
                background.sprite = bgSprite;
        }

        if (!string.IsNullOrEmpty(line.character))
        {
            Sprite charSprite = Resources.Load<Sprite>("Characters/" + line.character);
            if (charSprite != null)
            {
                character.sprite = charSprite;
                character.SetNativeSize();

                float maxSize = 200f;
                float width = character.rectTransform.sizeDelta.x;
                float height = character.rectTransform.sizeDelta.y;
                float scaleFactor = 1f;

                if (width > height && width > maxSize)
                    scaleFactor = maxSize / width;
                else if (height > width && height > maxSize)
                    scaleFactor = maxSize / height;
                else if (height == width && height > maxSize)
                    scaleFactor = maxSize / height;

                character.rectTransform.sizeDelta = new Vector2(width * scaleFactor, height * scaleFactor);
                SetCharacterAlpha(1f);
            }
            else
            {
                character.sprite = null;
                SetCharacterAlpha(0f);
            }
        }
        else
        {
            character.sprite = null;
            SetCharacterAlpha(0f);
        }


        pendingAction = string.IsNullOrEmpty(line.action) ? null : line.action;


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
        Debug.Log("Thực hiện hành động: " + action);

        switch (action)
        {
            case "Stay":
                SceneManager.LoadScene("Menu");
                break;
            case "Go":
                SceneManager.LoadScene("Lv1");
                break;
            case "Lv2":
                SceneManager.LoadScene("Lv2");
                break;
            case "Lv3":
                SceneManager.LoadScene("Lv3");
                break;
            case "Lv4":
                SceneManager.LoadScene("Lv4");
                break;
            case "Lv5":
                SceneManager.LoadScene("Lv5");
                break;
            case "Alice":
                SceneManager.LoadScene("Boss5");
                break;
            case "LoadEnding1":
                SceneManager.LoadScene("Ending 1");
                break;
            case "LoadEnding2":
                SceneManager.LoadScene("Ending 2");
                break;
            case "LoadEnding3":
                SceneManager.LoadScene("Ending 3");
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
