// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InGameScene
{
    //===========================================================
    // ������ ����� �����ϴ� Ŭ����
    //===========================================================
    public class ItemManager
    {
        private GameObject _bulletPrefab;
        private List<Sprite> _randomBulletSpriteList = new();


        public void Init(GameObject bulletPrefab)
        {
            //�Ѿ� ���� Prefab ����
            _bulletPrefab = bulletPrefab;
        }

        // RedBoom ������ ����, �ܺο��� ���� ������� �Ѿ� �߻�
        private bool AttackEnemy(float atk)
        {
            try
            {
                float speed = 8.0f;

                Vector3 createPos = new Vector3(-5, 5, 0);
                var bullet = Object.Instantiate(_bulletPrefab);
                bullet.transform.position = createPos;

                // �ش� ������ �������� ���� ���
                Vector3 relativePos = Managers.Process.GetEnemy().transform.position - createPos;
                Vector3 quaternionToTarget = Quaternion.Euler(0, 0, 0) * relativePos;
                // �̹����� �������� �����Բ� ����
                Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: quaternionToTarget);


                if (_randomBulletSpriteList.Count <= 0)
                {
                    foreach (var li in StaticManager.Backend.Chart.Weapon.Dictionary.Values)
                    {
                        _randomBulletSpriteList.Add(li.BulletSprite);
                    }
                }

                int random = UnityEngine.Random.Range(0, _randomBulletSpriteList.Count);
                Sprite bulletSprite = _randomBulletSpriteList[random];

                bullet.GetComponent<BulletObject>().Shoot(bulletSprite, targetRotation, speed, atk);
            }
            catch (Exception e)
            {
                StaticManager.Backend.SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), e.ToString());
            }

            return true;
        }

        // �� ������ Ŭ�������� �ڽ��� itemId �� �ش� �Լ� ȣ��
        public bool Use(int itemId)
        {
            bool isSuccess = false;
            try
            {
                // Chart���� �ڽ��� ������ ���� �ҷ�����
                BackendData.Chart.Item.Item item = StaticManager.Backend.Chart.Item.Dictionary[itemId];

                // �� �̸����� �����Ͽ� �˸´� �ൿ ����
                if (item.ItemType.Equals("RedPotion"))
                {
                    float atk = item.ItemStat["Atk"];
                    float time = item.ItemStat["Time"];
                    isSuccess = Managers.Buff.StartBuff(Buff.BuffStatType.Atk, atk, time,
                        Buff.BuffAdditionType.Plus);
                }
                else if (item.ItemType.Equals("BluePotion"))
                {
                    float delay = item.ItemStat["Delay"];
                    float time = item.ItemStat["Time"];
                    isSuccess = Managers.Buff.StartBuff(Buff.BuffStatType.Delay, delay, time,
                        Buff.BuffAdditionType.Multi);
                }
                else if (item.ItemType.Equals("RedBoom"))
                {
                    float atk = item.ItemStat["Atk"];
                    isSuccess = AttackEnemy(atk);
                }
                else if (item.ItemType.Equals("GoldenTime"))
                {
                    float multi = item.ItemStat["Multi"];
                    float time = item.ItemStat["Time"];
                    isSuccess = Managers.Buff.StartBuff(Buff.BuffStatType.Gold, multi, time,
                        Buff.BuffAdditionType.Multi);
                }
                else if (item.ItemType.Equals("StudyTime"))
                {
                    float multi = item.ItemStat["Multi"];
                    float time = item.ItemStat["Time"];
                    isSuccess = Managers.Buff.StartBuff(Buff.BuffStatType.Exp, multi, time,
                        Buff.BuffAdditionType.Multi);
                }
                else
                {
                    throw new Exception("��ϵ��� ���� �������� ����Ͽ����ϴ�.");
                }

                if (isSuccess)
                {
                    InGameScene.Managers.Game.UpdateItemInventory(itemId, -1);
                }
                return isSuccess;
            }
            catch (Exception e)
            {
                StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), e.ToString());
                return false;
            }
        }
    }
}