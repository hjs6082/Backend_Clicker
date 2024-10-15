using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGameScene.UI
{
    //===========================================================
    // 스테이지 , 보스 설정  UI
    //===========================================================
    public class InGameUI_Boss : InGameUI_LeftUIBase
    {
        [SerializeField]
        private GameObject _bossItemPrefab;
        [SerializeField]
        private Transform _bossItemTransofrm;
        [SerializeField]
        private GameObject _closeObject;

        [SerializeField]
        private List<InGameUI_BossItem> _bossItemList;

        public override void Init()
        {
            base.Init();

            foreach (var boss in StaticManager.Backend.Chart.Boss.Dictionary.Values)
            {
                var bossObject = Instantiate(_bossItemPrefab, _bossItemTransofrm);
                bossObject.GetComponent<InGameUI_BossItem>().Init(boss, OnClose);
                _bossItemList.Add(bossObject.GetComponent<InGameUI_BossItem>());
            }
        }

        public override void Open()
        {
            base.Open();
            foreach (var bossItem in _bossItemList)
            {
                bossItem.UnLockCheck();
            }
        }

        private void OnClose()
        {
            _closeObject.gameObject.SetActive(false);
        }
    }
}
