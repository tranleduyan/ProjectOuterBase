using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : Singleton<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup fadeCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;

    private IEnumerator Start()
    {
        //Set the initial alpha to start off with  a black screen
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        fadeCanvasGroup.alpha = 1f;

        //Start the first scene loading and wait for it to finish 
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        //if this event has any subscribers call it
        EventHandler.CallAfterSceneLoadEvent();

        SaveLoadManager.Instance.RestoreCurrentSceneData();

        //Once the scene is finished loading, start fading in
        StartCoroutine(Fade(0f));
    }

    //This is the main external point of contance and influence from the rest of the project
    //this will be called when the player wants to switch scenes.

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        //if a fade isn't happening then start fading and switching scenes
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }

    //This is the coroutine where the building blocks of the script are put together
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        //call before scene unload fadeout event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        //Start fading to black and wait for it to finish before continuing
        yield return StartCoroutine(Fade(1f));

        //Store scene data before switch to another scene
        SaveLoadManager.Instance.StoreCurrentSceneData();

        //SetPlayer position
        Player.Instance.gameObject.transform.position = spawnPosition;

        //Call before scene unload event
        EventHandler.CallBeforeSceneUnloadEvent();

        //Unload the current Active scene
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //start loading the given scene and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //Call after scene load event
        EventHandler.CallAfterSceneLoadEvent();

        //restore all the saved scene items of the scene before showing it
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        //Start fading back in and wait for it to finish before exiting the function
        yield return StartCoroutine(Fade(0));

        //Call after scene load fade in event;
        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    private IEnumerator Fade(float finalAlpha)
    {
        //Set the fading flag to true so the FadeAnDswitch coroutine wont be called again
        isFading = true;

        //make sure the canvasgroup blocks raycasts into the scene so no more input can be accepted
        fadeCanvasGroup.blocksRaycasts = true;

        //Calculate how fast the canvas group should fade based on its current alpha, its final alpha and how it has to change between the two
        float fadeSpeed = Mathf.Abs(fadeCanvasGroup.alpha - finalAlpha) / fadeDuration;

        //while the canvas group hasnt reach the final alpha yet
        while (!Mathf.Approximately(fadeCanvasGroup.alpha, finalAlpha))
        {
            //move the alpha towards its target alpha
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            //Wait for a frame then continue
            yield return null;
        }

        isFading = false;
        //stop the canvas group from blocking raycastm so input can be used
        fadeCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        //Allow the given scene to load over several frames and add it to the already loaded scenes(just the persistent scene at this poiint)
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //find the scene that was most recently loaded(the one at the last index of the loaded scenes)
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        //Set the newlyloadedScene as the active scene(this marks it as the one to be unloaded next
        SceneManager.SetActiveScene(newlyLoadedScene);

    }
}
