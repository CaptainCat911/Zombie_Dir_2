using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject screen;               // экран загрузки

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
        screen.SetActive(true);
    }

    public void Quit()
    {
        Debug.Log("Выход !");
        Application.Quit();
    }
}
