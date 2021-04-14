using System;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class Constants
	{
		public static bool IsGallatin
		{
			get
			{
				return RbacPrincipal.Current.IsInRole("IsGallatin");
			}
		}
	}
}
