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
        // PlayerPrefs에 닉네임을 저장한다.
        PlayerPrefs.SetString("PlayerNickname", inputField.text);

        // 하드디스크에 저장됨, 그리고 게임이 시작했을때 HasKey로 호출 가능
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }
}
