using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bootstrap
{
    public class MySceneManager : MonoBehaviour
    {
        [SerializeField] private SceneList sceneToActivate = SceneList.MainMenu;
        private readonly HashSet<string> _scenesLoaded = new();

        private readonly HashSet<string> _scenesToLoad = new();

        private void Start()
        {
            _scenesToLoad.Add(SceneList.Environment.GetSceneName());
            _scenesToLoad.Add(sceneToActivate.GetSceneName());

            foreach (string scene in _scenesToLoad)
            {
                StartCoroutine(LoadSceneAsync(scene));
            }

            StartCoroutine(WaitForAllScenesThenSetActive(sceneToActivate.GetSceneName()));
        }

        public void UnloadAndLoad(SceneList toUnload, SceneList toLoad) =>
            StartCoroutine(UnloadThenLoad(toUnload.GetSceneName(), toLoad.GetSceneName()));

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (asyncLoad is { isDone: false })
            {
                yield return null;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                _scenesLoaded.Add(sceneName);
            }
            else
            {
                Debug.LogError($"Failed to load scene '{sceneName}'.");
            }
        }

        private IEnumerator WaitForAllScenesThenSetActive(string sceneNameToActivate)
        {
            while (_scenesLoaded.Count < _scenesToLoad.Count)
            {
                yield return null;
            }

            Scene targetScene = SceneManager.GetSceneByName(sceneNameToActivate);
            if (targetScene.IsValid() && targetScene.isLoaded)
            {
                SceneManager.SetActiveScene(targetScene);
            }
            else
            {
                Debug.LogError($"Scene '{sceneNameToActivate}' not valid or not loaded.");
            }
        }

        private IEnumerator UnloadThenLoad(string unloadScene, string loadScene)
        {
            if (SceneManager.GetSceneByName(unloadScene).isLoaded)
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(unloadScene);
                while (unloadOp is { isDone: false })
                {
                    yield return null;
                }
            }

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
            while (loadOp is { isDone: false })
            {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByName(loadScene);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                SceneManager.SetActiveScene(loadedScene);
            }
        }
    }

    public enum SceneList
    {
        Bootstrap,
        GameScene,
        MainMenu,
        Environment
    }
}
