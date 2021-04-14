using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public abstract class BaseSprite : Image
	{
		public static bool IsDataCenter;

		public static GetSpriteImageSrcDelegate GetSpriteImageSrc;
	}
}
