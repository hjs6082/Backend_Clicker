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

        // 현재 위치에서 1.0f만큼 위로 이동하며, 0.5초 동안 움직이고 서서히 사라지도록 설정
        transform.DOMoveY(transform.position.y + 1.0f, 0.5f)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() => Destroy(gameObject)); // 애니메이션이 완료되면 오브젝트 파괴

        // 텍스트 색상이 점점 투명해지도록 설정
        attackText.DOFade(0, 0.5f);
    }

}
