﻿using DesignPatterns;
using Components.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

public class GameManager : Singleton<GameManager> {
    public enum SceneName {
        Core,
        Scene1,
        Scene2,
        Scene3,
        Scene4,
        Credits,
    }

    public RectTransform loadingRectTransform;
    public Image loadingImage;
    public Text loadingText;

    private Transparentable _loadingTransparentable;
    public Transparentable loadingTransparentable {
        get {
            if(_loadingTransparentable == null) {
                this._loadingTransparentable = this.loadingRectTransform.GetOrAddComponent<Transparentable>();
            }
            return this._loadingTransparentable;
        }
    }

    private void Start() {
        this.loadingRectTransform.gameObject.SetActive(false);
        this.loadingTransparentable.SetAlpha(0);
    }

    public void OnSceneButtonClicked(string sceneName) {
        LoadScene(sceneName, OnSceneLoaded);
    }

    private void OnSceneLoaded() {
        Debug.Log("Scene Loaded");
    }

    public void LoadScene(string sceneName, UnityAction onCompleteAction) {
        StartCoroutine(LoadSceneCoroutine(sceneName, onCompleteAction));
    }
    private IEnumerator LoadSceneCoroutine(string sceneName, UnityAction actionOnComplete) {
        this.loadingRectTransform.gameObject.SetActive(true);
        this.loadingTransparentable.SetAlpha(1, 0.25f, 0);
        this.loadingText.text = "0%";
        this.loadingImage.rectTransform.sizeDelta = new Vector2(0, this.loadingImage.rectTransform.sizeDelta.y);

        //yield return new WaitForSeconds(1);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while(!asyncOperation.isDone) {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f); //De 0 a 0.9 es la carga de la escena. De 0.9 a 1 es la activación de la escena
            this.loadingImage.rectTransform.sizeDelta = new Vector2(256 * progress, this.loadingImage.rectTransform.sizeDelta.y);
            this. loadingText.text = ((int) progress * 100) + "%";
            yield return null;
        }

        this.loadingTransparentable.SetAlpha(0, 0.25f, 0);
        if(actionOnComplete != null) {
            actionOnComplete.Invoke();
        }

        yield return new WaitForSeconds(0.25f);

        this.loadingRectTransform.gameObject.SetActive(false);
    }
}
