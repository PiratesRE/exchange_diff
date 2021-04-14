using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[OutputType(new Type[]
	{
		typeof(DlpPolicyTemplate)
	})]
	[Cmdlet("Get", "DlpPolicyTemplate", DefaultParameterSetName = "Identity")]
	public sealed class GetDlpPolicyTemplate : GetSystemConfigurationObjectTask<DlpPolicyIdParameter, ADComplianceProgram>
	{
		public OptionalIdentityData IdentityData
		{
			get
			{
				return base.OptionalIdentityData;
			}
		}

		public GetDlpPolicyTemplate()
		{
			this.impl = new GetDlpPolicyTemplateImpl(this);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 73, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\DlpPolicy\\GetDlpPolicyTemplate.cs");
		}

		protected override void InternalValidate()
		{
			this.SetupImpl();
			this.impl.Validate();
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

		private GetDlpPolicyTemplateImpl impl;

		public delegate void WriteDelegate(IConfigurable obj);
	}
}
