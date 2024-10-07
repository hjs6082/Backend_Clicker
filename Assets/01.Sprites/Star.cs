using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    SpriteRenderer sprite;
    Vector2 direction;
    [SerializeField]
    private float moveSpeed = 0.1f;
    [SerializeField]
    private float minSize = 0.1f;
    [SerializeField]
    private float maxSize = 0.3f;
    [SerializeField]
    private float sizeSpeed = 1;
    [SerializeField]
    private Color[] colors;
    [SerializeField]
    private float colorSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        float size = Random.Range(minSize, maxSize);
        transform.localScale = new Vector2(size, size);
        sprite.color = colors[Random.Range(0, colors.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(direction * moveSpeed);
        transform.localScale = Vector2.Lerp(transform.localScale, Vector2.zero, Time.deltaTime * sizeSpeed);

        Color color = sprite.color;
        color.a = Mathf.Lerp(sprite.color.a, 0, Time.deltaTime * colorSpeed);
        sprite.color = color;

        if (sprite.color.a <= 0.01f)
        {
            Destroy(gameObject);
        }
    }
}
