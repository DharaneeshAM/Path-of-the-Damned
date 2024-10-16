using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dungeon"))
        {
            SceneManager.LoadScene("Dungeon");
        }
        if (other.CompareTag("Dungeon 2"))
        {
            SceneManager.LoadScene("Dungeon 2");
        }
        if (other.CompareTag("Dungeon 3"))
        {
            SceneManager.LoadScene("Dungeon 3");
        }
        if (other.CompareTag("Dungeon 4"))
        {
            SceneManager.LoadScene("Dungeon 4");
        }
        if (other.CompareTag("Quit"))
        {
            Debug.Log("Ouit");
            Application.Quit();
        }
    }

}
