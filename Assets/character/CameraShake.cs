using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 originalPos;
    private float shakeDurationRemaining = 0f;
    private float shakeIntensityCurrent = 0f;

    void Awake()
    {
        camTransform = GetComponent<Transform>();
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (shakeDurationRemaining > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeIntensityCurrent;

            shakeDurationRemaining -= Time.deltaTime;
        }
        else
        {
            shakeDurationRemaining = 0f;
            camTransform.localPosition = originalPos;
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
        shakeIntensityCurrent = intensity;
        shakeDurationRemaining = duration;
    }
}
