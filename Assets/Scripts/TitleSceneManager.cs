using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nickNameInputField;

    public void EnterGame()
    {
        GameManager.Instance.nickName = nickNameInputField.text;
        SceneManager.LoadScene(1);
    }
}
