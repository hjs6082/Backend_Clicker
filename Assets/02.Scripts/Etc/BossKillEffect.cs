using UnityEngine;
using UnityEngine.UI;  // Image를 사용하기 위한 네임스페이스
using DG.Tweening;
using TMPro;

public class BossKillEffect : MonoBehaviour
{
    private RectTransform uiElement;  // 이동시킬 UI 오브젝트
    private Image image;              // 알파값을 제어할 Image 컴포넌트
    [SerializeField] private float durationToCenter = 1f;  // 중앙까지 이동하는 시간
    [SerializeField] private float pauseDuration = 1f;     // 중앙에서 멈추는 시간
    [SerializeField] private float durationToLeft = 1f;    // 좌측으로 사라지는 시간

    [SerializeField] private TMP_Text getMoneyText;
    [SerializeField] private TMP_Text getExpText;

    private Vector3 startPos;  // 시작 위치 (우측 2211)
    private Vector3 centerPos; // 중앙 위치 (0)
    private Vector3 endPos;    // 종료 위치 (좌측 -2211)

    public void Init(float money, float exp)
    {
        getMoneyText.text = "Gold +" + money.ToString();
        getExpText.text = "Exp +" + exp.ToString();
    }

    void Start()
    {
        uiElement = this.gameObject.GetComponent<RectTransform>();
        image = this.gameObject.GetComponent<Image>();

        // 위치 설정
        startPos = new Vector3(2211, uiElement.anchoredPosition.y, 0);
        centerPos = new Vector3(0, uiElement.anchoredPosition.y, 0);
        endPos = new Vector3(-2211, uiElement.anchoredPosition.y, 0);

        // 초기 위치를 우측(2211)으로 설정
        uiElement.anchoredPosition = startPos;

        // 이미지 알파값을 0으로 설정 (처음엔 보이지 않음)
        Color color = image.color;
        color.a = 0;
        image.color = color;

        // 애니메이션 시작
        MoveObject();
    }

    void MoveObject()
    {
        // 우측에서 중앙으로 이동, 동시에 알파값을 1로
        Sequence moveSequence = DOTween.Sequence();

        moveSequence.Append(uiElement.DOAnchorPos(centerPos, durationToCenter).SetEase(Ease.OutQuad));
        moveSequence.Join(image.DOFade(1, durationToCenter));  // 알파값 1로 변경

        // 1초 대기 후 좌측으로 이동, 동시에 알파값을 0으로
        moveSequence.AppendInterval(pauseDuration);
        moveSequence.Append(uiElement.DOAnchorPos(endPos, durationToLeft).SetEase(Ease.InQuad));
        moveSequence.Join(image.DOFade(0, durationToLeft));  // 알파값 0으로 변경

        // 애니메이션이 완료되면 오브젝트 제거
        moveSequence.OnComplete(() => Destroy(gameObject));
    }
}
