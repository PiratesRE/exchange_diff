using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class GetUserSettingsForUser : GetUserSettingsCommandBase
	{
		internal GetUserSettingsForUser(ADRecipient callerADRecipient, SecurityIdentifier callerSid, CallContext callContext) : base(callContext)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "GetUserSettingsForUser constructor called for '{0}'.", callerADRecipient);
			this.callerSid = callerSid;
			this.callerADRecipient = callerADRecipient;
			Common.LoadAuthenticatingUserInfo(this.callerADRecipient);
			ADUser aduser = callerADRecipient as ADUser;
			this.callerSearchRoot = ((aduser != null) ? aduser.QueryBaseDN : null);
		}

		protected override IStandardBudget AcquireBudget()
		{
			return StandardBudget.Acquire(this.callerSid, BudgetType.Ews, Common.GetSessionSettingsForCallerScope());
		}

		protected override void AddToQueryList(UserResultMapping userResultMapping, IBudget budget)
		{
			if (this.IsCaller(userResultMapping))
			{
				this.SetCallerResult(userResultMapping);
				return;
			}
			base.AddToADQueryList(userResultMapping, this.callerADRecipient.OrganizationId, this.callerSearchRoot, budget);
		}

		private bool IsCaller(UserResultMapping userResultMapping)
		{
			bool result = false;
			if (this.callerADRecipient.EmailAddresses != null)
			{
				foreach (ProxyAddress proxyAddress in this.callerADRecipient.EmailAddresses)
				{
					if (proxyAddress != null && userResultMapping.SmtpProxyAddress.CompareTo(proxyAddress) == 0)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private void SetCallerResult(UserResultMapping userResultMapping)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "SetCallerResult() called for '{0}'.", userResultMapping.Mailbox);
			userResultMapping.Result = new ADQueryResult(userResultMapping)
			{
				Result = new Result<ADRecipient>(this.callerADRecipient, null)
			};
		}

		private SecurityIdentifier callerSid;

		private ADRecipient callerADRecipient;

		private ADObjectId callerSearchRoot;
	}
}
