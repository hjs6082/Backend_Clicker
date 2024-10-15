using UnityEngine;
using UnityEngine.UI;  // Image�� ����ϱ� ���� ���ӽ����̽�
using DG.Tweening;
using TMPro;

public class BossKillEffect : MonoBehaviour
{
    private RectTransform uiElement;  // �̵���ų UI ������Ʈ
    private Image image;              // ���İ��� ������ Image ������Ʈ
    [SerializeField] private float durationToCenter = 1f;  // �߾ӱ��� �̵��ϴ� �ð�
    [SerializeField] private float pauseDuration = 1f;     // �߾ӿ��� ���ߴ� �ð�
    [SerializeField] private float durationToLeft = 1f;    // �������� ������� �ð�

    [SerializeField] private TMP_Text getMoneyText;
    [SerializeField] private TMP_Text getExpText;

    private Vector3 startPos;  // ���� ��ġ (���� 2211)
    private Vector3 centerPos; // �߾� ��ġ (0)
    private Vector3 endPos;    // ���� ��ġ (���� -2211)

    public void Init(float money, float exp)
    {
        getMoneyText.text = "Gold +" + money.ToString();
        getExpText.text = "Exp +" + exp.ToString();
    }

    void Start()
    {
        uiElement = this.gameObject.GetComponent<RectTransform>();
        image = this.gameObject.GetComponent<Image>();

        // ��ġ ����
        startPos = new Vector3(2211, uiElement.anchoredPosition.y, 0);
        centerPos = new Vector3(0, uiElement.anchoredPosition.y, 0);
        endPos = new Vector3(-2211, uiElement.anchoredPosition.y, 0);

        // �ʱ� ��ġ�� ����(2211)���� ����
        uiElement.anchoredPosition = startPos;

        // �̹��� ���İ��� 0���� ���� (ó���� ������ ����)
        Color color = image.color;
        color.a = 0;
        image.color = color;

        // �ִϸ��̼� ����
        MoveObject();
    }

    void MoveObject()
    {
        // �������� �߾����� �̵�, ���ÿ� ���İ��� 1��
        Sequence moveSequence = DOTween.Sequence();

        moveSequence.Append(uiElement.DOAnchorPos(centerPos, durationToCenter).SetEase(Ease.OutQuad));
        moveSequence.Join(image.DOFade(1, durationToCenter));  // ���İ� 1�� ����

        // 1�� ��� �� �������� �̵�, ���ÿ� ���İ��� 0����
        moveSequence.AppendInterval(pauseDuration);
        moveSequence.Append(uiElement.DOAnchorPos(endPos, durationToLeft).SetEase(Ease.InQuad));
        moveSequence.Join(image.DOFade(0, durationToLeft));  // ���İ� 0���� ����

        // �ִϸ��̼��� �Ϸ�Ǹ� ������Ʈ ����
        moveSequence.OnComplete(() => Destroy(gameObject));
    }
}
