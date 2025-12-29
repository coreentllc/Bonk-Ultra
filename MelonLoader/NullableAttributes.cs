// Compatibility shim for Unity IL2CPP assemblies missing nullable metadata attributes.
#if !MELONLOADER_STUBS
namespace System.Runtime.CompilerServices
{
  [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
  internal sealed class NullableAttribute : System.Attribute
  {
    public NullableAttribute(byte flag) { }
    public NullableAttribute(byte[] flags) { }
  }

  [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
  internal sealed class NullableContextAttribute : System.Attribute
  {
    public NullableContextAttribute(byte flag) { }
  }
}
#endif
