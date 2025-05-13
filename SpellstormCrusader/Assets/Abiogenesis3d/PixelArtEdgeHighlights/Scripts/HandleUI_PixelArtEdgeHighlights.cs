using UnityEngine;
using UnityEngine.UI;

namespace Abiogenesis3d.UPixelator_Demo
{
    public class HandleUI_PixelArtEdgeHighlights : MonoBehaviour
    {
        [HideInInspector]
        public PixelArtEdgeHighlights paeh;
        public Toggle effectEnabled;
        public Slider convexHighlight;
        public Slider outlineShadow;
        public Slider concaveShadow;
        public Slider depthSensitivity;
        public Slider debugEffect;
        public Text debugEffectName;

        string previousValues;

        void Start()
        {
            paeh = Object.FindAnyObjectByType<PixelArtEdgeHighlights>();
            if (!paeh) return;
            effectEnabled.isOn = paeh.gameObject.activeInHierarchy;
            convexHighlight.value = paeh.settings.convexHighlight;
            outlineShadow.value = paeh.settings.outlineShadow;
            concaveShadow.value = paeh.settings.concaveShadow;
            depthSensitivity.value = paeh.settings.depthSensitivity;
            debugEffect.value = (int)paeh.settings.debugEffect;
        }

        void Update()
        {
            if (!paeh) return;
            paeh.gameObject.SetActive(effectEnabled.isOn);
            paeh.settings.convexHighlight = convexHighlight.value;
            paeh.settings.outlineShadow = outlineShadow.value;
            paeh.settings.concaveShadow = concaveShadow.value;
            paeh.settings.depthSensitivity = depthSensitivity.value;
            paeh.settings.debugEffect = (PixelArtEdgeHighlightsDebugEffect)debugEffect.value;
            var d = paeh.settings.debugEffect.ToString();
            if (d == "None") d = "";
            debugEffectName.text = d;

            string currentValues = string.Join(",",
                convexHighlight.value,
                outlineShadow.value,
                concaveShadow.value,
                depthSensitivity.value,
                debugEffect.value
            );

            if (currentValues != previousValues)
            {
                previousValues = currentValues;
                paeh.HandleSettings();
            }
        }
    }
}
