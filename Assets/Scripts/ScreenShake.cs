using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{

    private Transform transform;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.05f;
    private float dampingSpeed = 3.0f;

    Vector3 startPos;

    void Awake()
    {
        if (transform == null)
            transform = GetComponent(typeof(Transform)) as Transform;
    }

    void OnEnable()
    {
        startPos = transform.localPosition;
    }
    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = startPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = startPos;
        }
    }
    public void TriggerShake(float dur, float mag)
    {
        shakeDuration = dur;
        shakeMagnitude = mag;
    }


}
