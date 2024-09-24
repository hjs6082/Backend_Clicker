// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace InGameScene.UI
{
    //===========================================================
    // ��ŷ UI
    //===========================================================
    public class InGameUI_Rank : InGameUI_LeftUIBase
    {
        [SerializeField] private GameObject _rankUUIDSelectButtonGroup;
        [SerializeField] private GameObject _rankItemParentGroup;

        [SerializeField] private GameObject _rankUIItemPrefab;
        [SerializeField] private InGameUI_RankItem _myInGameUI_RankItem;

        private int selectedUUIDNum = 0; // ������ ��ŷ�� UUID
        private Button[] selectedUUIDButtons; // ���ð����� ��ư�� �迭


        private List<InGameUI_RankItem> _userRankingItemList = new List<InGameUI_RankItem>();

        //try catch�� RightButtonGroupManager���� ����
        public override void Init()
        {
            base.Init();

            // UI�� �ִ� ��ŷ �̸��� ��ư ��ü ��������
            GameObject button = _rankUUIDSelectButtonGroup.GetComponentInChildren<Button>().gameObject;

            // ��ŷ ������ŭ ��ư�� ũ�⸦ �ٿ� ���ٷ� ������ �� �ֵ��� ũ�� ����
/*            _rankUUIDSelectButtonGroup.GetComponent<GridLayoutGroup>().cellSize =
                new Vector2(
                    _rankUUIDSelectButtonGroup.GetComponent<RectTransform>().sizeDelta.x /
                    StaticManager.Backend.Rank.List.Count, 100);*/

            // ��ŷ ������ŭ ��ư �迭 �Ҵ�
            selectedUUIDButtons = new Button[StaticManager.Backend.Rank.List.Count];

            // ��ŷ ��ư �ϳ��� ����
            for (int i = 0; i < StaticManager.Backend.Rank.List.Count; i++)
            {
                GameObject obj;

                // ù��ư�� �̹� �����ϹǷ� �׳� ����
                if (i == 0)
                {
                    obj = button;
                }
                else
                {
                    // �ι�° ��ư���ʹ� ���� �����Ͽ� ����
                    obj = Instantiate(button, _rankUUIDSelectButtonGroup.transform, true);
                }

                int index = i;

                // ��ŷ �̸� ����
                obj.GetComponentInChildren<TMP_Text>().text = StaticManager.Backend.Rank.List[i].title;
                // ��ư Ŭ�� �� �ش� ��ŷ�� uuid�� ��ŷ ������ �ҷ����� �Լ� ��ư���� ����
                obj.GetComponent<Button>().onClick.AddListener(() => ChangeButton(index));
                // ��ư ����
                selectedUUIDButtons[index] = obj.GetComponent<Button>();
            }

            // �ִ� 10�������� ��ŷ�� �����ٰű� ������ 10���� ����. ���� ��ŷ�� 10�� ������ ��� ��Ȱ��ȭ.
            for (int i = 0; i < 10; i++)
            {
                var obj = Instantiate(_rankUIItemPrefab, _rankItemParentGroup.transform, true);
                _userRankingItemList.Add(obj.GetComponent<InGameUI_RankItem>());
                obj.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public override void Open()
        {
            base.Open();

            ChangeButton(0);
        }

        //������ ��ŷ uuid�� �����ϴ� �Լ�
        private void ChangeButton(int selectNum)
        {
            selectedUUIDButtons[selectedUUIDNum].GetComponent<TMP_Text>().color = Color.white;
            selectedUUIDButtons[selectNum].GetComponent<TMP_Text>().color = Color.gray;

            selectedUUIDNum = selectNum;

            ChangeRankList(selectNum);
        }

        // uuid�� ���� ��ŷ�� �ҷ����� �Լ�.
        // GetRankList �Լ����� n�� ������ ��쿡�� �������� �����͸� �ҷ����� �ʰ� ���� ������ �ִ� �����ͷ� ��� �����Ѵ�.
        private void ChangeRankList(int index)
        {
            StaticManager.UI.SetLoadingIcon(true);
            // n���� ������� ���� �ҷ��� �� callback �Լ� ����, ������ �ʾ��� ��� ĳ�̵� ������ ����
            StaticManager.Backend.Rank.List[index].GetRankList((isSuccess, list) => {
                if (isSuccess)
                {
                    // ����Ʈ�� �ִ� ��������ŭ ��ŷ ������ Ȱ��ȭ
                    for (int i = 0; i < list.Count; i++)
                    {
                        _userRankingItemList[i].gameObject.SetActive(true);
                        _userRankingItemList[i].Init(list[i]);

                        // ���� ��쿡�� ���
                        if (i > _userRankingItemList.Count)
                        {
                            break;
                        }
                    }

                    // ������ 10������ �����Ͱ� ���� ��쿡�� ���� �����͸� �Ⱥ��̰� ����
                    for (int i = list.Count; i < _userRankingItemList.Count; i++)
                    {
                        _userRankingItemList[i].gameObject.SetActive(false);
                    }

                    //���������� ó���� �� ���Ŀ��� �� ��ŷ�� ����
                    UpdateMyRank(index);
                }
                else
                {
                    StaticManager.UI.SetLoadingIcon(false);
                }
            });
        }

        // �� ��ŷ �ҷ����� �Լ�
        // ���� ��ŷ�� ���� ���, Update�� �õ�
        private void UpdateMyRank(int index)
        {
            StaticManager.Backend.Rank.List[index].GetMyRank((isSuccess, myRank) => {
                if (isSuccess)
                {
                    if (myRank != null)
                    {
                        _myInGameUI_RankItem.Init(myRank);
                    }
                    else
                    {
                        StaticManager.Backend.SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), "myRank is null");
                    }
                }
                else
                {
                    Debug.Log("��ŷ�� ���������� �ε���� �ʾҽ��ϴ�.");
                }

                StaticManager.UI.SetLoadingIcon(false);
            });
        }
    }
}