using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour {
    GameObject graphics;
    Vector3 desiredPosition, smoothedPosition;


	void LateUpdate () {
        graphics = GameObject.Find("graphics");
        if (!graphics)
        {
            Debug.LogError("Could not find a child named 'graphics'!");
        }
        desiredPosition = new Vector3(0,0,0);
        smoothedPosition = Vector3.Lerp(graphics.transform.position, desiredPosition, 0.01f);
        graphics.transform.position = smoothedPosition;
        graphics.transform.localScale = Vector3.Lerp(graphics.transform.localScale, new Vector3(1.1f, 1, 1), 0.01f);
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Main Menu");
    }

}
