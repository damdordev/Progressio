using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// A progress bar that divides its progress across multiple child progress bar segments.
    /// </summary>
    public class SegmentedProgressBar : ProgressBar
    {
        public int SegmentCount
        {
            get => segmentCount;
            set
            {
                if (segmentCount == value) return;
                segmentCount = value;
                Refresh();
            }
        }

        public ProgressBar SegmentPrefab
        {
            get => segmentPrefab;
            set
            {
                if (segmentPrefab == value) return;
                segmentPrefab = value;
                Refresh();
            }
        }

        private readonly List<ProgressBar> segments = new();
        [SerializeField] private ProgressBar segmentPrefab;
        [SerializeField] private int segmentCount;

        private ProgressBar currentPrefab;
        
        protected override void Apply(float newValue)
        {
            var expectedSegmentCount = Math.Max(0, segmentCount);
            if (currentPrefab != segmentPrefab || expectedSegmentCount != segments.Count) RecreateSegments(expectedSegmentCount);
            if (segments == null || segments.Count == 0) return;

            var count = segments.Count;
            var segmentLength = 1f / count;
            
            for (var i = 0; i < count; i++)
            {
                var segment = segments[i];
                if (segment == null) continue;

                var min = i * segmentLength;
                var max = (i + 1) * segmentLength;
                var segmentProgress = 0f;
                if (newValue >= max) segmentProgress = 1f; 
                else if (newValue >= min) segmentProgress = Mathf.InverseLerp(min, max, newValue);
                segment.Value = segmentProgress;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            Refresh();
        }

        private void RecreateSegments(int count)
        {
            if (currentPrefab != segmentPrefab) RemoveSegments();
            currentPrefab = segmentPrefab;
            UpdateSegmentsCount(count);
        }

        private void RemoveSegments()
        {
            foreach (var segment in segments)
            {
                DestroySegment(segment);
            }
            segments.Clear();
            currentPrefab = null;
        }

        private void UpdateSegmentsCount(int count)
        {
            while (segments.Count < count) segments.Add(Create());
            while (segments.Count > count) DestroySegment(segments[^1]);
        }

        private ProgressBar Create()
        {
            var segment = Instantiate(segmentPrefab, transform);
            segment.gameObject.hideFlags = HideFlags.HideAndDontSave;
            return segment;
        }

        public void DestroySegment(ProgressBar segment)
        {
            if (segment == null) return;
            segments.Remove(segment);

            switch (Application.isPlaying)
            {
                case true:
                    Destroy(segment.gameObject);
                    return;
                case false:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += () => { DestroyImmediate(segment.gameObject); };
#endif
                    return;
            }
        }

    }
}