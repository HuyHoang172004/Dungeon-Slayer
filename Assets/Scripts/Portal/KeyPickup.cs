using UnityEngine;

/// <summary>
/// Chìa khóa bắt buộc để mở cổng của màn hiện tại.
/// Trạng thái chỉ tồn tại trong màn chơi, vì mỗi màn có một chìa khóa riêng.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    public static bool HasKey { get; private set; }

    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        transform.position = startPosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.Rotate(0f, 0f, 75f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || HasKey)
        {
            return;
        }

        HasKey = true;
        Debug.Log("Đã nhận Chìa Khóa Cổ. Cổng dịch chuyển đã được mở.");
        Destroy(gameObject);
    }

    public static void ResetForNewLevel()
    {
        HasKey = false;
    }

    public void PlaceAt(Vector3 position)
    {
        startPosition = position;
        transform.position = position;
    }
}
