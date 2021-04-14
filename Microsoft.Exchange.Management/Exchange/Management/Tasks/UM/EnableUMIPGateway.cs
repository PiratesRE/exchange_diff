using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Enable", "UMIPGateway", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class EnableUMIPGateway : SystemConfigurationObjectActionTask<UMIPGatewayIdParameter, UMIPGateway>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableUMIPGateway(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (this.DataObject.Status == GatewayStatus.Enabled)
				{
					IPGatewayAlreadEnabledException exception = new IPGatewayAlreadEnabledException(this.DataObject.Name);
					base.WriteError(exception, ErrorCategory.InvalidOperation, null);
					return;
				}
				LocalizedException ex = NewUMIPGateway.ValidateFQDNInTenantAcceptedDomain(this.DataObject, (IConfigurationSession)base.DataSession);
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.DataObject.Status = GatewayStatus.Enabled;
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_IPGatewayEnabled, null, new object[]
				{
					this.DataObject.Name,
					this.DataObject.Address.ToString()
				});
			}
			TaskLogger.LogExit();
		}
	}
}
