using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] int _goalPoints;
    [SerializeField] CanvasGroup _victoryPanel;

    private int _capturedPoints;

    private void Start()
    {
        _capturedPoints = 0;
    }

    public void PlusOnePoint()
    {
        _capturedPoints++;
        if (_capturedPoints == _goalPoints)
        {
            StartCoroutine(ShowVictoryPanel());
        }
    }

    private IEnumerator ShowVictoryPanel()
    {
        while (true)
        {
            while (_victoryPanel.alpha < 1.0f)
            {
                _victoryPanel.alpha += Time.deltaTime;
                yield return null;
            }
            while (_victoryPanel.alpha > 0.0f)
            {
                _victoryPanel.alpha -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
