using System;
using System.Management.Automation.Runspaces;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net
{
	public class RbacInitialSessionStateFactory : InitialSessionStateSectionFactory
	{
		public override InitialSessionState GetInitialSessionState()
		{
			RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
			return rbacPrincipal.RbacConfiguration.CreateInitialSessionState();
		}
	}
}
