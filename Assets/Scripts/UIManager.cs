using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreTextUI;

    public void UpdateScoreText(int score)
    {
        scoreTextUI.text = $"Score: {score}";
    }
}
