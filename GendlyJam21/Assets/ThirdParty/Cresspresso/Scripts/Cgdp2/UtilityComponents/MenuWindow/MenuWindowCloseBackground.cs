using UnityEngine;
using UnityEngine.EventSystems;

namespace CGDP2.UtilityComponents
{
	public class MenuWindowCloseBackground : MonoBehaviour,
		IPointerDownHandler,
		IPointerUpHandler
	{
		[SerializeField]
		private MenuWindow m_window;
		private MenuWindow window
		{
			get => m_window;
			set => m_window = value;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!window)
			{
				Debug.LogError(nameof(window) + " is null", this);
			}

			window.Close();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
		}
	}
}
