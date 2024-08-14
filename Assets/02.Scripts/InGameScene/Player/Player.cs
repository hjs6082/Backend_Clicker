// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGameScene
{
    //===============================================================
    // �÷��̾� ������ ���� Ŭ����
    //===============================================================
    public class Player : MonoBehaviour
    {
        public enum MoveState
        {
            None,
            MoveToAttack,
            MoveToNextStage
        }

        private Vector3 _destination; // Move �� ��������
        private MoveState _moveState = MoveState.None;

        private float _moveSpeed = 10f; // �̵��ӵ�

        // �̵� �� ����
        public delegate void PlayerAfterMove();
        private PlayerAfterMove _playerAfterMove;

        private GameObject _bulletGameObject; // �Ѿ� Prefab
        private WeaponObject _weaponObject; // ����ִ� ����

        public void Init(GameObject bulletPrefab)
        {
            Debug.Log("�Լ��� ����Ǿ����ϴ�.");
            _weaponObject = GetComponentInChildren<WeaponObject>();
            _bulletGameObject = bulletPrefab;
            SetWeapon();
        }

        // ���� ������ ������ ���ο� ���� ��ġ ����
        public void SetNewEnemy(EnemyObject newEnemy)
        {
            if(_weaponObject.enabled)
            {
                _weaponObject.SetEnemy(newEnemy);
            }
        }

        //  
        void Update()
        {
            // MoveState�� None�� �ƴ� �ٸ� ���¶�� �������� �̵�
            if (_moveState != MoveState.None)
            {
                transform.localPosition =
                    Vector3.MoveTowards(transform.localPosition, _destination, _moveSpeed * Time.deltaTime);

                // �̵��� �Ϸ�Ǿ��ٸ�
                if (transform.localPosition.Equals(_destination))
                {
                    if (_playerAfterMove != null)
                    {
                        // �̵� �Ŀ� �ൿ�� �Լ� ȣ��
                        _playerAfterMove.Invoke();
                    }

                    // ������ �� ������ �� ���, �ٽ� ���� �ǳ����� �̵�
                    if (_moveState == MoveState.MoveToNextStage)
                    {
                        //�ٽ� �����ڸ�
                        transform.localPosition = new Vector3(-4, 1.7f, 0);
                    }

                    // �̵��� �Ϸ�� ��� �� �ʱ�ȭ
                    _moveState = MoveState.None;
                    _playerAfterMove = null;
                }
            }
        }

        // �÷��̾� ������ ���� ����
        public void SetMove(MoveState state, PlayerAfterMove playerAfterMove = null)
        {
            _moveState = state;

            switch (_moveState)
            {
                case MoveState.None: // ������ ���� ���
                    break;
                case MoveState.MoveToAttack: // ������ ���� �̵��Ұ��(���� �ǳ� -> �߰�)
                    _destination = new Vector3(-4.41f, 0f, 0);
                    break;
                case MoveState.MoveToNextStage: // ���� óġ�ϰ� ���� ���������� �Ѿ ���(�߰� -> ������ ��)
                    _destination = new Vector3(-13.01f, 0f, 0);
                    break;
            }

            // �������� ����ǰ� �� �Ŀ� ȣ���� �Լ� ����
            _playerAfterMove = playerAfterMove;
        }

        // �������� ���� ����
        public void SetWeapon()
        {

            // ���� �������� ���� ������ �ҷ�����
            var weaponInventoryDic = StaticManager.Backend.GameData.WeaponInventory.Dictionary;

            _weaponObject.ReleaseGun();

            // ������ ����� �ٽ� ���� ���
            foreach (var weaponEquip in StaticManager.Backend.GameData.WeaponEquip.Dictionary)
            {
                string myWeaponId = weaponEquip.Key;
                int position = weaponEquip.Value;

                // ���Ⱑ ������ ��� �ش� ����� ����
                if (weaponInventoryDic.ContainsKey(myWeaponId))
                {
                    var weaponInventory = weaponInventoryDic[myWeaponId];
                    _weaponObject.ActiveGun(weaponInventory, _bulletGameObject);
                }
                else
                {
                    throw new Exception($"�κ��丮�� �������� �ʽ��ϴ�.\n {myWeaponId}");
                }
            }
        }
    }
}