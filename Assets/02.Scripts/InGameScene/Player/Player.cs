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

        private Vector3 _destination; // Move 시 도착지점
        private MoveState _moveState = MoveState.None;

        private float _moveSpeed = 10f; // 이동속도

        public delegate void PlayerAfterMove();
        private PlayerAfterMove _playerAfterMove;

        private GameObject _bulletGameObject; // 총알 Prefab

        private EnemyObject _enemy; // 지정중인 적
        private Transform _gunSpriteTransform; // 무기의 이미지 Transform
        private float _reloadingTime = 3; // 리로딩 시간
        private GameObject _bulletObject; // 발사할 총알 객체
        private float _currentTime = 0; // 다음 총알까지 시간
        private Sprite _bulletImageSprite;
        private BackendData.GameData.WeaponInventory.Item _weaponData;
        private bool _isFireArrow = true; // 화살 발사 여부

        public void Init(GameObject bulletPrefab)
        {
            Debug.Log("함수가 실행되었습니다.");
            _bulletGameObject = bulletPrefab;
            _gunSpriteTransform = gameObject.transform.GetChild(1).transform;
            _gearEquipper = this.gameObject.GetComponent<GearEquipper>();
            _playerAnimator = GetComponentInChildren<SkeletonAnimation>();
            _playerAnimator.AnimationState.Event += OnEventAnimation; // 애니메이션 이벤트 구독
            SetWeapon();
        }

        public void SetNewEnemy(EnemyObject newEnemy)
        {
            _enemy = newEnemy;
        }

        void Update()
        {
            // 이동 상태에 따른 처리
            if (_moveState != MoveState.None)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, _destination, _moveSpeed * Time.deltaTime);

                if (transform.localPosition.Equals(_destination))
                {
                    _playerAfterMove?.Invoke();

                    if (_moveState == MoveState.MoveToNextStage)
                    {
                        transform.localPosition = new Vector3(-4, 1.7f, 0);
                    }

                    _moveState = MoveState.None;
                    SetAnimation("Idle ARCHER");
                    _playerAfterMove = null;
                }
            }

            // 적이 지정되어 있을 경우 무기를 적 방향으로 회전시키고, 클릭 시 발사
            if (_enemy != null)
            {
                RotateToEnemyUpdate();

                // 주석 처리된 총알 발사 타이밍 계산
                ShootUpdate();
            }
            else
            {
                SetAnimation("Idle ARCHER");
            }
        }

        // 애니메이션 이벤트 발생 시 호출되는 함수
        void OnEventAnimation(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == "OnArrowLeftBow" && _isFireArrow)
            {
                Shoot(); // 애니메이션 이벤트 발생 시 화살 발사
            }
        }

        // 주석 처리된 총알 발사 타이밍 계산
        void ShootUpdate()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > _reloadingTime)
            {
                SetAnimation("Shoot1"); // 애니메이션 재생
                _currentTime = 0;
            }
        }

        void Shoot()
        {
            float speed = _weaponData.GetCurrentWeaponStat().Spd;
            float normalAtk = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Atk, _weaponData.GetCurrentWeaponStat().Atk);
            _reloadingTime = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Delay, _weaponData.GetCurrentWeaponStat().Delay);

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
                    SetAnimation("Run");
                    _destination = new Vector3(-4.41f, 0f, 0);
                    break;
                case MoveState.MoveToNextStage:
                    SetAnimation("Run");
                    _destination = new Vector3(-13.01f, 0f, 0);
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
                }
                else
                {
                    throw new Exception($"인벤토리에 존재하지 않습니다.\n {myWeaponId}");
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

        private void SetAnimation(string animationName)
        {
            if (_playerAnimator == null)
                return;

            bool isLoop = animationName != "Dead";
            _playerAnimator.AnimationState.SetAnimation(0, animationName, isLoop);
        }
    }
}
