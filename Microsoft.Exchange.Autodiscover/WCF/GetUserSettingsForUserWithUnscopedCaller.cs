using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class GetUserSettingsForUserWithUnscopedCaller : GetUserSettingsCommandBase
	{
		internal GetUserSettingsForUserWithUnscopedCaller(SecurityIdentifier callerSid, CallContext callContext) : base(callContext)
		{
			this.callerSid = callerSid;
		}

		protected override IStandardBudget AcquireBudget()
		{
			return StandardBudget.Acquire(this.callerSid, BudgetType.Ews, Common.GetSessionSettingsForCallerScope());
		}

		protected override void AddToQueryList(UserResultMapping userResultMapping, IBudget budget)
		{
			base.AddToADQueryList(userResultMapping, OrganizationId.ForestWideOrgId, null, budget);
		}

		private SecurityIdentifier callerSid;
	}
}
