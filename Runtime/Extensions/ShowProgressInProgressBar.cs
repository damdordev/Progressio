#if DAMDOR_PROGRESSIO_UGUI

using TMPro;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// An extension that displays the current progress as a text in a TextMeshPro component.
    /// </summary>
    public class ShowProgressInProgressBar : ProgressBarExtension
    {
        /// <summary>
        /// The TextMeshPro component used to display the progress percentage.
        /// </summary>
        public TMP_Text ProgressText
        {
            get => progressText;
            set
            {
                progressText = value;
                if (isActiveAndEnabled && Target != null)
                {
                    OnDisplayedValueChanged(Target.DisplayedValue);
                }
            }
        }
        
        [SerializeField] private TMP_Text progressText;
        
        protected override void OnDisplayedValueChanged(float displayedValue)
        {
            if (progressText == null) return;
            progressText.text = $"{(int) (100f * displayedValue)}";
        }
        
    }
}

#endif