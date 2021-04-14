using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "RetentionPolicy", SupportsShouldProcess = true)]
	public sealed class NewRetentionPolicy : NewMailboxPolicyBase<RetentionPolicy>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid RetentionId
		{
			get
			{
				return (Guid)base.Fields["RetentionId"];
			}
			set
			{
				base.Fields["RetentionId"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewRetentionPolicy(base.Name.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public RetentionPolicyTagIdParameter[] RetentionPolicyTagLinks
		{
			get
			{
				return (RetentionPolicyTagIdParameter[])base.Fields["PolicyTagLinks"];
			}
			set
			{
				base.Fields["PolicyTagLinks"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefault
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefault"] ?? false);
			}
			set
			{
				base.Fields["IsDefault"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefaultArbitrationMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefaultArbitrationMailbox"] ?? false);
			}
			set
			{
				base.Fields["IsDefaultArbitrationMailbox"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (Datacenter.IsMicrosoftHostedOnly(false))
			{
				List<RetentionPolicy> allRetentionPolicies = AdPolicyReader.GetAllRetentionPolicies(this.ConfigurationSession, base.OrganizationId);
				if (allRetentionPolicies.Count >= 100)
				{
					base.WriteError(new RetentionPolicyTagTaskException(Strings.ErrorTenantRetentionPolicyLimitReached(100)), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			if (this.IsDefault && this.IsDefaultArbitrationMailbox)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMultipleDefaultRetentionPolicy), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			if (!this.IgnoreDehydratedFlag && SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
			{
				base.WriteError(new ArgumentException(Strings.ErrorWriteOpOnDehydratedTenant), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			if (this.IsDefault && this.IsDefaultArbitrationMailbox)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMultipleDefaultRetentionPolicy), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			if (this.IsDefault)
			{
				this.DataObject.IsDefault = true;
				this.existingDefaultPolicies = RetentionPolicyUtility.GetDefaultPolicies((IConfigurationSession)base.DataSession, false);
			}
			else if (this.IsDefaultArbitrationMailbox)
			{
				this.DataObject.IsDefaultArbitrationMailbox = true;
				this.existingDefaultPolicies = RetentionPolicyUtility.GetDefaultPolicies((IConfigurationSession)base.DataSession, true);
			}
			if (this.existingDefaultPolicies != null && this.existingDefaultPolicies.Count > 0)
			{
				this.updateExistingDefaultPolicies = true;
			}
			if (this.RetentionPolicyTagLinks != null)
			{
				this.DataObject.RetentionPolicyTagLinks.Clear();
				PresentationRetentionPolicyTag[] array = (from x in (from x in this.RetentionPolicyTagLinks
				select (RetentionPolicyTag)base.GetDataObject<RetentionPolicyTag>(x, base.DataSession, null, new LocalizedString?(Strings.ErrorRetentionTagNotFound(x.ToString())), new LocalizedString?(Strings.ErrorAmbiguousRetentionPolicyTagId(x.ToString())))).Distinct(new ADObjectComparer<RetentionPolicyTag>())
				select new PresentationRetentionPolicyTag(x)).ToArray<PresentationRetentionPolicyTag>();
				RetentionPolicyValidator.ValicateDefaultTags(this.DataObject, array, new Task.TaskErrorLoggingDelegate(base.WriteError));
				RetentionPolicyValidator.ValidateSystemFolderTags(this.DataObject, array, new Task.TaskErrorLoggingDelegate(base.WriteError));
				array.ForEach(delegate(PresentationRetentionPolicyTag x)
				{
					this.DataObject.RetentionPolicyTagLinks.Add(x.Id);
				});
			}
			if (base.Fields.Contains("RetentionId"))
			{
				this.DataObject.RetentionId = this.RetentionId;
				string policyName;
				if (!(base.DataSession as IConfigurationSession).CheckForRetentionPolicyWithConflictingRetentionId(this.DataObject.RetentionId, out policyName))
				{
					base.WriteError(new RetentionPolicyTagTaskException(Strings.ErrorRetentionIdConflictsWithRetentionPolicy(this.DataObject.RetentionId.ToString(), policyName)), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.DataObject != null && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.updateExistingDefaultPolicies)
			{
				if (!base.ShouldContinue(Strings.ConfirmationMessageSwitchMailboxPolicy("RetentionPolicy", this.DataObject.Name)))
				{
					return;
				}
				try
				{
					RetentionPolicyUtility.ClearDefaultPolicies(base.DataSession as IConfigurationSession, this.existingDefaultPolicies, this.IsDefaultArbitrationMailbox);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private const int TenantPolicyLimit = 100;
	}
}
