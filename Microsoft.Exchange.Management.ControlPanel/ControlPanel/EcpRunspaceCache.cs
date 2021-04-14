using System;
using System.Web;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class EcpRunspaceCache : RbacRunspaceCache
	{
		protected override string GetSessionKey()
		{
			return "Exchange" + HttpContext.Current.GetSessionID() + RbacPrincipal.Current.CacheKeys[0];
		}
	}
}
