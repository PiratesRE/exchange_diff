using System;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "GroupMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class SetGroupMailbox : SetRecipientObjectTask<RecipientIdParameter, GroupMailbox, ADUser>
	{
		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)base.Fields[ADObjectSchema.Name];
			}
			set
			{
				base.Fields[ADObjectSchema.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)base.Fields[ADRecipientSchema.DisplayName];
			}
			set
			{
				base.Fields[ADRecipientSchema.DisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Description
		{
			get
			{
				return (string)base.Fields[ADRecipientSchema.Description];
			}
			set
			{
				base.Fields[ADRecipientSchema.Description] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public RecipientIdParameter ExecutingUser
		{
			get
			{
				return (RecipientIdParameter)base.Fields["ExecutingUser"];
			}
			set
			{
				base.Fields["ExecutingUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] Owners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[ADUserSchema.Owners];
			}
			set
			{
				base.Fields[ADUserSchema.Owners] = (value ?? new RecipientIdParameter[0]);
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AddOwners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AddOwners"];
			}
			set
			{
				base.Fields["AddOwners"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] RemoveOwners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["RemoveOwners"];
			}
			set
			{
				base.Fields["RemoveOwners"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AddedMembers
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AddedMembers"];
			}
			set
			{
				base.Fields["AddedMembers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] RemovedMembers
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["RemovedMembers"];
			}
			set
			{
				base.Fields["RemovedMembers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri SharePointUrl
		{
			get
			{
				return (Uri)base.Fields[ADMailboxRecipientSchema.SharePointUrl];
			}
			set
			{
				base.Fields[ADMailboxRecipientSchema.SharePointUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> SharePointResources
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields[ADMailboxRecipientSchema.SharePointResources];
			}
			set
			{
				base.Fields[ADMailboxRecipientSchema.SharePointResources] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public ModernGroupTypeInfo SwitchToGroupType
		{
			get
			{
				return (ModernGroupTypeInfo)base.Fields[ADRecipientSchema.ModernGroupType];
			}
			set
			{
				base.Fields[ADRecipientSchema.ModernGroupType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return (bool)base.Fields["RequireSenderAuthenticationEnabled"];
			}
			set
			{
				base.Fields["RequireSenderAuthenticationEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string YammerGroupEmailAddress
		{
			get
			{
				return (string)base.Fields["YammerGroupEmailAddress"];
			}
			set
			{
				base.Fields["YammerGroupEmailAddress"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdType RecipientIdType
		{
			get
			{
				return (RecipientIdType)base.Fields["RecipientIdType"];
			}
			set
			{
				base.Fields["RecipientIdType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter FromSyncClient
		{
			get
			{
				return (SwitchParameter)(base.Fields["FromSyncClient"] ?? false);
			}
			set
			{
				base.Fields["FromSyncClient"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		private RecipientIdParameter[] PublicToGroups
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[ADMailboxRecipientSchema.DelegateListLink];
			}
			set
			{
				base.Fields[ADMailboxRecipientSchema.DelegateListLink] = (value ?? new RecipientIdParameter[0]);
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)base.Fields[GroupMailboxSchema.PrimarySmtpAddress];
			}
			set
			{
				base.Fields[GroupMailboxSchema.PrimarySmtpAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)base.Fields[GroupMailboxSchema.EmailAddresses];
			}
			set
			{
				base.Fields[GroupMailboxSchema.EmailAddresses] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)base.Fields[ADRecipientSchema.ExternalDirectoryObjectId];
			}
			set
			{
				base.Fields[ADRecipientSchema.ExternalDirectoryObjectId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ForcePublishExternalResources
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForcePublishExternalResources"] ?? false);
			}
			set
			{
				base.Fields["ForcePublishExternalResources"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<GroupMailboxConfigurationActionType> ConfigurationActions
		{
			get
			{
				return (MultiValuedProperty<GroupMailboxConfigurationActionType>)base.Fields["ConfigurationActions"];
			}
			set
			{
				base.Fields["ConfigurationActions"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)base.Fields["Language"];
			}
			set
			{
				base.Fields["Language"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AutoSubscribeNewGroupMembers
		{
			get
			{
				return (SwitchParameter)base.Fields["AutoSubscribeNewGroupMembers"];
			}
			set
			{
				base.Fields["AutoSubscribeNewGroupMembers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PermissionsVersion
		{
			get
			{
				return (int)base.Fields["PermissionsVersion"];
			}
			set
			{
				base.Fields["PermissionsVersion"] = value;
			}
		}

		private new SwitchParameter IgnoreDefaultScope { get; set; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.Fields.IsModified(ADMailboxRecipientSchema.SharePointUrl))
			{
				Uri sharePointUrl = this.SharePointUrl;
				if (sharePointUrl != null && (!sharePointUrl.IsAbsoluteUri || (!(sharePointUrl.Scheme == Uri.UriSchemeHttps) && !(sharePointUrl.Scheme == Uri.UriSchemeHttp))))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxSharePointUrl), ExchangeErrorCategory.Client, null);
				}
			}
			if (base.Fields.IsChanged(ADMailboxRecipientSchema.DelegateListLink) && this.DataObject.ModernGroupType != ModernGroupObjectType.Public)
			{
				base.WriteError(new GroupMailboxInvalidOperationException(Strings.ErrorInvalidGroupTypeForPublicToGroups), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsChanged(ADUserSchema.Owners) && this.Owners != null && this.Owners.Length == 0)
			{
				base.WriteError(new GroupMailboxInvalidOperationException(Strings.ErrorSetGroupMailboxNoOwners), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsChanged("YammerGroupEmailAddress") && !string.IsNullOrEmpty(this.YammerGroupEmailAddress) && !ProxyAddressBase.IsAddressStringValid(this.YammerGroupEmailAddress))
			{
				base.WriteError(new GroupMailboxInvalidOperationException(Strings.ErrorSetGroupMailboxInvalidYammerEmailAddress(this.YammerGroupEmailAddress)), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsChanged(GroupMailboxSchema.EmailAddresses) && base.Fields.IsChanged(GroupMailboxSchema.PrimarySmtpAddress))
			{
				base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorPrimarySmtpAndEmailAddressesSpecified), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsChanged(ADRecipientSchema.ExternalDirectoryObjectId) && !string.IsNullOrEmpty(this.ExternalDirectoryObjectId) && !RecipientTaskHelper.IsPropertyValueUnique(base.TenantGlobalCatalogSession, ADScope.Empty, null, new ADPropertyDefinition[]
			{
				ADRecipientSchema.ExternalDirectoryObjectId
			}, ADRecipientSchema.ExternalDirectoryObjectId, this.ExternalDirectoryObjectId, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, ExchangeErrorCategory.Client))
			{
				base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorExternalDirectoryObjectIdNotUnique(this.ExternalDirectoryObjectId)), ExchangeErrorCategory.Client, null);
			}
		}

		private bool MembersChanged
		{
			get
			{
				return (base.Fields.IsModified("AddedMembers") && this.AddedMembers != null && this.AddedMembers.Length > 0) || (base.Fields.IsModified("RemovedMembers") && this.RemovedMembers != null && this.RemovedMembers.Length > 0);
			}
		}

		private bool OwnersChanged
		{
			get
			{
				return (base.Fields.IsChanged("AddOwners") && this.AddOwners != null && this.AddOwners.Length > 0) || (base.Fields.IsChanged("RemoveOwners") && this.RemoveOwners != null && this.RemoveOwners.Length > 0) || (base.Fields.IsChanged(ADUserSchema.Owners) && this.Owners != null && this.Owners.Length > 0);
			}
		}

		internal bool RequireSenderAuthenticationEnabledChanged
		{
			get
			{
				return base.Fields.IsChanged("RequireSenderAuthenticationEnabled");
			}
		}

		protected override void InternalProcessRecord()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.DataSession;
			this.groupMailboxContext = new GroupMailboxContext(this.DataObject, base.CurrentOrganizationId, recipientSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADGroup>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.ErrorLoggerDelegate(base.WriteError));
			if (this.ExecutingUser != null)
			{
				this.groupMailboxContext.SetExecutingUser(this.ExecutingUser);
				this.executingUserIsOwner = this.DataObject.Owners.Any((ADObjectId ownerId) => ADObjectId.Equals(this.groupMailboxContext.ExecutingUser.Id, ownerId));
			}
			else
			{
				this.executingUserIsOwner = true;
			}
			if (base.Fields.IsChanged(ADObjectSchema.Name))
			{
				this.ThrowIfNotOwner("Name");
				this.DataObject.Name = (string)base.Fields[ADObjectSchema.Name];
			}
			if (base.Fields.IsChanged(ADRecipientSchema.DisplayName))
			{
				this.ThrowIfNotOwner("DisplayName");
				this.DataObject.DisplayName = (string)base.Fields[ADRecipientSchema.DisplayName];
			}
			if (base.Fields.IsChanged(ADRecipientSchema.Description))
			{
				this.ThrowIfNotOwner("Description");
				this.DataObject.Description.Clear();
				this.DataObject.Description.Add((string)base.Fields[ADRecipientSchema.Description]);
			}
			bool flag = false;
			if (base.Fields.IsChanged(ADUserSchema.Owners) && this.Owners != null)
			{
				this.ThrowIfNotOwner("Owners");
				flag |= this.groupMailboxContext.SetOwners(this.Owners);
			}
			if (base.Fields.IsModified("AddOwners") && this.AddOwners != null)
			{
				this.ThrowIfNotOwner("AddOwners");
				flag |= this.groupMailboxContext.AddOwners(this.AddOwners);
			}
			if (base.Fields.IsModified("RemoveOwners") && this.RemoveOwners != null)
			{
				this.ThrowIfNotOwner("RemoveOwners");
				flag |= this.groupMailboxContext.RemoveOwners(this.RemoveOwners);
			}
			if (base.Fields.IsChanged(ADMailboxRecipientSchema.DelegateListLink))
			{
				this.ThrowIfNotOwner("PublicToGroups");
				this.DataObject.DelegateListLink.Clear();
				this.groupMailboxContext.AddPublicToGroups(this.PublicToGroups);
			}
			if (base.Fields.IsChanged(ADRecipientSchema.ModernGroupType))
			{
				this.DataObject.ModernGroupType = (ModernGroupObjectType)base.Fields[ADRecipientSchema.ModernGroupType];
			}
			if (base.Fields.IsChanged("RequireSenderAuthenticationEnabled"))
			{
				this.ThrowIfNotOwner("RequireSenderAuthenticationEnabled");
				this.DataObject.RequireAllSendersAreAuthenticated = this.RequireSenderAuthenticationEnabled;
			}
			if (base.Fields.IsChanged("YammerGroupEmailAddress"))
			{
				this.DataObject.YammerGroupAddress = this.YammerGroupEmailAddress;
			}
			if (base.Fields.IsChanged("AutoSubscribeNewGroupMembers"))
			{
				this.ThrowIfNotOwner("AutoSubscribeNewGroupMembers");
				this.DataObject.AutoSubscribeNewGroupMembers = this.AutoSubscribeNewGroupMembers;
			}
			if (base.Fields.IsChanged("Language") && this.Language != null)
			{
				this.DataObject.Languages.Clear();
				this.DataObject.Languages.Add(this.Language);
			}
			if (this.MembersChanged)
			{
				this.AuthorizeAddedAndRemovedMembers(this.groupMailboxContext.ExecutingUser);
				this.groupMailboxContext.AddAndRemoveMembers(this.AddedMembers, this.RemovedMembers);
			}
			if (base.Fields.IsModified(ADMailboxRecipientSchema.SharePointUrl))
			{
				this.ThrowIfNotOwner("SharePointUrl");
				if (this.DataObject.SharePointResources == null)
				{
					this.DataObject.SharePointResources = new MultiValuedProperty<string>();
				}
				else
				{
					foreach (string text in this.DataObject.SharePointResources)
					{
						if (text.StartsWith("SiteUrl=", StringComparison.OrdinalIgnoreCase))
						{
							this.DataObject.SharePointResources.Remove(text);
							break;
						}
					}
				}
				if (this.SharePointUrl != null)
				{
					this.DataObject.SharePointResources.Add("SiteUrl=" + this.SharePointUrl);
				}
			}
			if (base.Fields.IsModified(ADMailboxRecipientSchema.SharePointResources))
			{
				this.ThrowIfNotOwner("SharePointResources");
				this.DataObject.SharePointResources = this.SharePointResources;
				this.DataObject.SharePointUrl = null;
			}
			if (base.Fields.IsModified("PermissionsVersion"))
			{
				this.groupMailboxContext.SetPermissionsVersion(this.PermissionsVersion);
			}
			GroupMailboxConfigurationActionType groupMailboxConfigurationActionType = (GroupMailboxConfigurationActionType)0;
			if (base.Fields.IsModified("ConfigurationActions") && this.ConfigurationActions != null)
			{
				foreach (GroupMailboxConfigurationActionType groupMailboxConfigurationActionType2 in this.ConfigurationActions)
				{
					groupMailboxConfigurationActionType |= groupMailboxConfigurationActionType2;
				}
			}
			Exception ex;
			ExchangeErrorCategory? exchangeErrorCategory;
			this.groupMailboxContext.SetGroupMailbox(groupMailboxConfigurationActionType, out ex, out exchangeErrorCategory);
			if (ex != null)
			{
				base.WriteError(new GroupMailboxFailedToLogonException(Strings.ErrorUnableToLogonGroupMailbox(this.DataObject.ExchangeGuid, string.Empty, recipientSession.LastUsedDc, ex.Message)), exchangeErrorCategory.GetValueOrDefault(ExchangeErrorCategory.ServerTransient), null);
			}
			bool flag2 = false;
			if (!this.FromSyncClient)
			{
				if (base.Fields.IsChanged(GroupMailboxSchema.PrimarySmtpAddress))
				{
					this.DataObject.PrimarySmtpAddress = this.PrimarySmtpAddress;
					flag2 = true;
				}
				if (base.Fields.IsChanged(GroupMailboxSchema.EmailAddresses))
				{
					this.DataObject.EmailAddresses = this.EmailAddresses;
					flag2 = true;
				}
			}
			else if (base.Fields.IsChanged(GroupMailboxSchema.EmailAddresses))
			{
				foreach (ProxyAddress proxyAddress in this.EmailAddresses)
				{
					if (!this.DataObject.EmailAddresses.Contains(proxyAddress))
					{
						if (proxyAddress.IsPrimaryAddress && proxyAddress is SmtpProxyAddress)
						{
							this.DataObject.EmailAddresses.Add(proxyAddress.ToSecondary());
						}
						else
						{
							this.DataObject.EmailAddresses.Add(proxyAddress);
						}
						flag2 = true;
					}
				}
			}
			if (flag2 && this.DataObject.EmailAddressPolicyEnabled)
			{
				this.DataObject.EmailAddressPolicyEnabled = false;
			}
			if (base.Fields.IsModified(ADRecipientSchema.ExternalDirectoryObjectId))
			{
				this.DataObject.ExternalDirectoryObjectId = this.ExternalDirectoryObjectId;
			}
			base.InternalProcessRecord();
			this.DataObject = recipientSession.FindADUserByObjectId(this.DataObject.ObjectId);
			if (flag)
			{
				this.groupMailboxContext.RefreshStoreCache();
			}
			if (!this.DataObject.GroupMailboxExternalResourcesSet || this.ForcePublishExternalResources)
			{
				this.groupMailboxContext.SetExternalResources(this.FromSyncClient);
			}
			this.groupMailboxContext.EnsureGroupIsInDirectoryCache("SetGroupMailbox.InternalProcessRecord");
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new SetGroupMailboxTaskModuleFactory();
		}

		private void ThrowIfNotOwner(string parameterName)
		{
			if (!this.executingUserIsOwner)
			{
				base.WriteError(new GroupMailboxNotAuthorizedException(Strings.ErrorNotAuthorizedForParameter(parameterName)), ExchangeErrorCategory.Client, null);
			}
		}

		private void AuthorizeAddedAndRemovedMembers(ADRawEntry executingUser)
		{
			if (!this.executingUserIsOwner)
			{
				if (this.DataObject.ModernGroupType == ModernGroupObjectType.Private)
				{
					if (this.ContainsOtherUser(this.AddedMembers, executingUser))
					{
						base.WriteError(new GroupMailboxNotAuthorizedException(Strings.ErrorSetGroupMailboxAddMembersOtherUser), ExchangeErrorCategory.Client, null);
					}
					else if (this.AddedMembers != null && this.AddedMembers.Length > 0)
					{
						base.WriteError(new GroupMailboxNotAuthorizedException(Strings.ErrorNotAuthorizedForParameter("AddedMembers")), ExchangeErrorCategory.Client, null);
					}
				}
				if (this.ContainsOtherUser(this.RemovedMembers, executingUser))
				{
					base.WriteError(new GroupMailboxNotAuthorizedException(Strings.ErrorSetGroupMailboxRemoveMembersOtherUser), ExchangeErrorCategory.Client, null);
				}
			}
		}

		private bool ContainsOtherUser(RecipientIdParameter[] members, ADRawEntry executingUser)
		{
			if (members == null || executingUser == null)
			{
				return false;
			}
			foreach (RecipientIdParameter id in members)
			{
				Exception ex;
				ADUser aduser = this.groupMailboxContext.ResolveUser(id, out ex);
				if (ex == null && !aduser.Id.Equals(executingUser.Id))
				{
					return true;
				}
			}
			return false;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return GroupMailbox.FromDataObject((ADUser)dataObject);
		}

		private const string ParameterExecutingUser = "ExecutingUser";

		private const string ParameterAddedMembers = "AddedMembers";

		private const string ParameterRemovedMembers = "RemovedMembers";

		private const string ParameterRequireSenderAuthenticationEnabled = "RequireSenderAuthenticationEnabled";

		private const string ParameterYammerGroupEmailAddress = "YammerGroupEmailAddress";

		private const string ParameterAddOwners = "AddOwners";

		private const string ParameterRemoveOwners = "RemoveOwners";

		private const string ParameterRecipientIdType = "RecipientIdType";

		private const string ParameterFromSyncClient = "FromSyncClient";

		private const string ParameterForcePublishExternalResources = "ForcePublishExternalResources";

		private const string ParameterConfigurationActions = "ConfigurationActions";

		private const string ParameterAutoSubscribeNewGroupMembers = "AutoSubscribeNewGroupMembers";

		private const string ParameterPermissionsVersion = "PermissionsVersion";

		private const string ParameterLanguage = "Language";

		private GroupMailboxContext groupMailboxContext;

		private bool executingUserIsOwner;
	}
}
