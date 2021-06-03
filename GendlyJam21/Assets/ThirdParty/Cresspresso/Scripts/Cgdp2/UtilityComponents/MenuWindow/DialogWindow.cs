using System;
using UnityEngine;
using CGDP2.Utilities;

namespace CGDP2.UtilityComponents
{
	[RequireComponent(typeof(MenuWindow))]
	public class DialogWindow : MenuWindowEventListener
	{
		private Action<object> m_onDialogWindowReturned;
		private object m_result = null;

		public override void OnMenuWindowOpening(object eventArgs)
		{
			m_result = null;

			var eventData = (IDialogWindowOpenEventArgs)eventArgs;
			m_onDialogWindowReturned = eventData.callbackOnDialogWindowReturned;
		}

		public void Return(object result)
		{
			m_result = result;
			window.Close();
		}

		public override void OnMenuWindowClosing()
		{
			this.TryElseLog(() => m_onDialogWindowReturned?.Invoke(m_result));

			m_result = null;
			m_onDialogWindowReturned = null;
		}
	}

	public interface IDialogWindowOpenEventArgs
	{
		Action<object> callbackOnDialogWindowReturned { get; }
	}

	public sealed class BasicDialogWindowOpenEventArgs : IDialogWindowOpenEventArgs
	{
		public BasicDialogWindowOpenEventArgs(Action<object> callbackOnDialogWindowReturned)
		{
			this.callbackOnDialogWindowReturned = callbackOnDialogWindowReturned;
		}

		public Action<object> callbackOnDialogWindowReturned { get; }
	}
}
