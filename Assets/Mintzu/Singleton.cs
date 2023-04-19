using UnityEngine;

[DisallowMultipleComponent]
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T Reference;

    /// <summary>
    /// The singleton instance of a reference.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (Reference == null)
            {
                if (!(Reference = FindObjectOfType<T>()))
                {
                    //throw new MissingReferenceException($"The singleton reference to a {typeof(T).Name} does not found!");
                }
            }

            return Reference;
        }
    }

    public static bool HasReference
    {
        get
        {
            if (Reference == null)
            {
                return (Reference = FindObjectOfType<T>()) != null;
            }

            return true;
        }
    }
}