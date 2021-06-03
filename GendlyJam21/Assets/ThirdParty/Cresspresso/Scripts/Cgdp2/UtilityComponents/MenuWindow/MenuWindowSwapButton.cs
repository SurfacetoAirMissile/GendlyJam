using UnityEngine;

namespace CGDP2.UtilityComponents
{
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="03/02/2021">
	///			Created this script.
	///		</log>
	/// </changelog>
	/// 
	public class MenuWindowSwapButton : ButtonEventListener
	{
		[SerializeField]
		private MenuWindow m_fromMenuWindow;

		[SerializeField]
		private MenuWindow m_toMenuWindow;

		public override void OnClick()
		{
			if (m_fromMenuWindow)
			{
				m_fromMenuWindow.Close();
			}
			if (m_toMenuWindow)
			{
				m_toMenuWindow.Open(null);
			}
		}
	}
}
