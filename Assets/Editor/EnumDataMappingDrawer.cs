using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(EnumDataMapping<,>))]
public class EnumDataMappingDrawer : PropertyDrawer
{
    private SerializedProperty mapping;

    private Type GetEnumType() {
        return (fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType).GetGenericArguments()[1];
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

        if (mapping == null) {
            mapping = property.FindPropertyRelative("mapping");
        }
        
        Type enumType = GetEnumType();
        

        float height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded) {
            if (mapping.arraySize != enumType.GetEnumNames().Length)
                mapping.arraySize = enumType.GetEnumNames().Length;

            for(int i = 0; i < mapping.arraySize; i++) {
                height += EditorGUI.GetPropertyHeight(mapping.GetArrayElementAtIndex(i));

            }
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        if (mapping == null) {
            mapping = property.FindPropertyRelative("mapping");
        }

        Type enumType = GetEnumType();

        EditorGUI.BeginProperty(position, label, property);
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

        if (property.isExpanded) {

            float offY = EditorGUIUtility.singleLineHeight;
            EditorGUI.indentLevel++;

            for (int i = 0; i < mapping.arraySize; i++) {
                Rect rect = new Rect(position.x, position.y + offY, position.width, EditorGUI.GetPropertyHeight(mapping.GetArrayElementAtIndex(i)));
                EditorGUI.PropertyField(rect, mapping.GetArrayElementAtIndex(i), new GUIContent(enumType.GetEnumNames()[i]), true);
                
                offY += rect.height;
            }
            EditorGUI.indentLevel--;

        }


        EditorGUI.EndProperty();
    }
}
