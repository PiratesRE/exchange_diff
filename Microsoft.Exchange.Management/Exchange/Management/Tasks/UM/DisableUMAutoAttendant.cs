using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Disable", "UMAutoAttendant", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class DisableUMAutoAttendant : SystemConfigurationObjectActionTask<UMAutoAttendantIdParameter, UMAutoAttendant>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableUMAutoAttendant(this.Identity.ToString());
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || ValidationHelper.IsKnownException(exception);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (this.DataObject.Status == StatusEnum.Disabled)
				{
					AutoAttendantAlreadDisabledException exception = new AutoAttendantAlreadDisabledException(this.DataObject.Name);
					base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				}
				else
				{
					UMDialPlan dialPlan = this.DataObject.GetDialPlan();
					if (dialPlan == null)
					{
						DialPlanNotFoundException exception2 = new DialPlanNotFoundException(this.DataObject.UMDialPlan.Name);
						base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
					}
					else
					{
						ValidationHelper.ValidateDisabledAA(this.ConfigurationSession, dialPlan, this.DataObject);
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.DataObject.SetStatus(StatusEnum.Disabled);
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AutoAttendantDisabled, null, new object[]
				{
					this.DataObject.Identity
				});
			}
			TaskLogger.LogExit();
		}
	}
}
