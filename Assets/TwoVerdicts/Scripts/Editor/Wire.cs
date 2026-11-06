using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// [SerializeField] private 필드를 에디터에서 연결하는 도우미.
    /// </summary>
    public static class Wire
    {
        public static void Ref(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            Find(so, field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void RefList<T>(Object target, string field, IList<T> values) where T : Object
        {
            var so = new SerializedObject(target);
            SerializedProperty list = Find(so, field);
            list.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
            {
                list.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static SerializedProperty Find(SerializedObject so, string field)
        {
            SerializedProperty prop = so.FindProperty(field);
            if (prop == null)
            {
                throw new InvalidOperationException($"필드 없음: {so.targetObject.GetType().Name}.{field}");
            }
            return prop;
        }
    }
}
