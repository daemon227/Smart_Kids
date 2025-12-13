using System.Collections;
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;
using UnityEngine.Serialization;

public class Butterfly : MonoBehaviour 
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;          
    [SerializeField] private float rotateSpeed = 2.0f;      
    [SerializeField] private float viewPadding = 0.5f;      
    [SerializeField] private Vector2 waitIdleTimeRange = new Vector2(0.5f, 2f); 
    [Header("Wobble Settings")]
    [SerializeField] private float wobbleFrequency = 5.0f;  // Tần số lượn (lượn nhanh hay chậm)
    [SerializeField] private float wobbleAmplitude = 2.0f;  // Độ rộng vòng lượn (lượn to hay nhỏ)

    private Vector3 _targetPosition;
    private bool _isIdle = false;
    private float _timeOffset; // Dùng để random hóa noise

    private void Start()
    {
        _timeOffset = Random.Range(0f, 100f);
        SetNewRandomTarget();
    }

    private void Update()
    {
        if (GameManager.Instance.isPaused) return;
        if (_isIdle) return;

        MoveWobbly();
        CheckDestination();
    }

    private void MoveWobbly()
    {
        Vector3 desiredDirection = (_targetPosition - transform.position).normalized;

        Vector3 perpendicular = new Vector3(-desiredDirection.y, desiredDirection.x, 0);

        float wave = Mathf.Sin((Time.time + _timeOffset) * wobbleFrequency);

        Vector3 finalDirection = (desiredDirection + perpendicular * wave * wobbleAmplitude).normalized;
        
        transform.position += finalDirection * moveSpeed * Time.deltaTime;

        if (finalDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    private void CheckDestination()
    {
        if (Vector3.Distance(transform.position, _targetPosition) < 0.5f)
        {
            StartCoroutine(WaitAndPickNewTarget());
        }
    }

    private IEnumerator WaitAndPickNewTarget()
    {
        _isIdle = true;
        float waitTime = Random.Range(waitIdleTimeRange.x, waitIdleTimeRange.y);
        yield return new WaitForSeconds(waitTime);
        
        SetNewRandomTarget();
        _isIdle = false;
    }

    private void SetNewRandomTarget()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = cam.transform.position.x - camWidth / 2f + viewPadding;
        float maxX = cam.transform.position.x + camWidth / 2f - viewPadding;
        float minY = cam.transform.position.y - camHeight / 2f + viewPadding;
        float maxY = cam.transform.position.y + camHeight / 2f - viewPadding;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        _targetPosition = new Vector3(randomX, randomY, transform.position.z);
    }
}
