using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Trigger the registered event on enter
public class TriggerUtility : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the object that will trigger the corresponding event on collision")]
    public string targetTag = "Player";

    [Tooltip("Trigger automatically when the game starts")]
    public bool triggerOnAwake = false;

    [Header("Events")]
    [Tooltip("The event(s) to be triggered on colliding with the specified tảgeet")]
    public UnityEvent triggerEvent;

    private void Awake()
    {
        if (triggerOnAwake)
        {
            triggerEvent.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            triggerEvent.Invoke();
        }
    }
}
