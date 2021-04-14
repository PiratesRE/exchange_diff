using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "TransportServer", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetTransportServer : SetTransportServiceBase
	{
		protected override void InternalValidate()
		{
			string cmdletName = SystemConfigurationTasksHelper.GetCmdletName(typeof(SetTransportServer));
			string cmdletName2 = SystemConfigurationTasksHelper.GetCmdletName(typeof(SetTransportService));
			this.WriteWarning(Strings.TransportServerCmdletDeprecated(cmdletName, cmdletName2));
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_TransportServerCmdletsDeprecated, new string[]
			{
				cmdletName,
				cmdletName2
			});
			base.InternalValidate();
		}
	}
}
