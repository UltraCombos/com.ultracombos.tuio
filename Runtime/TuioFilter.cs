using System;
using System.Collections;
using System.Collections.Generic;
using TUIO;
using UnityEngine;
using UnityUtils.TUIO;

public class TuioFilter : MonoBehaviour, ITuioFilterHandler
{
    public virtual void Filter(TuioContainer tcur) { }
}
