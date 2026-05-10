using System.Collections.Generic;
using UnityEngine;

namespace Damdor.Progressio
{
    internal class ProgressBarUpdateProvider : MonoBehaviour
    {
        public delegate void UpdateReceiver(float deltaTime, float unscaledDeltaTime);

        private static readonly List<UpdateReceiver> receivers = new();
        private static ProgressBarUpdateProvider instance;

        public static void Register(UpdateReceiver receiver)
        {
            if (receiver == null || !Application.isPlaying) return;
            if (instance == null) CreateInstance();
            receivers.Add(receiver);
        }

        public static void Unregister(UpdateReceiver receiver)
        {
            receivers.Remove(receiver);
            if (receivers.Count == 0) DestroyInstance();
        }

        private void Update()
        {
            foreach (var updateReceiver in receivers)
            {
                updateReceiver(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        private static void CreateInstance()
        {
            var go = new GameObject("ProgressBarUpdateProvider")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            DontDestroyOnLoad(go);
            go.AddComponent<ProgressBarUpdateProvider>();
            instance = go.GetComponent<ProgressBarUpdateProvider>();
        }

        private static void DestroyInstance()
        {
            if (instance == null) return;
            Destroy(instance.gameObject);
            instance = null;
        }
        
    }
}