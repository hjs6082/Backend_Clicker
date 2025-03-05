using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace InGameScene
{
    public class InGameUI_Gacha : MonoBehaviour
    {
        // 보상 상자 UI를 위한 텍스트 및 오브젝트 변수들을 정의합니다.
        //public TextMeshProUGUI textNoOfCardAvailable; // 남은 보상 카드 수를 표시하는 텍스트
        public Image chestBoxStatus;
        public Sprite[] chestStatusSprites;
        //public GameObject chestBoxClose, chestBoxOpen; // 상자가 닫힌 상태와 열린 상태를 나타내는 오브젝트
        public GameObject rewardUIToFly; // 보상 UI가 날아다니는 애니메이션을 위한 오브젝트
        public GameObject rewardUIStartPos, rewardUIMiddlePos, rewardUIEndPos; // 보상 UI의 시작, 중간, 끝 위치
        public GameObject glowEffect;

        // 보상 관련 변수를 정의합니다.
        //public List<GameObject> rewardUIPopingEffect; // 보상 팝업 효과
        public GameObject rewardUITextGrp; // 보상 텍스트 그룹
        public bool isInAnimation; // 현재 애니메이션 진행 여부

        private ProbabilityItem nowItem;

        //public List<ChestRewardData> rewardsData = new List<ChestRewardData>(); // 보상 데이터를 저장하는 리스트

        public TextMeshProUGUI textRewardUIHeader; // 보상 UI의 헤더 텍스트
        public TextMeshProUGUI textRewardUIQuantity; // 보상 수량 텍스트
        public Image rewardIcon; //보상 아이콘
        public GameObject rewardObject;
        public Button closeButton;

        // 보상 결과를 보여줄 그룹 및 오브젝트
        //public GameObject rewardRevealingGrp;

        private string chestPath = "Sprite/Item/";

        // 스크립트가 실행될 때 호출되지만, 현재는 아무런 동작을 하지 않음
        public void Init(ProbabilityItem probabilityItem, string chestName)
        {
            nowItem = probabilityItem;
            Debug.Log(chestName);
            chestStatusSprites[0] = Resources.Load<Sprite>(chestPath + chestName);
            chestStatusSprites[1] = Resources.Load<Sprite>(chestPath + chestName + "_Open");
        }


        // 이 오브젝트가 활성화될 때 호출되는 메서드
        private void OnEnable()
        {
            //rewardRevealingGrp.SetActive(true); // 보상을 보여주는 그룹 활성화
            SetRewardChest(); // 보상 상자 설정
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClickCloseButton);
        }

        // 보상 상자를 초기 설정하는 메서드
        void SetRewardChest()
        {
            /*        textNoOfCardAvailable.text = totalRewardsAvailable + ""; // 남은 보상 카드 수를 텍스트로 표시
                    chestBoxClose.gameObject.SetActive(true); // 닫힌 상자를 활성화
                    chestBoxOpen.gameObject.SetActive(false); // 열린 상자는 비활성화*/
            //rewardUIToFly.transform.position = rewardUIStartPos.transform.position; // 보상 UI를 시작 위치로 설정
            rewardUIToFly.gameObject.SetActive(false); // 보상 UI를 비활성화
        }

        // 현재 보상을 설정하는 메서드
        void SetCurrentReward()
        {
            textRewardUIHeader.text = nowItem.itemName.ToString(); // 보상 헤더 설정
            textRewardUIQuantity.text = "x" + nowItem.num.ToString(); // 보상 수량 설정

            switch (nowItem.itemType)
            {
                case ProbabilityItemType.Exp:
                    rewardIcon.sprite = Resources.Load<Sprite>("Images/Icon/Icon_" + nowItem.itemType.ToString());
                    Managers.Game.UpdateUserData(0, nowItem.num);
                    break;
                case ProbabilityItemType.Gold:
                    rewardIcon.sprite = Resources.Load<Sprite>("Images/Icon/Icon_" + nowItem.itemType.ToString());
                    Managers.Game.UpdateUserData(nowItem.num, 0);
                    break;
                case ProbabilityItemType.Item:
                    foreach (var chartItem in StaticManager.Backend.Chart.Item.Dictionary)
                    {
                        if (nowItem.itemName == chartItem.Value.ItemName)
                        {
                            rewardIcon.sprite = chartItem.Value.ImageSprite;
                            Managers.Game.UpdateItemInventory(chartItem.Value.ItemID, nowItem.num);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        // 보상이 팝업되는 애니메이션을 시작하는 코루틴
        IEnumerator StartRewardPoping()
        {
            if (isInAnimation) yield break; // 애니메이션이 진행 중이면 종료
            SetRewardChest(); // 보상 상자를 설정
            isInAnimation = true; // 애니메이션 시작
            SetCurrentReward(); // 현재 보상을 설정
            chestBoxStatus.sprite = chestStatusSprites[1];
            rewardUIToFly.gameObject.SetActive(true); // 보상 UI 활성화
            glowEffect.SetActive(true);
            /*            foreach (GameObject obj in rewardUIPopingEffect)
                        {
                            obj.SetActive(true); // 보상 팝업 효과 활성화
                        }*/
            rewardUITextGrp.gameObject.SetActive(false); // 보상 텍스트 그룹 비활성화
            rewardUIToFly.transform.localScale = Vector3.zero; // 보상 UI 크기 0으로 설정
            rewardUIToFly.transform.DOScale(new Vector3(1, 1, 1), 0.25f); // 보상 UI가 점점 커지는 애니메이션 (0.25초)
            //rewardUIToFly.transform.DOLocalMove(rewardUIMiddlePos.transform.localPosition, 0.5f); // 보상 UI가 중간 위치로 이동하는 애니메이션 (0.5초)
            yield return new WaitForSeconds(0.5f); // 0.5초 대기
/*            foreach (GameObject obj in rewardUIPopingEffect)
            {
                obj.SetActive(false); // 보상 팝업 효과 비활성화
            }*/
            yield return new WaitForSeconds(0.5f); // 0.5초 대기
            rewardUIToFly.transform.DOLocalMove(rewardUIMiddlePos.transform.localPosition, 0.5f); // 보상 UI가 끝 위치로 이동하는 애니메이션 (0.5초)
            rewardObject.SetActive(true);
            chestBoxStatus.gameObject.SetActive(false);
            //yield return new WaitForSeconds(0.5f); // 0.5초 대기
            rewardUITextGrp.gameObject.SetActive(true); // 보상 텍스트 그룹 활성화
            //chestBoxStatus.sprite = chestStatusSprites[0];
            yield return new WaitForSeconds(0.5f); // 0.5초 대기
            isInAnimation = false; // 애니메이션 종료
            closeButton.gameObject.SetActive(true);
            //textNoOfCardAvailable.text = totalRewardsAvailable + ""; // 남은 보상 수를 텍스트로 업데이트
        }

        // 업데이트 메서드, 매 프레임 호출됨
        void Update()
        {
            // 마우스 클릭 시 동작
            if (Input.GetMouseButtonDown(0) && isInAnimation == false)
            {
                if (!closeButton.gameObject.activeSelf)
                {
                    StartCoroutine(StartRewardPoping()); // 보상이 남아있으면 팝업 애니메이션 시작
                }
/*                if (totalRewardsAvailable > 0)
                {
                    
                }
                else
                {
                    // 보상이 모두 소진되었을 때
                    if (!rewardEndResult.activeInHierarchy)
                    {
                        rewardRevealingGrp.SetActive(false); // 보상 그룹 비활성화
                        rewardEndResult.SetActive(true); // 보상 결과 UI 활성화
                        AddRewardValuesToEndResult(); // 보상 결과 추가
                    }
                    else
                    {
                        gameObject.SetActive(false); // 현재 게임 오브젝트 비활성화
                        TopCurrencyBar.instance.UpdateAllText(); // 상단 화폐 바 텍스트 업데이트
                    }
                    Debug.Log("Do Something else"); // 추가적인 동작이 가능하도록 로그 출력
                }*/
            }
        }

        
        void OnClickCloseButton()
        {
            Destroy(this.gameObject);
        }

        // 보상 결과를 설정하는 메서드
/*        public TextMeshProUGUI textGold, textGems, textRuby; // 각 화폐 타입의 텍스트 UI
        public void AddRewardValuesToEndResult()
        {
            textRuby.transform.parent.gameObject.SetActive(false); // 루비 보상은 비활성화
            textGold.transform.parent.gameObject.SetActive(false); // 금화 보상 비활성화
            textGems.transform.parent.gameObject.SetActive(false); // 보석 보상 비활성화
            int expectedGold = 0; // 예상 금화 보상 초기화
            int expectedGems = 0; // 예상 보석 보상 초기화
            for (int i = 0; i < rewardsData.Count; i++)
            {
                // 보상 타입에 따라 값을 누적하여 계산
                switch (rewardsData[i].currencyType)
                {
                    case eCurrencyType.gold:
                        expectedGold += rewardsData[i].quantity; // 금화 보상을 누적
                        textGold.transform.parent.gameObject.SetActive(true); // 금화 보상 활성화
                        break;
                    case eCurrencyType.gems:
                        textGems.transform.parent.gameObject.SetActive(true); // 보석 보상 활성화
                        expectedGems += rewardsData[i].quantity; // 보석 보상을 누적
                        break;
                }
            }
            textGold.text = expectedGold + ""; // 금화 보상 텍스트로 표시
            textGems.text = expectedGems + ""; // 보석 보상 텍스트로 표시
        }*/
    }
}
