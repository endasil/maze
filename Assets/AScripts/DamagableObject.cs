using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int hp = 300;

    [Header("Audio")]
    public List<AudioClip> hitSounds;
    [SerializeField]
    public AudioClip deathSound;
    
    [Header("Init Configs")]
    protected float nextHitSoundTime = 1.0f;
    protected float hitSoundRepeatDelay = 1;
    private List<Color> originalColors = new List<Color>();
    [SerializeField]
    protected bool dead = false;

    protected List<Renderer> rendererList = new List<Renderer>();
    protected void GetRenderers()
    {
        foreach (Renderer objectRenderer in gameObject.GetComponentsInChildren<Renderer>())
        {

            if (objectRenderer.material.HasProperty("_Color"))
            {
                rendererList.Add(objectRenderer);
            }
            else
            {
                Debug.Log($"No renderer on object {objectRenderer.name}");
            }
        }
    }

    protected virtual void Awake()
    {
        GetRenderers();
        nextHitSoundTime = Time.time + hitSoundRepeatDelay;
        foreach (var r in rendererList)
        {
            originalColors.Add(r.material.color);
        }
    }

    public virtual bool Heal(int health)
    {
        if (hp >= maxHealth)
        {
            return false;
        }
        hp += health;
        if (hp > maxHealth)
        {
            hp = maxHealth;
        }
        Debug.Log($"{this.gameObject.name} Hp left: " + hp);
        StartCoroutine(FlashObject(Color.green, 0.3f, .05f));
        return true;
    }
    public virtual void TakeDamage(int damage)
    {
        if (dead)
            return;

        hp -= damage;
        StartCoroutine(FlashObject(Color.red, 0.5f, .07f));
        if (hp <= 0)
        {
            hp = 0;
            dead = true;
            Die();
        }
        else if (nextHitSoundTime < Time.time && hitSounds.Count > 0)
        {
            nextHitSoundTime = Time.time + hitSoundRepeatDelay;
            var soundToPlay = Random.Range(0, hitSounds.Count);
            AudioSource.PlayClipAtPoint(hitSounds[soundToPlay], transform.position);
        }
    }

    protected IEnumerator FlashObject(Color flashColor, float totalTimeToKeepFlashingBetweenColors, float delayBetweenColorChanges)
    {
        var flashingFor = 0.0f;
        var setFToFlashColor = true;
        while (flashingFor < totalTimeToKeepFlashingBetweenColors)
        {
            for (int i = 0; i < rendererList.Count; i++)
            {
                rendererList[i].material.color = setFToFlashColor ? flashColor : originalColors[i];
            }

            flashingFor += Time.deltaTime;
            yield return new WaitForSeconds(delayBetweenColorChanges);
            flashingFor += delayBetweenColorChanges;
            setFToFlashColor = !setFToFlashColor;


        }
        
        // Set back to original color after flashing is done
        for (int i = 0; i < rendererList.Count; i++)
        {
            rendererList[i].material.color = originalColors[i];

        }
    }


    public virtual void Die()
    {
        Destroy(gameObject);
    }

}
