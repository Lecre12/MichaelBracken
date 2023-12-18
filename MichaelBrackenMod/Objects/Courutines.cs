using System.Collections;
using UnityEngine;

public static class CoroutineHelper
{
    private static GameObject coroutineObject;

    public static void StartCoroutine(IEnumerator coroutine)
    {
        if (coroutineObject == null)
        {
            coroutineObject = new GameObject("CoroutineHelper");
            Object.DontDestroyOnLoad(coroutineObject);
        }

        coroutineObject.AddComponent<CoroutineRunner>().StartCoroutine(coroutine);
    }

    private class CoroutineRunner : MonoBehaviour
    {
        // No es necesario implementar nada aquí, MonoBehaviour se encargará de ejecutar las coroutines.
    }
}
