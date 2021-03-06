using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "UMCallAnsweringRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetUMCallAnsweringRule : SetTenantADTaskBase<UMCallAnsweringRuleIdParameter, UMCallAnsweringRule, UMCallAnsweringRule>
	{
		[Parameter(Mandatory = false)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetCallAnsweringRule(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId executingUserId;
			base.TryGetExecutingUserId(out executingUserId);
			return UMCallAnsweringRuleUtils.GetDataProviderForCallAnsweringRuleTasks(this.Identity, this.Mailbox, base.SessionSettings, base.TenantGlobalCatalogSession, executingUserId, "set-callansweringrule", new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			UMCallAnsweringRuleDataProvider umcallAnsweringRuleDataProvider = (UMCallAnsweringRuleDataProvider)base.DataSession;
			umcallAnsweringRuleDataProvider.ValidateUMCallAnsweringRuleProperties(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		protected override bool IsKnownException(Exception exception)
		{
			return UMCallAnsweringRuleUtils.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void InternalStateReset()
		{
			UMCallAnsweringRuleUtils.DisposeCallAnsweringRuleDataProvider(base.DataSession);
			base.InternalStateReset();
		}

		protected override void Dispose(bool disposing)
		{
			UMCallAnsweringRuleUtils.DisposeCallAnsweringRuleDataProvider(base.DataSession);
			base.Dispose(disposing);
		}
	}
}
