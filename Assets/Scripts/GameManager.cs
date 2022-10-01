using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public string nickName;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }

            return instance;
        }
    }
}
