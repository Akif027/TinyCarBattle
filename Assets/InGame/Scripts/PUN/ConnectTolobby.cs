using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectTolobby : MonoBehaviourPunCallbacks
{
    public GameObject loadingScreen; // Reference to the loading screen UI object
    public Slider loadingBar; // Reference to the loading bar UI element

    private bool isLoading = false;

    void Start()
    {
        loadingBar.value = 0;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(LoadMainMenuWithLoading());
        }
    }

    private IEnumerator LoadMainMenuWithLoading()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");
        asyncLoad.allowSceneActivation = false;

        loadingScreen.SetActive(true);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Adjusting for scene activation
            loadingBar.value = progress;

            if (progress >= 1f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}