using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Cmdlet("Remove", "DlpPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDlpPolicy : RemoveSystemConfigurationObjectTask<DlpPolicyIdParameter, ADComplianceProgram>
	{
		internal ADComplianceProgram GetDataObject()
		{
			return base.DataObject;
		}

		public RemoveDlpPolicy()
		{
			this.impl = new RemoveDlpPolicyImpl(this);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUninstallDlpPolicy(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			this.SetupImpl();
			this.impl.ProcessRecord();
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = DlpPolicyIdParameter.GetDlpPolicyCollectionRdn();
			}
			base.InternalValidate();
			this.SetupImpl();
			this.impl.Validate();
		}

		private void SetupImpl()
		{
			this.impl.DataSession = new MessagingPoliciesSyncLogDataSession(base.DataSession, null, null);
			this.impl.ShouldContinue = new CmdletImplementation.ShouldContinueMethod(base.ShouldContinue);
		}

		private RemoveDlpPolicyImpl impl;
	}
}
