using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOutText : MonoBehaviour
{
    private Coroutine fadeOutCoroutine; // 코루틴 참조 저장을 위한 변수
    public Text textComponent; // 텍스트 컴포넌트 참조

    // 투명도를 초기화하고 코루틴을 재시작하는 메서드
    public void ResetOpacity()
    {
        // 실행 중인 코루틴이 있다면 중지
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }

        // 텍스트의 투명도를 완전 불투명하게 초기화
        Color color = textComponent.color;
        color.a = 1f;
        textComponent.color = color;

        // 페이드 아웃 코루틴 시작
        fadeOutCoroutine = StartCoroutine(StartFadingOut());
    }

    // 텍스트의 투명도를 점차 감소시키는 코루틴
    public IEnumerator StartFadingOut()
    {
        float duration = 2.0f; // 페이드 아웃 지속 시간
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // 경과 시간에 따라 투명도 감소
            float t = (Time.time - startTime) / duration;
            Color color = textComponent.color;
            color.a = Mathf.Lerp(1f, 0f, t);
            textComponent.color = color;

            yield return null;
        }
    }
}