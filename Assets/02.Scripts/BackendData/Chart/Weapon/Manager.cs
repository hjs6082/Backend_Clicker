// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Chart.Weapon
{
    //===============================================================
    // Weapon ��Ʈ�� �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.Chart
    {

        // �� ��Ʈ�� row ������ ��� Dictionary
        readonly Dictionary<int, Item> _dictionary = new();

        // �ٸ� Ŭ�������� Add, Delete�� ������ �Ұ����ϵ��� �б� ���� Dictionary
        public IReadOnlyDictionary<int, Item> Dictionary => (IReadOnlyDictionary<int, Item>)_dictionary.AsReadOnlyCollection();

        //�̹��� ĳ�� ó���� Dictionary
        readonly Dictionary<string, Sprite> _weaponImages = new();
        readonly Dictionary<string, Sprite> _bulletImages = new();
        readonly Dictionary<string, Sprite> _effectImages = new();

        // ��Ʈ ���� �̸� ���� �Լ�
        // ��Ʈ �ҷ����⸦ ���������� ó���ϴ� BackendChartDataLoad() �Լ����� �ش� �Լ��� ���� ��Ʈ ���� �̸��� ��´�.
        public override string GetChartFileName()
        {
            return "weaponChart";
        }

        // Backend.Chart.GetChartContents���� �� ��Ʈ ���¿� �°� �Ľ��ϴ� Ŭ����
        // ��Ʈ ���� �ҷ����� �Լ��� BackendData.Base.Chart�� BackendChartDataLoad�� �������ּ���
        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item info = new Item(eachItem);

                _dictionary.Add(info.WeaponID, info);
                info.WeaponSprite = base.AddOrGetImageDictionary(_weaponImages, "Sprite/Weapon/", info.WeaponImageName);
                info.EffectSprite = base.AddOrGetImageDictionary(_effectImages, "Sprite/Effect/", info.EffectImageName);
                info.BulletSprite = base.AddOrGetImageDictionary(_bulletImages, "Sprite/Bullet/", info.BulletImageName);
            }
        }


    }
}