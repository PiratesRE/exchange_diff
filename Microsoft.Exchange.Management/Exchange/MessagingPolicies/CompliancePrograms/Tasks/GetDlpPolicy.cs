using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[OutputType(new Type[]
	{
		typeof(DlpPolicy)
	})]
	[Cmdlet("Get", "DlpPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetDlpPolicy : GetMultitenancySystemConfigurationObjectTask<DlpPolicyIdParameter, ADComplianceProgram>
	{
		public OptionalIdentityData IdentityData
		{
			get
			{
				return base.OptionalIdentityData;
			}
		}

		public GetDlpPolicy()
		{
			this.impl = new GetDlpPolicyImpl(this);
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity != null)
				{
					return null;
				}
				return RuleIdParameter.GetRuleCollectionId(base.DataSession, DlpUtils.TenantDlpPoliciesCollectionName);
			}
		}

		protected override void InternalValidate()
		{
			this.SetupImpl();
			this.impl.Validate();
			base.InternalValidate();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			this.SetupImpl();
			this.impl.WriteResult((IEnumerable<ADComplianceProgram>)dataObjects, new GetDlpPolicy.WriteDelegate(this.WriteResult));
		}

		private void SetupImpl()
		{
			this.impl.DataSession = base.DataSession;
			this.impl.ShouldContinue = new CmdletImplementation.ShouldContinueMethod(base.ShouldContinue);
		}

		private readonly GetDlpPolicyImpl impl;

		public delegate void WriteDelegate(IConfigurable obj);
	}
}
