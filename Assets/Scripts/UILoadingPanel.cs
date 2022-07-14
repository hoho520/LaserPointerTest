using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UILoadingPanel : MonoBehaviour
{
    private readonly string LOADING_TEXT = "Loading";

    [SerializeField]
    private TextMeshProUGUI _loadingText;

    private Coroutine _coroutine;

    private void OnEnable()
    {
        _coroutine = StartCoroutine(UpdateLoadingText());
    }

    private void OnDisable()
    {
        StopCoroutine(_coroutine);
    }

    private IEnumerator UpdateLoadingText()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(LOADING_TEXT);
        int dotCount = 0;
        while (true)
        {
            if (dotCount >= 3)
            {
                sb.Clear();
                sb.Append(LOADING_TEXT);
                dotCount = 0;
            }
            else
            {
                sb.Append('.');
                dotCount++;
            }

            _loadingText.text = sb.ToString();

            yield return CoroutineUtil.GetWaitForSeconds(0.25f);
        }
    }
}
