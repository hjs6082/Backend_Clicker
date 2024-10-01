// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{

    //===========================================================
    // ���� HP��, �̸����� �� ���� UI
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
            // hp�� ��Ȱ��ȭ
            ShowUI(false);

            _enemyAttackEffectTextPrefab = Resources.Load<GameObject>("Prefabs/AttackEffectObject");
        }

        // �� ���� ����
        public void SetEnemyInfo(string enemyName, float maxHp)
        {
            _enemyNameText.text = enemyName;

            _maxHp = maxHp;
            _currentHp = maxHp;
            _enemyHpSlider.maxValue = _maxHp;

            SetCurrentHp(_currentHp);
        }

        // �� HP ����
        public void SetCurrentHp(float currentHp)
        {
            _currentHp = currentHp;

            _enemyHpText.text = string.Format("{0:0.##} / {1:0}", _currentHp, _maxHp);
            _enemyHpSlider.value = _currentHp;
        }

        // HP�� Ȱ��ȭ ����
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
