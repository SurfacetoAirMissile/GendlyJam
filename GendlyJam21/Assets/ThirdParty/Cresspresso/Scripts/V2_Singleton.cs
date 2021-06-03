///
/// This script was taken from a separate project Elijah Shadbolt worked on.
/// It was copied over 03/06/2021.
///

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public enum V2_SingletonDuplicateMode { Ignore, DestroyComponent, DestroyGameObject, }

/// <summary>
/// Static class for managing instances of singleton scripts in the scene.
/// </summary>
/// <example><![CDATA[
/// using UnityEngine;
/// 
/// public class MyScript : MonoBehaviour
/// {
///		public static MyScript instance => Singleton<MyScript>.instance;
///		
///		void Awake()
///		{
///			if (!Singleton<MyScript>.OnAwake(this))
///			{
///				Destroy(gameObject);
///			}
///		}
/// }
/// 
/// // in other scripts:
/// var myScript = MyScript.instance;
/// 
/// ]]></example>
/// <typeparam name="T">Script type</typeparam>
/// <author>Elijah Shadbolt</author>
public static class V2_Singleton<T> where T : Component
{
	private static T m_instance;

	/// <summary>
	/// The current instance of this singleton in the scene.
	/// Use this in listening scripts' OnDestroy() methods, instead of <see cref="instanceElseLogError"/>.
	/// </summary>
	public static T instanceNullable
	{
		get
		{
			if (!m_instance)
			{
				m_instance = Object.FindObjectOfType<T>();
				// can still be null
			}
			return m_instance;
		}
	}

	/// <summary>
	/// The current instance of this singleton in the scene.
	/// </summary>
	public static T instanceElseLogError
	{
		get
		{
			var m = instanceNullable;
			if (!m)
			{
				Debug.LogError(typeof(T).Name + " Singleton instance not found in scene!");
			}
			return m;
		}
	}

	public static T GetOrCreate(Func<T> create)
	{
		var m = instanceNullable;
		if (!m)
		{
			m_instance = create();
			m = m_instance;
		}
		return m;
	}

	/// <summary>
	/// This must be called in the Awake method of a singleton script.
	/// </summary>
	/// <param name="instance">The instance that was awakened.</param>
	/// <param name="mode">Whether or not to destroy the GameObject or Component if it is a duplicate instance.</param>
	/// <returns>
	/// <see langword="false"/> if there was already a singleton instance and this new instance is a duplicate.
	/// <see langword="true"/> if the new instance successfully registered as the current singleton instance.
	/// </returns>
	public static bool OnAwake(
		T instance,
		V2_SingletonDuplicateMode mode,
		bool log = true)
	{
		if (m_instance && m_instance != instance)
		{
			var typename = typeof(T).Name;
			if (log)
			{
				Debug.LogError($"Duplicate instance of {typename} singleton in the scene.", instance);
			}
			switch (mode)
			{
				case V2_SingletonDuplicateMode.Ignore:
					break;
				case V2_SingletonDuplicateMode.DestroyComponent:
					Object.Destroy(instance);
					if (log)
					{
						Debug.Log($"Destroying duplicate {typename} singleton Component.", instance);
					}
					break;
				default:
				case V2_SingletonDuplicateMode.DestroyGameObject:
					Object.Destroy(instance.gameObject);
					if (log)
					{
						Debug.Log($"Destroying duplicate {typename} singleton GameObject.", instance);
					}
					break;
			}
			return false;
		}
		m_instance = instance;
		return true;
	}
}