// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace InGameScene
{
    //===============================================================
    // �Ѿ� �߻� ���� Ŭ����
    //===============================================================
    public class BulletObject : MonoBehaviour
    {
        private float _speed = 0;
        private float _atk = 0;

        // ����� ���ǵ�, ������ ���� �� �ִ� ������ ����(������ ������)
        public void Shoot(Sprite bulletSprite, Quaternion destinationTransform, float speed, float atk)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = bulletSprite;
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            transform.rotation = destinationTransform;
            _speed = speed;
            _atk = atk;
        }

        // rotation�� ����ä�� ������ �̵�
        void Update()
        {
            transform.Translate(_speed * Vector3.up * Time.deltaTime);
        }

        // �� ������
        public float GetDamage()
        {
            return _atk;
        }

        // �Ѿ��� ���� ��� �ش� �Ѿ� ��ü �ı�
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.transform.CompareTag("BulletDestroyer"))
            {
                Destroy(this.gameObject);
            }

            if (col.transform.CompareTag("Enemy"))
            {
                Destroy(this.gameObject);
            }
        }
    }
}