using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class NewDistributionGroupBase : NewMailEnabledRecipientObjectTask<ADGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewDistributionGroup(base.Name.ToString(), base.RecipientContainerId.ToString());
			}
		}

		[Parameter]
		public GroupType Type
		{
			get
			{
				return (GroupType)(base.Fields[ADGroupSchema.GroupType] ?? GroupType.Distribution);
			}
			set
			{
				base.Fields[ADGroupSchema.GroupType] = value;
			}
		}

		[Parameter]
		public string SamAccountName
		{
			get
			{
				return this.DataObject.SamAccountName;
			}
			set
			{
				this.DataObject.SamAccountName = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<GeneralRecipientIdParameter> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<GeneralRecipientIdParameter>)base.Fields[DistributionGroupSchema.ManagedBy];
			}
			set
			{
				base.Fields[DistributionGroupSchema.ManagedBy] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>> Members
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>)base.Fields[ADGroupSchema.Members];
			}
			set
			{
				base.Fields[ADGroupSchema.Members] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MemberUpdateType MemberJoinRestriction
		{
			get
			{
				return this.DataObject.MemberJoinRestriction;
			}
			set
			{
				this.DataObject.MemberJoinRestriction = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MemberUpdateType MemberDepartRestriction
		{
			get
			{
				return this.DataObject.MemberDepartRestriction;
			}
			set
			{
				this.DataObject.MemberDepartRestriction = value;
			}
		}

		[Parameter]
		public bool BypassNestedModerationEnabled
		{
			get
			{
				return this.DataObject.BypassNestedModerationEnabled;
			}
			set
			{
				this.DataObject.BypassNestedModerationEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return (string)this.DataObject[ADRecipientSchema.Notes];
			}
			set
			{
				this.DataObject[ADRecipientSchema.Notes] = value;
			}
		}

		[Parameter]
		public SwitchParameter CopyOwnerToMember
		{
			get
			{
				return (SwitchParameter)(base.Fields["CopyOwnerToMember"] ?? false);
			}
			set
			{
				base.Fields["CopyOwnerToMember"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RoomList
		{
			get
			{
				return (SwitchParameter)(base.Fields["RoomList"] ?? false);
			}
			set
			{
				base.Fields["RoomList"] = value;
			}
		}

		public NewDistributionGroupBase()
		{
		}

		protected override void StampDefaultValues(ADGroup dataObject)
		{
			base.StampDefaultValues(dataObject);
			dataObject.StampDefaultValues(RecipientType.MailUniversalSecurityGroup);
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreNamingPolicy
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreNamingPolicy"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreNamingPolicy"] = value;
			}
		}

		protected override void PrepareRecipientObject(ADGroup group)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(group);
			Organization organization;
			if (base.Organization == null)
			{
				organization = this.ConfigurationSession.GetOrgContainer();
			}
			else
			{
				organization = this.ConfigurationSession.Read<ExchangeConfigurationUnit>(base.CurrentOrgContainerId);
			}
			ADObjectId adobjectId = null;
			base.TryGetExecutingUserId(out adobjectId);
			if (!this.IgnoreNamingPolicy.IsPresent && adobjectId != null)
			{
				ADUser user = (ADUser)RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(null, adobjectId).Read(adobjectId);
				string groupNameWithNamingPolicy = DistributionGroupTaskHelper.GetGroupNameWithNamingPolicy(organization, user, group, base.Name, ADObjectSchema.Name, new Task.ErrorLoggerDelegate(base.WriteError));
				if (groupNameWithNamingPolicy.Length > 64)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorDistributionGroupNameTooLong), ExchangeErrorCategory.Client, null);
				}
				base.Name = groupNameWithNamingPolicy;
				if (!string.IsNullOrEmpty(base.DisplayName))
				{
					base.DisplayName = DistributionGroupTaskHelper.GetGroupNameWithNamingPolicy(organization, user, group, base.DisplayName, ADRecipientSchema.DisplayName, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (base.OrganizationalUnit == null && !ADObjectId.IsNullOrEmpty(organization.DistributionGroupDefaultOU))
			{
				group.SetId(organization.DistributionGroupDefaultOU.GetChildId(base.Name));
			}
			if (base.OrganizationalUnit == null && group[ADRecipientSchema.DefaultDistributionListOU] != null)
			{
				ADObjectId adobjectId2 = (ADObjectId)group[ADRecipientSchema.DefaultDistributionListOU];
				RecipientTaskHelper.ResolveOrganizationalUnitInOrganization(new OrganizationalUnitIdParameter(adobjectId2), this.ConfigurationSession, base.CurrentOrganizationId, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), ExchangeErrorCategory.Client, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				group.SetId(adobjectId2.GetChildId(base.Name));
			}
			if (this.Type != GroupType.Distribution && this.Type != GroupType.Security)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorGroupTypeInvalid), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsModified(DistributionGroupSchema.ManagedBy))
			{
				MailboxTaskHelper.StampOnManagedBy(this.DataObject, this.managedByRecipients, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (this.RoomList.IsPresent)
			{
				if (this.Type != GroupType.Distribution)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCreateRoomListSecurityGroup(base.Name)), ExchangeErrorCategory.Client, base.Name);
				}
				group.RecipientTypeDetails = RecipientTypeDetails.RoomList;
				if (group.ManagedBy != null)
				{
					group.AcceptMessagesOnlyFromSendersOrMembers = new MultiValuedProperty<ADObjectId>(group.ManagedBy);
				}
			}
			MailboxTaskHelper.ValidateGroupManagedBy(base.TenantGlobalCatalogSession, group, this.managedByRecipients, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError));
			MailboxTaskHelper.ValidateGroupManagedByRecipientRestriction(base.TenantGlobalCatalogSession, group, this.managedByRecipients, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			group.GroupType = (GroupTypeFlags)((GroupType)8 | this.Type);
			if (!group.IsChanged(ADRecipientSchema.RecipientDisplayType))
			{
				if ((group.GroupType & GroupTypeFlags.SecurityEnabled) == GroupTypeFlags.SecurityEnabled)
				{
					group.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.SecurityDistributionGroup);
				}
				else
				{
					group.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.DistributionGroup);
				}
			}
			if (string.IsNullOrEmpty(group.SamAccountName))
			{
				IRecipientSession[] recipientSessions = new IRecipientSession[]
				{
					base.RootOrgGlobalCatalogSession
				};
				if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled && base.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
				{
					recipientSessions = new IRecipientSession[]
					{
						base.RootOrgGlobalCatalogSession,
						base.PartitionOrRootOrgGlobalCatalogSession
					};
				}
				group.SamAccountName = RecipientTaskHelper.GenerateUniqueSamAccountName(recipientSessions, group.Id.DomainId, group.Name, true, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), false);
			}
			else
			{
				RecipientTaskHelper.IsSamAccountNameUnique(group, group.SamAccountName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
			if (string.IsNullOrEmpty(group.Alias))
			{
				group.Alias = RecipientTaskHelper.GenerateUniqueAlias(base.TenantGlobalCatalogSession, base.CurrentOrganizationId, group.SamAccountName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (base.Fields.IsChanged(ADGroupSchema.Members) && this.Members != null)
			{
				foreach (RecipientIdParameter member in this.Members)
				{
					MailboxTaskHelper.ValidateAndAddMember(base.TenantGlobalCatalogSession, group, member, false, new Task.ErrorLoggerDelegate(base.WriteError), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>));
				}
			}
			if (this.CopyOwnerToMember.IsPresent && this.managedByRecipients != null)
			{
				foreach (ADRecipient adrecipient in this.managedByRecipients)
				{
					if (!group.Members.Contains(adrecipient.Id))
					{
						MailboxTaskHelper.ValidateMemberInGroup(adrecipient, group, new Task.ErrorLoggerDelegate(base.WriteError));
						group.Members.Add(adrecipient.Id);
					}
				}
			}
			if ((group.GroupType & GroupTypeFlags.Universal) == GroupTypeFlags.Universal)
			{
				MailboxTaskHelper.ValidateAddedMembers(base.TenantGlobalCatalogSession, group, new Task.ErrorLoggerDelegate(base.WriteError), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>));
			}
			if (!this.DataObject.IsModified(ADGroupSchema.MemberDepartRestriction))
			{
				this.DataObject.MemberDepartRestriction = ((this.Type == GroupType.Security) ? MemberUpdateType.Closed : MemberUpdateType.Open);
			}
			if (group.ArbitrationMailbox == null)
			{
				group.ArbitrationMailbox = MailboxTaskHelper.GetArbitrationMailbox(base.TenantGlobalCatalogSession, base.CurrentOrgContainerId);
				if (group.ArbitrationMailbox == null)
				{
					if (group.MemberJoinRestriction == MemberUpdateType.ApprovalRequired || group.MemberDepartRestriction == MemberUpdateType.ApprovalRequired)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorArbitrationMbxNotSetForApproval(base.Name)), ExchangeErrorCategory.Client, group.Identity);
					}
					if (group.ModerationEnabled)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorArbitrationMbxNotSetForModeration(base.Name)), ExchangeErrorCategory.Client, group.Identity);
					}
				}
			}
			DistributionGroupTaskHelper.CheckMembershipRestriction(group, new Task.ErrorLoggerDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.EmailAddressPolicy.Enabled)
			{
				this.DataObject.EmailAddressPolicyEnabled = false;
			}
			DistributionGroupTaskHelper.CheckModerationInMixedEnvironment(this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning), Strings.WarningLegacyExchangeServer);
			TaskLogger.LogExit();
		}

		protected MultiValuedProperty<ADRecipient> managedByRecipients;

		internal delegate IConfigurable GetUniqueObject(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError);
	}
}
