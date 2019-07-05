//#define TUIO_DEBUG 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUIO;
using System;
using UnityEngine.Events;

public class ARTag
{
    public int sessionId { get; set; }
    public int symbolId { get; set; }
    public Vector2 position { get; set; }
    public TouchPhase phase { get; set; }
    public float angle { get; set; }
}
    
public class TUIOManager : MonoBehaviour, TuioListener
{
    const int MAX_OBJ_NUM_PRE_FRAME = 500;
    public static TUIOManager Instance  
	{
		get
		{
			if (_instance == null)
			{
				_instance = (TUIOManager)GameObject.FindObjectOfType(typeof(TUIOManager));
				if (_instance == null)
				{
					Debug.LogWarning("TUIOManager component required - adding dynamically now");
					GameObject go = new GameObject("TUIOManager");
					_instance = go.AddComponent<TUIOManager>();
				}
			}
			return _instance;
		}
	}

    public List<TuioFilter> blobFilters;
    public List<TuioFilter> objectFilters;

    public float TouchFps { get; private set; }
    public float ObjectFps { get; private set; }

    public Dictionary<int, Touch> touches
    {
        get;
        private set;
    }
    public Dictionary<int, ARTag> objects
    {
        get;
        private set;
    }

    public void addTuioCursor(TuioCursor tcur)
    {
        tuio_buffer[TuioType.TOUCH].Add(new TuioData(tcur, TouchPhase.Began));
    }
    public void updateTuioCursor(TuioCursor tcur)
    {
        tuio_buffer[TuioType.TOUCH].Add(new TuioData(tcur, TouchPhase.Moved));
    }
    public void removeTuioCursor(TuioCursor tcur)
    {
        tuio_buffer[TuioType.TOUCH].Add(new TuioData(tcur, TouchPhase.Ended));
    }

    public void addTuioBlob(TuioBlob tblb)
    {
        tuio_buffer[TuioType.TOUCH].Add(new TuioData(tblb, TouchPhase.Began));
    }
    public void updateTuioBlob(TuioBlob tblb)
    {
        tuio_buffer[TuioType.TOUCH].Add(new TuioData(tblb, TouchPhase.Moved));
    }
    public void removeTuioBlob(TuioBlob tblb)
    {
        tuio_buffer[TuioType.TOUCH].Add(new TuioData(tblb, TouchPhase.Ended));
    }

    public void addTuioObject(TuioObject tobj)
    {
        tuio_buffer[TuioType.OBJECT].Add(new TuioData(tobj, TouchPhase.Began));
    }
    public void updateTuioObject(TuioObject tobj)
    {
        tuio_buffer[TuioType.OBJECT].Add(new TuioData(tobj, TouchPhase.Moved));
    }
    public void removeTuioObject(TuioObject tobj)
    {
        tuio_buffer[TuioType.OBJECT].Add(new TuioData(tobj, TouchPhase.Ended));
    }

    public void refresh(TuioTime ftime)
    {
        foreach (var buffer in tuio_buffer)
        {
            buffer.Value.Refresh();
        }
    }

    public bool IsReady()
	{
		return client.isConnected ();
	}

    private Touch ToTouch(TuioContainer tcur, TouchPhase phase)
    {
        Touch result = new Touch();
        result.phase = phase;
        if (tcur is TuioCursor)
            result.fingerId = (int)(tcur as TuioCursor).SessionID;
        else if (tcur is TuioBlob)
            result.fingerId = (int)(tcur as TuioBlob).SessionID;
        result.position = new Vector2(tcur.X, tcur.Y);
        result.type = TouchType.Direct;
        return result;
    }
    private ARTag ToARTag(TuioContainer tcur, TouchPhase phase)
    {
        TuioObject obj = (TuioObject)tcur;
        ARTag result = new ARTag();
        result.phase = phase;
        result.sessionId = (int)obj.SessionID;
        result.symbolId = (int)obj.SymbolID;
        result.position = new Vector2(tcur.X, tcur.Y);
        return result;
    }
    
