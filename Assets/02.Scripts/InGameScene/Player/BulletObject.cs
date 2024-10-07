using UnityEngine;

namespace InGameScene
{
    public class BulletObject : MonoBehaviour
    {
        private float _atk = 0;
        [SerializeField]
        private ParticleSystem _particleSystem;

        void Start()
        {
            // ��ƼŬ �ý��� ������Ʈ ��������
            _particleSystem = this.gameObject.GetComponent<ParticleSystem>();
        }

        // ������ ���� �� �ִ� ������ ����
        public void Shoot(float atk)
        {
            _atk = atk;
            _particleSystem.Play();  // ��ƼŬ �߻�
        }

        // �� ������
        public float GetDamage()
        {
            return _atk;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.transform.CompareTag("BulletDestroyer") || col.transform.CompareTag("Enemy"))
            {
                //_particleSystem.Stop();  // ���� �� ��ƼŬ ���߱�
                Destroy(gameObject, 0.5f);  // 0.5�� �� ������Ʈ ����
            }
        }
    }
}
