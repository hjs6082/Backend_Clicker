// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // 스킬 UI의 아이템 클래스
    //===========================================================
    public class InGameUI_SkillItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _skillName;
        [SerializeField] private TMP_Text _skillExplane;
        [SerializeField] private TMP_Text _skillCool;
        [SerializeField] private Button _skillBuyButton;
        [SerializeField] private Button _skillEquipButton;
        [SerializeField] private TMP_Text _skillStatusText;
        [SerializeField] private TMP_Text _skillPrice;

        private BackendData.Chart.Skill.Item _skillInfo;

        private UnityAction _update;

        // 무기 차트의 정보에 따라 초기화
        public void Init(Sprite sprite, BackendData.Chart.Skill.Item skillInfo, UnityAction action)
        {
            _skillInfo = skillInfo;

            _image.sprite = sprite;
            _skillName.text = _skillInfo.SkillName;
            _skillExplane.text = _skillInfo.SkillContent;
            _skillCool.text = "쿨타임 : " + _skillInfo.SkillCooltime.ToString();

            _skillPrice.text = "가격 : " + _skillInfo.Price.ToString();

            _skillBuyButton.onClick.RemoveAllListeners();
            _skillEquipButton.onClick.RemoveAllListeners();

            _skillBuyButton.onClick.AddListener(BuyButton);
            _skillEquipButton.onClick.AddListener(EquipButton);

            _update = action;
            _update.Invoke();

            EquipCheck();
        }

        // 구매 버튼 클릭시 현재 자금 무기를 비교하여 구매 여부 판단
        // 구매 가능 시, UserData를 뺄셈하고 무기 지급.
        void BuyButton()
        {
            if (StaticManager.Backend.GameData.UserData.Money < _skillInfo.Price)
            {
                StaticManager.UI.AlertUI.OpenWarningUI("구매 불가", "현재 자금이 부족하여 해당 아이템을 구매할 수 없습니다");
                return;
            }

            InGameScene.Managers.Game.UpdateUserData(-_skillInfo.Price, 0);
            StaticManager.UI.AlertUI.OpenAlertUI("구매 완료", _skillInfo.SkillName + "이(가) 구매 완료되었습니다.");
            //TODO : 바로 착용시킬것
            //this.gameObject.SetActive(false);
            InGameScene.Managers.Game.UpdateSkillIndex(_skillInfo.SkillID);
            _skillEquipButton.gameObject.SetActive(true);
            _skillBuyButton.gameObject.SetActive(false);
            _skillStatusText.text = "착용";
            _update.Invoke();
        }

        void EquipButton()
        {
            InGameScene.Managers.Game.UpdateSkillIndex(_skillInfo.SkillID);
            StaticManager.UI.AlertUI.OpenAlertUI("착용 완료", _skillInfo.SkillName + "이(가) 착용되었습니다.");
            SoundManager.Instance.PlaySFX("Equip");
            Equip();
        }

        void Equip()
        {
            Transform parentTransform = this.transform.parent;

            // 부모 객체가 있는지 확인합니다.
            if (parentTransform != null)
            {
                // 부모 객체의 자식 객체 수를 가져옵니다.
                int childCount = parentTransform.childCount;

                // Dictionary의 값을 순회합니다.
                int index = 0;
                foreach (var skillEntry in StaticManager.Backend.Chart.Skill.Dictionary)
                {
                    // 자식 개체의 개수와 Dictionary의 개수를 비교
                    if (index >= childCount)
                    {
                        Debug.LogWarning("부모 객체의 자식 수보다 Dictionary 항목 수가 많습니다.");
                        break;
                    }

                    // 자식 객체를 가져옵니다.
                    Transform sibling = parentTransform.GetChild(index);

                    // 자식 객체에 InGameUI_UpgradeItem 스크립트가 있는지 확인합니다.
                    InGameUI_SkillItem skillItem = sibling.GetComponent<InGameUI_SkillItem>();

                    // 스크립트가 있으면 Init 함수를 호출합니다.
                    if (skillItem != null)
                    {
                        skillItem.Init(skillEntry.Value.SkillSprite, skillEntry.Value, _update);
                    }

                    index++;
                }
            }
        }

        public void EquipCheck()
        {
            //TODO: Weapon이 Inventory에 있을경우 EquipButton을 활성화.
            if (StaticManager.Backend.GameData.UserData.SkillIndex == _skillInfo.SkillID)
            {
                _skillBuyButton.gameObject.SetActive(false);
                _skillEquipButton.gameObject.SetActive(true);
                _skillStatusText.text = "착용 중";
                _skillEquipButton.enabled = false;
            }
            else
            {
                _skillEquipButton.enabled = true;
                _skillStatusText.text = "착용";
            }
        }
    }
}