// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace InGameScene.UI
{
    //===========================================================
    // ������ �κ��丮�� ���� UI
    //===========================================================
    public class InGameUI_ItemInventory : MonoBehaviour
    {
        [SerializeField] private GameObject _ItemParentObject;
        [SerializeField] private GameObject _useItemPrefab;

        // �ߺ� �������� count�� �����ֱ� ���� Dictionary�� ����
        private Dictionary<int, InGameUI_UseItem> _useItemDic = new Dictionary<int, InGameUI_UseItem>();

        // ItemInventory�� �ִ� 
        public void Init()
        {
            foreach (var shopChartItem in StaticManager.Backend.GameData.ItemInventory.Dictionary)
            {
                var newItem = Instantiate(_useItemPrefab, _ItemParentObject.transform, true);
                newItem.transform.localPosition = new Vector3(0, 0, 0);
                newItem.transform.localScale = new Vector3(1, 1, 1);

                var useItem = newItem.GetComponent<InGameUI_UseItem>();
                useItem.Init(this, shopChartItem.Key, shopChartItem.Value);

                _useItemDic.Add(shopChartItem.Key, useItem);
            }
        }

        // �������� ������ ���, �ش� �����ۿ��� �����ִ� ������ 1����
        // �������� �������� ���� ���, ���ο� ������ Ŭ���� �߰�
        public void UpdateUI(int item, int count)
        {
            if (_useItemDic.ContainsKey(item))
            {
                _useItemDic[item].Update();
            }
            else
            {
                var newItem = Instantiate(_useItemPrefab, _ItemParentObject.transform, true);
                newItem.transform.localPosition = new Vector3(0, 0, 0);
                newItem.transform.localScale = new Vector3(1, 1, 1);

                var useItem = newItem.GetComponent<InGameUI_UseItem>();
                useItem.Init(this, item, count);
                _useItemDic.Add(item, useItem);
            }
        }

        // �������� �� ����Ͽ��� ��, ������ 0�� �� ���, ����Ʈ���� ����(InGameUI_UseItem Ŭ�������� ���)
        public void DeleteUseItem(int itemID)
        {
            Destroy(_useItemDic[itemID].gameObject);
            _useItemDic.Remove(itemID);
        }
    }
}