using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = this.gameObject.GetComponent<ParticleSystem>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"Effect : {other.name}");
    }
}
