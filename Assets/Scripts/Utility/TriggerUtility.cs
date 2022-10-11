using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Trigger the registered event on enter
public class TriggerUtility : MonoBehaviour
{
    public UnityEvent triggerEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerEvent.Invoke();
        }
    }
}
