using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Disable", "UMService", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class DisableUMServer : SystemConfigurationObjectActionTask<UMServerIdParameter, Server>
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
					return Strings.ConfirmationMessageDisableUMServerImmediately;
				}
				return Strings.ConfirmationMessageDisableUMServer;
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
				if (base.Fields["Immediate"] != null && (bool)base.Fields["Immediate"])
				{
					if (this.DataObject.Status == ServerStatus.Disabled)
					{
						UMServerAlreadDisabledException exception = new UMServerAlreadDisabledException(this.DataObject.Name);
						base.WriteError(exception, ErrorCategory.InvalidOperation, null);
						return;
					}
				}
				else
				{
					if (this.DataObject.Status == ServerStatus.NoNewCalls)
					{
						UMServerAlreadDisabledException exception2 = new UMServerAlreadDisabledException(this.DataObject.Name);
						base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
						return;
					}
					if (this.DataObject.Status == ServerStatus.Disabled)
					{
						InvalidUMServerStateOperationException exception3 = new InvalidUMServerStateOperationException(this.DataObject.Name);
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
				this.DataObject.Status = ServerStatus.Disabled;
			}
			else
			{
				this.DataObject.Status = ServerStatus.NoNewCalls;
			}
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServerDisabled, null, new object[]
				{
					this.DataObject.Name
				});
			}
			TaskLogger.LogExit();
		}

		private const string ImmediateField = "Immediate";
	}
}
