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
            // 파티클 시스템 컴포넌트 가져오기
            _particleSystem = this.gameObject.GetComponent<ParticleSystem>();
        }

        // 적에게 닿을 시 주는 데미지 지정
        public void Shoot(float atk)
        {
            _atk = atk;
            _particleSystem.Play();  // 파티클 발사
        }

        // 내 데미지
        public float GetDamage()
        {
            return _atk;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.transform.CompareTag("BulletDestroyer") || col.transform.CompareTag("Enemy"))
            {
                //_particleSystem.Stop();  // 적중 시 파티클 멈추기
                Destroy(gameObject, 0.5f);  // 0.5초 후 오브젝트 삭제
            }
        }
    }
}
