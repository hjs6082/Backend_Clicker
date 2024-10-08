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

        private const float _moveSpeed = 5f;
        private bool isAnimationSet = false; // �ִϸ��̼� �ߺ� ȣ�� ����

        public enum EnemyState
        {
            Init,
            Normal,
            Dead
        }

        public EnemyState CurrentEnemyState { get; private set; }
        private BackendData.Chart.Enemy.Item _currentEnemyChartItem;
        private BackendData.Chart.Boss.Item _currentBossChartItem;

        public bool isBoss { get; private set; }

        void Update()
        {
            switch (CurrentEnemyState)
            {
                case EnemyState.Init:
                    InitUpdate();
                    break;
                case EnemyState.Normal:
                    NormalUpdate();
                    CheckForTouch();
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
                Debug.Log("�� �ʱ�ȭ �Ϸ�");
                Managers.Process.UpdateEnemyStatus(this);
                SetState(EnemyState.Normal); // ���� ��ȯ �� �ִϸ��̼ǵ� ����
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
                monsterAnimator.timeScale = 3f;
                isAnimationSet = true;
            }
        }

        private void CheckForTouch()
        {
            // ����� ��ġ�� ���콺 Ŭ�� ��� ó��
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CheckHit(touchPosition);
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                if (touch.phase == TouchPhase.Began)
                {
                    CheckHit(touchPosition);
                }
            }
        }

        private void CheckHit(Vector3 touchPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                // ��ġ�� ��� ü�� ����
                GameObject touchEffect = Instantiate(Resources.Load<GameObject>("Prefabs/InGameScene/TouchEffect"), hit.transform);
                Destroy(touchEffect, 0.5f);
                TakeDamage(10f); // ü�� ���ҷ��� ������ ����
                Managers.Process.AttackEffect(10f);
            }
        }

        private void TakeDamage(float damage)
        {
            Hp -= damage;
            if (Hp <= 0)
            {
                Dead();
            }
            Managers.Process.UpdateEnemyStatus(this);
        }

        public void Init(BackendData.Chart.Enemy.Item enemyInfo, float multiStat, Vector3 stayPosition)
        {
            _currentEnemyChartItem = enemyInfo;
            isBoss = false;

            Name = enemyInfo.EnemyName;
            MaxHp = enemyInfo.Hp * multiStat;
            Hp = MaxHp;
            Money = enemyInfo.Money * multiStat;
            Exp = enemyInfo.Exp * multiStat;

            _stayPosition = stayPosition;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            monsterAnimator = GetComponent<SkeletonAnimation>();

            SetState(EnemyState.Init); // �ʱ� ���� ����
            monsterAnimator.Skeleton.ScaleX *= -1;
        }

        public void InitBoss(BackendData.Chart.Boss.Item bossInfo, Vector3 stayPosition)
        {
            _currentBossChartItem = bossInfo;
            isBoss = true;

            Name = bossInfo.BossName;
            MaxHp = bossInfo.Hp;
            Hp = MaxHp;
            Money = bossInfo.Money;
            Exp = bossInfo.Exp;

            _stayPosition = stayPosition;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            monsterAnimator = GetComponent<SkeletonAnimation>();

            SetState(EnemyState.Init); // �ʱ� ���� ����
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
            isAnimationSet = false; // ���°� ����Ǹ� �ٽ� �ִϸ��̼��� ������ �� �ֵ��� ��
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

                Managers.Process.AttackEffect(damage);

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
            Destroy(gameObject, 0.5f);

/*            foreach (var bulletObject in FindObjectsOfType<BulletObject>())
            {
                Destroy(bulletObject.gameObject);
            }*/
        }

        private void SetDropItem()
        {
            if (isBoss)
            {
                foreach (var dropItem in _currentBossChartItem.DropItemList)
                {
                    double dropPercent = Math.Round((double)Random.Range(0, 100), 2);
                    if (dropItem.DropPercent > dropPercent)
                    {
                        Managers.Game.UpdateItemInventory(dropItem.ItemID, 1);
                        Managers.Process.DropItem(transform.position, dropItem.ItemID);
                    }
                }
            }
            else
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
}
