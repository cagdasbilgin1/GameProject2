using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LoadingManager : MonoBehaviour
{
    [Inject] Player player;

    [SerializeField] GameObject _loadingPanel;
    [SerializeField] Slider _progressBar;
    [SerializeField] TextMeshProUGUI _loadingTxt;
    [SerializeField] float _totalDuration = 5f;

    public event Action OnLoadingPanelClosed;

    void Start()
    {
        StartLoading();
    }

    public void StartLoading()
    {
        StartCoroutine(LoadData());
    }

    IEnumerator LoadData()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _totalDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / _totalDuration);
            _progressBar.value = progress;
            _loadingTxt.text = "Loading... " + (int)(progress * 100) + "%";
            yield return null;
        }

        _loadingPanel.SetActive(false);
        OnLoadingPanelClosed?.Invoke();
    }
}
