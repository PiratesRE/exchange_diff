using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "InboundConnector", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class RemoveInboundConnector : RemoveSystemConfigurationObjectTask<InboundConnectorIdParameter, TenantInboundConnector>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveInboundConnector(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			FfoDualWriter.DeleteFromFfo<TenantInboundConnector>(this, base.DataObject);
			TaskLogger.LogExit();
		}
	}
}
