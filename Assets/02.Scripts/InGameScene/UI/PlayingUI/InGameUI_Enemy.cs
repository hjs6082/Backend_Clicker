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
        [SerializeField] private GameObject _bossEffectPrefab;
        [SerializeField] private GameObject _bossKillEffectPrefab;
        [SerializeField] private Transform _bossEffectTransform;
        [SerializeField] private TMP_Text _bossText;

        private float _maxHp;
        private float _currentHp;

        private void Start()
        {
            // hp바 비활성화
            ShowUI(false);

            _enemyAttackEffectTextPrefab = Resources.Load<GameObject>("Prefabs/AttackEffectObject");
            _bossEffectPrefab = Resources.Load<GameObject>("Prefabs/BossEffectPrefab");
            _bossKillEffectPrefab = Resources.Load<GameObject>("Prefabs/BossKillEffectPrefab");
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

            _enemyHpText.text = string.Format("{0:0.##}K / {1:0}K", _currentHp, _maxHp);
            _enemyHpSlider.value = _currentHp;
        }

        // HP바 활성화 여부
        public void ShowUI(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        public void ShowBossUI(bool isBoss)
        {
            if (isBoss)
            {
                _bossText.gameObject.SetActive(true);
            }
            else
            {
                _bossText.gameObject.SetActive(false);
            }
        }

        public void ResetUI()
        {
            _enemyHpSlider.value = 0;
            _enemyHpText.text = "0 / "+ _maxHp + "K";
        }

        public void OnAttackAnimation(float damage)
        {
            GameObject attackEffectObj = Instantiate(_enemyAttackEffectTextPrefab, _enemyAttackEffectTransform.position, Quaternion.identity);
            attackEffectObj.transform.SetParent(_enemyAttackEffectTransform, false);
            attackEffectObj.transform.localPosition = Vector2.zero;
            attackEffectObj.transform.localScale = Vector2.one;
            attackEffectObj.GetComponent<AttackTextEffect>().Play(damage);
        }

        public void OnBossAnimation()
        {
            _bossText.gameObject.SetActive(true);
            GameObject attackEffectObj = Instantiate(_bossEffectPrefab, _bossEffectTransform.position, Quaternion.identity);
            attackEffectObj.transform.SetParent(_bossEffectTransform, false);
            attackEffectObj.transform.localPosition = Vector2.zero;
            //_bossEffectPrefab.GetComponent<BossEffect>().StartBossEffect();
        }

        public void OnBossKillAnimation(float money, float exp)
        {
            GameObject bossKillEffectObj = Instantiate(_bossKillEffectPrefab, _bossEffectTransform.position, Quaternion.identity);
            bossKillEffectObj.transform.SetParent(_bossEffectTransform, false);
            bossKillEffectObj.transform.localPosition = Vector2.zero;
            bossKillEffectObj.GetComponent<BossKillEffect>().Init(money, exp);
        }
    }
}
