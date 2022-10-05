using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameMessageUIHandler : MonoBehaviour
{
    public TextMeshProUGUI[] textMeshProUGUIs;

    Queue messageQueue = new Queue();

    void Update()
    {
        
    }

    public void OnGameMessageReceived(string message)
    {
        Debug.Log($"InGameMessageUIHandler {message}");

        messageQueue.Enqueue(message);

        if (messageQueue.Count > 3)
        {
            messageQueue.Dequeue();
        }

        int queueIndex = 0;
        foreach (string messageInQueue in messageQueue)
        {
            textMeshProUGUIs[queueIndex].text = messageInQueue;
            queueIndex++;
        }
    }
}
