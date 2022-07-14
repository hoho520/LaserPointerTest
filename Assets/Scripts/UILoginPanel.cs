using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILoginPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _connectionStatusText;
    [SerializeField]
    private InputField _userIDInputText;
    [SerializeField]
    private Button _connectionButton;
    [SerializeField]
    private UILoadingPanel _loadingPanel;

    private void Awake()
    {
        _loadingPanel.gameObject.SetActive(false);

        if (_connectionButton.onClick.GetPersistentEventCount() == 0)
        {
            _connectionButton.onClick.AddListener(OnClickConnectionButon);
        }
    }

    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        _connectionButton.onClick.RemoveListener(OnClickConnectionButon);
    }

    private void Init()
    {
        _connectionStatusText.text = PUN2ConnectionManager.Instance.GetConnectionState();
    }

    private void OnClickConnectionButon()
    {
        this.gameObject.SetActive(false);
        _loadingPanel.gameObject.SetActive(true);

        string nickname = _userIDInputText == null || string.IsNullOrEmpty(_userIDInputText.text) ? "TempPlayer" : _userIDInputText.text;        
        PUN2ConnectionManager.Instance.Connect(nickname, () => 
        {
            GameSceneManager.Instance.MoveScene(eSceneType.Lobby);
        });
    }

    public void OnClickQuitGameButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
