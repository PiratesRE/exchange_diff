using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class InlineSearchBarCommand : Command
	{
		public InlineSearchBarCommand()
		{
			this.Name = "MoveToToolbar";
		}

		public string ControlIdToMove { get; set; }

		public string MovedControlCss { get; set; }
	}
}
