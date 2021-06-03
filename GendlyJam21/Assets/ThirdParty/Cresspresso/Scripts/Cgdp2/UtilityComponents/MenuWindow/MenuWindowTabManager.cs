
using System;
using System.Collections.Generic;
using UnityEngine;
using CGDP2.Utilities;

namespace CGDP2.UtilityComponents
{
	/// <summary>
	///		It is the game developer's responsibility to initialise this script
	///		by calling <see cref="SwitchToTab(int, object, bool)"/>.
	/// </summary>
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="25/02/2021">
	///			Created this script.
	///		</log>
	/// </changelog>
	/// 
	public class MenuWindowTabManager : MonoBehaviour
	{
		[SerializeField]
		private MenuWindow[] m_tabs = new MenuWindow[0];
		public IReadOnlyList<MenuWindow> tabs => m_tabs;

		private int m_currentTabIndex = -1;
		public int currentTabIndex => m_currentTabIndex;

		private MenuWindow m_currentTab = null;
		public MenuWindow currentTab => m_currentTab;



		private void InvokeTabSwitched() => m_onTabSwitched.Invoke(this);
		private NothrowUnityLogEvent<MenuWindowTabManager> m_onTabSwitched;
		public event Action<MenuWindowTabManager> onTabSwitched
		{
			add => m_onTabSwitched.invoked += value;
			remove => m_onTabSwitched.invoked -= value;
		}



		private bool m_isInitialised = false;
		public bool isInitialised => m_isInitialised;



		/// <returns>
		///		<see langword="true"/> if it switched to a different tab.
		///		<see langword="false"/> if it stayed on the same tab or reopened it.
		/// </returns>
		public bool SwitchToTab(int index, object menuWindowOpenEventArgs, bool reopen = false)
		{
			m_isInitialised = true;

			bool sameTab = index == m_currentTabIndex;
			if (sameTab && !reopen)
				return false;

			if (m_currentTab)
			{
				m_currentTab.Close();
			}
			m_currentTabIndex = index;
			m_currentTab = tabs.TryGetClassNullable(m_currentTabIndex);
			if (m_currentTab)
			{
				m_currentTab.Open(menuWindowOpenEventArgs);
			}
			InvokeTabSwitched();
			return !sameTab;
		}
	}
}
