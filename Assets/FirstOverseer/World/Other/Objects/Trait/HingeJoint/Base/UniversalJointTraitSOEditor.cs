using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using FirstOverseer.World.Objects;

namespace FirstOverseer.World.Editor
{
    [CustomEditor(typeof(UniversalJointTraitSO))]
    public class UniversalJointTraitSOEditor : UnityEditor.Editor
    {
        private ReorderableList _statesList;

        private void OnEnable()
        {
            SerializedProperty statesProp = serializedObject.FindProperty("<States>k__BackingField");
            if (statesProp == null)
                statesProp = serializedObject.FindProperty("States");

            _statesList = new ReorderableList(serializedObject, statesProp, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "States");
                },

                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element = statesProp.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(element, true)),
                        element,
                        true
                    );
                },

                elementHeightCallback = (int index) =>
                {
                    SerializedProperty element = statesProp.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, true) + 6;
                },

                onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent(nameof(LinearJointStateData)), false, () => AddState(new LinearJointStateData()));
                    menu.AddItem(new GUIContent(nameof(AngularJointStateData)), false, () => AddState(new AngularJointStateData()));

                    menu.ShowAsContext();
                }
            };
        }

        private void AddState(JointStateData newState)
        {
            serializedObject.Update();

            SerializedProperty statesProp = serializedObject.FindProperty("<States>k__BackingField");
            if (statesProp == null)
                statesProp = serializedObject.FindProperty("States");

            statesProp.arraySize++;
            SerializedProperty newElement = statesProp.GetArrayElementAtIndex(statesProp.arraySize - 1);
            newElement.managedReferenceValue = newState;

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "States", "<States>k__BackingField");

            EditorGUILayout.Space(5);

            _statesList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}