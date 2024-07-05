using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace PPPredictor.Utilities
{

    public class ActionQueue
    {
        private readonly Queue<IEnumerator> _actions = new Queue<IEnumerator>();
        private readonly HashSet<string> _actionSet = new HashSet<string>();
        private readonly Timer _timer;
        private readonly object _lockObject = new object();
        private readonly MonoBehaviour _monoBehaviour;

        public ActionQueue(double interval, MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
            _timer = new Timer(interval);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public void Enqueue(IEnumerator action)
        {
            lock (_lockObject)
            {
                if (_actionSet.Add(action.ToString()))
                {
                    _actions.Enqueue(action);
                }
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerator actionToExecute = null;

            lock (_lockObject)
            {
                if (_monoBehaviour.isActiveAndEnabled && _actions.Count > 0)
                {
                    actionToExecute = _actions.Dequeue();
                    _actionSet.Remove(actionToExecute.ToString());
                }
            }

            if(actionToExecute != null)
            {
                _monoBehaviour.StartCoroutine(actionToExecute);
            }
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void SetInterval(double interval)
        {
            _timer.Interval = interval;
        }
    }
}
