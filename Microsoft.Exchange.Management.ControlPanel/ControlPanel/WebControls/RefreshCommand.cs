using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class RefreshCommand : Command
	{
		public RefreshCommand()
		{
			this.Name = "Refresh";
			base.ImageAltText = Strings.RefreshCommandText;
			base.ImageId = CommandSprite.SpriteId.ListViewRefresh;
		}
	}
}
