using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(DamagableObject), true)]

    public class DamageTrackerInspectorAddon : UnityEditor.Editor
    {
        private DamagableObject _damagableObject;
        
        void OnEnable()
        {
            _damagableObject = (DamagableObject) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            // Save the old GUI.enabled state
            bool oldGuiEnabled = GUI.enabled;
            GUI.enabled = EditorApplication.isPlaying;

            GUILayout.Space(20);
            GUILayout.Label("Debug tools", EditorStyles.boldLabel);
            GUILayout.Label("Max Health: " + _damagableObject.maxHealth, EditorStyles.boldLabel);
            if (_damagableObject.gameObject.activeSelf == false)
            {           
                GUILayout.Label("GameObject is disabled");
            }
            else 
            {
                GUILayout.Label("Health: " + _damagableObject.hp, EditorStyles.boldLabel);
                GUILayout.Label("Is dead: " + _damagableObject.IsDead, EditorStyles.boldLabel);
            }
            if (GUILayout.Button("Do damage"))
            {
                Debug.Log("Doing damage to object with button press");
                _damagableObject.TakeDamage(10);
            }

            // Restore the old GUI.enabled state
            GUI.enabled = oldGuiEnabled;
        }
    }
}