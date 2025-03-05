using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 버튼에 대한 처리를 위해 추가

namespace InGameScene.UI
{
    //===========================================================
    // 업그레이드 UI
    //===========================================================
    public class InGameUI_Upgrade : InGameUI_LeftUIBase
    {
        [SerializeField] private GameObject _upgradeParentObject;  // 무기와 스킬 아이템을 담을 부모 오브젝트
        [SerializeField] private GameObject _upgradeItemPrefab;    // 무기 아이템 프리팹
        [SerializeField] private GameObject _skillItemPrefab;      // 스킬 아이템 프리팹 추가

        [SerializeField] private List<InGameUI_UpgradeItem> _upgradeItemList; // 무기 아이템 리스트
        [SerializeField] private List<InGameUI_SkillItem> _skillItemList;    // 스킬 아이템 리스트 추가

        [SerializeField] private Button _weaponShopButton;  // 무기 상점 버튼
        [SerializeField] private Button _skillShopButton;   // 스킬 상점 버튼
        [SerializeField] private GameObject[] _shopFocusObject;

        private bool isWeaponShop = true; // 상점 전환 플래그, true면 무기 상점, false면 스킬 상점

        // 무기 상점 및 스킬 상점 아이템 생성
        public override void Init()
        {
            base.Init();

            // 무기 상점 초기화
            foreach (var weapon in StaticManager.Backend.Chart.Weapon.Dictionary)
            {
                Sprite sprite = weapon.Value.WeaponSprite;

                var newWeapon = Instantiate(_upgradeItemPrefab, _upgradeParentObject.transform, true);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localScale = Vector3.one;

                newWeapon.GetComponent<InGameUI_UpgradeItem>().Init(sprite, weapon.Value, UpdateUI);
                _upgradeItemList.Add(newWeapon.GetComponent<InGameUI_UpgradeItem>());
            }

            // 스킬 상점 초기화
            foreach (var skill in StaticManager.Backend.Chart.Skill.Dictionary)
            {
                Sprite sprite = skill.Value.SkillSprite;

                var newSkill = Instantiate(_skillItemPrefab, _upgradeParentObject.transform, true);
                newSkill.transform.localPosition = Vector3.zero;
                newSkill.transform.localScale = Vector3.one;

                newSkill.GetComponent<InGameUI_SkillItem>().Init(sprite, skill.Value, UpdateUI);
                _skillItemList.Add(newSkill.GetComponent<InGameUI_SkillItem>());
            }

            // 초기에는 무기 상점만 보이도록 설정
            SwitchShop(true);

            // 버튼 이벤트 추가
            _weaponShopButton.onClick.AddListener(() => SwitchShop(true));
            _skillShopButton.onClick.AddListener(() => SwitchShop(false));
        }

        // 상점 전환 기능
        public void SwitchShop(bool showWeaponShop)
        {
            isWeaponShop = showWeaponShop;

            // 무기 상점 아이템 활성화/비활성화
            foreach (var weaponItem in _upgradeItemList)
            {
                weaponItem.gameObject.SetActive(isWeaponShop);
            }

            // 스킬 상점 아이템 활성화/비활성화
            foreach (var skillItem in _skillItemList)
            {
                skillItem.gameObject.SetActive(!isWeaponShop);
            }

            // 버튼 선택 상태 업데이트 (선택된 버튼 강조)
/*            _weaponShopButton.interactable = !isWeaponShop;
            _skillShopButton.interactable = isWeaponShop;*/

            _shopFocusObject[0].SetActive(showWeaponShop);
            _shopFocusObject[1].SetActive(!showWeaponShop);
        }

        public override void Open()
        {
            base.Open();

            // 무기 또는 스킬 장착 상태 체크
            if (isWeaponShop)
            {
                foreach (var upgradeItem in _upgradeItemList)
                {
                    upgradeItem.EquipCheck();
                }
            }
            else
            {
                foreach (var skillItem in _skillItemList)
                {
                    skillItem.EquipCheck();
                }
            }
        }
    }
}
