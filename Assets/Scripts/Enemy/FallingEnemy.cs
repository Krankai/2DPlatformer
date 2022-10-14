using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingEnemy : EnemyBase
{
    // The sprite renderer associated with this enemy
    private SpriteRenderer spriteRenderer = null;

    // The rigid body 2D associated with this enemy
    private Rigidbody2D rgb2D = null;

    /// <summary>
    /// Description:
    /// Turn on gravity scale so that this enemy starts falling (by setting scale to 1.0f)
    /// Input: 
    /// none
    /// Return: 
    /// void (no return)
    /// </summary>
    public void FreeFall()
    {
        rgb2D.gravityScale = 1.0f;
    }

    /// <summary>
    /// Description:
    /// Sets up this script (it's reference to a waypoint mover)
    /// Input: 
    /// none
    /// Return: 
    /// void (no return)
    /// </summary>
    protected override void Setup()
    {
        base.Setup();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rgb2D = GetComponent<Rigidbody2D>();
    }
}
