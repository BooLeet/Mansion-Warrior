using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    private static CameraShaker instance;
    public float maxSourceDistance = 50;
    public float maxOffset = 0.1f;
    public float maxAngle = 2;
    [Space]
    public float shakeTime = 1;
    private float shakeTimeCounter = 0;
    [Space]

    public float newOffsetTimeDelay = 0.02f;
    private float delayTimeCounter;
    private Vector3 previousOffset;
    private Vector3 currentOffset;

    private float currentAngleSign = -1;
    private float previousAngle;
    private float currentAngle;

    private float currentStrength;

    /// <summary>
    /// Shakes the camera
    /// </summary>
    /// <param name="source"></param>
    /// <param name="strength"> clamped between 0 and 1</param>
    public static void Shake(Vector3 source, float strength = 1)
    {
        if (instance != null)
            instance._Shake(source, strength);
    }

    private void _Shake(Vector3 source, float strength)
    {
        if (Vector3.Distance(transform.position, source) > maxSourceDistance)
            return;

        strength = Mathf.Clamp(strength, 0, 1);
        if(shakeTimeCounter <= 0 || currentStrength < strength)
            currentStrength = strength;

        shakeTimeCounter = shakeTime;
        CalculateNewOffset();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        if (shakeTimeCounter == 0)
            return;

        if (shakeTimeCounter < 0)
        {
            shakeTimeCounter = 0;
            delayTimeCounter = 0;
            previousAngle = 0;
            transform.localPosition = previousOffset = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            return;
        }

        delayTimeCounter += Time.deltaTime;
        if (delayTimeCounter >= newOffsetTimeDelay)
        {
            CalculateNewOffset();
            delayTimeCounter %= newOffsetTimeDelay;
        }

        transform.localPosition = Vector3.Lerp(previousOffset, currentOffset, delayTimeCounter / newOffsetTimeDelay);
        transform.localRotation = Quaternion.Euler(previousAngle, currentAngle, delayTimeCounter / newOffsetTimeDelay);
        shakeTimeCounter -= Time.deltaTime;
    }

    private void CalculateNewOffset()
    {
        previousOffset = currentOffset;
        currentOffset = currentStrength * Random.insideUnitSphere.normalized * (shakeTimeCounter / shakeTime) * maxOffset;

        currentAngleSign *= -1;
        previousAngle = currentAngle;
        currentAngle = currentStrength * currentAngleSign * (shakeTimeCounter / shakeTime) * maxAngle;
    }
}
