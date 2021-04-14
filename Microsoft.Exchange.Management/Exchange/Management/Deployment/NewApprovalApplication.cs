using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("New", "ApprovalApplication")]
	public sealed class NewApprovalApplication : NewMultitenancyFixedNameSystemConfigurationObjectTask<ApprovalApplication>
	{
		[Parameter(Mandatory = false)]
		public RetentionPolicyTagIdParameter ELCRetentionPolicyTag
		{
			get
			{
				return (RetentionPolicyTagIdParameter)base.Fields[ApprovalApplicationSchema.ELCRetentionPolicyTag];
			}
			set
			{
				base.Fields[ApprovalApplicationSchema.ELCRetentionPolicyTag] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AutoGroup")]
		public SwitchParameter AutoGroup
		{
			get
			{
				return (SwitchParameter)(base.Fields["AutoGroup"] ?? false);
			}
			set
			{
				base.Fields["AutoGroup"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ModeratedRecipients")]
		public SwitchParameter ModeratedRecipients
		{
			get
			{
				return (SwitchParameter)(base.Fields["ModeratedRecipients"] ?? false);
			}
			set
			{
				base.Fields["ModeratedRecipients"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ApprovalApplication approvalApplication = (ApprovalApplication)base.PrepareDataObject();
			approvalApplication.Name = base.ParameterSetName;
			approvalApplication.SetId(this.ConfigurationSession, approvalApplication.Name);
			if (this.ELCRetentionPolicyTag != null)
			{
				approvalApplication.ELCRetentionPolicyTag = this.ELCRetentionPolicyTag.InternalADObjectId;
			}
			return approvalApplication;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADRawEntry adrawEntry = ((IDirectorySession)base.DataSession).ReadADRawEntry(this.DataObject.Id, new PropertyDefinition[0]);
			if (adrawEntry != null)
			{
				base.WriteVerbose(Strings.VerboseApprovalApplicationObjectExists(this.DataObject.Name));
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
