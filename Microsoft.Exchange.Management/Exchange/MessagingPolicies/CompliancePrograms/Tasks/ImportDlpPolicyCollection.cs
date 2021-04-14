using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Cmdlet("Import", "DlpPolicyCollection", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ImportDlpPolicyCollection : GetMultitenancySystemConfigurationObjectTask<DlpPolicyIdParameter, ADComplianceProgram>
	{
		public ImportDlpPolicyCollection()
		{
			this.impl = new ImportDlpPolicyCollectionImpl(this);
		}

		[Parameter(Mandatory = true, Position = 0)]
		public byte[] FileData
		{
			get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageImportDlpPolicy;
			}
		}

		protected override void InternalProcessRecord()
		{
			this.SetupImpl();
			this.impl.ProcessRecord();
		}

		protected override void InternalValidate()
		{
			this.SetupImpl();
			this.impl.Validate();
			base.InternalValidate();
		}

		private void SetupImpl()
		{
			this.impl.DataSession = new MessagingPoliciesSyncLogDataSession(base.DataSession, null, null);
			this.impl.ShouldContinue = new CmdletImplementation.ShouldContinueMethod(base.ShouldContinue);
		}

		private ImportDlpPolicyCollectionImpl impl;
	}
}
