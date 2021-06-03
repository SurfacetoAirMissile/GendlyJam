using UnityEngine;
using CGDP2.UtilityComponents;

namespace CGDP2.ActorLayer.DiscoveryInfo
{
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="25/02/2021">
	///			Created this script.
	///		</log>
	/// </changelog>
	/// 
	public class MenuWindowTabButton : ButtonEventListener
	{
		[SerializeField]
		private MenuWindowTabManager m_tabManager;
		public MenuWindowTabManager tabManager
		{
			get => m_tabManager;
			set
			{
				RemoveSelfFromTabManager();
				m_tabManager = value;
				AddSelfToTabManager();
			}
		}

		private void AddSelfToTabManager()
		{
			if (m_tabManager)
			{
				m_tabManager.onTabSwitched += OnTabSwitched;
			}

			OnTabSwitched();
		}

		private void RemoveSelfFromTabManager()
		{
			if (!m_tabManager)
				return;

			m_tabManager.onTabSwitched -= OnTabSwitched;
		}

		[SerializeField]
		private int m_tabIndex = -1;
		public int tabIndex
		{
			get => m_tabIndex;
			set => m_tabIndex = value;
		}

		[SerializeField]
		private bool m_reopen = false;
		public bool reopen
		{
			get => m_reopen;
			set => m_reopen = value;
		}

		protected override void Awake()
		{
			base.Awake();
			AddSelfToTabManager();
		}

		protected override void OnDestroy()
		{
			RemoveSelfFromTabManager();
			base.OnDestroy();
		}

		private void OnTabSwitched(MenuWindowTabManager sender)
		{
			OnTabSwitched();
		}

		private void OnTabSwitched()
		{
			button.interactable = ShouldBeInteractable();
		}

		private bool ShouldBeInteractable()
		{
			if (!tabManager)
				return false;

			if (reopen)
				return true;

			return tabIndex != tabManager.currentTabIndex;
		}

		protected virtual object MakeTabOpenEventArgs() => null;

		public override void OnClick()
		{
			if (!tabManager)
				return;

			var eventArgs = MakeTabOpenEventArgs();
			tabManager.SwitchToTab(tabIndex, eventArgs, reopen);
		}
	}
}
