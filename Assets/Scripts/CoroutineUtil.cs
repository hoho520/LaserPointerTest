using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineUtil
{
    private static readonly Dictionary<float, WaitForSeconds> _waitTimeDictionary = new Dictionary<float, WaitForSeconds>();

    public static void SafeStopCoroutine(this Coroutine coroutine, MonoBehaviour behaviour)
    {
        if (coroutine == null)
            return;

        behaviour.StopCoroutine(coroutine);
    }

    public static Coroutine SafeStartCoroutine(this MonoBehaviour behaviour, IEnumerator enumeratorMethod)
    {
        if (behaviour == null)
            return null;

        return behaviour.StartCoroutine(enumeratorMethod);
    }

    public static WaitForSeconds GetWaitForSeconds(float waitTime)
    {
        WaitForSeconds waitForSeconds;

        if (_waitTimeDictionary.TryGetValue(waitTime, out waitForSeconds) == true)
        {
            return waitForSeconds;
        }

        waitForSeconds = new WaitForSeconds(waitTime);

        _waitTimeDictionary.Add(waitTime, waitForSeconds);

        return waitForSeconds;
    }

    public static IEnumerator WaitUntil(Func<bool> func, Action callback = null)
    {
        while (func.Invoke())
        {
            yield return null;
        }

        callback?.Invoke();
    }
}
