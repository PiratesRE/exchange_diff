using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "TransportServer", DefaultParameterSetName = "Identity")]
	public sealed class GetTransportServer : GetTransportServiceBase
	{
		protected override void InternalValidate()
		{
			string cmdletName = SystemConfigurationTasksHelper.GetCmdletName(typeof(GetTransportServer));
			string cmdletName2 = SystemConfigurationTasksHelper.GetCmdletName(typeof(GetTransportService));
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
