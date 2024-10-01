using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class AttackTextEffect : MonoBehaviour
{
    [SerializeField]
    private TMP_Text attackText;

    public void Play(float damage)
    {
        attackText.text = damage.ToString();

        // ���� ��ġ���� 1.0f��ŭ ���� �̵��ϸ�, 0.5�� ���� �����̰� ������ ��������� ����
        transform.DOMoveY(transform.position.y + 1.0f, 0.5f)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() => Destroy(gameObject)); // �ִϸ��̼��� �Ϸ�Ǹ� ������Ʈ �ı�

        // �ؽ�Ʈ ������ ���� ������������ ����
        attackText.DOFade(0, 0.5f);
    }

}
