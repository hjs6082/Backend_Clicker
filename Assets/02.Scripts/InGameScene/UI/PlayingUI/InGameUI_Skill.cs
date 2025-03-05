using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI_Skill : MonoBehaviour
{
    [SerializeField] private Image skillImage;
    [SerializeField] private TMP_Text skillCoolText;
    [SerializeField] private Image skillCoolTimeImage;

    [SerializeField] private Transform skillSpawnTransform;
    [SerializeField] private GameObject skillObject;

    [SerializeField] private Button useSkillButton;

    private void Start()
    {
        useSkillButton.onClick.AddListener(OnUseSkillButtonClick);    
    }

    public void SetSkill(int skillIndex)
    {
        Debug.Log("전달받은 skillIndex : " + skillIndex);
        foreach (var item in StaticManager.Backend.Chart.Skill.Dictionary)
        {
            if(item.Value.SkillID == skillIndex)
            {
                skillImage.sprite = item.Value.SkillSprite;
                skillCoolText.text = item.Value.SkillCooltime.ToString();
                skillObject = Resources.Load<GameObject>("Prefabs/InGameScene/Skill/Skill_" + item.Value.SkillID);
            }
        }
    }

    void OnUseSkillButtonClick()
    {
        GameObject so = Instantiate(skillObject, skillSpawnTransform);
    }
}
