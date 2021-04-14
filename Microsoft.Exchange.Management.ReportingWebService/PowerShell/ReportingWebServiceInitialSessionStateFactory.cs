using System;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal class ReportingWebServiceInitialSessionStateFactory : RbacInitialSessionStateFactory
	{
		public override InitialSessionState GetInitialSessionState()
		{
			InitialSessionState state = null;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.GetInitialSessionStateLatency, delegate
			{
				state = this.<>n__FabricatedMethod3();
			});
			return state;
		}
	}
}
