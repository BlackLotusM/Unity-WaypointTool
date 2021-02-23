using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[CreateAssetMenu(fileName = "game_", menuName = "Scrip/test")]
public class gameobecte : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private bool autoReset = true;
    [SerializeField] private GameObject startValue;
    
    [Header("Value")]
    [Space(50)]
    public GameObject active;

    #if UNITY_EDITOR
        public gameobecte()
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
