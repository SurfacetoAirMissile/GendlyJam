using UnityEngine;

namespace CGDP2.UtilityComponents
{
	public class DialogWindowBoolButton : ButtonEventListener
	{
		[SerializeField]
		private DialogWindow m_dialogWindow;
		public DialogWindow dialogWindow
		{
			get => m_dialogWindow;
			set => m_dialogWindow = value;
		}

		[SerializeField]
		private bool m_value = false;
		public bool value
		{
			get => m_value;
			set => m_value = value;
		}

		public override void OnClick()
		{
			if (!dialogWindow)
			{
				Debug.LogError(nameof(dialogWindow) + " is null", this);
				return;
			}

			dialogWindow.Return(value);
		}
	}
}
