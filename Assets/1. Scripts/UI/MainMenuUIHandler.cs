using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            inputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
    }

    public void OnJoinGameClicked()
    {
        // PlayerPrefs�� �г����� �����Ѵ�.
        PlayerPrefs.SetString("PlayerNickname", inputField.text);

        // �ϵ��ũ�� �����, �׸��� ������ ���������� HasKey�� ȣ�� ����
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }
}
