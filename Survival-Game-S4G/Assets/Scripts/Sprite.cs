using UnityEngine;

public class TreeSway : MonoBehaviour
{
    [Header("Wind Settings")]
    public float windStrength = 1f;
    public float windSpeed = 1f;
    public float windRandomness = 0.5f;
    [Tooltip("Сила ветра при которой дерево вырывается")]
    public float windUprootThreshold = 3f;

    [Header("Sway Settings")]
    public float maxSwayAngle = 15f;
    public float swaySmoothing = 2f;
    [Range(0, 1)] public float rootStrength = 0.8f;

    [Header("Ragdoll Settings")]
    public float uprootForce = 5f;
    public float ragdollDrag = 0.5f;
    public float ragdollAngularDrag = 0.5f;
    public float destroyAfterUproot = 10f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float windOffset;
    private float randomFactor;
    private bool isUprooted = false;
    private Rigidbody rb;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        windOffset = Random.Range(0f, 100f);
        randomFactor = Random.Range(0.8f, 1.2f);

        // Добавляем Rigidbody, но делаем его кинематическим изначально
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (isUprooted) return;

        float windTime = Time.time * windSpeed * 0.1f + windOffset;
        float windWave = Mathf.PerlinNoise(windTime, windTime * 0.5f) * 2f - 1f;
        float randomSway = Mathf.Sin(Time.time * windSpeed * randomFactor) * windRandomness;
        float totalWind = (windWave + randomSway) * windStrength;

        // Проверка на вырывание
        if (Mathf.Abs(totalWind) > windUprootThreshold && Random.value < 0.01f * Time.deltaTime * 60f)
        {
            UprootTree(totalWind);
            return;
        }

        // Нормальное качание
        float swayAngle = totalWind * maxSwayAngle;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, swayAngle);
        
        // Эффект "сопротивления корней" - дерево пытается вернуться в исходное положение
        transform.rotation = Quaternion.Lerp(
            transform.rotation, 
            targetRotation, 
            Time.deltaTime * swaySmoothing * (1 - rootStrength));

        // Смещение с сопротивлением
        Vector3 swayPosition = initialPosition + new Vector3(
            totalWind * 0.02f * (1 - rootStrength), 
            Mathf.Abs(totalWind) * 0.01f * (1 - rootStrength), 
            0);
        
        transform.position = Vector3.Lerp(
            transform.position, 
            swayPosition, 
            Time.deltaTime * swaySmoothing * (1 - rootStrength));
    }

    void UprootTree(float windForce)
    {
        isUprooted = true;
        
        // Активируем физику
        rb.isKinematic = false;
        rb.linearDamping = ragdollDrag;
        rb.angularDamping = ragdollAngularDrag;
        
        // Применяем силу ветра
        Vector3 forceDirection = new Vector3(
            Mathf.Sign(windForce) * uprootForce, 
            uprootForce * 0.5f, 
            0);
        
        rb.AddForce(forceDirection, ForceMode.Impulse);
        rb.AddTorque(new Vector3(0, 0, Random.Range(-1f, 1f) * uprootForce), ForceMode.Impulse);

        // Уничтожаем дерево через время
        Destroy(gameObject, destroyAfterUproot);

        // Можно добавить дополнительные эффекты (частицы, звук и т.д.)
        Debug.Log("Tree uprooted by strong wind!");
    }

    // Для отладки - визуализация порога вырывания
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, windUprootThreshold * 0.1f);
    }
}