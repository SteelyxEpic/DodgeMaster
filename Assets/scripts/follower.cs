using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class follower : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.15f;
    public float offset = 0.15f;
    private Vector3 velocity = Vector3.zero;
    static public follower ins;
    public LayerMask obstacleLayer;

    private Camera cam;

    private void Awake()
    {
        if (GetComponent<Camera>() != null)
        {
            if (ins == null)
            {
                ins = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }


        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;

        float targetx = target.position.x;

        if (Physics2D.Raycast(
            transform.position,
            new Vector2((target.position - transform.position).x, 0),
            halfWidth,
            obstacleLayer))
        {
            targetx = transform.position.x;
        }

        Vector3 targetPos = new Vector3(
            targetx,
            target.position.y + offset,
            -10f
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
