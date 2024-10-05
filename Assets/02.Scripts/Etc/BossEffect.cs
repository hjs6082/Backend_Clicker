using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Image 컴포넌트 사용을 위해 추가
using DG.Tweening;

public class BossEffect : MonoBehaviour
{
    [SerializeField]
    private TMP_Text[] bossTextArray;        // 각 글자를 담을 TMP_Text 배열 ('보 스 출 현' 각각)
    [SerializeField]
    private RectTransform topScrollContent;  // 상단 텍스트가 들어갈 Content
    [SerializeField]
    private RectTransform bottomScrollContent; // 하단 텍스트가 들어갈 Content

    [SerializeField] private float letterStartScale = 3f;     // 각 글자의 시작 크기
    [SerializeField] private float letterEndScale = 1f;       // 각 글자의 최종 크기
    [SerializeField] private float letterDelay = 0.2f;        // 각 글자 나타나는 딜레이
    [SerializeField] private float scrollDuration = 2f;       // 텍스트 이동 시간
    [SerializeField] private float fadeOutDuration = 1f;      // 사라지는 시간

    private void Start()
    {
        StartBossEffect();
    }

    private void StartBossEffect()
    {
        // 1. bossTextArray의 각 글자에 애니메이션 적용
        for (int i = 0; i < bossTextArray.Length; i++)
        {
            TMP_Text letterObj = bossTextArray[i]; // 배열의 각 요소를 참조
            letterObj.transform.localScale = Vector3.one * letterStartScale; // 3배 크기로 시작
            letterObj.DOFade(0, 0); // 투명도 0으로 시작

            // 각 글자에 애니메이션 적용
            letterObj.transform.DOScale(letterEndScale, 0.5f)
                .SetEase(Ease.OutBounce)
                .SetDelay(i * letterDelay);  // 각 글자마다 딜레이 추가
            letterObj.DOFade(1, 0.5f).SetDelay(i * letterDelay); // 페이드 인
        }

        // 2. 상단 텍스트 원래 위치에서 오른쪽 끝으로 이동
        topScrollContent.anchoredPosition = new Vector2(-500f, topScrollContent.anchoredPosition.y); // 원래 위치
        topScrollContent.DOAnchorPosX(0, scrollDuration).SetEase(Ease.Linear);  // 오른쪽 끝으로 이동

        // 3. 하단 텍스트 원래 위치에서 왼쪽 끝으로 이동
        bottomScrollContent.anchoredPosition = new Vector2(0f, bottomScrollContent.anchoredPosition.y); // 원래 위치
        bottomScrollContent.DOAnchorPosX(-500f, scrollDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            // 4. 배경 Image의 페이드 아웃
            this.gameObject.GetComponent<Image>().DOFade(0, fadeOutDuration).OnComplete(() =>
            {
                // 5. 페이드 아웃 완료 후 오브젝트 삭제
                Destroy(this.gameObject);
            });
        });
    }
}
