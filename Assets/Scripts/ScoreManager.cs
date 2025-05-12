using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public Text curScoreUI;
    private int curScore;

    public Text bestScoreUI;
    private int bestScore;
    
    private int _comboCount;
    public Text ComboUI;

    public int Score
    {
        get => curScore;
        set
        {
            curScore = value;
            curScoreUI.text = "SCORE : " + curScore;

            if (curScore > bestScore)
            {
                bestScore = curScore;
                bestScoreUI.text = "BEST : " + bestScore;
                PlayerPrefs.SetInt("Best Score", bestScore);
            }
        }
    }

    public int comboCount
    {
        get { return _comboCount; }
        set
        {
            _comboCount = value;
            ComboUI.text = "x" + _comboCount + " COMBO";
            if (_comboCount > 0)
            {
                // 콤보가 발생하면 콤보 UI를 활성화하고, ResetCombo를 시작합니다.
                ComboUI.gameObject.SetActive(true);
                StopCoroutine("ResetCombo");
                StartCoroutine("ResetCombo");
            }
        }
    }
    
    IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(2f);
        
        comboCount = 0;
        
        yield return new WaitForSeconds(1f);
        
        ComboUI.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("Best Score", 0);
        bestScoreUI.text = "BEST : " + bestScore;
        curScore = 0;
        curScoreUI.text = "SCORE : " + curScore;
    }
}
