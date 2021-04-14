using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal class OverBudgetException : LocalizedException
	{
		public OverBudgetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null)
			{
				this.owner = info.GetString("owner");
				this.policyPart = info.GetString("policyPart");
				this.isServiceAccountBudget = info.GetBoolean("IsServiceAccountBudget");
				this.throttlingPolicyDN = info.GetString("throttlingPolicyDN");
				this.budgetType = (BudgetType)info.GetValue("budgetType", typeof(BudgetType));
				this.backoffTime = info.GetInt32("backoffTime");
				this.snapshot = info.GetString("snapshot");
				this.policyValue = info.GetString("policyValue");
			}
		}

		public OverBudgetException(Budget budget, string policyPart, string policyValue, int backoffTime) : base(DirectoryStrings.ExceptionOverBudget(policyPart, policyValue, budget.Owner.BudgetType, backoffTime))
		{
			if (budget == null)
			{
				throw new ArgumentNullException("budget");
			}
			this.Initialize(budget.Owner, budget.ThrottlingPolicy.FullPolicy, policyPart, policyValue, backoffTime, budget.ToString());
		}

		public OverBudgetException(IBudget budget, string policyPart, string policyValue, int backoffTime) : base(DirectoryStrings.ExceptionOverBudget(policyPart, policyValue, budget.Owner.BudgetType, backoffTime))
		{
			if (budget == null)
			{
				throw new ArgumentNullException("budget");
			}
			this.Initialize(budget.Owner, budget.ThrottlingPolicy, policyPart, policyValue, backoffTime, budget.ToString());
		}

		public OverBudgetException(LocalizedString errorMessage, IBudget budget, string policyPart, string policyValue, int backoffTime) : base(errorMessage)
		{
			if (budget == null)
			{
				throw new ArgumentNullException("budget");
			}
			this.Initialize(budget.Owner, budget.ThrottlingPolicy, policyPart, policyValue, backoffTime, budget.ToString());
		}

		private void Initialize(BudgetKey owner, IThrottlingPolicy policy, string policyPart, string policyValue, int backoffTime, string snapshot)
		{
			this.owner = owner.ToString();
			this.isServiceAccountBudget = owner.IsServiceAccountBudget;
			this.throttlingPolicyDN = policy.GetIdentityString();
			this.budgetType = owner.BudgetType;
			this.policyPart = policyPart;
			this.policyValue = policyValue;
			this.backoffTime = backoffTime;
			this.snapshot = snapshot;
			ThrottlingPerfCounterWrapper.IncrementOverBudget(owner, TimeSpan.FromMilliseconds((double)backoffTime));
			WorkloadManagementLogger.SetOverBudget(policyPart, policyValue, null);
			WorkloadManagementLogger.SetBudgetType(owner.BudgetType.ToString(), null);
		}

		public OverBudgetException() : base(DirectoryStrings.ExceptionOverBudget("no part", "no policy", BudgetType.Ews, 0))
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("owner", this.owner);
				info.AddValue("policyPart", this.policyPart);
				info.AddValue("IsServiceAccountBudget", this.isServiceAccountBudget);
				info.AddValue("throttlingPolicyDN", this.throttlingPolicyDN);
				info.AddValue("budgetType", this.budgetType);
				info.AddValue("backoffTime", this.backoffTime);
				info.AddValue("snapshot", this.snapshot);
				info.AddValue("policyValue", this.policyValue);
			}
		}

		public string ThrottlingPolicyDN
		{
			get
			{
				return this.throttlingPolicyDN;
			}
		}

		public BudgetType BudgetType
		{
			get
			{
				return this.budgetType;
			}
		}

		public string Owner
		{
			get
			{
				return this.owner;
			}
		}

		public string PolicyPart
		{
			get
			{
				return this.policyPart;
			}
		}

		public string PolicyValue
		{
			get
			{
				return this.policyValue;
			}
		}

		public string Snapshot
		{
			get
			{
				return this.snapshot;
			}
		}

		public bool IsServiceAccountBudget
		{
			get
			{
				return this.isServiceAccountBudget;
			}
		}

		public int BackoffTime
		{
			get
			{
				return this.backoffTime;
			}
		}

		private const string OwnerField = "owner";

		private const string PolicyPartField = "policyPart";

		private const string IsServiceAccountBudgetField = "IsServiceAccountBudget";

		private const string ThrottlingPolicyDNField = "throttlingPolicyDN";

		private const string BudgetTypeField = "budgetType";

		private const string BackOffTimeField = "backoffTime";

		private const string SnapshotField = "snapshot";

		private const string PolicyValueField = "policyValue";

		private string owner;

		private string policyPart;

		private bool isServiceAccountBudget;

		private string throttlingPolicyDN;

		private BudgetType budgetType;

		private int backoffTime;

		private string snapshot;

		private string policyValue;
	}
}
