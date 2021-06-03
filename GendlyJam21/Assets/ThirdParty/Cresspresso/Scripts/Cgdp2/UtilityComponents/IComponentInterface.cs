using UnityEngine;

namespace CGDP2
{
	/// <summary>
	/// Derived types must also derive <see cref="Component"/> so that they can be found in the scene hierarchy.
	/// </summary>
	[System.Obsolete("Practice the Companion Component Technique instead.")]
	public interface IComponentInterface { }

	/// <summary>
	/// Derived types must also derive <typeparamref name="T"/> so that they can be found in the scene hierarchy.
	/// </summary>
	[System.Obsolete("Practice the Companion Component Technique instead.")]
	public interface IComponentInterface<T> where T : Component { }

	[System.Obsolete("Practice the Companion Component Technique instead.")]
	public static class ComponentInterfaceExtensions
	{
		public static Component AsComponent(this IComponentInterface obj)
			=> (obj is null) ? null : (Component)obj;

		public static T AsComponent<T>(this IComponentInterface<T> obj) where T : Component
			=> (obj is null) ? null : (T)obj;

		public static bool UnityAlive(this IComponentInterface obj)
			=> !(obj is null) && (bool)(Component)obj;

		public static T AsTrueNullable<T>(this T obj) where T : class, IComponentInterface
			=> AsComponent(obj).Pipe(c => (bool)c ? obj : null);

		public static T AsTrueNullable<T, U>(this T obj) where T : class, IComponentInterface<U> where U : Component
			=> AsComponent(obj).Pipe(c => (bool)c ? obj : null);

		public static bool UnityEqual(this IComponentInterface lhs, IComponentInterface rhs)
			=> AsComponent(lhs) == AsComponent(rhs);
	}
}
