using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorController : MonoBehaviour
{
    public Vector3 moveVelocity;
    public float existenceTime;
    private Text indicatorText;


    private float lifetime;



    // Start is called before the first frame update
    void Start()
    {
        indicatorText = GetComponentInChildren<Text>();
        lifetime = existenceTime;
    }

    // Update is called once per frame
    void Update()
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