    void Awake()
    {
        foreach(TuioType tuiotype in Enum.GetValues(typeof(TuioType)))
        {
            TuioBuffer buffer = new TuioBuffer();
            tuio_buffer.Add(tuiotype, buffer);
        }

        touches = new Dictionary<int, Touch>();
        objects = new Dictionary<int, ARTag>();
    }
    // Use this for initialization
    void Start()
    {
        client = new TuioClient(TuioPort);
        client.addTuioListener(this);
        client.connect();
    }
	void OnDestroy()
	{
		client.disconnect ();
	}

    // Update is called once per frame
    void LateUpdate()
    {
        foreach (var buffer in tuio_buffer)
        {
            buffer.Value.Update();
        }

        TouchFps =  (float)tuio_buffer[TuioType.TOUCH].FPS;
        ObjectFps = (float)tuio_buffer[TuioType.OBJECT].FPS;

        touches.Clear();
        foreach (var key_tuiodata in tuio_buffer[ TuioType.TOUCH] .DataList)
        {
            TuioContainer tcon = null;
            TuioCursor tcur  = key_tuiodata.Value.container as TuioCursor;
            TuioBlob   tblob = key_tuiodata.Value.container as TuioBlob;
            if (tcur != null)
                tcon = new TuioCursor(tcur);
            if (tblob != null)
                tcon = tblob = new TuioBlob(tblob);
            foreach (var f in blobFilters)
            {
                f.Filter(tcon);
            }
            touches[(int)key_tuiodata.Key] = ToTouch(tcon, key_tuiodata.Value.phase);


            if(tblob!=null)
            {
                switch (key_tuiodata.Value.phase)
                {
                    case TouchPhase.Began:
                        AddTuioBlobEvent.Invoke(tblob);
                        break;
                    case TouchPhase.Moved:
                        UpdateTuioBlobEvent.Invoke(tblob);
                        break;
                    case TouchPhase.Ended:
                    default:
                        RemoveTuioBlobEvent.Invoke(tblob);
                        break;
                }
            }

        }



        objects.Clear();
        foreach (var key_tuiodata in tuio_buffer[TuioType.OBJECT].DataList)
        {
            TuioContainer tcur = new TuioObject(key_tuiodata.Value.container as TuioObject);
            foreach (var f in objectFilters)
            {
                f.Filter(tcur);
            }
            ARTag artag = ToARTag(tcur, key_tuiodata.Value.phase); ;
            objects[(int)key_tuiodata.Key] = artag;
        }

        foreach (var key_tuiodata in objects)
        {
            switch (key_tuiodata.Value.phase)
            {
                case TouchPhase.Began:
                    AddTuioObjectEvent.Invoke(key_tuiodata.Value);
                    break;
                case TouchPhase.Moved:
                    UpdateTuioObjectEvent.Invoke(key_tuiodata.Value);
                    break;
                case TouchPhase.Ended:
                    RemoveTuioObjectEvent.Invoke(key_tuiodata.Value);
                    break;
                default:
                    break;
            }
        }
    }
   
    private static void swap(ref int a, ref int b)
    {
        int tmp = a;
        a = b;
        b = tmp;
        
    }
    public int TuioPort = 3333;
    private static TUIOManager _instance;

    private TuioClient client;

    enum TuioType
    {
        TOUCH,
        OBJECT
    }

    Dictionary<TuioType, TuioBuffer> tuio_buffer = new Dictionary<TuioType, TuioBuffer>();
      
    [System.Serializable]
    public class TuioObjectEvent : UnityEvent<ARTag> { };
    public TuioObjectEvent AddTuioObjectEvent    = new TuioObjectEvent();
    public TuioObjectEvent UpdateTuioObjectEvent = new TuioObjectEvent();
    public TuioObjectEvent RemoveTuioObjectEvent = new TuioObjectEvent();

    [System.Serializable]
    public class TuioBlobEvent : UnityEvent<TuioBlob> { }
    public TuioBlobEvent AddTuioBlobEvent;
    public TuioBlobEvent UpdateTuioBlobEvent;
    public TuioBlobEvent RemoveTuioBlobEvent;

}