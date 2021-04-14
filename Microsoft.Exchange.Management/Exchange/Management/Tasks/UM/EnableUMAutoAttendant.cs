using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Enable", "UMAutoAttendant", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class EnableUMAutoAttendant : SystemConfigurationObjectActionTask<UMAutoAttendantIdParameter, UMAutoAttendant>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableUMAutoAttendant(this.Identity.ToString());
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
				if (this.DataObject.Status == StatusEnum.Enabled)
				{
					AutoAttendantAlreadEnabledException exception = new AutoAttendantAlreadEnabledException(this.DataObject.Name);
					base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				}
				if (this.DataObject.DTMFFallbackAutoAttendant != null)
				{
					ValidationHelper.ValidateLinkedAutoAttendant(this.ConfigurationSession, this.DataObject.DTMFFallbackAutoAttendant.Name, true, this.DataObject);
				}
				if (this.DataObject.BusinessHoursKeyMappingEnabled)
				{
					foreach (CustomMenuKeyMapping customMenuKeyMapping in this.DataObject.BusinessHoursKeyMapping)
					{
						if (!string.IsNullOrEmpty(customMenuKeyMapping.AutoAttendantName))
						{
							ValidationHelper.ValidateLinkedAutoAttendant(this.ConfigurationSession, customMenuKeyMapping.AutoAttendantName, true, this.DataObject);
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.DataObject.SetStatus(StatusEnum.Enabled);
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AutoAttendantEnabled, null, new object[]
				{
					this.DataObject.Identity.ToString()
				});
			}
			TaskLogger.LogExit();
		}
	}
}
