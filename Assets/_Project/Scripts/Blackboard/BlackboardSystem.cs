using System;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

[Serializable]
public readonly struct BlackboardKey : IEquatable<BlackboardKey>
{
    readonly string name;
    readonly int hashedKey;

    public BlackboardKey(string name)
    {
        this.name = name;
        hashedKey = name.ComputeFNV1aHash();
    }

    public bool Equals(BlackboardKey other) => hashedKey == other.hashedKey;

    public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
    public override int GetHashCode() => hashedKey;
    public override string ToString() => name;

    public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs.hashedKey == rhs.hashedKey;
    public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
}

[Serializable]
public class BlackboardEntry<T>
{
    public BlackboardKey Key
    {
        get;
    }
    public T Value
    {
        get;
    }
    public Type ValueType
    {
        get;
    }

    public BlackboardEntry(BlackboardKey key, T value)
    {
        Key = key;
        Value = value;
        ValueType = typeof(T);
    }

    public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
    public override int GetHashCode() => Key.GetHashCode();
}

[Serializable]
public class Blackboard
{
    Dictionary<string, BlackboardKey> keyRegistry = new();
    Dictionary<BlackboardKey, object> entries = new();

    private BlackboardController controller;

    public List<Action> PassedActions { get; } = new();

    public void AddAction(Action action)
    {
        Preconditions.CheckNotNull(action);
        PassedActions.Add(action);
    }

    public void ClearActions() => PassedActions.Clear();

    public void Debug()
    {
        foreach (var entry in entries)
        {
            var entryType = entry.Value.GetType();
            if (entryType.IsGenericType)
            {
                if (entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
                {
                    var valueProperty = entryType.GetProperty("Value");
                    if (valueProperty == null) continue;
                    if (!valueProperty.PropertyType.IsGenericType) continue;
                    if (valueProperty.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        if (valueProperty.PropertyType.Equals(typeof(List<Transform>)))
                        {
                            List<Transform> listValue = (List<Transform>)valueProperty.GetValue(entry.Value);
                            //UnityEngine.Debug.Log($"Key: {entry.Key}");
                            if (listValue.Count > 0)
                            {
                                foreach (var obj in listValue)
                                {
                                    if (obj != null)
                                    {
                                        //UnityEngine.Debug.Log($"-Name: {obj.name}, Position: {obj.position}");
                                    }
                                }
                            }
                        }
                        else if (valueProperty.PropertyType.Equals(typeof(List<GameObject>)))
                        {
                            List<GameObject> listValue = (List<GameObject>)valueProperty.GetValue(entry.Value);
                            UnityEngine.Debug.Log($"Key: {entry.Key}");
                            if (listValue.Count > 0)
                            {
                                foreach (var obj in listValue)
                                {
                                    if (obj != null)
                                    {
                                        //UnityEngine.Debug.Log($"-Value: {listValue}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var value = valueProperty.GetValue(entry.Value);
                        //UnityEngine.Debug.Log($"Key: {entry.Key}, Value: {Value}");
                    }
                }
            }
        }
    }

    private BlackboardController GetController()
    {
        if (controller == null)
        {
            controller = ServiceLocator.Global.Get<BlackboardController>();
        }

        return controller;
    }

    public bool TryGetValue<T>(BlackboardKey key, out T value)
    {
        var test = entries.TryGetValue(key, out var testEntry);
        if (entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
        {
            value = castedEntry.Value;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetValue<T>(BlackboardController.BlackboardKeyStrings keyString, out T value)
    {
        BlackboardKey key = GetController().GetKey(keyString);
        var test = entries.TryGetValue(key, out var testEntry);
        if (entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
        {
            value = castedEntry.Value;
            return true;
        }

        value = default;
        return false;
    }

    public void SetValue<T>(BlackboardKey key, T value) => entries[key] = new BlackboardEntry<T>(key, value);

    public void SetValue<T>(BlackboardController.BlackboardKeyStrings keyString, T value)
    {
        BlackboardKey key = GetController().GetKey(keyString);
        entries[key] = new BlackboardEntry<T>(key, value);
    }

    public void SetListValue<T>(BlackboardKey key, T listValue, int index = -1, bool remove = false)
    {
        List<T> list = new();
        if (TryGetValue<List<T>>(key, out list))
        {
            if (remove)
            {
                if (index >= 0 && index <= list.Count)
                {
                    if (list[index].Equals(listValue))
                    {
                        list.RemoveAt(index);
                    }
                }
                else if (list.Contains(listValue))
                {
                    list.Remove(listValue);
                }
            }
            else
            {
                if (index >= 0 && index <= list.Count)
                {
                    if (list.Contains(listValue))
                    {
                        list.Remove(listValue);
                    }

                    list.Insert(index, listValue);
                }
                else if (!list.Contains(listValue))
                {
                    list.Add(listValue);
                }
            }
        }
        else
        {
            //entry does not exist yet, so we have to make a new one

            //BlackboardEntry<List<T>> obj = (BlackboardEntry <List<T>>)entries[key];
            //list = (List<T>)obj.Value;
            list ??= new List<T>();
            if (remove)
            {
                if (list.Contains(listValue))
                {
                    list.Remove(listValue);
                }
            }
            else
            {
                if (!list.Contains(listValue))
                {
                    list.Add(listValue);
                }
            }

            entries.Add(key, new BlackboardEntry<List<T>>(key, list));
        }
    }

    public void SetListValue<T>(BlackboardController.BlackboardKeyStrings keyString, T listValue, int index = -1, bool remove = false)
    {
        BlackboardKey key = GetController().GetKey(keyString);
        List<T> list = new();
        if (TryGetValue<List<T>>(key, out list))
        {
            if (remove)
            {
                if (index >= 0 && index <= list.Count)
                {
                    if (list[index].Equals(listValue))
                    {
                        list.RemoveAt(index);
                    }
                }
                else if (list.Contains(listValue))
                {
                    list.Remove(listValue);
                }
            }
            else
            {
                if (index >= 0 && index <= list.Count)
                {
                    if (list.Contains(listValue))
                    {
                        list.Remove(listValue);
                    }

                    list.Insert(index, listValue);
                }
                else if (!list.Contains(listValue))
                {
                    list.Add(listValue);
                }
            }
        }
        else
        {
            //entry does not exist yet, so we have to make a new one

            //BlackboardEntry<List<T>> obj = (BlackboardEntry <List<T>>)entries[key];
            //list = (List<T>)obj.Value;
            list ??= new List<T>();
            if (remove)
            {
                if (list.Contains(listValue))
                {
                    list.Remove(listValue);
                }
            }
            else
            {
                if (!list.Contains(listValue))
                {
                    list.Add(listValue);
                }
            }

            entries.Add(key, new BlackboardEntry<List<T>>(key, list));
        }
    }

    public BlackboardKey GetOrRegisterKey(string keyName)
    {
        Preconditions.CheckNotNull(keyName);

        if (!keyRegistry.TryGetValue(keyName, out BlackboardKey key))
        {
            key = new BlackboardKey(keyName);
            keyRegistry[keyName] = key;
        }

        return key;
    }

    public bool ContainsKey(BlackboardKey key) => entries.ContainsKey(key);

    public void Remove(BlackboardKey key) => entries.Remove(key);
}
