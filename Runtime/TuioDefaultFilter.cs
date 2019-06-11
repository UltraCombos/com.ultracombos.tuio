using System.Collections;
using System.Collections.Generic;
using TUIO;
using UnityEngine;

public class TuioDefaultFilter : TuioFilter
{
    public Rect customRoi;

    private Rect roi = Rect.zero;

    public void Start()
    {
        if (Camera.main == null)
            Debug.LogWarning("Main Camera is null");
        else
        {
            roi.width = Camera.main.pixelWidth;
            roi.height = Camera.main.pixelHeight;
        }
    }

    protected void FixedUpdate()
    {
        if (customRoi != Rect.zero)
        {
            roi = customRoi;
        }
        else if(Camera.main != null)
        {
            roi.width = Camera.main.pixelWidth;
            roi.height = Camera.main.pixelHeight;
        }
    }

    public override void Filter(TuioContainer tcur)
    {
        if (isActiveAndEnabled == false)
            return;

        tcur.update(tcur.X * roi.width + roi.x, (1.0f - tcur.Y) * roi.height + roi.y);
    }
}
