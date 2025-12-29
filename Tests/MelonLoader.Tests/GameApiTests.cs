using System;
using Megabonk;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using Xunit;

namespace MelonLoader.Tests;

public sealed class GameApiTests
{
  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void CountEntitiesByType_ReturnsZero_WhenInputBlank(string? entityType)
  {
    Assert.Equal(0, GameApi.CountEntitiesByType(entityType!));
  }

  [Fact]
  public void CountEntitiesByType_UsesTagLookupWhenAvailable()
  {
    var tagged = Objects("moai", "moai_2");

    using var scope = new UnityTestScope(
      tagHandler: TagHandlerFor("moai", tagged));

    Assert.Equal(tagged.Length, GameApi.CountEntitiesByType("moai"));
  }

  [Fact]
  public void CountEntitiesByType_FallsBackToNameMatch_WhenNoTagMatch()
  {
    var objects = Objects("rare_vendor_01", "RARE_VENDOR", "legendary_vendor");

    using var scope = new UnityTestScope(
      tagHandler: TagHandlerFor("rare_vendor", Array.Empty<GameObject>()),
      findHandler: FindObjectsOfTypeHandler(objects: objects));

    Assert.Equal(2, GameApi.CountEntitiesByType("rare_vendor"));
  }

  [Fact]
  public void GetInstanceTier_ReadsFieldFromTierManager()
  {
    var components = new MonoBehaviour[] { new TierManagerWithField() };

    using var scope = new UnityTestScope(
      findHandler: FindObjectsOfTypeHandler(components: components));

    Assert.Equal(3, GameApi.GetInstanceTier());
  }

  [Fact]
  public void GetInstanceTier_ReadsPropertyFromTierManager()
  {
    var components = new MonoBehaviour[] { new GameManagerWithProperty() };

    using var scope = new UnityTestScope(
      findHandler: FindObjectsOfTypeHandler(components: components));

    Assert.Equal(2, GameApi.GetInstanceTier());
  }

  [Fact]
  public void GetInstanceTier_ReturnsNull_WhenNoMatchingManagers()
  {
    var components = new MonoBehaviour[] { new UnrelatedComponent() };

    using var scope = new UnityTestScope(
      findHandler: FindObjectsOfTypeHandler(components: components));

    Assert.Null(GameApi.GetInstanceTier());
  }

  private static GameObject[] Objects(params string[] names)
  {
    var objects = new GameObject[names.Length];
    for (int i = 0; i < names.Length; i++)
    {
      objects[i] = new GameObject { name = names[i] };
    }

    return objects;
  }

  private static Func<string, GameObject[]> TagHandlerFor(string expectedTag, GameObject[] objects)
  {
    return tag => tag == expectedTag ? objects : Array.Empty<GameObject>();
  }

  private static Func<Type, Array> FindObjectsOfTypeHandler(
    MonoBehaviour[]? components = null,
    GameObject[]? objects = null)
  {
    var componentList = components ?? Array.Empty<MonoBehaviour>();
    var objectList = objects ?? Array.Empty<GameObject>();

    return type =>
    {
      if (type == typeof(MonoBehaviour))
      {
        return componentList;
      }

      if (type == typeof(GameObject))
      {
        return objectList;
      }

      return Array.Empty<MonoBehaviour>();
    };
  }

  private sealed class UnityTestScope : IDisposable
  {
    private readonly Func<string, GameObject[]>? _previousTagHandler;
    private readonly Func<Type, Array>? _previousFindHandler;

    public UnityTestScope(
      Func<string, GameObject[]>? tagHandler = null,
      Func<Type, Array>? findHandler = null)
    {
      _previousTagHandler = GameObject.FindGameObjectsWithTagHandler;
      _previousFindHandler = UnityObject.FindObjectsOfTypeHandler;

      GameObject.FindGameObjectsWithTagHandler = tagHandler;
      UnityObject.FindObjectsOfTypeHandler = findHandler;
    }

    public void Dispose()
    {
      GameObject.FindGameObjectsWithTagHandler = _previousTagHandler;
      UnityObject.FindObjectsOfTypeHandler = _previousFindHandler;
    }
  }

  private sealed class TierManagerWithField : MonoBehaviour
  {
    public int tier = 3;
  }

  private sealed class GameManagerWithProperty : MonoBehaviour
  {
    public int CurrentTier { get; } = 2;
  }

  private sealed class UnrelatedComponent : MonoBehaviour
  {
    public int Tier = 5;
  }
}
