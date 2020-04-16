using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : MonoBehaviour
{
    public int hp = 300;
    public Renderer renderer;
    Color originalColor;
    public bool dead = false;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        originalColor = renderer.material.color;
        Debug.Log($"{this.name} original color" + originalColor);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void TakeDamage(int damage)
    {
        
        if(dead)
            return;
        
        hp -= damage;
        Debug.Log($"{this.gameObject.name} Hp left: " + hp);
        StartCoroutine(FlashObject(Color.red, 0.5f, .07f));
        if (hp <= 0)
        {
            dead = true;
            Die();
        }

    }

    protected IEnumerator FlashObject(Color flashColor, float flashTime, float flashSpeed)
    {

        var flashingFor = 0.0f;
        var newColor = flashColor;
        while (flashingFor < flashTime)
        {
            renderer.material.color = newColor;
            flashingFor += Time.deltaTime;
            yield return new WaitForSeconds(flashSpeed);
            flashingFor += flashSpeed;
            if (newColor == flashColor)
            {
                newColor = originalColor;
            }
            else
            {
                newColor = flashColor;
            }
        }

        renderer.material.color = originalColor;
    }


    public virtual void Die()
    {
        Destroy(gameObject);
    }

}
