using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("New", "ApprovalApplicationContainer")]
	public sealed class NewApprovalApplicationContainer : NewMultitenancyFixedNameSystemConfigurationObjectTask<ApprovalApplicationContainer>
	{
		[Parameter]
		public MailboxPolicyIdParameter RetentionPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields["RetentionPolicy"];
			}
			set
			{
				base.Fields["RetentionPolicy"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.RetentionPolicy != null)
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new LocalizedException(Strings.ErrorLinkOpOnDehydratedTenant("RetentionPolicy")), ExchangeErrorCategory.Client, null);
				}
				RetentionPolicy retentionPolicy = (RetentionPolicy)base.GetDataObject<RetentionPolicy>(this.RetentionPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRetentionPolicyNotFound(this.RetentionPolicy.ToString())), new LocalizedString?(Strings.ErrorRetentionPolicyNotUnique(this.RetentionPolicy.ToString())));
				this.retentionPolicyId = (ADObjectId)retentionPolicy.Identity;
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ApprovalApplicationContainer approvalApplicationContainer = (ApprovalApplicationContainer)base.PrepareDataObject();
			approvalApplicationContainer.Name = ApprovalApplicationContainer.DefaultName;
			approvalApplicationContainer.SetId(this.ConfigurationSession, approvalApplicationContainer.Name);
			if (base.Fields.IsModified("RetentionPolicy") && this.retentionPolicyId != null)
			{
				approvalApplicationContainer.RetentionPolicy = this.retentionPolicyId;
			}
			TaskLogger.LogExit();
			return approvalApplicationContainer;
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

		private ADObjectId retentionPolicyId;
	}
}
