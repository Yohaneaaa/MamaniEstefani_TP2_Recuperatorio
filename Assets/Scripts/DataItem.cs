using UnityEngine;

public class DataItem : MonoBehaviour
{
    private int points;
    private float spinSpeed;

    private void Start()
    {
        points = 10;
        spinSpeed = 90f;
    }

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(points);

        Destroy(gameObject);
    }
}