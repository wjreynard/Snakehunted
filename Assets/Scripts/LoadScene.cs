using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public ObjectManager objectManager;
    public void LoadByIndex(int sceneIndex)
    {
        objectManager.DisableObjects();
        SceneManager.LoadScene(sceneIndex);
    }
}
