using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Disable", "UMIPGateway", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class DisableUMIPGateway : SystemConfigurationObjectActionTask<UMIPGatewayIdParameter, UMIPGateway>
	{
		[Parameter(Mandatory = false)]
		public bool Immediate
		{
			get
			{
				return (bool)base.Fields["Immediate"];
			}
			set
			{
				base.Fields["Immediate"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (base.Fields["Immediate"] != null && (bool)base.Fields["Immediate"])
				{
					return Strings.ConfirmationMessageDisableUMIPGatewayImmediately;
				}
				return Strings.ConfirmationMessageDisableUMIPGateway;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (base.Fields["Immediate"] != null && (bool)base.Fields["Immediate"])
				{
					if (this.DataObject.Status == GatewayStatus.Disabled)
					{
						IPGatewayAlreadDisabledException exception = new IPGatewayAlreadDisabledException(this.DataObject.Name);
						base.WriteError(exception, ErrorCategory.InvalidOperation, null);
						return;
					}
				}
				else
				{
					if (this.DataObject.Status == GatewayStatus.NoNewCalls)
					{
						IPGatewayAlreadDisabledException exception2 = new IPGatewayAlreadDisabledException(this.DataObject.Name);
						base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
						return;
					}
					if (this.DataObject.Status == GatewayStatus.Disabled)
					{
						InvalidIPGatewayStateOperationException exception3 = new InvalidIPGatewayStateOperationException(this.DataObject.Name);
						base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
						return;
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.Fields["Immediate"] != null && (bool)base.Fields["Immediate"])
			{
				this.DataObject.Status = GatewayStatus.Disabled;
			}
			else
			{
				this.DataObject.Status = GatewayStatus.NoNewCalls;
			}
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_IPGatewayDisabled, null, new object[]
				{
					this.DataObject.Name,
					this.DataObject.Address.ToString()
				});
			}
			TaskLogger.LogExit();
		}

		private const string ImmediateField = "Immediate";
	}
}
