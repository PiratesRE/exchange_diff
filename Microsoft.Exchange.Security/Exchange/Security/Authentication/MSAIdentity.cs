using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class MSAIdentity : GenericIdentity
	{
		public string MemberName
		{
			get
			{
				return this.member;
			}
		}

		public MSAIdentity(string netId, string memberName) : base(memberName, "MSA")
		{
			this.member = memberName;
			this.netId = netId;
		}

		public IStandardBudget AcquireBudget(BudgetType budgetType)
		{
			BudgetKey budgetKey = new StringBudgetKey(this.MemberName, false, budgetType);
			return StandardBudget.Acquire(budgetKey);
		}

		public string NetId
		{
			get
			{
				return this.netId;
			}
		}

		public override string Name
		{
			get
			{
				return this.MemberName;
			}
		}

		private const string MSAAuthenticationType = "MSA";

		private readonly string member;

		private readonly string netId;
	}
}
