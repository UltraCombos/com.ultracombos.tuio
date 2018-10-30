using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUIO;
using UnityEngine;

public class TuioData
{
    public TouchPhase phase = TouchPhase.Began;
    public TuioContainer container = null;
    public TuioData() { }
    public TuioData(TuioContainer container, TouchPhase phase)
    {
        this.container = container;
        this.phase = phase;
    }
}

public class TuioFrame
{
    public List<long> available_list = new List<long>();
    public List<TuioData> data_list = new List<TuioData>();
}

public class TuioUpdater
{
    public Dictionary<long, TuioData> tuio_data_list = new Dictionary<long, TuioData>();
    public void Update(List<TuioData> raw_data_list = null, List<long> raw_available_list = null)
    {
        foreach (var key_touch in tuio_data_list)
        {
            switch (key_touch.Value.phase)
            {
                case TouchPhase.Ended:
                    removal_ids.Add(key_touch.Key);
                    break;
                case TouchPhase.Began:
                    moved_ids.Add(key_touch.Key);
                    break;
                case TouchPhase.Moved:
                    //Do nothing!!!
                    break;
                default:
                    Debug.LogWarning("Imposible touch-phase " + key_touch.Value.phase + "!!!");
                    break;
            }
        }
        foreach (int id in removal_ids)
            tuio_data_list.Remove(id);
        removal_ids.Clear();
        foreach (int id in moved_ids)
        {
            TuioData new_touch = tuio_data_list[id];
            new_touch.phase = TouchPhase.Moved;
            tuio_data_list[id] = new_touch;
        }
        moved_ids.Clear();


        if (raw_data_list == null || raw_available_list == null)
            return;

        foreach (TuioData tuio_data in raw_data_list)
        {
            if (tuio_data_list.ContainsKey(tuio_data.container.SessionID))
            {
                switch (tuio_data.phase)
                {
                    case TouchPhase.Ended:
                        tuio_data_list[tuio_data.container.SessionID] = tuio_data;
                        break;
                    case TouchPhase.Moved:
                        tuio_data_list[tuio_data.container.SessionID] = tuio_data;
                        break;
                    case TouchPhase.Began:
                        Debug.LogError("SessionID " + tuio_data.container.SessionID + " already exists!!!");
                        break;
                    default:
                        Debug.LogError("Imposible touch-phase " + tuio_data.phase + "!!!");
                        break;
                }
            }
            else
            {
                switch (tuio_data.phase)
                {
                    case TouchPhase.Began:
                    case TouchPhase.Moved:
                        {
                            TuioData new_tuio_data = tuio_data;
                            new_tuio_data.phase = TouchPhase.Began;
                            tuio_data_list.Add(new_tuio_data.container.SessionID, new_tuio_data);
                        }
                        break;
                    case TouchPhase.Ended:
                        //do nothing.
                        break;
                    default:
                        Debug.LogWarning("Imposible touch-phase " + tuio_data.phase + "!!!");
                        break;
                }
            }
        }

        foreach (var key_touch in tuio_data_list)
        {
            if (raw_available_list.FindIndex(id => { return id == key_touch.Value.container.SessionID; }) == -1)
            {
                ended_ids.Add(key_touch.Value.container.SessionID);
            }
        }

        foreach (int id in ended_ids)
        {
            TuioData new_touch = tuio_data_list[id];
            new_touch.phase = TouchPhase.Ended;
            tuio_data_list[id] = new_touch;
        }
        ended_ids.Clear();

    }
    private List<long> removal_ids = new List<long>();
    private List<long> moved_ids = new List<long>();
    private List<long> ended_ids = new List<long>();
}