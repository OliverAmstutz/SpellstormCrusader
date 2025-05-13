using Bootstrap;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        private MySceneManager _mySceneManager;

        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void Start()
        {
            _mySceneManager = FindAnyObjectByType<MySceneManager>();
            if (!_mySceneManager)
            {
                Debug.LogError("MySceneManager not found in the scene.");
            }
        }

        private void OnPlayClicked() => _mySceneManager.UnloadAndLoad(SceneList.MainMenu, SceneList.GameScene);

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
