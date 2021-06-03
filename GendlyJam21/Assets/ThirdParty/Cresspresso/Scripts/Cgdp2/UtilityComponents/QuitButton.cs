using CGDP2.Utilities;

namespace CGDP2.UtilityComponents
{
	public class QuitButton : ButtonEventListener
	{
		public override void OnClick()
		{
			CgdUtils.QuitUnityApp();
		}
	}
}
