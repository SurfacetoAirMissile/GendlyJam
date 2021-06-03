using UnityEngine;
using UnityEngine.UI;

namespace CGDP2.UtilityComponents
{
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="03/02/2021">
	///			Created this script.
	///		</log>
	/// </changelog>
	/// 
	[RequireComponent(typeof(Button))]
	public abstract class ButtonEventListener : MonoBehaviour
	{
		private Button m_button;
		public Button button
		{
			get
			{
				if (!m_button)
				{
					m_button = GetComponent<Button>();
				}
				return m_button;
			}
		}

		protected virtual void Awake()
		{
			button.onClick.AddListener(OnClick);
		}

		protected virtual void OnDestroy()
		{
			if (m_button)
			{
				m_button.onClick.RemoveListener(OnClick);
			}
		}

		public abstract void OnClick();
	}
}
