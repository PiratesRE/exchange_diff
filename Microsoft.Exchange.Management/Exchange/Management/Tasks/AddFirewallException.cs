using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("add", "firewallexception")]
	public sealed class AddFirewallException : ManageFirewallException
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.FirewallRule != null)
			{
				try
				{
					TaskLogger.Trace("Adding Windows Firewall Rule: {0}", new object[]
					{
						base.FirewallRule.Name
					});
					base.FirewallRule.Add();
					goto IL_C5;
				}
				catch (COMException ex)
				{
					TaskLogger.Trace("Failed to Add Windows Firewall Exception: {0}", new object[]
					{
						ex.Message
					});
					goto IL_C5;
				}
			}
			if (!string.IsNullOrEmpty(base.Name) && base.BinaryPath != null)
			{
				TaskLogger.Trace("Adding binary {0} to windows firewall exception", new object[]
				{
					base.Name
				});
				ManageService.AddAssemblyToFirewallExceptions(base.Name, base.BinaryPath.PathName, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			else
			{
				TaskLogger.Trace("ERROR: No Firewall Rule/Exception specified!", new object[0]);
			}
			IL_C5:
			TaskLogger.LogExit();
		}
	}
}
