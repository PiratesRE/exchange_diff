using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "UMCallAnsweringRule", DefaultParameterSetName = "Identity")]
	public sealed class GetUMCallAnsweringRule : GetTenantADObjectWithIdentityTaskBase<UMCallAnsweringRuleIdParameter, UMCallAnsweringRule>
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

		protected override void WriteResult(IConfigurable dataObject)
		{
			UMCallAnsweringRule umcallAnsweringRule = dataObject as UMCallAnsweringRule;
			if (umcallAnsweringRule.InError)
			{
				base.WriteWarning(Strings.WarningUMCallAnsweringRuleInError(umcallAnsweringRule.Name));
			}
			base.WriteResult(dataObject);
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId executingUserId;
			base.TryGetExecutingUserId(out executingUserId);
			return UMCallAnsweringRuleUtils.GetDataProviderForCallAnsweringRuleTasks(this.Identity, this.Mailbox, base.SessionSettings, base.TenantGlobalCatalogSession, executingUserId, "get-umcallansweringrule", new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskErrorLoggingDelegate(base.WriteError));
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
