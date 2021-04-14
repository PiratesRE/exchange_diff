using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "MailboxComplianceConfiguration", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetMailboxComplianceConfiguration : SetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAutoTagging(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetentionAutoTaggingEnabled
		{
			get
			{
				return (bool)base.Fields["RetentionAutoTaggingEnabled"];
			}
			set
			{
				base.Fields["RetentionAutoTaggingEnabled"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ELCTaskHelper.VerifyIsInScopes(this.DataObject, base.ScopeSet, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.DataObject.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				base.WriteError(new InvalidOperationException(Strings.NotSupportedForPre14Mailbox(ExchangeObjectVersion.Exchange2010.ToString(), this.Identity.ToString(), this.DataObject.ExchangeVersion.ToString())), ErrorCategory.InvalidOperation, this.Identity);
			}
			TaskLogger.LogExit();
		}

		public override object GetDynamicParameters()
		{
			return null;
		}
	}
}
