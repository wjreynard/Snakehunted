using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : MonoBehaviour {


    public void StartScene() {
        // If on last level, go to menu scene
        // This could be improved by automatically detecting last level in build rather than manually setting it
        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            SceneManager.LoadScene(0);
        }
        // By default, go to next scene
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
