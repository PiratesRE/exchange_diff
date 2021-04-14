using System;
using System.Drawing;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class LocalSearchFilterCommand : PopupCommand
	{
		public LocalSearchFilterCommand()
		{
			this.DialogSize = new Size(670, 450);
		}

		internal const int DefaultSearchFilterPopupWidth = 670;

		internal const int DefaultSearchFilterPopupHeight = 450;
	}
}
