using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Image ������Ʈ ����� ���� �߰�
using DG.Tweening;

public class BossEffect : MonoBehaviour
{
    [SerializeField]
    private TMP_Text[] bossTextArray;        // �� ���ڸ� ���� TMP_Text �迭 ('�� �� �� ��' ����)
    [SerializeField]
    private RectTransform topScrollContent;  // ��� �ؽ�Ʈ�� �� Content
    [SerializeField]
    private RectTransform bottomScrollContent; // �ϴ� �ؽ�Ʈ�� �� Content

    [SerializeField] private float letterStartScale = 3f;     // �� ������ ���� ũ��
    [SerializeField] private float letterEndScale = 1f;       // �� ������ ���� ũ��
    [SerializeField] private float letterDelay = 0.2f;        // �� ���� ��Ÿ���� ������
    [SerializeField] private float scrollDuration = 2f;       // �ؽ�Ʈ �̵� �ð�
    [SerializeField] private float fadeOutDuration = 1f;      // ������� �ð�

    private void Start()
    {
        StartBossEffect();
    }

    private void StartBossEffect()
    {
        // 1. bossTextArray�� �� ���ڿ� �ִϸ��̼� ����
        for (int i = 0; i < bossTextArray.Length; i++)
        {
            TMP_Text letterObj = bossTextArray[i]; // �迭�� �� ��Ҹ� ����
            letterObj.transform.localScale = Vector3.one * letterStartScale; // 3�� ũ��� ����
            letterObj.DOFade(0, 0); // ���� 0���� ����

            // �� ���ڿ� �ִϸ��̼� ����
            letterObj.transform.DOScale(letterEndScale, 0.5f)
                .SetEase(Ease.OutBounce)
                .SetDelay(i * letterDelay);  // �� ���ڸ��� ������ �߰�
            letterObj.DOFade(1, 0.5f).SetDelay(i * letterDelay); // ���̵� ��
        }

        // 2. ��� �ؽ�Ʈ ���� ��ġ���� ������ ������ �̵�
        topScrollContent.anchoredPosition = new Vector2(-500f, topScrollContent.anchoredPosition.y); // ���� ��ġ
        topScrollContent.DOAnchorPosX(0, scrollDuration).SetEase(Ease.Linear);  // ������ ������ �̵�

        // 3. �ϴ� �ؽ�Ʈ ���� ��ġ���� ���� ������ �̵�
        bottomScrollContent.anchoredPosition = new Vector2(0f, bottomScrollContent.anchoredPosition.y); // ���� ��ġ
        bottomScrollContent.DOAnchorPosX(-500f, scrollDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            // 4. ��� Image�� ���̵� �ƿ�
            this.gameObject.GetComponent<Image>().DOFade(0, fadeOutDuration).OnComplete(() =>
            {
                // 5. ���̵� �ƿ� �Ϸ� �� ������Ʈ ����
                Destroy(this.gameObject);
            });
        });
    }
}
