using System;
using System.ServiceModel;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal class ReportingWebServiceRunspaceCache : RbacRunspaceCache
	{
		protected override string GetSessionKey()
		{
			string str = string.Empty;
			if (OperationContext.Current != null)
			{
				str = OperationContext.Current.SessionId;
			}
			string str2 = RbacPrincipal.Current.CacheKeys[0];
			return "Exchange" + str + str2;
		}
	}
}
