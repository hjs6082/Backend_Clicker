using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGameScene
{
    public class Player : MonoBehaviour
    {
        private GearEquipper _gearEquipper;
        private SkeletonAnimation _playerAnimator;

        public enum MoveState
        {
            None,
            MoveToAttack,
            MoveToNextStage
        }

        private Vector3 _destination; // Move �� ��������
        private MoveState _moveState = MoveState.None;

        private float _moveSpeed = 10f; // �̵��ӵ�

        public delegate void PlayerAfterMove();
        private PlayerAfterMove _playerAfterMove;

        private GameObject _bulletGameObject; // �Ѿ� Prefab

        private EnemyObject _enemy; // �������� ��
        private Transform _gunSpriteTransform; // ������ �̹��� Transform
        [SerializeField]
        private float _reloadingTime = 3; // ���ε� �ð�
        private float _shootTimeScale = 1f;
        private GameObject _bulletObject; // �߻��� �Ѿ� ��ü
        private float _currentTime = 0; // ���� �Ѿ˱��� �ð�
        private Sprite _bulletImageSprite;
        private BackendData.GameData.WeaponInventory.Item _weaponData;
        private bool _isFireArrow = true; // ȭ�� �߻� ����

        public void Init(GameObject bulletPrefab)
        {
            Debug.Log("�Լ��� ����Ǿ����ϴ�.");
            _bulletGameObject = bulletPrefab;
            _gunSpriteTransform = gameObject.transform.GetChild(1).transform;
            _gearEquipper = this.gameObject.GetComponent<GearEquipper>();
            _playerAnimator = GetComponentInChildren<SkeletonAnimation>();
            _playerAnimator.AnimationState.Event += OnEventAnimation; // �ִϸ��̼� �̺�Ʈ ����
            SetWeapon();
        }

        public void SetNewEnemy(EnemyObject newEnemy)
        {
            _enemy = newEnemy;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.K))
                Managers.Game.UpdateItemInventory(2, 1);
            // �̵� ���¿� ���� ó��
            if (_moveState != MoveState.None)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, _destination, _moveSpeed * Time.deltaTime);

                // �̵� ���� �� �ִϸ��̼� ����
                if (_moveState == MoveState.MoveToAttack)
                {
                    SetAnimation("Run ARCHER");
                }
                else if (_moveState == MoveState.MoveToNextStage)
                {
                    SetAnimation("Run ARCHER"); // �ٸ� �̵� �ִϸ��̼� ����
                }

                if (transform.localPosition.Equals(_destination))
                {
                    _playerAfterMove?.Invoke();

                    if (_moveState == MoveState.MoveToNextStage)
                    {
                        transform.localPosition = new Vector3(-15.25f, -0.98f, 0);
                    }

                    _moveState = MoveState.None;

                    // ���� �� �ִϸ��̼��� Idle�� ����
                    SetAnimation("Idle ARCHER");
                    _playerAfterMove = null;
                }
            }
            else
            {
                // ���� �����Ǿ� ���� ��� ���⸦ �� �������� ȸ����Ű��, Ŭ�� �� �߻�
                if (_enemy != null)
                {
                    RotateToEnemyUpdate();
                    ShootUpdate();
                }
                else
                {
                    // ���� ���� �̵� ���°� None�� ���� Idle �ִϸ��̼� ����
                    SetAnimation("Idle ARCHER");
                }
            }
        }


        // �ִϸ��̼� �̺�Ʈ �߻� �� ȣ��Ǵ� �Լ�
        void OnEventAnimation(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == "OnArrowLeftBow" && _isFireArrow)
            {
                Shoot(); // �ִϸ��̼� �̺�Ʈ �߻� �� ȭ�� �߻�
            }
        }

        // �ּ� ó���� �Ѿ� �߻� Ÿ�̹� ���
        void ShootUpdate()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > _reloadingTime)
            {
                SetAnimation("Shoot1"); // �ִϸ��̼� ���
                _currentTime = 0;
            }
        }

        void Shoot()
        {
            float speed = _weaponData.GetCurrentWeaponStat().Spd;
            float normalAtk = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Atk, _weaponData.GetCurrentWeaponStat().Atk);

            // ���ε� �ð� ����
            _reloadingTime = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Delay, _weaponData.GetCurrentWeaponStat().Delay);

            if (Managers.Buff.IsBuffActive(Buff.BuffStatType.Delay))
            {
                _reloadingTime = 0.2f;
            }

            // Shoot1�� Duration�� �°� timeScale ����
            float shootDuration = 1.733f; // �ִϸ��̼� Duration
            _shootTimeScale = (shootDuration / _reloadingTime) * 1.8f; // timeScale ���

            // timeScale�� 1���� �۾����� �ʵ��� ����
            if (_shootTimeScale < 1f)
            {
                _shootTimeScale = 1f; // �ּ� 1�� ����
            }
            _playerAnimator.timeScale = _shootTimeScale;

            var bullet = Instantiate(_bulletObject);
            bullet.GetComponent<BulletObject>().Shoot(_gunSpriteTransform.rotation, speed, normalAtk);
            bullet.transform.position = _gunSpriteTransform.position;
        }

        public void SetMove(MoveState state, PlayerAfterMove playerAfterMove = null)
        {
            _moveState = state;

            switch (_moveState)
            {
                case MoveState.None:
                    SetAnimation("Idle ARCHER");
                    break;
                case MoveState.MoveToAttack:
                    SetAnimation("Run ARCHER");
                    _destination = new Vector3(-4.26f, -0.98f, 0);
                    break;
                case MoveState.MoveToNextStage:
                    SetAnimation("Run ARCHER");
                    _destination = new Vector3(14.22f, 0.98f, 0);
                    break;
            }

            _playerAfterMove = playerAfterMove;
        }

        public void SetWeapon()
        {
            var weaponInventoryDic = StaticManager.Backend.GameData.WeaponInventory.Dictionary;

            ReleaseGun();

            foreach (var weaponEquip in StaticManager.Backend.GameData.WeaponEquip.Dictionary)
            {
                string myWeaponId = weaponEquip.Key;
                int position = weaponEquip.Value;

                if (weaponInventoryDic.ContainsKey(myWeaponId))
                {
                    var weaponInventory = weaponInventoryDic[myWeaponId];
                    ActiveGun(weaponInventory, _bulletGameObject);
                    _gearEquipper.Bow = weaponInventory.GetWeaponChartData().WeaponID - 1;
                    _gearEquipper.ApplySkinChanges();
                }
                else
                {
                    throw new Exception($"�κ��丮�� �������� �ʽ��ϴ�.\n {myWeaponId}");
                }
            }
        }

        public void ActiveGun(BackendData.GameData.WeaponInventory.Item weaponInventoryData, GameObject bulletObject)
        {
            _weaponData = weaponInventoryData;

            gameObject.SetActive(true);
            _bulletObject = bulletObject;
        }

        public void ReleaseGun()
        {
            gameObject.SetActive(false);
        }

        void RotateToEnemyUpdate()
        {
            Vector3 relativePos = _enemy.transform.position - _gunSpriteTransform.position;
            Vector3 quaternionToTarget = Quaternion.Euler(0, 0, 0) * relativePos;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0, 0, 1), upwards: quaternionToTarget);
            _gunSpriteTransform.rotation = targetRotation;
        }


        // �ִϸ��̼� ���� �� Shoot1 �ִϸ��̼ǿ� ���� timeScale�� ����
        private void SetAnimation(string animationName)
        {
            if (_playerAnimator == null)
                return;

            bool isLoop = animationName != "Dead";

            if (animationName == "Shoot1")
            {
                _playerAnimator.timeScale = _shootTimeScale; // Shoot1 �� timeScale ����
            }
            else
            {
                _playerAnimator.timeScale = 1f; // �� �� �ִϸ��̼��� �⺻ �ӵ�
            }

            _playerAnimator.AnimationState.SetAnimation(0, animationName, isLoop);
        }
    }
}
