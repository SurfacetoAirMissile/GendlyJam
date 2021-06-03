using System.Collections.Generic;
using System;
using UnityEngine;

namespace CGDP2.Utilities
{
	// Example:
	/*
	// MyScript.cs
	using UnityEngine;
	using CGDP2.Utilities;

	public class MyScript
	{
		private void InvokeMyEvent() => m_onMyEvent.Invoke(this);
		private NothrowUnityLogEvent<MyScript> m_onMyEvent;
		public event Action<MyScript> onMyEvent
		{
			add => m_onMyEvent.invoked += value;
			remove => m_onMyEvent.invoked -= value;
		}

		private void Update()
		{
			InvokeMyEvent();
		}
	}
	
	// MyListenerScript.cs
	using UnityEngine;

	public class MyListenerScript : MonoBehaviour
	{
		public MyScript myScript; // assign in the inspector

		private void Awake()
		{
			myScript.onMyEvent += OnMyEvent;
		}

		private void OnDestroy()
		{
			if (myScript)
			{
				myScript.onMyEvent -= OnMyEvent;
			}
		}

		private void OnMyEvent(MyScript sender)
		{
			Debug.Log("Listened to Event from " + sender.name, this);
		}
	}
	*/
	public struct NothrowUnityLogEvent<A>
	{
		public event Action<A> invoked
		{
			add
			{
				if (m_handlers is null) m_handlers = new List<Action<A>>();
				m_handlers.Add(value);
			}
			remove
			{
				if (m_handlers is null)
				{
					LogFailedToRemove();
					return;
				}

				if (!m_handlers.Remove(value))
				{
					LogFailedToRemove();
				}
			}
		}

		private void LogFailedToRemove()
		{
			Debug.LogWarning("Event handler could not be removed because it was not subscribed.");
		}

		public void Invoke(A arg)
		{
			if (m_handlers is null)
				return;

			foreach (var handler in m_handlers.ToArray())
			{
				Invoke(handler, arg);
			}
		}



		private List<Action<A>> m_handlers;

		private void Invoke(Action<A> handler, A arg)
		{
			try
			{
				handler?.Invoke(arg);
			}
			catch (Exception e)
			{
				var context = handler.Target as UnityEngine.Object;
				Debug.LogException(e, context);
			}
		}
	}
}
