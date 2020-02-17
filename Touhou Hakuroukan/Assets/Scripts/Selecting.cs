using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecting : MonoBehaviour
{
    Animator animator;

    public void SelectingEffect()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("selected");
    }

}
