using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class TabbedResultPaneProfile : ResultPaneProfile
	{
		public ResultPaneProfile[] ResultPanes { get; set; }

		public override bool HasPermission()
		{
			bool result = false;
			foreach (ResultPaneProfile resultPaneProfile in this.ResultPanes)
			{
				if (resultPaneProfile.HasPermission())
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
