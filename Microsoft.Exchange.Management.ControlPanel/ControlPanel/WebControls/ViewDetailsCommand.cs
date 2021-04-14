using System;
using System.Drawing;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ViewDetailsCommand : PropertiesCommand
	{
		public ViewDetailsCommand()
		{
			this.DialogSize = ViewDetailsCommand.DefaultDialogSize;
			this.OnClientClick = "ViewDetailsCommandHandler";
			base.ImageAltText = Strings.ViewDetailsCommandText;
		}

		internal const int DefaultViewDetailWidth = 515;

		internal const int DefaultViewDetailHeight = 475;

		internal static readonly Size DefaultDialogSize = new Size(515, 475);
	}
}
