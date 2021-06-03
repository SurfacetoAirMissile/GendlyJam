using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGDP2.UtilityComponents
{
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="04/02/2021">
	///			Created this script.
	///		</log>
	/// </changelog>
	/// 
	public abstract class MenuWindowEventListener : MonoBehaviour
	{
		private MenuWindow m_window;
		public MenuWindow window =>  m_window;

		public virtual int eventListenerPriority => 0;

		protected virtual void Awake()
		{
			m_window = GetComponentInParent<MenuWindow>();
			if (!m_window)
			{
				Debug.LogError(nameof(MenuWindow) + " not found in parent", this);
			}
			else
			{
				m_window.RegisterEventListener(this);
			}
		}

		protected virtual void OnDestroy()
		{
			if (m_window)
			{
				m_window.DeregisterEventListener(this);
			}
		}

		/// <seealso cref="MenuWindow.Open(object)"/>
		public virtual void OnMenuWindowOpening(/*Nullable*/object eventArgs) { }

		/// <seealso cref="MenuWindow.Open(object)"/>
		public virtual void OnMenuWindowOpened(/*Nullable*/object eventArgs) { }

		/// <seealso cref="MenuWindow.Close()"/>
		public virtual void OnMenuWindowClosing() { }

		/// <seealso cref="MenuWindow.Close()"/>
		public virtual void OnMenuWindowClosed() { }
	}
}
