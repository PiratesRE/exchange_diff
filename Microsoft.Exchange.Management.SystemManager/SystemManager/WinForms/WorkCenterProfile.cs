using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class WorkCenterProfile : ResultPaneProfile
	{
		public ResultPaneProfile TopResultPane { get; set; }

		public ResultPaneProfile BottomResultPane { get; set; }

		public override bool HasPermission()
		{
			return this.TopResultPane.HasPermission();
		}
	}
}
