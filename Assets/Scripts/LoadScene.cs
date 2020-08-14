using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public ObjectManager objectManager;
    public void LoadByIndex(int sceneIndex)
    {
        StartCoroutine(ILoadByIndex(sceneIndex));
    }

    public IEnumerator ILoadByIndex(int sceneIndex)
    {
        objectManager.DisableObjects();
        yield return new WaitForSeconds(objectManager.objects.Length + 1);
        SceneManager.LoadScene(sceneIndex);
    }
}
