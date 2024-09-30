using System;
using BackendData.Chart.Enemy;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameScene
{
    public class EnemyObject : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private Vector3 _stayPosition;
        private SkeletonAnimation monsterAnimator;

        public string Name { get; private set; }
        public float MaxHp { get; private set; }
        public float Hp { get; private set; }
        public float Money { get; private set; }
        public float Exp { get; private set; }

        private const float _moveSpeed = 2f;
        private bool isAnimationSet = false; // 애니메이션 중복 호출 방지

        public enum EnemyState
        {
            Init,
            Normal,
            Dead
        }

        public EnemyState CurrentEnemyState { get; private set; }
        private BackendData.Chart.Enemy.Item _currentEnemyChartItem;

        void Update()
        {
            switch (CurrentEnemyState)
            {
                case EnemyState.Init:
                    InitUpdate();
                    break;
                case EnemyState.Normal:
                    NormalUpdate();
                    break;
                case EnemyState.Dead:
                    DeadUpdate();
                    break;
            }
        }

        void InitUpdate()
        {
            if (!isAnimationSet)
            {
                SetAnimation("Walk");
                isAnimationSet = true;
            }

            transform.localPosition =
                Vector3.MoveTowards(transform.localPosition, _stayPosition, _moveSpeed * Time.deltaTime);

            if (transform.localPosition.Equals(_stayPosition))
            {
                Debug.Log("적 초기화 완료");
                Managers.Process.UpdateEnemyStatus(this);
                SetState(EnemyState.Normal); // 상태 전환 시 애니메이션도 변경
            }
        }

        void NormalUpdate()
        {
            if (!isAnimationSet)
            {
                SetAnimation("Idle");
                isAnimationSet = true;
            }
        }

        void DeadUpdate()
        {
            if (!isAnimationSet)
            {
                SetAnimation("Dead");
                isAnimationSet = true;
            }
        }

        public void Init(BackendData.Chart.Enemy.Item enemyInfo, float multiStat, Vector3 stayPosition)
        {
            _currentEnemyChartItem = enemyInfo;

            Name = enemyInfo.EnemyName;
            MaxHp = enemyInfo.Hp * multiStat;
            Hp = MaxHp;
            Money = enemyInfo.Money * multiStat;
            Exp = enemyInfo.Exp * multiStat;

            _stayPosition = stayPosition;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            monsterAnimator = GetComponent<SkeletonAnimation>();

            SetState(EnemyState.Init); // 초기 상태 설정
            monsterAnimator.Skeleton.ScaleX *= -1;
        }

        private void SetAnimation(string AnimationName)
        {
            if (monsterAnimator == null)
                return;

            bool IsLoop = AnimationName != "Dead";
            monsterAnimator.AnimationState.SetAnimation(0, AnimationName, IsLoop);
        }

        private void SetState(EnemyState newState)
        {
            CurrentEnemyState = newState;
            isAnimationSet = false; // 상태가 변경되면 다시 애니메이션을 설정할 수 있도록 함
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (CurrentEnemyState != EnemyState.Normal)
                return;

            if (col.transform.CompareTag("BulletDestroyer"))
            {
                Dead();
                Destroy(gameObject);
            }

            if (col.transform.CompareTag("Bullet"))
            {
                float damage = col.gameObject.GetComponent<BulletObject>().GetDamage();

                Hp -= damage;

                if (Hp <= 0)
                {
                    Dead();
                }

                Managers.Process.UpdateEnemyStatus(this);
            }
        }

        private void Dead()
        {
            SetState(EnemyState.Dead);
            SetDropItem();
            Destroy(gameObject, 1);

            foreach (var bulletObject in FindObjectsOfType<BulletObject>())
            {
                Destroy(bulletObject.gameObject);
            }
        }

        private void SetDropItem()
        {
            foreach (var dropItem in _currentEnemyChartItem.DropItemList)
            {
                double dropPercent = Math.Round((double)Random.Range(0, 100), 2);
                if (dropItem.DropPercent > dropPercent)
                {
                    Managers.Game.UpdateItemInventory(dropItem.ItemID, 1);
                    Managers.Process.DropItem(transform.position, dropItem.ItemID);
                }
            }
        }
    }
}
