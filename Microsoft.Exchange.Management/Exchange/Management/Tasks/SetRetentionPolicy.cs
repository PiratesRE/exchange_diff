using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("set", "RetentionPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetRetentionPolicy : SetMailboxPolicyBase<RetentionPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetRetentionPolicy(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

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

		[Parameter(Mandatory = false)]
		public RetentionPolicyTagIdParameter[] RetentionPolicyTagLinks
		{
			get
			{
				return (RetentionPolicyTagIdParameter[])base.Fields["RetentionPolicyLinks"];
			}
			set
			{
				base.Fields["RetentionPolicyLinks"] = value;
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

		protected override IConfigurable ResolveDataObject()
		{
			if (!this.IgnoreDehydratedFlag)
			{
				SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			return base.ResolveDataObject();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.IsDefault && this.IsDefaultArbitrationMailbox)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMultipleDefaultRetentionPolicy), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			if (this.IsDefault)
			{
				this.DataObject.IsDefaultArbitrationMailbox = false;
				this.DataObject.IsDefault = true;
				this.otherDefaultPolicies = RetentionPolicyUtility.GetDefaultPolicies((IConfigurationSession)base.DataSession, false);
			}
			else if (this.IsDefaultArbitrationMailbox)
			{
				this.DataObject.IsDefault = false;
				this.DataObject.IsDefaultArbitrationMailbox = true;
				this.otherDefaultPolicies = RetentionPolicyUtility.GetDefaultPolicies((IConfigurationSession)base.DataSession, true);
			}
			else if ((!this.IsDefault && base.Fields.IsChanged("IsDefault") && this.DataObject.IsDefault) || (!this.IsDefaultArbitrationMailbox && base.Fields.IsChanged("IsDefaultArbitrationMailbox") && this.DataObject.IsDefaultArbitrationMailbox))
			{
				base.WriteError(new InvalidOperationException(Strings.ResettingIsDefaultIsNotSupported(this.DataObject.IsDefault ? "IsDefault" : "IsDefaultArbitrationMailbox", "RetentionPolicy")), ErrorCategory.WriteError, this.DataObject);
			}
			if (this.otherDefaultPolicies != null && this.otherDefaultPolicies.Count > 0)
			{
				this.updateOtherDefaultPolicies = true;
			}
			if (base.Fields.IsModified("RetentionPolicyLinks"))
			{
				this.DataObject.RetentionPolicyTagLinks.Clear();
				if (this.RetentionPolicyTagLinks != null)
				{
					PresentationRetentionPolicyTag[] array = (from x in (from x in this.RetentionPolicyTagLinks
					select (RetentionPolicyTag)base.GetDataObject<RetentionPolicyTag>(x, base.DataSession, null, new LocalizedString?(Strings.ErrorRetentionTagNotFound(x.ToString())), new LocalizedString?(Strings.ErrorAmbiguousRetentionPolicyTagId(x.ToString())))).Distinct<RetentionPolicyTag>()
					select new PresentationRetentionPolicyTag(x)).ToArray<PresentationRetentionPolicyTag>();
					RetentionPolicyValidator.ValicateDefaultTags(this.DataObject, array, new Task.TaskErrorLoggingDelegate(base.WriteError));
					RetentionPolicyValidator.ValidateSystemFolderTags(this.DataObject, array, new Task.TaskErrorLoggingDelegate(base.WriteError));
					array.ForEach(delegate(PresentationRetentionPolicyTag x)
					{
						this.DataObject.RetentionPolicyTagLinks.Add(x.Id);
					});
				}
			}
			string policyName;
			if (this.DataObject.IsChanged(RetentionPolicySchema.RetentionId) && !(base.DataSession as IConfigurationSession).CheckForRetentionPolicyWithConflictingRetentionId(this.DataObject.RetentionId, this.DataObject.Identity.ToString(), out policyName))
			{
				base.WriteError(new RetentionPolicyTagTaskException(Strings.ErrorRetentionIdConflictsWithRetentionPolicy(this.DataObject.RetentionId.ToString(), policyName)), ErrorCategory.InvalidOperation, this.DataObject);
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
			if (this.DataObject.IsChanged(RetentionPolicySchema.RetentionId) && !this.Force && !base.ShouldContinue(Strings.WarningRetentionPolicyIdChange(this.DataObject.Identity.ToString())))
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.updateOtherDefaultPolicies)
			{
				if (!base.ShouldContinue(Strings.ConfirmationMessageSwitchMailboxPolicy("RetentionPolicy", this.Identity.ToString())))
				{
					return;
				}
				try
				{
					RetentionPolicyUtility.ClearDefaultPolicies(base.DataSession as IConfigurationSession, this.otherDefaultPolicies, this.IsDefaultArbitrationMailbox);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
