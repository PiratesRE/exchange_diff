using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OrganizationConfig", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOrganizationConfig : BaseOrganization
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0)]
		public new OrganizationIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return base.AccountPartition;
			}
			set
			{
				base.AccountPartition = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter MicrosoftExchangeRecipientReplyRecipient
		{
			get
			{
				return (RecipientIdParameter)base.Fields[OrganizationSchema.MicrosoftExchangeRecipientReplyRecipient];
			}
			set
			{
				base.Fields[OrganizationSchema.MicrosoftExchangeRecipientReplyRecipient] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UserContactGroupIdParameter HierarchicalAddressBookRoot
		{
			get
			{
				return (UserContactGroupIdParameter)base.Fields[OrganizationSchema.HABRootDepartmentLink];
			}
			set
			{
				base.Fields[OrganizationSchema.HABRootDepartmentLink] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationalUnitIdParameter DistributionGroupDefaultOU
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields[OrganizationSchema.DistributionGroupDefaultOU];
			}
			set
			{
				base.Fields[OrganizationSchema.DistributionGroupDefaultOU] = value;
			}
		}

		[ValidateCount(0, 64)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientIdParameter> ExchangeNotificationRecipients
		{
			get
			{
				return (MultiValuedProperty<RecipientIdParameter>)base.Fields["ExchangeNotificationRecipients"];
			}
			set
			{
				base.Fields["ExchangeNotificationRecipients"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<MailboxOrMailUserIdParameter> RemotePublicFolderMailboxes
		{
			get
			{
				return (MultiValuedProperty<MailboxOrMailUserIdParameter>)base.Fields[OrganizationSchema.RemotePublicFolderMailboxes];
			}
			set
			{
				base.Fields[OrganizationSchema.RemotePublicFolderMailboxes] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			Organization organization = (Organization)this.GetDynamicParameters();
			if (organization.MicrosoftExchangeRecipientEmailAddresses.Changed && organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientPrimarySmtpAddress))
			{
				base.ThrowTerminatingError(new InvalidOperationException(Strings.ErrorPrimarySmtpAndEmailAddressesSpecified), ErrorCategory.InvalidOperation, null);
			}
			if (organization.IsModified(OrganizationSchema.EwsApplicationAccessPolicy))
			{
				if (!organization.EwsAllowListSpecified && !organization.EwsBlockListSpecified)
				{
					organization.EwsExceptions = null;
				}
			}
			else
			{
				if (organization.EwsAllowListSpecified)
				{
					organization.EwsApplicationAccessPolicy = new EwsApplicationAccessPolicy?(EwsApplicationAccessPolicy.EnforceAllowList);
				}
				if (organization.EwsBlockListSpecified)
				{
					organization.EwsApplicationAccessPolicy = new EwsApplicationAccessPolicy?(EwsApplicationAccessPolicy.EnforceBlockList);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			ADSessionSettings sessionSettings;
			if (this.AccountPartition != null)
			{
				PartitionId partitionId = RecipientTaskHelper.ResolvePartitionId(this.AccountPartition, new Task.TaskErrorLoggingDelegate(base.WriteError));
				sessionSettings = ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId);
			}
			else
			{
				sessionSettings = ADSessionSettings.RescopeToSubtree(base.OrgWideSessionSettings);
			}
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 176, "InternalStateReset", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\organization\\SetOrganization.cs");
			this.recipientSession.UseConfigNC = true;
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			Organization organization = (Organization)this.GetDynamicParameters();
			if (base.Fields.IsModified(OrganizationSchema.MicrosoftExchangeRecipientReplyRecipient))
			{
				if (this.MicrosoftExchangeRecipientReplyRecipient != null)
				{
					ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.MicrosoftExchangeRecipientReplyRecipient, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.MicrosoftExchangeRecipientReplyRecipient.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.MicrosoftExchangeRecipientReplyRecipient.ToString())));
					organization.MicrosoftExchangeRecipientReplyRecipient = (ADObjectId)adrecipient.Identity;
				}
				else
				{
					organization.MicrosoftExchangeRecipientReplyRecipient = null;
				}
			}
			if (base.Fields.IsModified(OrganizationSchema.HABRootDepartmentLink))
			{
				if (this.HierarchicalAddressBookRoot != null)
				{
					ADRecipient adrecipient2 = (ADRecipient)base.GetDataObject<ADRecipient>(this.HierarchicalAddressBookRoot, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.HierarchicalAddressBookRoot.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.HierarchicalAddressBookRoot.ToString())));
					organization.HierarchicalAddressBookRoot = (ADObjectId)adrecipient2.Identity;
				}
				else
				{
					organization.HierarchicalAddressBookRoot = null;
				}
			}
			if (base.Fields.IsModified(OrganizationSchema.DistributionGroupDefaultOU))
			{
				if (this.DistributionGroupDefaultOU != null)
				{
					ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.DistributionGroupDefaultOU, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.DistributionGroupDefaultOU.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.DistributionGroupDefaultOU.ToString())));
					organization.DistributionGroupDefaultOU = (ADObjectId)adorganizationalUnit.Identity;
					return;
				}
				organization.DistributionGroupDefaultOU = null;
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			Organization organization = (Organization)dataObject;
			this.mergedCollection = new MultiValuedProperty<OrganizationSummaryEntry>();
			foreach (OrganizationSummaryEntry organizationSummaryEntry in organization.OrganizationSummary)
			{
				if (organizationSummaryEntry.NumberOfFields >= 3 || OrganizationSummaryEntry.IsValidKeyForCurrentAndPreviousRelease(organizationSummaryEntry.Key))
				{
					this.mergedCollection.Add(organizationSummaryEntry);
				}
			}
			this.microsoftExchangeRecipient = MailboxTaskHelper.FindMicrosoftExchangeRecipient(this.recipientSession, (IConfigurationSession)base.DataSession);
			if (this.microsoftExchangeRecipient != null)
			{
				organization.MicrosoftExchangeRecipientEmailAddresses = this.microsoftExchangeRecipient.EmailAddresses;
				organization.MicrosoftExchangeRecipientReplyRecipient = this.microsoftExchangeRecipient.ForwardingAddress;
				organization.MicrosoftExchangeRecipientEmailAddressPolicyEnabled = this.microsoftExchangeRecipient.EmailAddressPolicyEnabled;
				organization.MicrosoftExchangeRecipientPrimarySmtpAddress = this.microsoftExchangeRecipient.PrimarySmtpAddress;
				organization.ResetChangeTracking();
			}
			if (base.Fields.IsModified("ExchangeNotificationRecipients"))
			{
				this.SetMultiValuedProperty<RecipientIdParameter, SmtpAddress>(this.ExchangeNotificationRecipients, organization.ExchangeNotificationRecipients, new SetOrganizationConfig.Resolver<RecipientIdParameter, SmtpAddress>(this.ResolveRecipients));
			}
			if (base.Fields.IsModified(OrganizationSchema.RemotePublicFolderMailboxes))
			{
				this.SetMultiValuedProperty<MailboxOrMailUserIdParameter, ADObjectId>(this.RemotePublicFolderMailboxes, organization.RemotePublicFolderMailboxes, new SetOrganizationConfig.Resolver<MailboxOrMailUserIdParameter, ADObjectId>(this.ResolveRemotePublicFolderMailboxes));
			}
			base.StampChangesOn(dataObject);
			TaskLogger.LogExit();
		}

		private void SetMultiValuedProperty<T, V>(MultiValuedProperty<T> inputValues, MultiValuedProperty<V> existingValues, SetOrganizationConfig.Resolver<T, V> resolver)
		{
			if (inputValues == null)
			{
				existingValues.Clear();
				return;
			}
			if (!inputValues.IsChangesOnlyCopy)
			{
				existingValues.Clear();
				using (IEnumerator<V> enumerator = resolver(inputValues).Distinct<V>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						V item = enumerator.Current;
						existingValues.Add(item);
					}
					return;
				}
			}
			HashSet<V> first = new HashSet<V>(existingValues);
			IEnumerable<V> second = resolver(inputValues.Added.Cast<T>());
			IEnumerable<V> second2 = resolver(inputValues.Removed.Cast<T>());
			existingValues.Clear();
			foreach (V item2 in first.Union(second).Except(second2))
			{
				existingValues.Add(item2);
			}
		}

		private IEnumerable<ADObjectId> ResolveRemotePublicFolderMailboxes(IEnumerable<MailboxOrMailUserIdParameter> adUsers)
		{
			foreach (MailboxOrMailUserIdParameter adUser in adUsers)
			{
				ADUser adObject = (ADUser)base.GetDataObject<ADUser>(adUser, base.TenantGlobalCatalogSession, this.RootId, new LocalizedString?(Strings.ErrorRecipientNotFound(adUser.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(adUser.ToString())));
				yield return (ADObjectId)adObject.Identity;
			}
			yield break;
		}

		private void MergeAdded(MultiValuedProperty<OrganizationSummaryEntry> summaryInfo)
		{
			foreach (OrganizationSummaryEntry organizationSummaryEntry in summaryInfo.Added)
			{
				if (!organizationSummaryEntry.Key.Equals(OrganizationSummaryEntry.SummaryInfoUpdateDate) && OrganizationSummaryEntry.IsValidKeyForCurrentRelease(organizationSummaryEntry.Key))
				{
					OrganizationSummaryEntry organizationSummaryEntry2 = null;
					if (this.FindInMergedCollection(organizationSummaryEntry, out organizationSummaryEntry2))
					{
						this.mergedCollection.Remove(organizationSummaryEntry2);
						this.mergedCollection.Add(new OrganizationSummaryEntry(organizationSummaryEntry2.Key, organizationSummaryEntry.HasError ? organizationSummaryEntry2.Value : organizationSummaryEntry.Value, organizationSummaryEntry.HasError));
					}
					else if (!organizationSummaryEntry.HasError)
					{
						this.mergedCollection.Add(organizationSummaryEntry.Clone());
					}
				}
			}
		}

		private void MergeRemoved(MultiValuedProperty<OrganizationSummaryEntry> summaryInfo)
		{
			foreach (OrganizationSummaryEntry organizationSummaryEntry in summaryInfo.Removed)
			{
				OrganizationSummaryEntry item;
				if (!organizationSummaryEntry.Key.Equals(OrganizationSummaryEntry.SummaryInfoUpdateDate) && OrganizationSummaryEntry.IsValidKeyForCurrentRelease(organizationSummaryEntry.Key) && this.FindInMergedCollection(organizationSummaryEntry, out item))
				{
					this.mergedCollection.Remove(item);
				}
			}
		}

		private bool FindInMergedCollection(OrganizationSummaryEntry entry, out OrganizationSummaryEntry foundEntry)
		{
			foundEntry = null;
			foreach (OrganizationSummaryEntry organizationSummaryEntry in this.mergedCollection)
			{
				if (organizationSummaryEntry.Key.Equals(entry.Key))
				{
					foundEntry = organizationSummaryEntry;
					return true;
				}
			}
			return false;
		}

		private void UpdateUploadDate()
		{
			OrganizationSummaryEntry organizationSummaryEntry = new OrganizationSummaryEntry(OrganizationSummaryEntry.SummaryInfoUpdateDate, DateTime.UtcNow.ToString(), false);
			OrganizationSummaryEntry item;
			if (this.FindInMergedCollection(organizationSummaryEntry, out item))
			{
				this.mergedCollection.Remove(item);
			}
			this.mergedCollection.Add(organizationSummaryEntry);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.IsChanged(OrganizationSchema.CustomerFeedbackEnabled) && this.DataObject.CustomerFeedbackEnabled == null)
			{
				base.WriteError(new InvalidOperationException(Strings.CustomerFeedbackEnabledError), ErrorCategory.InvalidOperation, null);
			}
			if (this.DataObject.IsChanged(OrganizationSchema.OrganizationSummary))
			{
				MultiValuedProperty<OrganizationSummaryEntry> organizationSummary = this.DataObject.OrganizationSummary;
				this.MergeAdded(organizationSummary);
				this.MergeRemoved(organizationSummary);
				if (this.mergedCollection.Changed)
				{
					this.UpdateUploadDate();
				}
				this.DataObject.OrganizationSummary = this.mergedCollection;
			}
			Organization organization = (Organization)this.GetDynamicParameters();
			if (organization.EwsAllowListSpecified && organization.EwsBlockListSpecified)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorEwsAllowListAndEwsBlockListSpecified), ErrorCategory.InvalidArgument, null);
			}
			if (organization.IsModified(OrganizationSchema.EwsApplicationAccessPolicy))
			{
				if (organization.EwsApplicationAccessPolicy == EwsApplicationAccessPolicy.EnforceAllowList && organization.EwsBlockListSpecified)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorEwsEnforceAllowListAndEwsBlockListSpecified), ErrorCategory.InvalidArgument, null);
				}
				if (organization.EwsApplicationAccessPolicy == EwsApplicationAccessPolicy.EnforceBlockList && organization.EwsAllowListSpecified)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorEwsEnforceBlockListAndEwsAllowListSpecified), ErrorCategory.InvalidArgument, null);
				}
			}
			if (organization.IsModified(OrganizationSchema.DefaultPublicFolderMailbox))
			{
				Organization orgContainer = ((IConfigurationSession)base.DataSession).GetOrgContainer();
				PublicFolderInformation defaultPublicFolderMailbox = orgContainer.DefaultPublicFolderMailbox;
				if (!defaultPublicFolderMailbox.CanUpdate)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotUpdatePublicFolderHierarchyInformation), ExchangeErrorCategory.Client, null);
				}
				if (defaultPublicFolderMailbox.HierarchyMailboxGuid != Guid.Empty)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorPublicFolderHierarchyAlreadyProvisioned), ExchangeErrorCategory.Client, null);
				}
			}
			if (this.DataObject.IsChanged(OrganizationSchema.TenantRelocationsAllowed) && !this.DataObject.TenantRelocationsAllowed)
			{
				string ridMasterName = ForestTenantRelocationsCache.GetRidMasterName(this.DataObject.OrganizationId.PartitionId);
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ridMasterName, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAllTenantsPartitionId(this.DataObject.OrganizationId.PartitionId ?? PartitionId.LocalForest), 564, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\organization\\SetOrganization.cs");
				TenantRelocationRequest[] array = tenantConfigurationSession.Find<TenantRelocationRequest>(null, QueryScope.SubTree, new OrFilter(new QueryFilter[]
				{
					TenantRelocationRequest.TenantRelocationRequestFilter,
					TenantRelocationRequest.TenantRelocationLandingFilter
				}), null, 1);
				if (array.Length > 0)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorTenantRelocationInProgress(array[0].Name)), ErrorCategory.InvalidOperation, null);
				}
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			Organization organization = (Organization)base.PrepareDataObject();
			this.isMicrosoftExchangeRecipientChanged = (organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientEmailAddresses) || organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientEmailAddressPolicyEnabled) || organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientPrimarySmtpAddress) || organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientReplyRecipient));
			if (this.isMicrosoftExchangeRecipientChanged && this.microsoftExchangeRecipient == null)
			{
				this.WriteWarning(Strings.ErrorMicrosoftExchangeRecipientNotFound);
			}
			if (this.microsoftExchangeRecipient != null)
			{
				this.microsoftExchangeRecipient.DisplayName = ADMicrosoftExchangeRecipient.DefaultDisplayName;
				this.microsoftExchangeRecipient.DeliverToMailboxAndForward = false;
				if (organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientEmailAddresses))
				{
					this.microsoftExchangeRecipient.EmailAddresses.CopyChangesFrom(organization.MicrosoftExchangeRecipientEmailAddresses);
					if (Datacenter.IsMultiTenancyEnabled())
					{
						RecipientTaskHelper.ValidateSmtpAddress(this.ConfigurationSession, this.microsoftExchangeRecipient.EmailAddresses, this.microsoftExchangeRecipient, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
					}
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, this.microsoftExchangeRecipient.OrganizationId, base.CurrentOrganizationId, false);
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.TenantGlobalCatalogSession.DomainController, true, ConsistencyMode.PartiallyConsistent, base.TenantGlobalCatalogSession.NetworkCredential, sessionSettings, 632, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\organization\\SetOrganization.cs");
					RecipientTaskHelper.ValidateEmailAddressErrorOut(tenantOrRootOrgRecipientSession, this.microsoftExchangeRecipient.EmailAddresses, this.microsoftExchangeRecipient, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerReThrowDelegate(this.WriteError));
					if (this.microsoftExchangeRecipient.EmailAddresses.Count > 0)
					{
						RecipientTaskHelper.ValidateEmailAddress(base.TenantGlobalCatalogSession, this.microsoftExchangeRecipient.EmailAddresses, this.microsoftExchangeRecipient, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
					}
				}
				if (organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientPrimarySmtpAddress))
				{
					this.microsoftExchangeRecipient.PrimarySmtpAddress = organization.MicrosoftExchangeRecipientPrimarySmtpAddress;
				}
				if (organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientReplyRecipient))
				{
					this.microsoftExchangeRecipient.ForwardingAddress = organization.MicrosoftExchangeRecipientReplyRecipient;
				}
				if (organization.IsChanged(OrganizationSchema.MicrosoftExchangeRecipientEmailAddressPolicyEnabled))
				{
					this.microsoftExchangeRecipient.EmailAddressPolicyEnabled = organization.MicrosoftExchangeRecipientEmailAddressPolicyEnabled;
				}
				ValidationError[] array = this.microsoftExchangeRecipient.Validate();
				for (int i = 0; i < array.Length; i++)
				{
					this.WriteError(new DataValidationException(array[i]), ErrorCategory.InvalidData, null, i == array.Length - 1);
				}
				if (!ProvisioningLayer.Disabled)
				{
					if (base.IsProvisioningLayerAvailable)
					{
						ProvisioningLayer.UpdateAffectedIConfigurable(this, RecipientTaskHelper.ConvertRecipientToPresentationObject(this.microsoftExchangeRecipient), false);
					}
					else
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
					}
				}
				organization.MicrosoftExchangeRecipientEmailAddresses = this.microsoftExchangeRecipient.EmailAddresses;
				organization.MicrosoftExchangeRecipientPrimarySmtpAddress = this.microsoftExchangeRecipient.PrimarySmtpAddress;
				organization.MicrosoftExchangeRecipientEmailAddressPolicyEnabled = this.microsoftExchangeRecipient.EmailAddressPolicyEnabled;
			}
			if (organization.IsChanged(OrganizationSchema.AdfsAuthenticationRawConfiguration) && AdfsAuthenticationConfig.Validate((string)organization[OrganizationSchema.AdfsAuthenticationRawConfiguration]))
			{
				this.WriteWarning(Strings.NeedIisRestartWarning);
			}
			TaskLogger.LogExit();
			return organization;
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.microsoftExchangeRecipient != null && this.microsoftExchangeRecipient.ObjectState != ObjectState.Unchanged)
			{
				this.recipientSession.Save(this.microsoftExchangeRecipient);
			}
			if (this.DataObject.IsModified(OrganizationSchema.CustomerFeedbackEnabled))
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				IEnumerable<Server> enumerable = configurationSession.FindAllPaged<Server>();
				if (enumerable != null)
				{
					foreach (Server server in enumerable)
					{
						if (server.IsE14OrLater)
						{
							server.CustomerFeedbackEnabled = this.DataObject.CustomerFeedbackEnabled;
							configurationSession.Save(server);
						}
					}
				}
			}
			if (this.DataObject.IsModified(OrganizationSchema.HABRootDepartmentLink) && VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				ADOrganizationConfig dataObject = this.DataObject;
				ADObjectId hierarchicalAddressBookRoot = dataObject.HierarchicalAddressBookRoot;
				dataObject.HierarchicalAddressBookRoot = null;
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, this.ConfigurationSession.SessionSettings, 756, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\organization\\SetOrganization.cs");
				ADObjectId organizationalUnit = dataObject.OrganizationId.OrganizationalUnit;
				ExchangeOrganizationalUnit exchangeOrganizationalUnit = (ExchangeOrganizationalUnit)base.GetDataObject<ExchangeOrganizationalUnit>(new OrganizationalUnitIdParameter(organizationalUnit), tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(organizationalUnit.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(organizationalUnit.ToString())));
				exchangeOrganizationalUnit.HierarchicalAddressBookRoot = hierarchicalAddressBookRoot;
				tenantOrTopologyConfigurationSession.Save(exchangeOrganizationalUnit);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private IEnumerable<SmtpAddress> ResolveRecipients(IEnumerable<RecipientIdParameter> recipients)
		{
			LinkedList<SmtpAddress> linkedList = new LinkedList<SmtpAddress>();
			foreach (RecipientIdParameter recipientIdParameter in recipients)
			{
				if (SmtpAddress.IsValidSmtpAddress(recipientIdParameter.RawIdentity))
				{
					linkedList.AddLast(SmtpAddress.Parse(recipientIdParameter.RawIdentity));
				}
				else
				{
					ADRecipient adrecipient = null;
					IEnumerable<ADRecipient> objects = recipientIdParameter.GetObjects<ADRecipient>(null, base.TenantGlobalCatalogSession);
					using (IEnumerator<ADRecipient> enumerator2 = objects.GetEnumerator())
					{
						if (!enumerator2.MoveNext())
						{
							this.WriteError(new NoRecipientsForRecipientIdException(recipientIdParameter.ToString()), (ErrorCategory)1000, this.DataObject, true);
						}
						adrecipient = enumerator2.Current;
						if (enumerator2.MoveNext())
						{
							this.WriteError(new MoreThanOneRecipientForRecipientIdException(recipientIdParameter.ToString()), (ErrorCategory)1000, this.DataObject, true);
						}
					}
					if (SmtpAddress.Empty.Equals(adrecipient.PrimarySmtpAddress))
					{
						this.WriteError(new NoSmtpAddressForRecipientIdException(recipientIdParameter.ToString()), (ErrorCategory)1000, this.DataObject, true);
					}
					linkedList.AddLast(adrecipient.PrimarySmtpAddress);
				}
			}
			return linkedList;
		}

		private ADMicrosoftExchangeRecipient microsoftExchangeRecipient;

		private bool isMicrosoftExchangeRecipientChanged;

		private IRecipientSession recipientSession;

		private MultiValuedProperty<OrganizationSummaryEntry> mergedCollection;

		private delegate IEnumerable<V> Resolver<T, V>(IEnumerable<T> inputValues);
	}
}
