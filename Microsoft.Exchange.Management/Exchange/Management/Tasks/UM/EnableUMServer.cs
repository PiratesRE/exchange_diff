using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Enable", "UMService", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class EnableUMServer : SystemConfigurationObjectActionTask<UMServerIdParameter, Server>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableUMServer(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (this.DataObject.IsE15OrLater)
				{
					base.WriteError(new StatusChangeException(this.DataObject.Name), ErrorCategory.InvalidOperation, null);
				}
				if (this.DataObject.Status == ServerStatus.Enabled)
				{
					UMServerAlreadEnabledException exception = new UMServerAlreadEnabledException(this.DataObject.Name);
					base.WriteError(exception, ErrorCategory.InvalidOperation, null);
					return;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.DataObject.Status = ServerStatus.Enabled;
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServerEnabled, null, new object[]
				{
					this.DataObject.Name
				});
			}
			TaskLogger.LogExit();
		}
	}
}
