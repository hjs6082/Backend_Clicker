using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject starPrefab;
    [SerializeField]
    private float spawnsTime;
    [SerializeField]
    private float defaultTime = 0.05f;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && spawnsTime >= defaultTime)
        {
            StarCreate();
            spawnsTime = 0;
        }
        spawnsTime += Time.deltaTime;
    }

    void StarCreate()
    {
        // 터치/클릭 위치를 ScreenToWorldPoint로 변환할 때, z축을 0으로 고정
        Vector3 mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPosition.z = 0;
        GameObject star = Instantiate(starPrefab, mPosition, Quaternion.identity);
    }

}
