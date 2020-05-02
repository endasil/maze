using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : MonoBehaviour
{
    public int maxHealth = 100;
    
    public int hp = 300;
    public Renderer renderer;
    Color originalColor;
    public bool dead = false;
    // Start is called before the first frame update
    protected virtual void Start()
    {

        try
        {
            originalColor = renderer.material.color;
        }
        catch (Exception e)
        {
            Debug.Log("BLÄÄÄÄ");
            throw;
        }
    
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Heal(int health)
    {
        hp += health;
        if (hp > maxHealth)
        {
            hp = maxHealth;
        }
        Debug.Log($"{this.gameObject.name} Hp left: " + hp);
        StartCoroutine(FlashObject(Color.green, 0.3f, .05f));
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

    protected IEnumerator FlashObject(Color flashColor, float flashDuration, float delayBetweenFlashes)
    {

        var flashingFor = 0.0f;
        var newColor = flashColor;
        while (flashingFor < flashDuration)
        {
            renderer.material.color = newColor;
            flashingFor += Time.deltaTime;
            yield return new WaitForSeconds(delayBetweenFlashes);
            flashingFor += delayBetweenFlashes;
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
