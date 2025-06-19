using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blackboard Data", menuName = "Blackboard/Blackboard Data")]
public class BlackboardData : ScriptableObject
{
    public List<BlackboardEntryData> entries = new();

    public void SetValuesOnBlackboard(Blackboard blackboard)
    {
        foreach (var entry in entries)
        {
            entry.SetValueOnBlackboard(blackboard);
        }
    }
}

[Serializable]
public class BlackboardEntryData : ISerializationCallbackReceiver
{
    public string keyName;
    public AnyValue.ValueType valueType;
    public AnyValue value;

    public void SetValueOnBlackboard(Blackboard blackboard)
    {
        var key = blackboard.GetOrRegisterKey(keyName);
        setValueDispatchTable[value.type](blackboard, key, value);
    }

    // Dispatch table to set different types of Value on the blackboard
    static Dictionary<AnyValue.ValueType, Action<Blackboard, BlackboardKey, AnyValue>> setValueDispatchTable = new() {
            { AnyValue.ValueType.Int, (blackboard, key, anyValue) => blackboard.SetValue<int>(key, anyValue) },
            { AnyValue.ValueType.Int32, (blackboard, key, anyValue) => blackboard.SetValue<int>(key, anyValue) },
            { AnyValue.ValueType.Float, (blackboard, key, anyValue) => blackboard.SetValue<float>(key, anyValue) },
            { AnyValue.ValueType.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key, anyValue) },
            { AnyValue.ValueType.String, (blackboard, key, anyValue) => blackboard.SetValue<string>(key, anyValue) },
            { AnyValue.ValueType.Vector3, (blackboard, key, anyValue) => blackboard.SetValue<Vector3>(key, anyValue) },
            { AnyValue.ValueType.GameObject, (blackboard, key, anyValue) => blackboard.SetValue<GameObject>(key, anyValue) },
            { AnyValue.ValueType.Transform, (blackboard, key, anyValue) => blackboard.SetValue<Transform>(key, anyValue) },
            { AnyValue.ValueType.GameObjectList, (blackboard, key, anyValue) => blackboard.SetValue<List<GameObject>>(key, anyValue) },
            { AnyValue.ValueType.TransformList, (blackboard, key, anyValue) => blackboard.SetValue<List<Transform>>(key, anyValue) },
            { AnyValue.ValueType.StringList, (blackboard, key, anyValue) => blackboard.SetValue<List<String>>(key, anyValue) },
        };

    public void OnBeforeSerialize()
    {
    }
    public void OnAfterDeserialize() => value.type = valueType;
}

[Serializable]
public struct AnyValue
{
    public enum ValueType
    {
        Int, Int32, Float, Bool, String, Vector3, GameObject, Transform, GameObjectList, TransformList, StringList
    }
    public ValueType type;

    // Storage for different types of values
    public Int64 intValue;
    public Int32 int32Value;
    public float floatValue;
    public bool boolValue;
    public string stringValue;
    public Vector3 vector3Value;
    public GameObject gameObjectValue;
    public Transform transformValue;
    public List<GameObject> gameObjectListValue;
    public List<Transform> transformListValue;
    public List<String> stringListValue;
    // Add more types as needed, but remember to add them to the dispatch table above and the custom Editor

    // Implicit conversion operators to convert AnyValue to different types
    public static implicit operator Int64(AnyValue value) => value.ConvertValue<Int64>();
    public static implicit operator Int32(AnyValue value) => value.ConvertValue<Int32>();
    public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
    public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
    public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
    public static implicit operator Vector3(AnyValue value) => value.ConvertValue<Vector3>();
    public static implicit operator GameObject(AnyValue value) => value.ConvertValue<GameObject>();
    public static implicit operator Transform(AnyValue value) => value.ConvertValue<Transform>();
    public static implicit operator List<GameObject>(AnyValue value) => value.ConvertValue<List<GameObject>>();
    public static implicit operator List<Transform>(AnyValue value) => value.ConvertValue<List<Transform>>();
    public static implicit operator List<String>(AnyValue value) => value.ConvertValue<List<String>>();

    T ConvertValue<T>()
    {
        return type switch
        {
            ValueType.Int => AsInt<T>(intValue),
            ValueType.Int32 => AsInt32<T>(int32Value),
            ValueType.Float => AsFloat<T>(floatValue),
            ValueType.Bool => AsBool<T>(boolValue),
            ValueType.String => (T)(object)stringValue,
            ValueType.Vector3 => AsVector3<T>(vector3Value),
            ValueType.GameObject => AsGameObject<T>(gameObjectValue),
            ValueType.Transform => AsTransform<T>(transformValue),
            ValueType.GameObjectList => AsGameObjectList<T>(gameObjectListValue),
            ValueType.TransformList => AsTransformList<T>(transformListValue),
            ValueType.StringList => AsStringList<T>(stringListValue),
            _ => throw new NotSupportedException($"Not supported Value type: {typeof(T)}")
        };
    }

    // Helper methods for safe type conversions of the Value types without the cost of boxing
    T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
    T AsInt<T>(Int64 value) => typeof(T) == typeof(Int64) && value is T correctType ? correctType : default;
    T AsInt32<T>(Int32 value) => typeof(T) == typeof(Int32) && value is T correctType ? correctType : default;
    T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
    T AsVector3<T>(Vector3 value) => typeof(T) == typeof(Vector3) && value is T correctType ? correctType : default;
    T AsGameObject<T>(GameObject value) => typeof(T) == typeof(GameObject) && value is T correctType ? correctType : default;
    T AsTransform<T>(Transform value) => typeof(T) == typeof(Transform) && value is T correctType ? correctType : default;
    T AsGameObjectList<T>(List<GameObject> value) => typeof(T) == typeof(List<GameObject>) && value is T correctType ? correctType : default;
    T AsTransformList<T>(List<Transform> value) => typeof(T) == typeof(List<Transform>) && value is T correctType ? correctType : default;
    T AsStringList<T>(List<String> value) => typeof(T) == typeof(List<String>) && value is T correctType ? correctType : default;
}
