using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
    public int health;
    public float speed;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private void LateUpdate()
    {
        animator.SetBool("IsDead", false);
    }

    IEnumerator Die()
    {
        animator.SetBool("IsDead", true);
        float elapsed = 0f;
        float duration = AnimationLength("Crab_Death");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return 0;
        }
        Destroy(gameObject);
    }

    float AnimationLength(string name)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == name)
                time = ac.animationClips[i].length;

        return time;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(damage + " damage taken.");
    }
}
