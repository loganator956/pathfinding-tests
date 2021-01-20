using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AStarButtonPress()
    {
        SceneManager.LoadScene("A-Star", LoadSceneMode.Single);
    }

    public void SimpleButtonPress()
    {
        SceneManager.LoadScene("Simple", LoadSceneMode.Single);
    }

    public void QuitButtonPress()
    {
        Application.Quit();
    }
}
