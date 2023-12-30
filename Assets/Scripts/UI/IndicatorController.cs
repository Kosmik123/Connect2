using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorController : MonoBehaviour
{
    [SerializeField]
    private Vector3 moveVelocity;
    [SerializeField] private float existenceTime;
    private Text indicatorText;

    private float lifetime;

    private void Start()
    {
        indicatorText = GetComponentInChildren<Text>();
        lifetime = existenceTime;
    }

    private void Update()
    {
        transform.position += moveVelocity * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
            Destroy(gameObject);

        float newOpacity = 1.0f * lifetime / existenceTime;

        Color newCol = indicatorText.color;
        newCol.a = newOpacity;
        indicatorText.color = newCol;
    }
}
