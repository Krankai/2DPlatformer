using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Trigger the registered event on enter
public class TriggerUtility : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the object that will trigger the corresponding event on collision")]
    public string targetTag;

    [Header("Events")]
    [Tooltip("The event(s) to be triggered on colliding with the specified tảgeet")]
    public UnityEvent triggerEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetTag != null && collision.CompareTag(targetTag))
        {
            triggerEvent.Invoke();
        }
    }
}
