using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right }

    [Header("Laser Parts")]
    public GameObject laserStartPrefab;
    public GameObject laserMiddlePrefab;
    public GameObject laserEndPrefab;

    [Header("Settings")]
    [Tooltip("Maximum length of laser")]
    public float maxLaserLength = 100f;

    [Tooltip("Direction along which the laser is shot")]
    public Direction shootDirection = Direction.Right;

    private GameObject laserStartObject;
    private GameObject laserMiddleObject;
    private GameObject laserEndObject;

    private float laserEndOffset = 0.037f;

    [ContextMenu("Shoot coroutine")]
    private void ShootLaserWithAnimation()
    {
        ClearData();

        // Setup laser sprites
        if (laserStartObject == null)
        {
            laserStartObject = Instantiate(laserStartPrefab, this.transform.position, laserStartPrefab.transform.rotation, this.transform);
        }

        if (laserMiddleObject == null)
        {
            laserMiddleObject = Instantiate(laserMiddlePrefab, this.transform.position, laserMiddlePrefab.transform.rotation, this.transform);
        }

        // Determine and rotate the sprites into shooting direction
        Vector2 laserDirection = Vector2.right;
        switch (shootDirection)
        {
            case Direction.Up:
                laserDirection = Vector2.up;
                break;
            case Direction.Down:
                laserDirection = -Vector2.up;
                break;
            case Direction.Left:
                laserDirection = -Vector2.right;
                break;
            case Direction.Right:
            default:
                break;
        }

        // Raycast along the shooting direction to get the nearest collider
        LayerMask mask = LayerMask.GetMask("Foreground");
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, laserDirection, maxLaserLength, mask);

        float laserDistance = maxLaserLength;
        if (hit.collider != null)
        {
            // === Hit something
            laserDistance = Vector2.Distance(hit.point, this.transform.position);
            if (laserDistance > maxLaserLength)
            {
                laserDistance = maxLaserLength;
            }

            // End section
            if (laserEndObject == null)
            {
                laserEndObject = Instantiate(laserEndPrefab, this.transform.position, laserEndPrefab.transform.rotation, this.transform);
                laserEndObject.SetActive(false);
            }
        }
        else
        {
            // === Hit nothing
            if (laserEndObject != null)
            {
                Destroy(laserEndObject);
            }
        }

        // Rotate following shooting direction
        float angle = Vector2.SignedAngle(this.transform.right, laserDirection);
        this.transform.Rotate(this.transform.forward, angle);

        // Animate laser shooting and retracting process
        StartCoroutine(AnimateLaser(laserDistance, 3.0f, laserEndObject != null));
    }

    private IEnumerator AnimateLaser(float distance, float duration, bool isHit)
    {
        yield return LaserExtendingRoutine(distance, isHit);
        yield return new WaitForSeconds(duration);
        yield return LaserRetractingRoutine(distance);
    }

    private IEnumerator LaserExtendingRoutine(float distance, bool isHit)
    {
        const float extendStep = 0.2f;
        float laserStartSpriteWidth = laserStartObject.GetComponent<SpriteRenderer>().size.x;

        // Position and extend laser middle section over time
        float currentLaserLength = laserStartSpriteWidth;
        while ((currentLaserLength += extendStep) < distance)
        {
            laserMiddleObject.transform.localScale =
                new Vector3(currentLaserLength - laserStartSpriteWidth, laserMiddleObject.transform.localScale.y, laserMiddleObject.transform.localScale.z);
            laserMiddleObject.transform.localPosition = new Vector2(currentLaserLength / 2, 0f);

            yield return new WaitForEndOfFrame();
        }
        currentLaserLength = distance;

        laserMiddleObject.transform.localScale =
            new Vector3(currentLaserLength - laserStartSpriteWidth, laserMiddleObject.transform.localScale.y, laserMiddleObject.transform.localScale.z);
        laserMiddleObject.transform.localPosition = new Vector2(currentLaserLength / 2, 0f);

        if (laserEndObject != null && isHit)
        {
            laserEndObject.SetActive(true);
            laserEndObject.transform.localPosition = new Vector2(currentLaserLength - laserEndOffset, 0f);
        }
    }

    private IEnumerator LaserRetractingRoutine(float distance)
    {
        yield return new WaitForSeconds(3);

        const float extendStep = 0.2f;
        float laserStartSpriteWidth = laserStartObject.GetComponent<SpriteRenderer>().size.x;

        if (laserEndObject != null)
        {
            Destroy(laserEndObject);
            yield return new WaitForEndOfFrame();
        }

        // Position and retract laser middle section over time
        float currentLaserLength = distance;
        while ((currentLaserLength -= extendStep) > laserStartSpriteWidth)
        {
            laserMiddleObject.transform.localScale =
                new Vector3(currentLaserLength - laserStartSpriteWidth, laserMiddleObject.transform.localScale.y, laserMiddleObject.transform.localScale.z);
            laserMiddleObject.transform.localPosition = new Vector2(currentLaserLength / 2, 0f);

            yield return new WaitForEndOfFrame();
        }
        currentLaserLength = laserStartSpriteWidth;

        // laserMiddleObject.transform.localScale =
        //     new Vector3(currentLaserLength - laserStartSpriteWidth, laserMiddleObject.transform.localScale.y, laserMiddleObject.transform.localScale.z);
        // laserMiddleObject.transform.localPosition = new Vector2(currentLaserLength / 2, 0f);

        Destroy(laserMiddleObject);
        yield return new WaitForEndOfFrame();

        Destroy(laserStartObject);
    }

    [ContextMenu("Shoot")]
    private void ShootLaser()
    {
        // Start section
        if (laserStartObject == null)
        {
            laserStartObject = Instantiate(laserStartPrefab, this.transform.position, laserStartPrefab.transform.rotation, this.transform);
        }

        // Middle section
        if (laserMiddleObject == null)
        {
            laserMiddleObject = Instantiate(laserMiddlePrefab, this.transform.position, laserMiddlePrefab.transform.rotation, this.transform);
        }

        float currentLaserLength = maxLaserLength;

        Vector2 laserDirection = Vector2.right;
        switch (shootDirection)
        {
            case Direction.Up:
                laserDirection = Vector2.up;
                break;
            case Direction.Down:
                laserDirection = -Vector2.up;
                break;
            case Direction.Left:
                laserDirection = -Vector2.right;
                break;
            case Direction.Right:
            default:
                break;
        }

        // Raycast along the shooting direction to get the nearest collider
        LayerMask mask = LayerMask.GetMask("Foreground");
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, laserDirection, maxLaserLength, mask);

        if (hit.collider != null)
        {
            // === Hit something
            currentLaserLength = Vector2.Distance(hit.point, this.transform.position);

            // End section
            if (laserEndObject == null)
            {
                laserEndObject = Instantiate(laserEndPrefab, this.transform.position, laserEndPrefab.transform.rotation, this.transform);
            }
        }
        else
        {
            // === Hit nothing

            if (laserEndObject != null)
            {
                Destroy(laserEndObject);
            }
        }

        // Re-position each section to the correct order
        float laserStartSpriteWidth = laserStartObject.GetComponent<SpriteRenderer>().size.x;
        float laserEndSpriteWidth = (laserEndObject != null) ? laserEndObject.GetComponent<SpriteRenderer>().size.x : 0;

        laserMiddleObject.transform.localScale =
            new Vector3(currentLaserLength - laserStartSpriteWidth, laserMiddleObject.transform.localScale.y, laserMiddleObject.transform.localScale.z);
        laserMiddleObject.transform.localPosition = new Vector2(currentLaserLength / 2, 0f);

        if (laserEndObject != null)
        {
            laserEndObject.transform.localPosition = new Vector2(currentLaserLength - laserEndOffset, 0f);
        }

        // Rotate following shooting direction
        this.transform.Rotate(-this.transform.forward, Vector2.Angle(this.transform.right, laserDirection));
    }

    // Debugging ???
    private void ClearData()
    {
        Destroy(laserStartObject);
        Destroy(laserMiddleObject);
        Destroy(laserEndObject);

        this.transform.rotation = Quaternion.identity;
    }
}
