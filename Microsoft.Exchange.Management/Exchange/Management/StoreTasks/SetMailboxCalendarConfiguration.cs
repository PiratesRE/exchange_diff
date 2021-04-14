using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "MailboxCalendarConfiguration", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxCalendarConfiguration : SetMailboxConfigurationTaskBase<MailboxCalendarConfiguration>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageMailboxCalendarConfiguration(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Instance.IsModified(MailboxCalendarConfigurationSchema.FirstWeekOfYear) && this.Instance.FirstWeekOfYear == FirstWeekRules.LegacyNotSet)
			{
				base.WriteError(new InvalidParamException(Strings.ErrorMailboxCalendarConfigurationNotAllowedParameterValue("FirstWeekOfYear", "LegacyNotSet", "FirstDay, FirstFourDayWeek, FirstFullWeek")), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.DataObject.FirstWeekOfYear == FirstWeekRules.LegacyNotSet)
			{
				this.DataObject.FirstWeekOfYear = FirstWeekRules.FirstDay;
			}
			base.InternalProcessRecord();
		}
	}
}
