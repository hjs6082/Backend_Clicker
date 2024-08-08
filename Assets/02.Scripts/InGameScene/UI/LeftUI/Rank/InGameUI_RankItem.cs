// Copyright 2013-2022 AFI, INC. All rights reserved.

using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BackendData.Rank;


namespace InGameScene.UI
{
    //===========================================================
    // ��ŷ UI�� �� ���� ����Ʈ�� ������ ������ Ŭ����(��������)
    //===========================================================
    public class InGameUI_RankItem : MonoBehaviour
    {

        [SerializeField] private TMP_Text rankText;

        [SerializeField] private TMP_Text nickNameText;
        [SerializeField] private TMP_Text rankScoreText;

        public void Init(BackendData.Rank.RankUserItem rankUserItem)
        {
            rankText.text = rankUserItem.rank;
            nickNameText.text = rankUserItem.nickname;
            rankScoreText.text = "���ھ� : " + rankUserItem.score;
        }
    }
}