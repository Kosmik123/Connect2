using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorChanger : MonoBehaviour
{
    private new SpriteRenderer renderer;
    public Color resultColor;

    [Range(-1f,1f)]
    public float hue;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float h, s, v;

        resultColor = renderer.color;
        Color.RGBToHSV(resultColor, out h, out s, out v);


    }


}
