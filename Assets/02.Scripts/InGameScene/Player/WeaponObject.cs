// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene
{
    //===============================================================
    // �յ� ���ٴϴ� �� ���� Ŭ����
    //===============================================================
    public class WeaponObject : MonoBehaviour
    {

        private EnemyObject _enemy; // �������� ��

        private Transform _gunSpriteTransform; // ������ �̹��� Transform

        private float _reloadingTime = 3; // ���ε� �ð�

        private GameObject _bulletObject; // �߻��� �Ѿ� ��ü

        private float _currentTime = 0; // ���� �Ѿ˱��� �ð�

        private Sprite _bulletImageSprite;
        // Start is called before the first frame update

        private BackendData.GameData.WeaponInventory.Item _weaponData;

        // ���� ���� �����͸� �̿��Ͽ� ���� ������ ����
        public void ActiveGun(BackendData.GameData.WeaponInventory.Item weaponInventoryData, GameObject bulletObject)
        {
            _weaponData = weaponInventoryData;

            if (_gunSpriteTransform == null)
            {
                _gunSpriteTransform = gameObject.GetComponentInChildren<SpriteRenderer>().transform;
            }

            _gunSpriteTransform.GetComponent<SpriteRenderer>().sprite =
                weaponInventoryData.GetWeaponChartData().WeaponSprite;

            _bulletImageSprite = weaponInventoryData.GetWeaponChartData().BulletSprite;

            gameObject.SetActive(true);
            _bulletObject = bulletObject;
        }

        // ���� ���� �� ��Ȱ��ȭ
        public void ReleaseGun()
        {
            if (_gunSpriteTransform == null)
            {
                _gunSpriteTransform = gameObject.GetComponentInChildren<SpriteRenderer>().transform;
            }

            _gunSpriteTransform.GetComponent<SpriteRenderer>().sprite = null;

            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (_enemy == null)
            {
                return;
            }

            RotateToEnemyUpdate();
            if(Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            //ShootUpdate();

        }

        // ����� ���� ������ ����Ͽ� �ܴ��� �Լ�
        void RotateToEnemyUpdate()
        {
            Vector3 relativePos = _enemy.transform.position - _gunSpriteTransform.position;
            Vector3 quaternionToTarget = Quaternion.Euler(0, 0, 0) * relativePos;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0, 0, 1), upwards: quaternionToTarget);
            _gunSpriteTransform.rotation = targetRotation;
        }

        //��ġ�� ���� �߻��ϹǷ� �ش��Լ� �ּ�ó�� 


/*        // ���� ���ε����� �ð��� ����ϴ� �Լ�
        // 
        void ShootUpdate()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > _reloadingTime)
            {
                Shoot();
                _currentTime = 0;
            }
        }*/

        // ���⸦ �ܴ� ���� �����ϴ� �Լ�
        public void SetEnemy(EnemyObject enemyItem)
        {
            _enemy = enemyItem;
        }

        // ���� �߻��ϴ� �Լ�
        void Shoot()
        {

            //�Ѿ��� ���ǵ�
            float speed = _weaponData.GetCurrentWeaponStat().Spd;

            // �������� ��ŭ ���ݷ� ��ȭ
            float normalAtk = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Atk, _weaponData.GetCurrentWeaponStat().Atk);

            // ���ε� Ÿ�� ���� ������ŭ ���
            _reloadingTime = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Delay, _weaponData.GetCurrentWeaponStat().Delay);

            // �Ѿ� �߻�
            var bullet = Instantiate(_bulletObject);
            bullet.GetComponent<BulletObject>().Shoot(_gunSpriteTransform.rotation, speed, normalAtk);
            bullet.transform.position = _gunSpriteTransform.position;
        }
    }
}
