using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 1.5f, 0);

    private Transform _target;
    private Camera _camera;

    public void Initialize(Transform target)
    {
        _target = target;
        _camera = Camera.main;
    }

    public void SetHealth(float current, float max)
    {
        fillImage.fillAmount = Mathf.Clamp01(current / max);
    }

    private void Update()
    {
        if (_target == null) return;

        Vector3 screenPos = _camera.WorldToScreenPoint(_target.position + worldOffset);
        transform.position = screenPos;
    }
}
