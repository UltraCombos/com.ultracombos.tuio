using System.Collections.Generic;
using System;
using UnityEngine;

public class TuioBuffer
{
    private int BACK = 0;
    private int FRONT = 1;
    private int READY = 2;
    private TuioFrame[] frame = new TuioFrame[3];
    private List<long> raw_available_list = new List<long>();
    public TuioBuffer()
    {
        for (int i = 0; i < frame.Length; ++i)
            frame[i] = new TuioFrame();
    }
    public void Add(TuioData data)
    {
        switch (data.phase)
        {
            case TouchPhase.Began:
                raw_available_list.Add(data.container.SessionID);
                break;
            case TouchPhase.Ended:
                raw_available_list.Remove(data.container.SessionID);
                break;
            default:
                break;
        }
        frame[BACK].data_list.Add(data);
    }
    public void Refresh()
    {
        if (frame[BACK].data_list.Count == 0)
            return;

        frame[BACK].available_list.AddRange(raw_available_list);

        lock (this)
        {
            swap(ref BACK, ref FRONT);
            is_front_ok = true;
        }

        DateTime now = DateTime.Now;
        double delta = (now - stamp).TotalSeconds;
        double duration = 0.5;
        smooth_delta += (delta - smooth_delta) * Math.Min(1.0, delta / duration);
        FPS = 1.0 / smooth_delta;
        stamp = now;        
        
        frame[BACK].data_list.Clear();
        frame[BACK].available_list.Clear();
    }

    public void Update()
    {
        is_ready_ok = false;
        if (is_front_ok)
        {
            lock (this)
            {
                swap(ref READY, ref FRONT);
                is_front_ok = false;            
            }
            is_ready_ok = true;
        }
        if (is_ready_ok)        
            updater.Update(frame[READY].data_list, frame[READY].available_list);        
        else
            updater.Update();        
    }
    private TuioUpdater updater = new TuioUpdater();
    //public List<long> AvailableList { get { return frame[READY].available_list; } }
    public Dictionary<long, TuioData> DataList { get { return updater.tuio_data_list; } }
    public double FPS { get; private set; }
    private bool is_front_ok = false;
    private bool is_ready_ok = false;
    private double smooth_delta;
    private DateTime stamp = DateTime.Now;

    private static void swap(ref int a, ref int b)
    {
        int tmp = a;
        a = b;
        b = tmp;
    }
    
}

