using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mahoubakudan : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem particle;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        particle = GetComponent<ParticleSystem>();
        StartCoroutine("Explode");
    }

    IEnumerator Explode()
    {
        //先等动画放完
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;

        //BOOM！
        particle.Play();

        //再等炸完
        while (particle.IsAlive())
            yield return null;

        //销毁炸弹
        Destroy(gameObject);
    }

    public void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<Player>() != null)
        {
            other.GetComponent<Player>().hitbox.Miss();
        }
    }

}
