using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[CreateAssetMenu(fileName = "Bool_", menuName = "Scrip/Bool")]
public class boole : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private bool autoReset = true;
    [SerializeField] private bool startValue;
    
    [Header("Value")]
    [Space(50)]
    public bool active;

    #if UNITY_EDITOR
        public boole()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && autoReset)
            {
                OnReset();
            }
            if (state == PlayModeStateChange.EnteredEditMode && autoReset)
            {
                OnReset();
            }
        }
    #endif

    public void OnReset()
    {
        active = startValue;
    }
}
