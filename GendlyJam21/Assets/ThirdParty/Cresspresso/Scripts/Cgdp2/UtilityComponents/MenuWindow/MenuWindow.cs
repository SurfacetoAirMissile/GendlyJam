using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace CGDP2.UtilityComponents
{
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="03/02/2021">
	///			Created this script.
	///		</log>
	///		<log author="Elijah Shadbolt" date="09/02/2021">
	///			Added listener priority ordering.
	///		</log>
	/// </changelog>
	/// 
	public class MenuWindow : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_container;
		private GameObject container => m_container;

		[SerializeField]
		private bool m_isOpen;
		public bool isOpen => m_isOpen;

		[SerializeField]
		private bool m_onStartSendEvents = false;
		public bool onStartSendEvents => m_onStartSendEvents;



		private void Awake()
		{
			if (!onStartSendEvents)
			{
				if (m_isOpen)
				{
					container.SetActive(true);
				}
				else
				{
					container.SetActive(false);
				}
			}
		}

		private void Start()
		{
			if (onStartSendEvents)
			{
				if (m_isOpen)
				{
					m_isOpen = false;
					Open(null);
				}
				else
				{
					m_isOpen = true;
					Close();
				}
			}
		}

		private SortedDictionary<int, List<MenuWindowEventListener>> m_eventListeners = new SortedDictionary<int, List<MenuWindowEventListener>>();
		public IEnumerable<MenuWindowEventListener> eventListeners => m_eventListeners.Values.SelectMany(v => v);

		private bool m_sendingEvents = false;

		public void RegisterEventListener(MenuWindowEventListener listener)
		{
			var priority = listener.eventListenerPriority;
			if (m_eventListeners.TryGetValue(priority, out var list))
			{
				list.Add(listener);
			}
			else
			{
				m_eventListeners.Add(priority, new List<MenuWindowEventListener> { listener });
			}
		}

		public void DeregisterEventListener(MenuWindowEventListener listener)
		{
			if (m_eventListeners.TryGetValue(listener.eventListenerPriority, out var list))
			{
				list.Remove(listener);
			}
		}



		public void Open(/*Nullable*/object eventArgs)
		{
			if (m_sendingEvents)
			{
				throw new InvalidOperationException("cannot open while already in a transition.");
			}
			if (m_isOpen)
			{
				throw new InvalidOperationException("window is already opened");
			}

			m_sendingEvents = true;
			try
			{
				m_isOpen = true;
				container.SetActive(true);

				foreach (var listener in eventListeners.ToArray())
				{
					try
					{
						listener.OnMenuWindowOpening(eventArgs);
					}
					catch (Exception e)
					{
						Debug.LogException(e, listener);
					}
				}

				foreach (var listener in eventListeners.ToArray())
				{
					try
					{
						listener.OnMenuWindowOpened(eventArgs);
					}
					catch (Exception e)
					{
						Debug.LogException(e, listener);
					}
				}
			}
			finally
			{
				m_sendingEvents = false;
			}
		}

		public void Close()
		{
			if (m_sendingEvents)
			{
				throw new InvalidOperationException("cannot close while already in a transition.");
			}
			if (!m_isOpen)
			{
				throw new InvalidOperationException("window is already closed");
			}

			m_sendingEvents = true;
			try
			{
				m_isOpen = false;

				foreach (var listener in eventListeners.Reverse().ToArray())
				{
					try
					{
						listener.OnMenuWindowClosing();
					}
					catch (Exception e)
					{
						Debug.LogException(e, listener);
					}
				}

				foreach (var listener in eventListeners.Reverse().ToArray())
				{
					try
					{
						listener.OnMenuWindowClosed();
					}
					catch (Exception e)
					{
						Debug.LogException(e, listener);
					}
				}

				container.SetActive(false);
			}
			finally
			{
				m_sendingEvents = false;
			}
		}
	}
}
