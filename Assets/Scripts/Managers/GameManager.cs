using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(1);
    }
}
