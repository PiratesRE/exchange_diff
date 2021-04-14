using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.Prompts.Provisioning;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Remove", "UMAutoAttendant", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveUMAutoAttendant : RemoveSystemConfigurationObjectTask<UMAutoAttendantIdParameter, UMAutoAttendant>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveUMAutoAttendant(this.Identity.ToString());
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
				UMDialPlan dialPlan = base.DataObject.GetDialPlan();
				if (dialPlan == null)
				{
					DialPlanNotFoundException exception = new DialPlanNotFoundException(base.DataObject.UMDialPlan.Name);
					base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				}
				else
				{
					ValidationHelper.ValidateDisabledAA(this.ConfigurationSession, dialPlan, base.DataObject);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.DeleteContentFromPublishingPoint();
			if (!base.HasErrors)
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private void DeleteContentFromPublishingPoint()
		{
			if (base.DataObject.GetDialPlan() == null)
			{
				throw new DialPlanNotFoundException(base.DataObject.UMDialPlan.Name);
			}
			try
			{
				using (IPublishingSession publishingSession = PublishingPoint.GetPublishingSession(Environment.UserName, base.DataObject))
				{
					publishingSession.Delete();
				}
			}
			catch (PublishingException ex)
			{
				base.WriteWarning(ex.Message);
			}
		}
	}
}
