using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public string background;
    public string character;
}

public class DialogueManager: MonoBehaviour
{
    public Text speakerText;
    public Text dialogueText;
    public Image background;
    public Image character;

    private List<DialogueLine> lines;
    private int index = 0;

    private void Start()
    {
        LoadDialogue();
        ShowLine();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            index++;
            if (index < lines.Count)
                ShowLine();
            else
                EndDialogue();
        }
    }

    void LoadDialogue()
    {
        String path = Path.Combine(Application.dataPath, "Data/dialogue.json");
        String json = File.ReadAllText(path);
        lines = JsonUtility.FromJson<DialogueWrapper>("{\"lines\" : " + json + "}").lines;
    }    

    void ShowLine()
    {
        var line = lines[index];
        speakerText.text = line.speaker;
        dialogueText.text = line.text;
        background.sprite = Resources.Load<Sprite>("Backgrounds/" + line.background);
        character.sprite = Resources.Load<Sprite>("Characters/" + line.character);

    }   
    
    void EndDialogue()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Lv1");
    }
    [System.Serializable]
    private class DialogueWrapper
    {
        public List<DialogueLine>lines;
    }

    
}
