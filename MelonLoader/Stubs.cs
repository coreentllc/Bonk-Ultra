// Stub definitions for CI builds without MelonLoader/UnityEngine references.
#if MELONLOADER_STUBS
using System;
using System.Collections;

namespace MelonLoader
{
  [AttributeUsage(AttributeTargets.Assembly)]
  public sealed class MelonInfoAttribute : Attribute
  {
    public MelonInfoAttribute(Type modType, string name, string version, string author) { }
  }

  [AttributeUsage(AttributeTargets.Assembly)]
  public sealed class MelonGameAttribute : Attribute
  {
    public MelonGameAttribute(string? developer, string? game) { }
  }

  public class MelonMod
  {
    public virtual void OnSceneWasInitialized(int buildIndex, string sceneName) { }
  }

  public static class MelonCoroutines
  {
    public static object Start(IEnumerator routine) => new object();
  }
}

namespace UnityEngine
{
  public class Object
  {
    public static T[] FindObjectsOfType<T>() => Array.Empty<T>();
  }

  public class MonoBehaviour : Object { }

  public class GameObject : Object
  {
    public string name { get; set; } = string.Empty;

    public static GameObject[] FindGameObjectsWithTag(string tag) => Array.Empty<GameObject>();
  }

  public sealed class WaitForSeconds
  {
    public WaitForSeconds(float seconds) { }
  }

  public class UnityException : Exception
  {
    public UnityException() { }
    public UnityException(string message) : base(message) { }
  }
}
#endif
