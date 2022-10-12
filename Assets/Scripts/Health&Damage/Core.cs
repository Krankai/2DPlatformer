using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The health component associated with this head")]
    public Health associatedHealth;
    [Tooltip("The amount of damage to deal when come in to contact with specified targets")]
    public int damage = 1;

    /// <summary>
    /// Description:
    /// Standard Unity function that is called when a collider enters a trigger
    /// Input:
    /// Collision2D collision
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="collision">The collision information of what has collided with the attached collider</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Foreground" || collision.gameObject.tag == "Body")
        {
            associatedHealth.TakeDamage(damage);
        }
    }
}
