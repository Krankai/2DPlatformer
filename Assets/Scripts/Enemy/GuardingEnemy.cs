using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardingEnemy : EnemyBase
{
    [Tooltip("The associated laser controller")]
    public LaserController laser;

    [Tooltip("Shooting interval")]
    public float interval = 3.0f;

    // The sprite renderer associated with this enemy
    private SpriteRenderer spriteRenderer = null;

    private float timer = 0;

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
    }

    /// <summary>
    /// Description:
    /// Override of base class Update function
    /// Determines walking direction in addition to moving
    /// Input: 
    /// none
    /// Return: 
    /// void (no return)
    /// </summary>
    protected override void Update()
    {
        ShootLaserRepeatedly();
        base.Update();
    }

    private void ShootLaserRepeatedly()
    {
        if (!laser.IsShooting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                laser.ShootLaserWithAnimation();
                timer = interval;
            }
        }
    }
}
