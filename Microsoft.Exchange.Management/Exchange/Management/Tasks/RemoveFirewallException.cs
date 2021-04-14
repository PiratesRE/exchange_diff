using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("remove", "firewallexception")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class RemoveFirewallException : ManageFirewallException
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.FirewallRule != null)
			{
				TaskLogger.Trace("Removing Windows Firewall Rule: {0}", new object[]
				{
					base.FirewallRule.Name
				});
				base.FirewallRule.Remove();
			}
			else if (!string.IsNullOrEmpty(base.Name) && base.BinaryPath != null)
			{
				TaskLogger.Trace("Removing binary {0} from windows firewall exception", new object[]
				{
					base.Name
				});
				ManageService.RemoveAssemblyFromFirewallExceptions(base.Name, base.BinaryPath.PathName, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			else
			{
				TaskLogger.Trace("ERROR: No Firewall Rule/Exception specified!", new object[0]);
			}
			TaskLogger.LogExit();
		}
	}
}
