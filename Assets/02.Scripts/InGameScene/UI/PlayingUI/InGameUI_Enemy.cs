// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{

    //===========================================================
    // 몬스터 HP바, 이름등의 적 관련 UI
    //===========================================================
    public class InGameUI_Enemy : MonoBehaviour
    {
        [SerializeField] private TMP_Text _enemyNameText;
        [SerializeField] private Slider _enemyHpSlider;
        [SerializeField] private TMP_Text _enemyHpText;
        [SerializeField] private GameObject _enemyAttackEffectTextPrefab;
        [SerializeField] private Transform _enemyAttackEffectTransform;

        private float _maxHp;
        private float _currentHp;

        private void Start()
        {
            // hp바 비활성화
            ShowUI(false);

            _enemyAttackEffectTextPrefab = Resources.Load<GameObject>("Prefabs/AttackEffectObject");
        }

        // 적 정보 갱신
        public void SetEnemyInfo(string enemyName, float maxHp)
        {
            _enemyNameText.text = enemyName;

            _maxHp = maxHp;
            _currentHp = maxHp;
            _enemyHpSlider.maxValue = _maxHp;

            SetCurrentHp(_currentHp);
        }

        // 적 HP 갱신
        public void SetCurrentHp(float currentHp)
        {
            _currentHp = currentHp;

            _enemyHpText.text = string.Format("{0:0.##} / {1:0}", _currentHp, _maxHp);
            _enemyHpSlider.value = _currentHp;
        }

        // HP바 활성화 여부
        public void ShowUI(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        public void OnAttackAnimation(float damage)
        {
            GameObject attackEffectObj = Instantiate(_enemyAttackEffectTextPrefab, _enemyAttackEffectTransform.position, Quaternion.identity);
            attackEffectObj.transform.SetParent(_enemyAttackEffectTransform, false);
            attackEffectObj.transform.localPosition = Vector2.zero;
            attackEffectObj.transform.localScale = Vector2.one;
            attackEffectObj.GetComponent<AttackTextEffect>().Play(damage);
        }
    }
}
