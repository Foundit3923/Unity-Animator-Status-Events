using Unity.VisualScripting;
using UnityEngine;
using UnityObject = UnityEngine.Object;

// See https://github.com/adammyhre/Unity-Utils for more extension methods
public static class GameObjectExtensions
{
    /// <summary>
    /// Returns the object itself if it exists, null otherwise.
    /// </summary>
    /// <remarks>
    /// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
    /// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
    /// Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is returned,
    /// aiding in correctly chaining operations and preventing NullReferenceExceptions.
    /// </remarks>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object being checked.</param>
    /// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
    public static T OrNull<T>(this T obj) where T : UnityObject => obj ? obj : null;

    public static TComponent GetOrAddComponent<TComponent>(this UnityObject uo) where TComponent : Component
    {
        var component = uo.GetComponent<TComponent>();
        if (component != null)
        {
            return component;
        }

        return component = uo.AddComponent<TComponent>();
    }

    //public static TComponent GetOrAddComponent<TComponent, TArgs>(this UnityObject uo, TArgs args) where TComponent : Component
    //{
    //    var component = uo.GetComponent<TComponent>() ?? uo.AddComponent<TComponent, TArgs>(args);
    //    return component;
    //    //return uo.GetComponent<TComponent>() ?? uo.AddComponent<TComponent, TArgs>(args);
    //}

    //public static TComponent AddComponent<TComponent, TArgs>(this UnityObject uo, TArgs args) where TComponent : Component
    //{
    //    if (uo is GameObject @object)
    //    {
    //        Arguments<TArgs>.ArgArray = args;
    //        var component = @object.AddComponent<TComponent>();
    //        Arguments<TArgs>.ArgArray = default;
    //        return component;
    //    }
    //    else if (uo is Component @componentObject)
    //    {
    //        Arguments<TArgs>.ArgArray = args;
    //        var component = @componentObject.gameObject.AddComponent<TComponent>();
    //        Arguments<TArgs>.ArgArray = default;
    //        return component;
    //    }
    //    else
    //    {
    //        throw new System.NotSupportedException();
    //    }
    //}

    public static void RemoveComponent<Component>(this GameObject obj, bool immediate = false)
    {
        Component component = obj.GetComponent<Component>();

        if (component != null)
        {
            if (immediate)
            {
                Object.DestroyImmediate(component as Object, true);
            }
            else
            {
                Object.Destroy(component as Object);
            }
        }
    }
}