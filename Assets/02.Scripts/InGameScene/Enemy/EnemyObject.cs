// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using BackendData.Chart.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameScene
{
    //===============================================================
    // ���ӿ��� �����ִ� �� ���� Ŭ����
    //===============================================================
    public class EnemyObject : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private Vector3 _stayPosition;

        public string Name { get; private set; }
        public float MaxHp { get; private set; }

        public float Hp { get; private set; }
        public float Money { get; private set; }
        public float Exp { get; private set; }

        private const float _moveSpeed = 3f;

        // ���� ����
        public enum EnemyState
        {
            Init,
            Normal,
            Dead
        }

        public EnemyState CurrentEnemyState { get; private set; }
        private BackendData.Chart.Enemy.Item _currentEnemyChartItem; // ���� ��Ʈ ����

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
        //�� ���� ���� ������ ��ǥ�� �ö����� ȣ��Ǵ� �Լ�.
        void InitUpdate()
        {
            transform.localPosition =
                Vector3.MoveTowards(transform.localPosition, _stayPosition, _moveSpeed * Time.deltaTime);

            if (transform.localPosition.Equals(_stayPosition))
            {
                Debug.Log("�� �ʱ�ȭ �Ϸ�");

                //Normal�� �Ǳ� ���� ȣ���Ͽ����մϴ�.
                Managers.Process.UpdateEnemyStatus(this);
                CurrentEnemyState = EnemyState.Normal;
            }
        }
        //������ ��ǥ�� ���� ���������� ȣ��Ǵ� �Լ�.
        void NormalUpdate()
        {
            transform.localPosition =
                Vector3.MoveTowards(transform.localPosition, _stayPosition, _moveSpeed * Time.deltaTime);
        }

        //�װ��� ���󰡴� �Լ�
        void DeadUpdate()
        {
            transform.Rotate(Vector3.back * (100f * Time.deltaTime));
        }

        //���� ������ �ʱ�ȭ�ϴ� �Լ�
        public void Init(BackendData.Chart.Enemy.Item enemyInfo, float multiStat, Vector3 stayPosition)
        {
            _currentEnemyChartItem = enemyInfo;

            Name = enemyInfo.EnemyName;
            MaxHp = enemyInfo.Hp * multiStat;
            Hp = MaxHp;
            Money = enemyInfo.Money * multiStat;
            Exp = enemyInfo.Exp * multiStat;

            gameObject.GetComponent<SpriteRenderer>().sprite = _currentEnemyChartItem.EnemySprite;

            _stayPosition = stayPosition;
            CurrentEnemyState = EnemyState.Init;
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (CurrentEnemyState != EnemyState.Normal)
            {
                return;
            }

            // ���� ȭ�� �� ȸ�� �ڽ��� �ε����� �Ҹ�
            if (col.transform.CompareTag("BulletDestroyer"))
            {
                Dead();
                Destroy(gameObject);
            }

            // �Ѿ˿� ���� ���
            if (col.transform.CompareTag("Bullet"))
            {
                float damage = col.gameObject.GetComponent<BulletObject>().GetDamage();
                Hp -= damage;

                // Hp�� 0�� �� ���
                if (Hp <= 0)
                {
                    Dead();
                }

                // ���� ������ ���� �ڽ��� hp ������ ������Ʈ
                Managers.Process.UpdateEnemyStatus(this);
            }
        }

        // �׾����� ȣ��Ǵ� �Լ�
        private void Dead()
        {
            CurrentEnemyState = EnemyState.Dead;

            SetDropItem();

            // ������ ���� ����
            _rigidbody2D.AddForce(new Vector2(200, 200), ForceMode2D.Force);
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;

            Destroy(gameObject, 5);
        }

        // ���� ��� ���� Ȯ���� �������� ����Ʈ���� �Լ�
        private void SetDropItem()
        {
            foreach (var dropItem in _currentEnemyChartItem.DropItemList)
            {
                double dropPercent = Math.Round((double)Random.Range(0, 100), 2);
                // Ȯ���� 50%�� ���, 100�߿� 50�̸��� ������ �ȴ�
                // Ȯ���� 10%�� ���, 100�߿� 1,2,3,4,5,6,7,8,9�� ���;��Ѵ�
                // Ȯ���� 1%�� ���, 100�߿�, 0.9 �̸��� ���� ���;��Ѵ�.
                if (dropItem.DropPercent > dropPercent)
                {
                    Managers.Game.UpdateItemInventory(dropItem.ItemID, 1);
                    Managers.Process.DropItem(transform.position, dropItem.ItemID);
                }
            }
        }
    }
}