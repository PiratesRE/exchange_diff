using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public enum ListViewFlags
	{
		Title = 1,
		SearchBar,
		Status = 4,
		IsEditable = 8,
		AllowSorting = 16,
		ShowHeader = 32,
		MultiSelect = 64,
		ViewsPanel = 128,
		EnableColumnResize = 256
	}
}
