using System;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "Group", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetGroup : SetRecipientObjectTask<GroupIdParameter, WindowsGroup, ADGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetGroup(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Universal", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override GroupIdParameter Identity
		{
			get
			{
				return (GroupIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public GeneralRecipientIdParameter[] ManagedBy
		{
			get
			{
				return (GeneralRecipientIdParameter[])base.Fields[WindowsGroupSchema.ManagedBy];
			}
			set
			{
				base.Fields[WindowsGroupSchema.ManagedBy] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Universal")]
		public SwitchParameter Universal
		{
			get
			{
				return (SwitchParameter)base.Fields["Universal"];
			}
			set
			{
				base.Fields["Universal"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter BypassSecurityGroupManagerCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassSecurityGroupManagerCheck"] ?? false);
			}
			set
			{
				base.Fields["BypassSecurityGroupManagerCheck"] = value;
			}
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return adObject.IsChanged(ADGroupSchema.IsOrganizationalGroup) || base.Fields.IsModified(WindowsGroupSchema.IsHierarchicalGroup);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.Fields.IsModified(WindowsGroupSchema.ManagedBy) && (this.ManagedBy == null || this.ManagedBy.Length == 0))
			{
				base.WriteError(new RecipientTaskException(Strings.AutoGroupManagedByCannotBeEmpty), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			if (base.Fields.IsModified(WindowsGroupSchema.ManagedBy))
			{
				this.managedByRecipients = new MultiValuedProperty<ADRecipient>();
				if (this.ManagedBy != null)
				{
					foreach (GeneralRecipientIdParameter recipientIdParameter in this.ManagedBy)
					{
						ADRecipient item = (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())));
						this.managedByRecipients.Add(item);
					}
				}
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADGroup adgroup = (ADGroup)base.PrepareDataObject();
			if (base.Fields.IsModified(DistributionGroupSchema.ManagedBy))
			{
				if (!this.BypassSecurityGroupManagerCheck && GroupTypeFlags.SecurityEnabled == (adgroup.GroupType & GroupTypeFlags.SecurityEnabled))
				{
					ADObjectId userId;
					if (!base.TryGetExecutingUserId(out userId))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorExecutingUserOutOfTargetOrg(base.MyInvocation.MyCommand.Name)), ExchangeErrorCategory.Client, adgroup.Identity.ToString());
					}
					RecipientTaskHelper.ValidateUserIsGroupManager(userId, adgroup, new Task.ErrorLoggerDelegate(base.WriteError), false, null);
				}
				MailboxTaskHelper.StampOnManagedBy(adgroup, this.managedByRecipients, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
			return adgroup;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			MailboxTaskHelper.ValidateGroupManagedBy(base.TenantGlobalCatalogSession, this.DataObject, this.managedByRecipients, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError));
			if (this.DataObject.IsModified(ADMailboxRecipientSchema.SamAccountName))
			{
				RecipientTaskHelper.IsSamAccountNameUnique(this.DataObject, this.DataObject.SamAccountName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
			if (base.ParameterSetName == "Universal")
			{
				if ((this.DataObject.GroupType & GroupTypeFlags.Universal) == GroupTypeFlags.Universal)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorIsUniversalGroupAlready(this.DataObject.Name)), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				}
				else
				{
					if ((this.DataObject.GroupType & GroupTypeFlags.BuiltinLocal) == GroupTypeFlags.BuiltinLocal || SetGroup.IsBuiltInObject(this.DataObject))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorCannotConvertBuiltInGroup(this.DataObject.Name)), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					}
					GroupTypeFlags groupTypeFlags = (GroupTypeFlags)7;
					this.DataObject.GroupType = ((this.DataObject.GroupType & ~groupTypeFlags) | GroupTypeFlags.Universal);
					base.DesiredRecipientType = this.DataObject.RecipientType;
				}
			}
			if (this.DataObject.IsChanged(ADGroupSchema.Members) || base.ParameterSetName == "Universal")
			{
				MailboxTaskHelper.ValidateAddedMembers(base.TenantGlobalCatalogSession, this.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>));
			}
			TaskLogger.LogExit();
		}

		private static bool IsBuiltInObject(IADSecurityPrincipal securityPrincipal)
		{
			bool result = false;
			if (securityPrincipal.IsSecurityPrincipal)
			{
				foreach (object obj in Enum.GetValues(typeof(WellKnownSidType)))
				{
					WellKnownSidType type = (WellKnownSidType)obj;
					if (securityPrincipal.Sid.IsWellKnown(type))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return WindowsGroup.FromDataObject((ADGroup)dataObject);
		}

		private const string ParameterSetUniversal = "Universal";

		private const string ParameterUniversal = "Universal";

		private MultiValuedProperty<ADRecipient> managedByRecipients;
	}
}
