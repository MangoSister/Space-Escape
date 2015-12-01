using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PSMoveTimer : MonoBehaviour {
	
	private Dictionary<CountdownTimer, CallbackStruct> timers = 
		new Dictionary<CountdownTimer, CallbackStruct>();

	// Use this for initialization
	void Start () {
		//DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		List<CountdownTimer> removeList = new List<CountdownTimer>();
        foreach (CountdownTimer timer in timers.Keys)
        {
            timer.Update(Time.deltaTime);
            if (!timer.IsStarted)
            {
                removeList.Add(timer);
            }
        }
        foreach (CountdownTimer timer in removeList)
        {
            timers.Remove(timer);
        }
	}
	
	public void AddTimer(float time, System.Action<object> callback, object param) {
		CountdownTimer timer = new CountdownTimer();
		timer.Start(time, TimesUp, null, null);
		timers[timer] = new CallbackStruct(callback, param);;
	}
	
	private void TimesUp(CountdownTimer timer) {
		timers[timer].callback(timers[timer].param);
	}
}

public class CallbackStruct {
	public Action<object> callback;
	public object param;
	
	public CallbackStruct(Action<object> callback, object param) {
		this.callback = callback;
		this.param = param;
	}
}

public class CountdownTimer {
    public bool IsStarted { get; private set; }
    public float Rate { get { return _timeLimit == 0 ? 0 : _timer / _timeLimit; } }

    private Action<float> _updateCallback;
    private Action<CountdownTimer> _finishCallback;
    private Action<float> _intervalCallback;

    private float _timer;
    private float _timeLimit;
    private float _defaultTimeLimit;
    private float _intervalTimer;
    private float _interval;
    private float _defaultInterval;

    public CountdownTimer() : this(0)
    {
    }

    public CountdownTimer(float defaultTimeLimit) : this(defaultTimeLimit, 0)
    {
    }

    public CountdownTimer(float defaultTimeLimit, float defaltInterval)
    {
        Reset();
        _defaultTimeLimit = defaultTimeLimit;
        _defaultInterval = defaltInterval;
    }

    public void Start(Action<CountdownTimer> finishCallback, Action<float> updateCallback, Action<float> intervalCallback)
    {
        Start(_defaultTimeLimit, finishCallback, updateCallback, intervalCallback);
    }

    public void Start(float timeLimit, Action<CountdownTimer> finishCallback, Action<float> updateCallback, Action<float> intervalCallback)
    {
        Start(timeLimit, _defaultInterval, finishCallback, updateCallback, intervalCallback);
    }

    public void Start(float timeLimit, float interval, Action<CountdownTimer> finishCallback, Action<float> updateCallback, Action<float> intervalCallback)
    {
        _finishCallback = finishCallback;
        _updateCallback = updateCallback;
        _intervalCallback = intervalCallback;
        _timeLimit = timeLimit;
        _interval = interval;
        _timer = 0;
        _intervalTimer = 0;
        IsStarted = true;
    }

    public void Interrupt()
    {
        if (IsStarted)
        {
            Reset();
            if (_finishCallback != null)
            {
                _finishCallback(this);
            }
        }
    }

    public void Update(float deltaTime)
    {
        if (IsStarted)
        {
            if (_timer > _timeLimit)
            {
                IsStarted = false;
                _timer = _timeLimit;
                if (_updateCallback != null)
                {
                    _updateCallback(Rate);
                }
                if (_finishCallback != null)
                {
                    _finishCallback(this);
                }
            }
            else
            {
                _timer += deltaTime;
                _intervalTimer += deltaTime;
                if (_interval > 0 && _intervalTimer > _interval)
                {
                    _intervalTimer -= _interval;
                    if (_intervalCallback != null)
                    {
                        _intervalCallback(Rate);
                    }
                }
                if (_updateCallback != null)
                {
                    _updateCallback(Rate);
                }
            }
        }
    }

    public void Reset()
    {
        IsStarted = false;
        _timer = 0;
        _timeLimit = 0; 
    }
}
