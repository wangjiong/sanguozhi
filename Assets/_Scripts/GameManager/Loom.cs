/***********************************
	Unity多线程（Thread）和主线程（MainThread）交互使用类
	这个类有两个主要方法：
		RunAsync(Action)
		QueueOnMainThread(Action, [optional] float time)
	可以轻松实现一个函数的两段代码在C#线程和Unity的主线程中交叉运行。
		1、用线程池去运行RunAsync(Action)的函数；
		2、在Update中运行QueueOnMainThread(Acition, [optional] float time)传入的函数。
***********************************/

//Loom.QueueOnMainThread(() => {
//                            for (int j = 0; j<pushMessages.Count; j++)
//                            {
//                                brocastCallback(pushMessages[j]);
//                            }
//                        });

        //Loom.RunAsync(() => {
        //    ReadData(text);
        //});

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class Loom : MonoBehaviour
{
    private const String TAG = "Loom";
    public static int maxThreads = 8;
	static int numThreads;

	private static Loom _current;
	private int _count;

	public static Loom Current {
		get {
			Initialize ();
			return _current;
		}
	}

	void Awake ()
	{
        if (_current == null){
            _current = this;
            initialized = true;
            DontDestroyOnLoad(gameObject);

            _delayed.AddRange(_pendingDelayed);
            _pendingDelayed.Clear();
        }
        else{
            DestroyImmediate(gameObject);
        }
	}

    void OnDestroy()
    {
        _current = null;
    }

    static volatile bool initialized;
    static object lockThis = new object();

	static void Initialize ()
	{
        lock (lockThis)
        {
            if (!initialized)
            {
                if (!Application.isPlaying)
                {
                    Debug.LogError(TAG + ", Application.isPlaying = False");
                    return;
                }
                Debug.Log(TAG + ", Initialize");
                initialized = true;
                var g = new GameObject("Loom");
                _current = g.AddComponent<Loom>();
            }
        }
	}

	private List<Action> _actions = new List<Action> ();

	public struct DelayedQueueItem
	{
		public float time;
		public Action action;
	}

	private List<DelayedQueueItem> _delayed = new  List<DelayedQueueItem> ();

    private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    private static List<DelayedQueueItem> _pendingDelayed = new List<DelayedQueueItem>();

	public static void QueueOnMainThread (Action action)
	{
		QueueOnMainThread (action, 0f);
	}

	public static void QueueOnMainThread (Action action, float time)
	{
        if (!initialized){

            lock (_pendingDelayed){
                _pendingDelayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }

            Initialize();
            return;
        }

        if (_current == null){
            return;
        }

		if (time != 0) {
			lock (Current._delayed) {
				Current._delayed.Add (new DelayedQueueItem { time = Time.time + time, action = action });
			}
		} else {
			lock (Current._actions) {
				Current._actions.Add (action);
			}
		}
	}

	public static Thread RunAsync (Action a)
	{
		Initialize ();
		while (numThreads >= maxThreads) {
			Thread.Sleep (1);
		}
		/*
			Interlocked.Increment实现的是原子性的加减,用于增减变量的
			并不是常用的Inc/Dec过程，而是用了InterlockedIncrement/InterlockedDecrement
			这一对过程，它们实现的功能完全一样，都是对变量加一或减一。
			但它们有一个最大的区别，那就是InterlockedIncrement/InterlockedDecrement是线程安全的。
			即它们在多线程下能保证执行结果正确，而Inc/Dec不能
		*/
		Interlocked.Increment (ref numThreads);
		ThreadPool.QueueUserWorkItem (RunAction, a); //可能有较长耗时
		return null;
	}

	private static void RunAction (object action)
	{
		try {
			((Action)action) ();
		} catch {
		} finally {
			Interlocked.Decrement (ref numThreads);
		}

	}

	void OnDisable ()
	{
		if (_current == this) {

			_current = null;
		}
	}

	List<Action> _currentActions = new List<Action> ();

	// Update is called once per frame
	void Update ()
	{
		lock (_actions) {
			_currentActions.Clear ();
			_currentActions.AddRange (_actions);
			_actions.Clear ();
		}
		foreach (var a in _currentActions) {
			a ();
		}
		lock (_delayed) {
			_currentDelayed.Clear ();
			_currentDelayed.AddRange (_delayed.Where (d => d.time <= Time.time));
			foreach (var item in _currentDelayed)
				_delayed.Remove (item);
		}
		foreach (var delayed in _currentDelayed) {
			delayed.action ();
		}
	}
}