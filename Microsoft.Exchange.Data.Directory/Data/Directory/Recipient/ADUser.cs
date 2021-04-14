using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class ADUser : ADMailboxRecipient, IADUser, IADMailboxRecipient, IADMailStorage, IADSecurityPrincipal, IADOrgPerson, IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag, IOriginatingChangeTimestamp, IFederatedIdentityParameters, IProvisioningCacheInvalidation
	{
		internal static string GetMonitoringMailboxName(Guid guid)
		{
			return string.Format("HealthMailbox{0}", guid.ToString("N"));
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADUser.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADUser.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return ADUser.ImplicitFilterInternal;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.PublicFolderMailboxObjectVersion;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return base.RecipientTypeDetails == RecipientTypeDetails.MailUser;
			}
		}

		internal override string ObjectCategoryName
		{
			get
			{
				return ADUser.ObjectCategoryNameInternal;
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (RecipientType.UserMailbox == base.RecipientType)
			{
				if (base.ExchangeGuid == Guid.Empty)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.PropertyRequired(ADMailboxRecipientSchema.ExchangeGuid.Name, base.RecipientType.ToString()), ADMailboxRecipientSchema.ExchangeGuid, null));
				}
				if (base.Database == null)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.PropertyRequired(ADMailboxRecipientSchema.Database.Name, base.RecipientType.ToString()), ADMailboxRecipientSchema.Database, null));
				}
				if (this.MailboxContainerGuid == null)
				{
					if (this.AggregatedMailboxGuids.Count != 0)
					{
						errors.Add(new PropertyValidationError(DirectoryStrings.PropertyDependencyRequired(ADUserSchema.MailboxContainerGuid.Name, ADUserSchema.AggregatedMailboxGuids.Name), ADUserSchema.MailboxContainerGuid, null));
					}
				}
				else if (this.AggregatedMailboxGuids.Count != 0 && this.UnifiedMailbox != null)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorCannotAggregateAndLinkMailbox, ADUserSchema.MailboxContainerGuid, null));
				}
			}
			if (RecipientType.MailUser == base.RecipientType && base.ExternalEmailAddress == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorNullExternalEmailAddress, ADRecipientSchema.ExternalEmailAddress, null));
			}
			if (RecipientTypeDetails.MailboxPlan == base.RecipientTypeDetails)
			{
				if (this.PersistedCapabilities.Count > 1)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidNumberOfCapabilitiesOnMailboxPlan(MultiValuedPropertyBase.FormatMultiValuedProperty(CapabilityHelper.AllowedSKUCapabilities)), ADUserSchema.PersistedCapabilities, null));
					return;
				}
				if (this.PersistedCapabilities.Count == 1 && !CapabilityHelper.IsAllowedSKUCapability(this.PersistedCapabilities[0]))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidCapabilityOnMailboxPlan(this.PersistedCapabilities[0].ToString(), MultiValuedPropertyBase.FormatMultiValuedProperty(CapabilityHelper.AllowedSKUCapabilities)), ADUserSchema.PersistedCapabilities, this.PersistedCapabilities[0]));
				}
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (base.UseDatabaseQuotaDefaults == null || !base.UseDatabaseQuotaDefaults.Value)
			{
				errors.AddRange(Microsoft.Exchange.Data.Directory.SystemConfiguration.Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
				{
					IADMailStorageSchema.IssueWarningQuota,
					IADMailStorageSchema.ProhibitSendQuota,
					IADMailStorageSchema.ProhibitSendReceiveQuota
				}, this.Identity));
				errors.AddRange(Microsoft.Exchange.Data.Directory.SystemConfiguration.Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
				{
					IADMailStorageSchema.RecoverableItemsWarningQuota,
					IADMailStorageSchema.RecoverableItemsQuota
				}, this.Identity));
				errors.AddRange(Microsoft.Exchange.Data.Directory.SystemConfiguration.Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
				{
					IADMailStorageSchema.ArchiveWarningQuota,
					IADMailStorageSchema.ArchiveQuota
				}, this.Identity));
				errors.AddRange(Microsoft.Exchange.Data.Directory.SystemConfiguration.Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
				{
					IADMailStorageSchema.CalendarLoggingQuota,
					IADMailStorageSchema.RecoverableItemsQuota
				}, this.Identity));
			}
			if (!this.RetentionHoldEnabled && (this.EndDateForRetentionHold != null || this.StartDateForRetentionHold != null))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorElcSuspensionNotEnabled, this.Identity, string.Empty));
			}
			if (this.StartDateForRetentionHold != null && this.EndDateForRetentionHold != null && (this.StartDateForRetentionHold.Value - this.EndDateForRetentionHold.Value).TotalSeconds >= 0.0)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorStartDateAfterEndDate(this.StartDateForRetentionHold.Value.ToString(), this.EndDateForRetentionHold.Value.ToString()), this.Identity, string.Empty));
			}
			if (RecipientType.UserMailbox == base.RecipientType)
			{
				if (base.ExchangeGuid == Guid.Empty)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.PropertyRequired(ADMailboxRecipientSchema.ExchangeGuid.Name, base.RecipientType.ToString()), ADMailboxRecipientSchema.ExchangeGuid, null));
				}
				if (base.Database == null)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.PropertyRequired(ADMailboxRecipientSchema.Database.Name, base.RecipientType.ToString()), ADMailboxRecipientSchema.Database, null));
				}
			}
			if (RecipientType.MailUser == base.RecipientType && base.ExternalEmailAddress == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorNullExternalEmailAddress, ADRecipientSchema.ExternalEmailAddress, null));
			}
			if (this.propertyBag.IsModified(ADMailboxRecipientSchema.WhenMailboxCreated) && base.WhenMailboxCreated == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorNotNullProperty(ADMailboxRecipientSchema.WhenMailboxCreated.Name), ADMailboxRecipientSchema.WhenMailboxCreated, base.WhenMailboxCreated));
			}
			if (this.propertyBag.IsModified(ADUserSchema.Owners) && this.Owners != null && this.Owners.Count > 0 && !this.IsAllowedToModifyOwners)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorOwnersUpdated, ADUserSchema.Owners, null));
			}
			if (this.propertyBag.IsModified(ADUserSchema.ArchiveName) && this.ArchiveName != null && this.ArchiveName.Count > 0)
			{
				foreach (string text in this.ArchiveName)
				{
					if (text.Length <= 0)
					{
						errors.Add(new PropertyValidationError(DirectoryStrings.ErrorEmptyArchiveName, ADUserSchema.ArchiveName, null));
						break;
					}
				}
			}
			if (this.MailboxLocations != null)
			{
				this.MailboxLocations.Validate(errors);
			}
		}

		internal ADUser(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
			if (base.RecipientTypeDetails != RecipientTypeDetails.None && !base.IsReadOnly)
			{
				ExchangeObjectVersion maximumSupportedExchangeObjectVersion = ADUser.GetMaximumSupportedExchangeObjectVersion(base.RecipientTypeDetails, true);
				if (maximumSupportedExchangeObjectVersion.IsOlderThan(base.ExchangeVersion))
				{
					this.SetIsReadOnly(true);
				}
			}
		}

		internal ADUser(IRecipientSession session, string commonName, ADObjectId containerId, UserObjectClass userObjectClass)
		{
			this.m_Session = session;
			base.SamAccountName = commonName;
			base.SetId(containerId.GetChildId("CN", commonName));
			base.SetObjectClass((userObjectClass == UserObjectClass.User) ? "user" : "inetOrgPerson");
			this.UserAccountControl = (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.NormalAccount);
		}

		public ADUser()
		{
			base.SetObjectClass("user");
		}

		public UserAccountControlFlags ExchangeUserAccountControl
		{
			get
			{
				return (UserAccountControlFlags)this[ADUserSchema.ExchangeUserAccountControl];
			}
			set
			{
				this[ADUserSchema.ExchangeUserAccountControl] = value;
			}
		}

		public MultiValuedProperty<int> LocaleID
		{
			get
			{
				return (MultiValuedProperty<int>)this[ADUserSchema.LocaleID];
			}
			set
			{
				this[ADUserSchema.LocaleID] = value;
			}
		}

		public bool RetentionHoldEnabled
		{
			get
			{
				return (bool)this[ADUserSchema.ElcExpirationSuspensionEnabled];
			}
			set
			{
				this[ADUserSchema.ElcExpirationSuspensionEnabled] = value;
			}
		}

		public DateTime? EndDateForRetentionHold
		{
			get
			{
				return (DateTime?)this[ADUserSchema.ElcExpirationSuspensionEndDate];
			}
			set
			{
				this[ADUserSchema.ElcExpirationSuspensionEndDate] = value;
			}
		}

		public DateTime? StartDateForRetentionHold
		{
			get
			{
				return (DateTime?)this[ADUserSchema.ElcExpirationSuspensionStartDate];
			}
			set
			{
				this[ADUserSchema.ElcExpirationSuspensionStartDate] = value;
			}
		}

		internal bool IsInLitigationHoldOrInplaceHold
		{
			get
			{
				return this.LitigationHoldEnabled || (base.InPlaceHolds != null && base.InPlaceHolds.Count > 0);
			}
		}

		public bool LitigationHoldEnabled
		{
			get
			{
				return (bool)this[IADMailStorageSchema.LitigationHoldEnabled];
			}
			set
			{
				this[IADMailStorageSchema.LitigationHoldEnabled] = value;
			}
		}

		public Unlimited<EnhancedTimeSpan>? LitigationHoldDuration
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>?)this[ADRecipientSchema.LitigationHoldDuration];
			}
			set
			{
				this[ADRecipientSchema.LitigationHoldDuration] = value;
			}
		}

		public bool SingleItemRecoveryEnabled
		{
			get
			{
				return (bool)this[IADMailStorageSchema.SingleItemRecoveryEnabled];
			}
			set
			{
				this[IADMailStorageSchema.SingleItemRecoveryEnabled] = value;
			}
		}

		public bool CalendarVersionStoreDisabled
		{
			get
			{
				return (bool)this[IADMailStorageSchema.CalendarVersionStoreDisabled];
			}
			set
			{
				this[IADMailStorageSchema.CalendarVersionStoreDisabled] = value;
			}
		}

		public bool SiteMailboxMessageDedupEnabled
		{
			get
			{
				return (bool)this[IADMailStorageSchema.SiteMailboxMessageDedupEnabled];
			}
			set
			{
				this[IADMailStorageSchema.SiteMailboxMessageDedupEnabled] = value;
			}
		}

		internal ElcMailboxFlags ElcMailboxFlags
		{
			get
			{
				return (ElcMailboxFlags)this[ADUserSchema.ElcMailboxFlags];
			}
			set
			{
				this[ADUserSchema.ElcMailboxFlags] = value;
			}
		}

		public string RetentionComment
		{
			get
			{
				return (string)this[ADUserSchema.RetentionComment];
			}
			set
			{
				this[ADUserSchema.RetentionComment] = value;
			}
		}

		public string RetentionUrl
		{
			get
			{
				return (string)this[ADUserSchema.RetentionUrl];
			}
			set
			{
				this[ADUserSchema.RetentionUrl] = value;
			}
		}

		public bool LEOEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.LEOEnabled];
			}
			set
			{
				this[ADRecipientSchema.LEOEnabled] = value;
			}
		}

		public DateTime? LitigationHoldDate
		{
			get
			{
				return (DateTime?)this[ADUserSchema.LitigationHoldDate];
			}
			set
			{
				this[ADUserSchema.LitigationHoldDate] = value;
			}
		}

		public string LitigationHoldOwner
		{
			get
			{
				return (string)this[ADUserSchema.LitigationHoldOwner];
			}
			set
			{
				this[ADUserSchema.LitigationHoldOwner] = value;
			}
		}

		public ADObjectId ManagedFolderMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.ManagedFolderMailboxPolicy];
			}
			set
			{
				this[ADUserSchema.ManagedFolderMailboxPolicy] = value;
			}
		}

		public ADObjectId RetentionPolicy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.RetentionPolicy];
			}
			set
			{
				this[ADUserSchema.RetentionPolicy] = value;
			}
		}

		internal bool ShouldUseDefaultRetentionPolicy
		{
			get
			{
				return (bool)this[ADUserSchema.ShouldUseDefaultRetentionPolicy];
			}
			set
			{
				this[ADUserSchema.ShouldUseDefaultRetentionPolicy] = value;
			}
		}

		public ADObjectId SharingPolicy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.SharingPolicy];
			}
			set
			{
				this[ADUserSchema.SharingPolicy] = value;
			}
		}

		public ADObjectId RemoteAccountPolicy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.RemoteAccountPolicy];
			}
			set
			{
				this[ADUserSchema.RemoteAccountPolicy] = value;
			}
		}

		public bool CalendarRepairDisabled
		{
			get
			{
				return (bool)this[ADUserSchema.CalendarRepairDisabled];
			}
			set
			{
				this[ADUserSchema.CalendarRepairDisabled] = value;
			}
		}

		public MobileFeaturesEnabled MobileFeaturesEnabled
		{
			get
			{
				return (MobileFeaturesEnabled)this[ADUserSchema.MobileFeaturesEnabled];
			}
			set
			{
				this[ADUserSchema.MobileFeaturesEnabled] = value;
			}
		}

		public MobileMailboxFlags MobileMailboxFlags
		{
			get
			{
				return (MobileMailboxFlags)this[ADUserSchema.MobileMailboxFlags];
			}
			set
			{
				this[ADUserSchema.MobileMailboxFlags] = value;
			}
		}

		public ADObjectId QueryBaseDN
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.QueryBaseDN];
			}
			set
			{
				this[ADUserSchema.QueryBaseDN] = value;
			}
		}

		public bool QueryBaseDNRestrictionEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.QueryBaseDNRestrictionEnabled];
			}
			set
			{
				this[ADRecipientSchema.QueryBaseDNRestrictionEnabled] = value;
			}
		}

		public string MailboxPlanName
		{
			get
			{
				return (string)this[ADRecipientSchema.MailboxPlanName];
			}
			set
			{
				this[ADRecipientSchema.MailboxPlanName] = value;
			}
		}

		public string IntendedMailboxPlanName
		{
			get
			{
				return (string)this[ADUserSchema.IntendedMailboxPlanName];
			}
			set
			{
				this[ADUserSchema.IntendedMailboxPlanName] = value;
			}
		}

		public ADObjectId IntendedMailboxPlan
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.IntendedMailboxPlan];
			}
			set
			{
				this[ADUserSchema.IntendedMailboxPlan] = value;
			}
		}

		public bool IsExcludedFromServingHierarchy
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsExcludedFromServingHierarchy];
			}
			set
			{
				this[ADRecipientSchema.IsExcludedFromServingHierarchy] = value;
			}
		}

		public bool IsHierarchyReady
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsHierarchyReady];
			}
			set
			{
				this[ADRecipientSchema.IsHierarchyReady] = value;
			}
		}

		public MailboxProvisioningConstraint MailboxProvisioningConstraint
		{
			get
			{
				return (MailboxProvisioningConstraint)this[ADRecipientSchema.MailboxProvisioningConstraint];
			}
			set
			{
				this[ADRecipientSchema.MailboxProvisioningConstraint] = value;
			}
		}

		public MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences
		{
			get
			{
				return (MultiValuedProperty<MailboxProvisioningConstraint>)this[ADRecipientSchema.MailboxProvisioningPreferences];
			}
			set
			{
				this[ADRecipientSchema.MailboxProvisioningPreferences] = value;
			}
		}

		public MultiValuedProperty<string> Description
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.Description];
			}
			set
			{
				this[ADRecipientSchema.Description] = value;
			}
		}

		public bool IsGroupMailboxConfigured
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsGroupMailboxConfigured];
			}
			set
			{
				this[ADRecipientSchema.IsGroupMailboxConfigured] = value;
			}
		}

		public bool GroupMailboxExternalResourcesSet
		{
			get
			{
				return (bool)this[ADRecipientSchema.GroupMailboxExternalResourcesSet];
			}
			set
			{
				this[ADRecipientSchema.GroupMailboxExternalResourcesSet] = value;
			}
		}

		public long? PasswordLastSetRaw
		{
			get
			{
				return (long?)this.propertyBag[ADUserSchema.PasswordLastSetRaw];
			}
		}

		internal static object PasswordLastSetGetter(IPropertyBag propertyBag)
		{
			long? num = (long?)propertyBag[ADUserSchema.PasswordLastSetRaw];
			if (num != null && num != -1L && num != 0L)
			{
				try
				{
					return new DateTime?(DateTime.FromFileTimeUtc(num.Value));
				}
				catch (ArgumentOutOfRangeException ex)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(ADUserSchema.PasswordLastSet.Name, ex.Message), ADUserSchema.PasswordLastSet, propertyBag[ADUserSchema.PasswordLastSetRaw]), ex);
				}
			}
			return null;
		}

		public DateTime? PasswordLastSet
		{
			get
			{
				return (DateTime?)this[ADUserSchema.PasswordLastSet];
			}
		}

		public bool ResetPasswordOnNextLogon
		{
			get
			{
				return (bool)this[ADUserSchema.ResetPasswordOnNextLogon];
			}
			set
			{
				this[ADUserSchema.ResetPasswordOnNextLogon] = value;
			}
		}

		internal static object ResetPasswordOnNextLogonGetter(IPropertyBag propertyBag)
		{
			long? num = (long?)propertyBag[ADUserSchema.PasswordLastSetRaw];
			if (num != null && num == 0L)
			{
				return true;
			}
			return false;
		}

		internal static void ResetPasswordOnNextLogonSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			if (flag)
			{
				propertyBag[ADUserSchema.PasswordLastSetRaw] = new long?(0L);
				return;
			}
			propertyBag[ADUserSchema.PasswordLastSetRaw] = new long?(-1L);
		}

		public MultiValuedProperty<ADObjectId> RMSComputerAccounts
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADUserSchema.RMSComputerAccounts];
			}
			set
			{
				this[ADUserSchema.RMSComputerAccounts] = value;
			}
		}

		internal bool UMEnabled
		{
			get
			{
				return (bool)this[ADUserSchema.UMEnabled];
			}
			set
			{
				this[ADUserSchema.UMEnabled] = value;
			}
		}

		public UMEnabledFlags UMEnabledFlags
		{
			get
			{
				return (UMEnabledFlags)this[ADUserSchema.UMEnabledFlags];
			}
			set
			{
				this[ADUserSchema.UMEnabledFlags] = value;
			}
		}

		public int UMEnabledFlags2
		{
			get
			{
				return (int)this[ADUserSchema.UMEnabledFlags2];
			}
			internal set
			{
				this[ADUserSchema.UMEnabledFlags2] = value;
			}
		}

		public string OperatorNumber
		{
			get
			{
				return (string)this[ADUserSchema.OperatorNumber];
			}
			set
			{
				this[ADUserSchema.OperatorNumber] = value;
			}
		}

		public string PhoneProviderId
		{
			get
			{
				return (string)this[ADUserSchema.PhoneProviderId];
			}
			set
			{
				this[ADUserSchema.PhoneProviderId] = value;
			}
		}

		public byte[] UMPinChecksum
		{
			get
			{
				return (byte[])this[ADUserSchema.UMPinChecksum];
			}
			set
			{
				this[ADUserSchema.UMPinChecksum] = value;
			}
		}

		public ADObjectId UMMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.UMMailboxPolicy];
			}
			internal set
			{
				this[ADUserSchema.UMMailboxPolicy] = value;
			}
		}

		public AudioCodecEnum? CallAnsweringAudioCodec
		{
			get
			{
				return (AudioCodecEnum?)this[ADUserSchema.CallAnsweringAudioCodec];
			}
			set
			{
				this[ADUserSchema.CallAnsweringAudioCodec] = value;
			}
		}

		internal UMServerWritableFlagsBits UMServerWritableFlags
		{
			get
			{
				return (UMServerWritableFlagsBits)this[ADUserSchema.UMServerWritableFlags];
			}
			set
			{
				this[ADUserSchema.UMServerWritableFlags] = value;
			}
		}

		public UserAccountControlFlags UserAccountControl
		{
			get
			{
				return (UserAccountControlFlags)this[ADUserSchema.UserAccountControl];
			}
			set
			{
				this[ADUserSchema.UserAccountControl] = value;
			}
		}

		public MultiValuedProperty<byte[]> UserCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ADRecipientSchema.Certificate];
			}
			set
			{
				this[ADRecipientSchema.Certificate] = value;
			}
		}

		public string UserPrincipalName
		{
			get
			{
				return (string)this[ADUserSchema.UserPrincipalName];
			}
			set
			{
				this[ADUserSchema.UserPrincipalName] = value;
			}
		}

		public MultiValuedProperty<byte[]> UserSMIMECertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ADRecipientSchema.SMimeCertificate];
			}
			set
			{
				this[ADRecipientSchema.SMimeCertificate] = value;
			}
		}

		internal static object UserPrincipalNameGetter(IPropertyBag propertyBag)
		{
			return ADUser.MangleUserPrincipalNameDomain(propertyBag, (string)propertyBag[ADUserSchema.UserPrincipalNameRaw], '#', '.');
		}

		internal static void UserPrincipalNameSetter(object value, IPropertyBag propertyBag)
		{
			string value2 = ADUser.MangleUserPrincipalNameDomain(propertyBag, (string)value, '.', '#');
			propertyBag[ADUserSchema.UserPrincipalNameRaw] = value2;
		}

		private static string MangleUserPrincipalNameDomain(IPropertyBag propertyBag, string userPrincipalName, char fromChar, char toChar)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (userPrincipalName != null && adobjectId != null && adobjectId.DistinguishedName.Contains(",OU=Soft Deleted Objects,"))
			{
				int num = userPrincipalName.IndexOf('@');
				if (num >= 0)
				{
					StringBuilder stringBuilder = new StringBuilder(userPrincipalName, userPrincipalName.Length);
					stringBuilder.Replace(fromChar, toChar, num + 1, userPrincipalName.Length - num - 1);
					userPrincipalName = stringBuilder.ToString();
				}
			}
			return userPrincipalName;
		}

		public MultiValuedProperty<string> ActiveSyncAllowedDeviceIDs
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.ActiveSyncAllowedDeviceIDs];
			}
			set
			{
				this[ADUserSchema.ActiveSyncAllowedDeviceIDs] = value;
			}
		}

		public MultiValuedProperty<string> ActiveSyncBlockedDeviceIDs
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.ActiveSyncBlockedDeviceIDs];
			}
			set
			{
				this[ADUserSchema.ActiveSyncBlockedDeviceIDs] = value;
			}
		}

		public ADObjectId ActiveSyncMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.ActiveSyncMailboxPolicy];
			}
			set
			{
				this[ADUserSchema.ActiveSyncMailboxPolicy] = value;
			}
		}

		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[ADUserSchema.PersistedCapabilities];
			}
			set
			{
				this[ADUserSchema.PersistedCapabilities] = value;
			}
		}

		public Capability? SKUCapability
		{
			get
			{
				return CapabilityHelper.GetSKUCapability(this.PersistedCapabilities);
			}
			set
			{
				CapabilityHelper.SetSKUCapability(value, this.PersistedCapabilities);
			}
		}

		public bool? SKUAssigned
		{
			get
			{
				return (bool?)this[ADRecipientSchema.SKUAssigned];
			}
			set
			{
				this[ADRecipientSchema.SKUAssigned] = value;
			}
		}

		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.OwaMailboxPolicy];
			}
			set
			{
				this[ADUserSchema.OwaMailboxPolicy] = value;
			}
		}

		internal ADObjectId PreviousDatabase
		{
			get
			{
				return (ADObjectId)this[IADMailStorageSchema.PreviousDatabase];
			}
			set
			{
				this[IADMailStorageSchema.PreviousDatabase] = value;
			}
		}

		public bool UseDatabaseRetentionDefaults
		{
			get
			{
				return (bool)this[IADMailStorageSchema.UseDatabaseRetentionDefaults];
			}
			set
			{
				this[IADMailStorageSchema.UseDatabaseRetentionDefaults] = value;
			}
		}

		public bool RetainDeletedItemsUntilBackup
		{
			get
			{
				return (bool)this[IADMailStorageSchema.RetainDeletedItemsUntilBackup];
			}
			set
			{
				this[IADMailStorageSchema.RetainDeletedItemsUntilBackup] = value;
			}
		}

		public Guid? MailboxContainerGuid
		{
			get
			{
				return (Guid?)this[ADUserSchema.MailboxContainerGuid];
			}
			set
			{
				this[ADUserSchema.MailboxContainerGuid] = value;
			}
		}

		public MultiValuedProperty<Guid> AggregatedMailboxGuids
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[ADUserSchema.AggregatedMailboxGuids];
			}
		}

		public CrossTenantObjectId UnifiedMailbox
		{
			get
			{
				return (CrossTenantObjectId)this[ADUserSchema.UnifiedMailbox];
			}
			set
			{
				this[ADUserSchema.UnifiedMailbox] = value;
			}
		}

		public Guid PreviousExchangeGuid
		{
			get
			{
				return (Guid)this[IADMailStorageSchema.PreviousExchangeGuid];
			}
			set
			{
				this[IADMailStorageSchema.PreviousExchangeGuid] = value;
			}
		}

		public bool MessageTrackingReadStatusEnabled
		{
			get
			{
				return !(bool)this[ADRecipientSchema.MessageTrackingReadStatusDisabled];
			}
			set
			{
				this[ADRecipientSchema.MessageTrackingReadStatusDisabled] = !value;
			}
		}

		internal MultiValuedProperty<string> Extensions
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.Extensions];
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		public Unlimited<ByteQuantifiedSize> RecoverableItemsQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADUserSchema.RecoverableItemsQuota];
			}
			set
			{
				this[ADUserSchema.RecoverableItemsQuota] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		public Unlimited<ByteQuantifiedSize> RecoverableItemsWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADUserSchema.RecoverableItemsWarningQuota];
			}
			set
			{
				this[ADUserSchema.RecoverableItemsWarningQuota] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		public Unlimited<ByteQuantifiedSize> CalendarLoggingQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADUserSchema.CalendarLoggingQuota];
			}
			set
			{
				this[ADUserSchema.CalendarLoggingQuota] = value;
			}
		}

		public bool DowngradeHighPriorityMessagesEnabled
		{
			get
			{
				return (bool)this[ADUserSchema.DowngradeHighPriorityMessagesEnabled];
			}
			set
			{
				this[ADUserSchema.DowngradeHighPriorityMessagesEnabled] = value;
			}
		}

		public string StorageGroupName
		{
			get
			{
				return (string)this[IADMailStorageSchema.StorageGroupName];
			}
		}

		public NetID NetID
		{
			get
			{
				return (NetID)this[IADSecurityPrincipalSchema.NetID];
			}
			internal set
			{
				this[IADSecurityPrincipalSchema.NetID] = value;
				if (!string.IsNullOrEmpty((string)this[IADSecurityPrincipalSchema.NetIDSuffix]))
				{
					this.NetIDSuffix = this.netIDSuffixCopy;
				}
			}
		}

		public string NetIDSuffix
		{
			get
			{
				if (!string.IsNullOrEmpty(this.netIDSuffixCopy))
				{
					return this.netIDSuffixCopy;
				}
				return (string)this[IADSecurityPrincipalSchema.NetIDSuffix];
			}
			internal set
			{
				this.netIDSuffixCopy = value;
				this[IADSecurityPrincipalSchema.NetIDSuffix] = value;
			}
		}

		public NetID OriginalNetID
		{
			get
			{
				return (NetID)this[IADSecurityPrincipalSchema.OriginalNetID];
			}
			internal set
			{
				this[IADSecurityPrincipalSchema.OriginalNetID] = value;
			}
		}

		public NetID ConsumerNetID
		{
			get
			{
				return (NetID)this[IADSecurityPrincipalSchema.ConsumerNetID];
			}
			internal set
			{
				this[IADSecurityPrincipalSchema.ConsumerNetID] = value;
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				return (SmtpAddress)this[ADRecipientSchema.WindowsLiveID];
			}
			set
			{
				this[ADRecipientSchema.WindowsLiveID] = value;
			}
		}

		public MultiValuedProperty<X509Identifier> CertificateSubject
		{
			get
			{
				return (MultiValuedProperty<X509Identifier>)this[ADUserSchema.CertificateSubject];
			}
			internal set
			{
				this[ADUserSchema.CertificateSubject] = value;
			}
		}

		public ADObjectId ArchiveDatabaseRaw
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.ArchiveDatabaseRaw];
			}
			set
			{
				this[ADUserSchema.ArchiveDatabaseRaw] = value;
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.ArchiveDatabase];
			}
			set
			{
				this[ADUserSchema.ArchiveDatabase] = value;
			}
		}

		public Guid ArchiveGuid
		{
			get
			{
				return (Guid)this[ADUserSchema.ArchiveGuid];
			}
			internal set
			{
				this[ADUserSchema.ArchiveGuid] = value;
			}
		}

		public bool IsAuxMailbox
		{
			get
			{
				return (bool)this[ADUserSchema.IsAuxMailbox];
			}
			internal set
			{
				this[ADUserSchema.IsAuxMailbox] = value;
			}
		}

		public ADObjectId AuxMailboxParentObjectId
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.AuxMailboxParentObjectId];
			}
			set
			{
				this[ADUserSchema.AuxMailboxParentObjectId] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ChildAuxMailboxObjectIds
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADUserSchema.AuxMailboxParentObjectIdBL];
			}
		}

		public MailboxRelationType MailboxRelationType
		{
			get
			{
				return (MailboxRelationType)this[ADUserSchema.MailboxRelationType];
			}
		}

		public MultiValuedProperty<string> ArchiveName
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.ArchiveName];
			}
			set
			{
				this[ADUserSchema.ArchiveName] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ArchiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADUserSchema.ArchiveQuota];
			}
			set
			{
				this[ADUserSchema.ArchiveQuota] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ArchiveWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADUserSchema.ArchiveWarningQuota];
			}
			set
			{
				this[ADUserSchema.ArchiveWarningQuota] = value;
			}
		}

		public SmtpDomain ArchiveDomain
		{
			get
			{
				return (SmtpDomain)this[ADUserSchema.ArchiveDomain];
			}
			internal set
			{
				this[ADUserSchema.ArchiveDomain] = value;
				if (value != null)
				{
					this[ADUserSchema.ArchiveDatabase] = null;
				}
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return (ArchiveStatusFlags)this[ADUserSchema.ArchiveStatus];
			}
			internal set
			{
				this[ADUserSchema.ArchiveStatus] = value;
			}
		}

		public ADObjectId DisabledArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.DisabledArchiveDatabase];
			}
			internal set
			{
				this[ADUserSchema.DisabledArchiveDatabase] = value;
			}
		}

		public Guid DisabledArchiveGuid
		{
			get
			{
				return (Guid)this[ADUserSchema.DisabledArchiveGuid];
			}
			internal set
			{
				this[ADUserSchema.DisabledArchiveGuid] = value;
			}
		}

		public ADObjectId MailboxMoveTargetMDB
		{
			get
			{
				return (ADObjectId)this[IADMailStorageSchema.MailboxMoveTargetMDB];
			}
			internal set
			{
				this[IADMailStorageSchema.MailboxMoveTargetMDB] = value;
			}
		}

		public ADObjectId MailboxMoveSourceMDB
		{
			get
			{
				return (ADObjectId)this[IADMailStorageSchema.MailboxMoveSourceMDB];
			}
			internal set
			{
				this[IADMailStorageSchema.MailboxMoveSourceMDB] = value;
			}
		}

		public ADObjectId MailboxMoveTargetArchiveMDB
		{
			get
			{
				return (ADObjectId)this[IADMailStorageSchema.MailboxMoveTargetArchiveMDB];
			}
			internal set
			{
				this[IADMailStorageSchema.MailboxMoveTargetArchiveMDB] = value;
			}
		}

		public ADObjectId MailboxMoveSourceArchiveMDB
		{
			get
			{
				return (ADObjectId)this[IADMailStorageSchema.MailboxMoveSourceArchiveMDB];
			}
			internal set
			{
				this[IADMailStorageSchema.MailboxMoveSourceArchiveMDB] = value;
			}
		}

		public RequestFlags MailboxMoveFlags
		{
			get
			{
				return (RequestFlags)this[IADMailStorageSchema.MailboxMoveFlags];
			}
			internal set
			{
				this[IADMailStorageSchema.MailboxMoveFlags] = value;
			}
		}

		public string MailboxMoveRemoteHostName
		{
			get
			{
				return (string)this[IADMailStorageSchema.MailboxMoveRemoteHostName];
			}
			internal set
			{
				this[IADMailStorageSchema.MailboxMoveRemoteHostName] = value;
			}
		}

		public string MailboxMoveBatchName
		{
			get
			{
				return (string)this[IADMailStorageSchema.MailboxMoveBatchName];
			}
			internal set
			{
				this[IADMailStorageSchema.MailboxMoveBatchName] = value;
			}
		}

		public RequestStatus MailboxMoveStatus
		{
			get
			{
				return (RequestStatus)this[IADMailStorageSchema.MailboxMoveStatus];
			}
			internal set
			{
				this[IADMailStorageSchema.MailboxMoveStatus] = value;
			}
		}

		public MailboxRelease MailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[ADUserSchema.MailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			internal set
			{
				this[ADUserSchema.MailboxRelease] = ((value == MailboxRelease.None) ? null : value.ToString());
			}
		}

		public MailboxRelease ArchiveRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[ADUserSchema.ArchiveRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			internal set
			{
				this[ADUserSchema.ArchiveRelease] = ((value == MailboxRelease.None) ? null : value.ToString());
			}
		}

		public string MailboxPlanIndex
		{
			get
			{
				return (string)this[ADRecipientSchema.MailboxPlanIndex];
			}
			set
			{
				this[ADRecipientSchema.MailboxPlanIndex] = value;
			}
		}

		public DateTime? TeamMailboxClosedTime
		{
			get
			{
				return (DateTime?)this[ADUserSchema.TeamMailboxClosedTime];
			}
			internal set
			{
				this[ADUserSchema.TeamMailboxClosedTime] = value;
			}
		}

		public ADObjectId SharePointLinkedBy
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.SharePointLinkedBy];
			}
			internal set
			{
				this[ADUserSchema.SharePointLinkedBy] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Owners
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADUserSchema.Owners];
			}
			internal set
			{
				this[ADUserSchema.Owners] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> TeamMailboxShowInClientList
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADUserSchema.TeamMailboxShowInClientList];
			}
			internal set
			{
				this[ADUserSchema.TeamMailboxShowInClientList] = value;
			}
		}

		public UserConfigXML ConfigXML
		{
			get
			{
				return (UserConfigXML)this[ADRecipientSchema.ConfigurationXML];
			}
			set
			{
				this[ADRecipientSchema.ConfigurationXML] = value;
			}
		}

		public UpgradeStatusTypes UpgradeStatus
		{
			get
			{
				return (UpgradeStatusTypes)this[ADRecipientSchema.UpgradeStatus];
			}
			set
			{
				this[ADRecipientSchema.UpgradeStatus] = value;
			}
		}

		public UpgradeRequestTypes UpgradeRequest
		{
			get
			{
				return (UpgradeRequestTypes)this[ADRecipientSchema.UpgradeRequest];
			}
			set
			{
				this[ADRecipientSchema.UpgradeRequest] = value;
			}
		}

		public string UpgradeDetails
		{
			get
			{
				return (string)this[ADRecipientSchema.UpgradeDetails];
			}
			set
			{
				this[ADRecipientSchema.UpgradeDetails] = value;
			}
		}

		public string UpgradeMessage
		{
			get
			{
				return (string)this[ADRecipientSchema.UpgradeMessage];
			}
			set
			{
				this[ADRecipientSchema.UpgradeMessage] = value;
			}
		}

		public UpgradeStage? UpgradeStage
		{
			get
			{
				return (UpgradeStage?)this[ADRecipientSchema.UpgradeStage];
			}
			set
			{
				this[ADRecipientSchema.UpgradeStage] = value;
			}
		}

		public DateTime? UpgradeStageTimeStamp
		{
			get
			{
				return (DateTime?)this[ADRecipientSchema.UpgradeStageTimeStamp];
			}
			set
			{
				this[ADRecipientSchema.UpgradeStageTimeStamp] = value;
			}
		}

		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)this[ADRecipientSchema.ReleaseTrack];
			}
			set
			{
				this[ADRecipientSchema.ReleaseTrack] = value;
			}
		}

		public MultiValuedProperty<Guid> MailboxGuids
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[ADRecipientSchema.MailboxGuidsRaw];
			}
		}

		public IMailboxLocationCollection MailboxLocations
		{
			get
			{
				if (ADRecipient.IsMailboxLocationsEnabled(this))
				{
					return (IMailboxLocationCollection)this[ADRecipientSchema.MailboxLocations];
				}
				return new MailboxLocationCollection();
			}
			internal set
			{
				if (ADRecipient.IsMailboxLocationsEnabled(this))
				{
					this[ADRecipientSchema.MailboxLocations] = value;
				}
			}
		}

		internal SharingPartnerIdentityCollection SharingPartnerIdentities
		{
			get
			{
				return (SharingPartnerIdentityCollection)this[ADUserSchema.SharingPartnerIdentities];
			}
			set
			{
				this[ADUserSchema.SharingPartnerIdentities] = value;
			}
		}

		internal SharingAnonymousIdentityCollection SharingAnonymousIdentities
		{
			get
			{
				return (SharingAnonymousIdentityCollection)this[ADUserSchema.SharingAnonymousIdentities];
			}
			set
			{
				this[ADUserSchema.SharingAnonymousIdentities] = value;
			}
		}

		internal static string StorageGroupNameGetter(IPropertyBag propertyBag)
		{
			return ADUser.GetStorageGroupNameFromDatabase((ADObjectId)propertyBag[IADMailStorageSchema.Database]);
		}

		internal static string GetStorageGroupNameFromDatabase(ADObjectId databaseId)
		{
			string result;
			try
			{
				if (databaseId != null && !string.IsNullOrEmpty(databaseId.DistinguishedName))
				{
					result = databaseId.AncestorDN(1).Rdn.UnescapedName;
				}
				else
				{
					result = string.Empty;
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("StorageGroupName", ex.Message), IADMailStorageSchema.StorageGroupName, databaseId), ex);
			}
			return result;
		}

		internal static object DatabaseNameGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				result = ADUser.DatabaseNameFromADObjectId((ADObjectId)propertyBag[IADMailStorageSchema.Database]);
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("DatabaseName", ex.Message), IADMailStorageSchema.DatabaseName, propertyBag[IADMailStorageSchema.Database]), ex);
			}
			return result;
		}

		internal static string DatabaseNameFromADObjectId(ADObjectId homMdb)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (homMdb != null && !string.IsNullOrEmpty(homMdb.DistinguishedName))
			{
				stringBuilder.Append(homMdb.AncestorDN(1).Rdn.UnescapedName);
				stringBuilder.Append('\\');
				stringBuilder.Append(homMdb.AncestorDN(0).Rdn.UnescapedName);
			}
			return stringBuilder.ToString();
		}

		internal static object IsMailboxEnabledGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[IADMailStorageSchema.Database];
			return obj != null;
		}

		internal static QueryFilter IsMailboxEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			bool flag = (bool)ADObject.PropertyValueFromEqualityFilter(filter);
			QueryFilter queryFilter = new ExistsFilter(IADMailStorageSchema.Database);
			if (!flag)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static object ServerNameGetter(IPropertyBag propertyBag)
		{
			return DNConvertor.ServerNameFromServerLegacyDN((string)propertyBag[IADMailStorageSchema.ServerLegacyDN]);
		}

		internal static QueryFilter ServerNameFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			QueryFilter queryFilter = new TextFilter(ADMailboxRecipientSchema.ServerLegacyDN, "/cn=Configuration/cn=Servers/cn=" + (string)comparisonFilter.PropertyValue, MatchOptions.Suffix, MatchFlags.Default);
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				return queryFilter;
			case ComparisonOperator.NotEqual:
				return new NotFilter(queryFilter);
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
		}

		internal static object UseDatabaseRetentionDefaultsGetter(IPropertyBag propertyBag)
		{
			return (DeletedItemRetention)propertyBag[IADMailStorageSchema.DeletedItemFlags] == DeletedItemRetention.DatabaseDefault;
		}

		internal static void UseDatabaseRetentionDefaultsSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[IADMailStorageSchema.DeletedItemFlags] = DeletedItemRetention.DatabaseDefault;
				return;
			}
			if ((DeletedItemRetention)propertyBag[IADMailStorageSchema.DeletedItemFlags] == DeletedItemRetention.DatabaseDefault)
			{
				propertyBag[IADMailStorageSchema.DeletedItemFlags] = DeletedItemRetention.RetainForCustomPeriod;
			}
		}

		internal static object RetainDeletedItemsUntilBackupGetter(IPropertyBag propertyBag)
		{
			return (DeletedItemRetention)propertyBag[IADMailStorageSchema.DeletedItemFlags] == DeletedItemRetention.RetainUntilBackupOrCustomPeriod;
		}

		internal static void RetainDeletedItemsUntilBackupSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[IADMailStorageSchema.DeletedItemFlags] = DeletedItemRetention.RetainUntilBackupOrCustomPeriod;
				return;
			}
			if ((DeletedItemRetention)propertyBag[IADMailStorageSchema.DeletedItemFlags] == DeletedItemRetention.RetainUntilBackupOrCustomPeriod)
			{
				propertyBag[IADMailStorageSchema.DeletedItemFlags] = DeletedItemRetention.RetainForCustomPeriod;
			}
		}

		internal static object DowngradeHighPriorityMessagesEnabledGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<byte[]> multiValuedProperty = (MultiValuedProperty<byte[]>)propertyBag[ADUserSchema.SecurityProtocol];
			bool flag = false;
			if (0 < multiValuedProperty.Count)
			{
				if (1 < multiValuedProperty.Count)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(ADUserSchema.DowngradeHighPriorityMessagesEnabled.Name, DirectoryStrings.TooManyDataInLdapProperty(ADUserSchema.SecurityProtocol.LdapDisplayName, 1)), ADUserSchema.DowngradeHighPriorityMessagesEnabled, multiValuedProperty));
				}
				byte[] array = multiValuedProperty[0];
				if (array.Length >= 4)
				{
					flag = (0 != (array[3] & 128));
				}
			}
			return flag;
		}

		internal static void DowngradeHighPriorityMessagesEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			bool flag2 = (bool)ADUser.DowngradeHighPriorityMessagesEnabledGetter(propertyBag);
			if (flag != flag2)
			{
				MultiValuedProperty<byte[]> multiValuedProperty = (MultiValuedProperty<byte[]>)propertyBag[ADUserSchema.SecurityProtocol];
				int num = 4;
				if (0 < multiValuedProperty.Count)
				{
					num = Math.Max(num, multiValuedProperty[0].Length);
				}
				byte[] array = new byte[num];
				Array.Clear(array, 0, num);
				if (0 < multiValuedProperty.Count)
				{
					multiValuedProperty[0].CopyTo(array, 0);
				}
				if (flag)
				{
					byte[] array2 = array;
					int num2 = 3;
					array2[num2] |= 128;
				}
				else
				{
					byte[] array3 = array;
					int num3 = 3;
					array3[num3] &= 127;
				}
				if (0 < multiValuedProperty.Count)
				{
					multiValuedProperty.Clear();
				}
				multiValuedProperty.Add(array);
			}
		}

		internal static object ElcExpirationSuspensionEnabledGetter(IPropertyBag propertyBag)
		{
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			return (elcMailboxFlags & ElcMailboxFlags.ExpirationSuspended) == ElcMailboxFlags.ExpirationSuspended;
		}

		internal static void ElcExpirationSuspensionEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)(value ?? false);
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			if (flag)
			{
				propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags | ElcMailboxFlags.ExpirationSuspended);
				return;
			}
			propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags & ~ElcMailboxFlags.ExpirationSuspended);
			propertyBag[ADUserSchema.ElcExpirationSuspensionStartDate] = null;
			propertyBag[ADUserSchema.ElcExpirationSuspensionEndDate] = null;
		}

		internal static object LitigationHoldEnabledGetter(IPropertyBag propertyBag)
		{
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			return (elcMailboxFlags & ElcMailboxFlags.LitigationHold) == ElcMailboxFlags.LitigationHold;
		}

		internal static void LitigationHoldEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)(value ?? false);
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			if (flag)
			{
				propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags | ElcMailboxFlags.LitigationHold);
				propertyBag[ADUserSchema.LitigationHoldDate] = DateTime.UtcNow;
				return;
			}
			propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags & ~ElcMailboxFlags.LitigationHold);
			propertyBag[ADUserSchema.RetentionComment] = null;
			propertyBag[ADUserSchema.RetentionUrl] = null;
			propertyBag[ADUserSchema.LitigationHoldDate] = null;
			propertyBag[ADUserSchema.LitigationHoldOwner] = null;
		}

		internal static object SingleItemRecoveryEnabledGetter(IPropertyBag propertyBag)
		{
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			return (elcMailboxFlags & ElcMailboxFlags.SingleItemRecovery) == ElcMailboxFlags.SingleItemRecovery;
		}

		internal static void SingleItemRecoveryEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)(value ?? false);
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			if (flag)
			{
				propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags | ElcMailboxFlags.SingleItemRecovery);
				return;
			}
			propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags & ~ElcMailboxFlags.SingleItemRecovery);
		}

		internal static object CalendarVersionStoreDisabledGetter(IPropertyBag propertyBag)
		{
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			return (elcMailboxFlags & ElcMailboxFlags.DisableCalendarLogging) == ElcMailboxFlags.DisableCalendarLogging;
		}

		internal static void CalendarVersionStoreDisabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)(value ?? false);
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			if (flag)
			{
				propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags | ElcMailboxFlags.DisableCalendarLogging);
				return;
			}
			propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags & ~ElcMailboxFlags.DisableCalendarLogging);
		}

		internal static object SiteMailboxMessageDedupEnabledGetter(IPropertyBag propertyBag)
		{
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			return (elcMailboxFlags & ElcMailboxFlags.EnableSiteMailboxMessageDedup) == ElcMailboxFlags.EnableSiteMailboxMessageDedup;
		}

		internal static void SiteMailboxMessageDedupEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)(value ?? false);
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			if (flag)
			{
				propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags | ElcMailboxFlags.EnableSiteMailboxMessageDedup);
				return;
			}
			propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags & ~ElcMailboxFlags.EnableSiteMailboxMessageDedup);
		}

		internal static QueryFilter ActiveSyncEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new NotFilter(new BitMaskAndFilter(ADUserSchema.MobileFeaturesEnabled, 4UL)));
		}

		internal static QueryFilter HasActiveSyncDevicePartnershipFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ADUserSchema.MobileMailboxFlags, 1UL));
		}

		internal static QueryFilter OWAforDevicesEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new NotFilter(new BitMaskAndFilter(ADUserSchema.MobileFeaturesEnabled, 8UL)));
		}

		internal static object ManagedFolderMailboxPolicyGetter(IPropertyBag propertyBag)
		{
			if (((ElcMailboxFlags)(propertyBag[ADUserSchema.ElcMailboxFlags] ?? ElcMailboxFlags.None) & ElcMailboxFlags.ElcV2) == ElcMailboxFlags.None)
			{
				return (ADObjectId)propertyBag[ADUserSchema.ElcPolicyTemplate];
			}
			return null;
		}

		internal static void ManagedFolderMailboxPolicySetter(object value, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				propertyBag[IADMailStorageSchema.ElcMailboxFlags] = ((ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags] & ~ElcMailboxFlags.ElcV2);
				propertyBag[ADUserSchema.ElcPolicyTemplate] = value;
			}
			else if (((ElcMailboxFlags)(propertyBag[ADUserSchema.ElcMailboxFlags] ?? ElcMailboxFlags.None) & ElcMailboxFlags.ElcV2) == ElcMailboxFlags.None)
			{
				propertyBag[ADUserSchema.ElcPolicyTemplate] = value;
			}
			propertyBag[IADMailStorageSchema.ElcMailboxFlags] = ((ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags] & ~ElcMailboxFlags.ShouldUseDefaultRetentionPolicy);
		}

		internal static object RetentionPolicyGetter(IPropertyBag propertyBag)
		{
			if (((ElcMailboxFlags)(propertyBag[ADUserSchema.ElcMailboxFlags] ?? ElcMailboxFlags.None) & ElcMailboxFlags.ElcV2) == ElcMailboxFlags.ElcV2)
			{
				return (ADObjectId)propertyBag[ADUserSchema.ElcPolicyTemplate];
			}
			return null;
		}

		internal static void RetentionPolicySetter(object value, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				propertyBag[IADMailStorageSchema.ElcMailboxFlags] = ((ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags] | ElcMailboxFlags.ElcV2);
				propertyBag[ADUserSchema.ElcPolicyTemplate] = value;
			}
			else if (((ElcMailboxFlags)(propertyBag[ADUserSchema.ElcMailboxFlags] ?? ElcMailboxFlags.None) & ElcMailboxFlags.ElcV2) == ElcMailboxFlags.ElcV2)
			{
				propertyBag[ADUserSchema.ElcPolicyTemplate] = value;
			}
			propertyBag[IADMailStorageSchema.ElcMailboxFlags] = ((ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags] & ~ElcMailboxFlags.ShouldUseDefaultRetentionPolicy);
		}

		internal static object ArchiveStateGetter(IPropertyBag propertyBag)
		{
			if (propertyBag[ADUserSchema.ArchiveGuid] == null || !((Guid)propertyBag[ADUserSchema.ArchiveGuid] != Guid.Empty))
			{
				return ArchiveState.None;
			}
			if (propertyBag[ADUserSchema.ArchiveDatabase] != null || (propertyBag[ADMailboxRecipientSchema.Database] != null && propertyBag[ADUserSchema.ArchiveDomain] == null))
			{
				return ArchiveState.Local;
			}
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)(propertyBag[ADRecipientSchema.RecipientTypeDetailsValue] ?? RecipientTypeDetails.None);
			if (propertyBag[ADUserSchema.ArchiveDomain] == null && !RemoteMailbox.IsRemoteMailbox(recipientTypeDetails))
			{
				return ArchiveState.OnPremise;
			}
			ArchiveStatusFlags archiveStatusFlags = (ArchiveStatusFlags)(propertyBag[ADUserSchema.ArchiveStatus] ?? ArchiveStatusFlags.None);
			if ((archiveStatusFlags & ArchiveStatusFlags.Active) == ArchiveStatusFlags.Active)
			{
				return ArchiveState.HostedProvisioned;
			}
			return ArchiveState.HostedPending;
		}

		internal static bool ParseSecIDValue(string value, out NetID puid, out string netIdSuffix)
		{
			puid = null;
			netIdSuffix = string.Empty;
			return !string.IsNullOrEmpty(value) && (ADUser.TryParseNetIDAndSuffix(value, out puid, out netIdSuffix) || (value.StartsWith("KERBEROS:", StringComparison.OrdinalIgnoreCase) && value.EndsWith("@live.com", StringComparison.OrdinalIgnoreCase) && NetID.TryParse(value.Substring("KERBEROS:".Length, value.Length - "@live.com".Length - "KERBEROS:".Length), out puid)) || (value.StartsWith("MS-WLID:", StringComparison.OrdinalIgnoreCase) && NetID.TryParse(value.Substring("MS-WLID:".Length), out puid)));
		}

		private static bool TryParseNetIDAndSuffix(string value, out NetID puid, out string netIdSuffix)
		{
			puid = null;
			netIdSuffix = string.Empty;
			int num = value.IndexOf('-');
			if (value.StartsWith("ExWLID:", StringComparison.OrdinalIgnoreCase) && num != -1 && NetID.TryParse(value.Substring("ExWLID:".Length, num - "ExWLID:".Length), out puid))
			{
				netIdSuffix = value.Substring(num + 1);
				return true;
			}
			return false;
		}

		internal static bool TryParseSecIDValueFromPropertyBag(IPropertyBag propertyBag, out NetID puid, out string netIdSuffix)
		{
			puid = null;
			netIdSuffix = string.Empty;
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[IADSecurityPrincipalSchema.AltSecurityIdentities];
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				foreach (string value in multiValuedProperty)
				{
					if (ADUser.ParseSecIDValue(value, out puid, out netIdSuffix))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal static void SetSecIDValue(NetID puid, string netIdSuffix, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[IADSecurityPrincipalSchema.AltSecurityIdentities];
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				NetID netID = null;
				string empty = string.Empty;
				int i = 0;
				while (i < multiValuedProperty.Count)
				{
					if (ADUser.ParseSecIDValue(multiValuedProperty[i], out netID, out empty))
					{
						multiValuedProperty.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
			if (puid != null)
			{
				if (!string.IsNullOrEmpty(netIdSuffix))
				{
					multiValuedProperty.Add(string.Format("ExWLID:{0}-{1}", puid, netIdSuffix));
					return;
				}
				multiValuedProperty.Add("KERBEROS:" + puid.ToString() + "@live.com");
				multiValuedProperty.Add("MS-WLID:" + puid.ToString());
			}
		}

		internal static object NetIdGetter(IPropertyBag propertyBag)
		{
			NetID result = null;
			string empty = string.Empty;
			if (ADUser.TryParseSecIDValueFromPropertyBag(propertyBag, out result, out empty))
			{
				return result;
			}
			return null;
		}

		internal static void NetIdSetter(object value, IPropertyBag propertyBag)
		{
			NetID puid = (NetID)value;
			string netIdSuffix = (string)propertyBag[IADSecurityPrincipalSchema.NetIDSuffix];
			ADUser.SetSecIDValue(puid, netIdSuffix, propertyBag);
		}

		internal static QueryFilter InPlaceHoldsFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			QueryFilter queryFilter = new TextFilter(ADRecipientSchema.InPlaceHoldsRaw, comparisonFilter.PropertyValue.ToString(), MatchOptions.FullString, MatchFlags.IgnoreCase);
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				return queryFilter;
			case ComparisonOperator.NotEqual:
				return new NotFilter(queryFilter);
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
		}

		internal static QueryFilter NetIdFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new TextFilter(IADSecurityPrincipalSchema.AltSecurityIdentities, "MS-WLID:" + ((NetID)comparisonFilter.PropertyValue).ToString(), MatchOptions.FullString, MatchFlags.IgnoreCase),
				new TextFilter(IADSecurityPrincipalSchema.AltSecurityIdentities, "KERBEROS:" + ((NetID)comparisonFilter.PropertyValue).ToString() + "@live.com", MatchOptions.FullString, MatchFlags.IgnoreCase),
				new TextFilter(IADSecurityPrincipalSchema.AltSecurityIdentities, "ExWLID:" + ((NetID)comparisonFilter.PropertyValue).ToString(), MatchOptions.Prefix, MatchFlags.IgnoreCase)
			});
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				return queryFilter;
			case ComparisonOperator.NotEqual:
				return new NotFilter(queryFilter);
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
		}

		internal static bool ParseConsumerSecIDValue(string value, out NetID puid)
		{
			return ADUser.ParseSecIDValue(value, "CS-WLID:", out puid);
		}

		internal static bool ParseSecIDValue(string value, string prefix, out NetID puid)
		{
			puid = null;
			return !string.IsNullOrEmpty(value) && value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && NetID.TryParse(value.Substring(prefix.Length), out puid);
		}

		internal static bool TryParseSecIDValueFromPropertyBag(IPropertyBag propertyBag, string prefix, out NetID puid)
		{
			puid = null;
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[IADSecurityPrincipalSchema.AltSecurityIdentities];
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				foreach (string value in multiValuedProperty)
				{
					if (ADUser.ParseSecIDValue(value, prefix, out puid))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal void UpdateSoftDeletedStatusForHold(bool enableHold)
		{
			int recipientSoftDeletedStatus;
			if (enableHold)
			{
				recipientSoftDeletedStatus = ((base.RecipientSoftDeletedStatus | 8) & -5);
			}
			else
			{
				recipientSoftDeletedStatus = ((base.RecipientSoftDeletedStatus & -9) | 4);
			}
			base.RecipientSoftDeletedStatus = recipientSoftDeletedStatus;
		}

		internal void SetLitigationHoldEnabledWellKnownInPlaceHoldGuid(bool litigationHoldEnabled)
		{
			MultiValuedProperty<string> multiValuedProperty = this.propertyBag[ADRecipientSchema.InPlaceHoldsRaw] as MultiValuedProperty<string>;
			if (litigationHoldEnabled)
			{
				if (multiValuedProperty == null)
				{
					multiValuedProperty = new MultiValuedProperty<string>();
				}
				if (!multiValuedProperty.Contains("98E9BABD09A04bcf8455A58C2AA74182"))
				{
					multiValuedProperty.Add("98E9BABD09A04bcf8455A58C2AA74182");
				}
				this.propertyBag[ADRecipientSchema.InPlaceHoldsRaw] = multiValuedProperty;
				return;
			}
			MultiValuedProperty<string> multiValuedProperty2 = new MultiValuedProperty<string>();
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				foreach (string text in multiValuedProperty)
				{
					if (!text.StartsWith("98E9BABD09A04bcf8455A58C2AA74182", StringComparison.OrdinalIgnoreCase))
					{
						multiValuedProperty2.Add(text);
					}
				}
			}
			this.propertyBag[ADRecipientSchema.InPlaceHoldsRaw] = multiValuedProperty2;
		}

		internal static void SetBasicSecIDValue(NetID puid, string prefix, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[IADSecurityPrincipalSchema.AltSecurityIdentities];
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				NetID netID = null;
				int i = 0;
				while (i < multiValuedProperty.Count)
				{
					if (ADUser.ParseSecIDValue(multiValuedProperty[i], prefix, out netID))
					{
						multiValuedProperty.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
			if (puid != null)
			{
				multiValuedProperty.Add(prefix + puid.ToString());
			}
		}

		internal static object ConsumerNetIdGetter(IPropertyBag propertyBag)
		{
			NetID result = null;
			if (ADUser.TryParseSecIDValueFromPropertyBag(propertyBag, "CS-WLID:", out result))
			{
				return result;
			}
			return null;
		}

		internal static void ConsumerNetIdSetter(object value, IPropertyBag propertyBag)
		{
			NetID puid = (NetID)value;
			ADUser.SetSecIDValue(puid, "CS-WLID:", propertyBag);
		}

		internal static QueryFilter ConsumerNetIdFilterBuilder(SinglePropertyFilter filter)
		{
			return ADUser.BasicNetIdFilterBuilder("CS-WLID:", filter);
		}

		internal static QueryFilter BasicNetIdFilterBuilder(string prefix, SinglePropertyFilter filter)
		{
			if (filter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
				QueryFilter queryFilter = new TextFilter(IADSecurityPrincipalSchema.AltSecurityIdentities, prefix + ((NetID)comparisonFilter.PropertyValue).ToString(), MatchOptions.FullString, MatchFlags.IgnoreCase);
				switch (comparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					return queryFilter;
				case ComparisonOperator.NotEqual:
					return new NotFilter(queryFilter);
				default:
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
				}
			}
			else
			{
				if (filter is ExistsFilter)
				{
					return new TextFilter(IADSecurityPrincipalSchema.AltSecurityIdentities, prefix, MatchOptions.Prefix, MatchFlags.IgnoreCase);
				}
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
		}

		internal static object OriginalNetIdGetter(IPropertyBag propertyBag)
		{
			NetID result = null;
			if (ADUser.TryParseSecIDValueFromPropertyBag(propertyBag, "EXORIGNETID:", out result))
			{
				return result;
			}
			return null;
		}

		internal static void OriginalNetIdSetter(object value, IPropertyBag propertyBag)
		{
			NetID puid = (NetID)value;
			ADUser.SetBasicSecIDValue(puid, "EXORIGNETID:", propertyBag);
		}

		internal static QueryFilter OriginalNetIdFilterBuilder(SinglePropertyFilter filter)
		{
			return ADUser.BasicNetIdFilterBuilder("EXORIGNETID:", filter);
		}

		internal static object NetIdSuffixGetter(IPropertyBag propertyBag)
		{
			NetID netID = null;
			string empty = string.Empty;
			if (ADUser.TryParseSecIDValueFromPropertyBag(propertyBag, out netID, out empty))
			{
				return empty;
			}
			return null;
		}

		internal static void NetIdSuffixSetter(object value, IPropertyBag propertyBag)
		{
			string netIdSuffix = (string)value;
			NetID puid = (NetID)propertyBag[IADSecurityPrincipalSchema.NetID];
			ADUser.SetSecIDValue(puid, netIdSuffix, propertyBag);
		}

		internal static object CertificateSubjectGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADUserSchema.AltSecurityIdentities];
			MultiValuedProperty<X509Identifier> multiValuedProperty2 = new MultiValuedProperty<X509Identifier>();
			foreach (string text in multiValuedProperty)
			{
				if (!string.IsNullOrEmpty(text) && text.StartsWith("X509:", StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						multiValuedProperty2.Add(X509Identifier.Parse(text));
					}
					catch (FormatException ex)
					{
						throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(ADUserSchema.CertificateSubject.Name, ex.Message), ADUserSchema.CertificateSubject, propertyBag[ADUserSchema.AltSecurityIdentities]), ex);
					}
					catch (InvalidOperationException ex2)
					{
						throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(ADUserSchema.CertificateSubject.Name, ex2.Message), ADUserSchema.CertificateSubject, propertyBag[ADUserSchema.AltSecurityIdentities]), ex2);
					}
				}
			}
			return multiValuedProperty2;
		}

		internal static void CertificateSubjectSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<X509Identifier> multiValuedProperty = (MultiValuedProperty<X509Identifier>)value;
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[ADUserSchema.AltSecurityIdentities];
			for (int i = multiValuedProperty2.Count - 1; i >= 0; i--)
			{
				if (!string.IsNullOrEmpty(multiValuedProperty2[i]) && multiValuedProperty2[i].StartsWith("X509:", StringComparison.OrdinalIgnoreCase))
				{
					multiValuedProperty2.RemoveAt(i);
				}
			}
			foreach (X509Identifier x509Identifier in multiValuedProperty)
			{
				multiValuedProperty2.Add(x509Identifier.ToString());
			}
		}

		internal static QueryFilter CertificateSubjectFilterBuilder(SinglePropertyFilter filter)
		{
			X509Identifier x509Identifier = (X509Identifier)ADObject.PropertyValueFromEqualityFilter(filter);
			if (x509Identifier == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.AltSecurityIdentities, x509Identifier.ToString());
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				return queryFilter;
			case ComparisonOperator.NotEqual:
				return new NotFilter(queryFilter);
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
		}

		internal static QueryFilter GetCertificateMatchFilter(X509Identifier identifier)
		{
			QueryFilter queryFilter = null;
			if (identifier != null)
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.CertificateSubject, identifier);
				if (!identifier.IsGenericIdentifier)
				{
					queryFilter = new OrFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.CertificateSubject, new X509Identifier(identifier.Issuer))
					});
				}
			}
			return queryFilter;
		}

		internal static object SharingPartnerIdentitiesGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> mvp = (MultiValuedProperty<string>)propertyBag[IADMailStorageSchema.SharingPartnerIdentitiesRaw];
			return new SharingPartnerIdentityCollection(mvp);
		}

		internal static void SharingPartnerIdentitiesSetter(object value, IPropertyBag propertyBag)
		{
			SharingPartnerIdentityCollection sharingPartnerIdentityCollection = (SharingPartnerIdentityCollection)value;
			propertyBag[IADMailStorageSchema.SharingPartnerIdentitiesRaw] = sharingPartnerIdentityCollection.InnerCollection;
		}

		internal static object SharingAnonymousIdentitiesGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> sharingAnonymousIdentities = (MultiValuedProperty<string>)propertyBag[IADMailStorageSchema.SharingAnonymousIdentitiesRaw];
			return new SharingAnonymousIdentityCollection(sharingAnonymousIdentities);
		}

		internal static void SharingAnonymousIdentitiesSetter(object value, IPropertyBag propertyBag)
		{
			SharingAnonymousIdentityCollection sharingAnonymousIdentityCollection = (SharingAnonymousIdentityCollection)value;
			propertyBag[IADMailStorageSchema.SharingAnonymousIdentitiesRaw] = sharingAnonymousIdentityCollection.GetRawSharingAnonymousIdentities();
		}

		internal static QueryFilter SKUAssignedFilterBuilder(SinglePropertyFilter filter)
		{
			QueryFilter queryFilter = new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 64UL);
			QueryFilter queryFilter2 = new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 128UL);
			if (filter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
				if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
				{
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
				}
				QueryFilter queryFilter3;
				if (comparisonFilter.PropertyValue == null)
				{
					queryFilter3 = new AndFilter(new QueryFilter[]
					{
						new NotFilter(queryFilter),
						new NotFilter(queryFilter2)
					});
				}
				else if ((bool)comparisonFilter.PropertyValue)
				{
					queryFilter3 = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new NotFilter(queryFilter2)
					});
				}
				else
				{
					queryFilter3 = queryFilter2;
				}
				if (ComparisonOperator.NotEqual == comparisonFilter.ComparisonOperator)
				{
					queryFilter3 = new NotFilter(queryFilter3);
				}
				return queryFilter3;
			}
			else
			{
				if (filter is ExistsFilter)
				{
					return new OrFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter2
					});
				}
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
		}

		internal static object SKUAssignedGetter(IPropertyBag bag)
		{
			bool flag = (bool)ADObject.FlagGetterDelegate(64, ADRecipientSchema.ProvisioningFlags)(bag);
			bool flag2 = (bool)ADObject.FlagGetterDelegate(128, ADRecipientSchema.ProvisioningFlags)(bag);
			if (flag2)
			{
				return false;
			}
			if (flag)
			{
				return true;
			}
			return null;
		}

		internal static void SKUAssignedSetter(object value, IPropertyBag bag)
		{
			bool? flag = (bool?)value;
			bool flag2 = flag != null && flag.Value;
			bool flag3 = flag != null && !flag.Value;
			ADObject.FlagSetterDelegate(64, ADRecipientSchema.ProvisioningFlags)(flag2, bag);
			ADObject.FlagSetterDelegate(128, ADRecipientSchema.ProvisioningFlags)(flag3, bag);
		}

		internal static object ExchangeSecurityDescriptorGetter(IPropertyBag bag)
		{
			return bag[IADMailStorageSchema.ExchangeSecurityDescriptorRaw];
		}

		internal static void ExchangeSecurityDescriptorSetter(object value, IPropertyBag bag)
		{
			RawSecurityDescriptor rawSecurityDescriptor = value as RawSecurityDescriptor;
			if (rawSecurityDescriptor != null)
			{
				value = ExchangeSecurityDescriptorHelper.RemoveInheritedACEs(rawSecurityDescriptor);
			}
			bag[IADMailStorageSchema.ExchangeSecurityDescriptorRaw] = value;
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			if (base.OrganizationId == null || base.RecipientTypeDetails != RecipientTypeDetails.MailboxPlan || base.ObjectState == ObjectState.Unchanged)
			{
				return false;
			}
			orgId = base.OrganizationId;
			keys = new Guid[3];
			keys[0] = CannedProvisioningCacheKeys.DefaultMailboxPlan;
			keys[1] = CannedProvisioningCacheKeys.CacheKeyMailboxPlanIdParameterId;
			keys[2] = CannedProvisioningCacheKeys.CacheKeyMailboxPlanId;
			return true;
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		public bool HasActiveSyncDevicePartnership
		{
			get
			{
				return (bool)this[ADUserSchema.HasActiveSyncDevicePartnership];
			}
			internal set
			{
				this[ADUserSchema.HasActiveSyncDevicePartnership] = value;
			}
		}

		public bool IsFromDatacenter
		{
			get
			{
				return this.NetID != null;
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				return (ArchiveState)this[ADUserSchema.ArchiveState];
			}
		}

		public bool IsAllowedToModifyOwners
		{
			get
			{
				return TeamMailbox.IsLocalTeamMailbox(this) || TeamMailbox.IsRemoteTeamMailbox(this) || GroupMailbox.IsLocalGroupMailbox(this);
			}
		}

		public bool HasLocalArchive
		{
			get
			{
				return this.ArchiveState == ArchiveState.Local;
			}
		}

		public bool HasSeparatedArchive
		{
			get
			{
				return !(this.ArchiveGuid == Guid.Empty) && ((this.ArchiveDatabase != null && !this.ArchiveDatabase.Equals(base.Database)) || (base.Database != null && !this.HasLocalArchive));
			}
		}

		public int? MaxSafeSenders
		{
			get
			{
				return (int?)this[ADUserSchema.MaxSafeSenders];
			}
			internal set
			{
				this[ADUserSchema.MaxSafeSenders] = value;
			}
		}

		public int? MaxBlockedSenders
		{
			get
			{
				return (int?)this[ADUserSchema.MaxBlockedSenders];
			}
			internal set
			{
				this[ADUserSchema.MaxBlockedSenders] = value;
			}
		}

		public bool IsDefault
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsDefault];
			}
			internal set
			{
				this[ADRecipientSchema.IsDefault] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public bool RemotePowerShellEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.RemotePowerShellEnabled];
			}
			set
			{
				this[ADRecipientSchema.RemotePowerShellEnabled] = value;
			}
		}

		internal object DatabaseAndLocation
		{
			get
			{
				return this[IADMailStorageSchema.DatabaseAndLocation];
			}
			set
			{
				this[IADMailStorageSchema.DatabaseAndLocation] = value;
			}
		}

		public RemoteRecipientType RemoteRecipientType
		{
			get
			{
				return (RemoteRecipientType)this[ADUserSchema.RemoteRecipientType];
			}
			internal set
			{
				this[ADUserSchema.RemoteRecipientType] = value;
			}
		}

		public DateTime? LastExchangeChangedTime
		{
			get
			{
				return (DateTime?)this[ADRecipientSchema.LastExchangeChangedTime];
			}
			set
			{
				this[ADRecipientSchema.LastExchangeChangedTime] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> CatchAllRecipientBL
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADUserSchema.CatchAllRecipientBL];
			}
		}

		public bool AutoSubscribeNewGroupMembers
		{
			get
			{
				return (bool)this[ADRecipientSchema.AutoSubscribeNewGroupMembers];
			}
			set
			{
				this[ADRecipientSchema.AutoSubscribeNewGroupMembers] = value;
			}
		}

		internal MultiValuedProperty<ADObjectId> GeneratedOfflineAddressBooks
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.GeneratedOfflineAddressBooks];
			}
		}

		public bool AccountDisabled
		{
			get
			{
				return (bool)this[ADUserSchema.AccountDisabled];
			}
			set
			{
				this[ADUserSchema.AccountDisabled] = value;
			}
		}

		public DateTime? StsRefreshTokensValidFrom
		{
			get
			{
				return (DateTime?)this[ADUserSchema.StsRefreshTokensValidFrom];
			}
			set
			{
				this[ADUserSchema.StsRefreshTokensValidFrom] = value;
			}
		}

		public ADObjectId ObjectId
		{
			get
			{
				return base.Id;
			}
		}

		internal FederatedIdentity GetFederatedIdentity()
		{
			return FederatedIdentityHelper.GetFederatedIdentity(this);
		}

		public SmtpAddress GetFederatedSmtpAddress()
		{
			return this.GetFederatedSmtpAddress(base.PrimarySmtpAddress);
		}

		internal SmtpAddress GetFederatedSmtpAddress(SmtpAddress preferredSmtpAddress)
		{
			OrganizationId organizationId = base.OrganizationId ?? OrganizationId.ForestWideOrgId;
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			if (organizationIdCacheValue.FederatedDomains == null)
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "No federated domains found for tenant '{0}'", organizationId);
				throw new UserWithoutFederatedProxyAddressException();
			}
			if (organizationIdCacheValue.DefaultFederatedDomain != null)
			{
				foreach (ProxyAddress proxyAddress in base.EmailAddresses)
				{
					if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
					{
						SmtpAddress smtpAddress = new SmtpAddress(proxyAddress.AddressString);
						if (StringComparer.OrdinalIgnoreCase.Equals(smtpAddress.Domain, organizationIdCacheValue.DefaultFederatedDomain))
						{
							ExTraceGlobals.FederatedIdentityTracer.TraceDebug<SmtpAddress, ADObjectId>((long)this.GetHashCode(), "Using SMTP address '{0}' for user {1} because it matches default federated domain", smtpAddress, base.Id);
							return smtpAddress;
						}
					}
				}
				ExTraceGlobals.FederatedIdentityTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "No proxy address for user '{0}' matches the default federated domain.", base.Id);
			}
			List<string> source = new List<string>(organizationIdCacheValue.FederatedDomains);
			bool isValidAddress = preferredSmtpAddress.IsValidAddress;
			if (isValidAddress && !base.EmailAddresses.Contains(new SmtpProxyAddress(preferredSmtpAddress.ToString(), false)))
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceDebug<SmtpAddress, ADObjectId>((long)this.GetHashCode(), "Preferred SMTP address '{0}' is not one of the proxy addresses for user '{1}' ", preferredSmtpAddress, base.Id);
				throw new ArgumentException("preferredSmtpAddress");
			}
			if (isValidAddress)
			{
				if (source.Contains(preferredSmtpAddress.Domain, StringComparer.OrdinalIgnoreCase))
				{
					ExTraceGlobals.FederatedIdentityTracer.TraceDebug<SmtpAddress, ADObjectId>((long)this.GetHashCode(), "Using preferred SMTP address '{0}' for user {1} because it is a federated domain", preferredSmtpAddress, base.Id);
					return preferredSmtpAddress;
				}
				if (base.PrimarySmtpAddress.IsValidAddress && !StringComparer.OrdinalIgnoreCase.Equals(base.PrimarySmtpAddress.Domain, preferredSmtpAddress.Domain) && source.Contains(base.PrimarySmtpAddress.Domain, StringComparer.OrdinalIgnoreCase))
				{
					ExTraceGlobals.FederatedIdentityTracer.TraceDebug<SmtpAddress, ADObjectId>((long)this.GetHashCode(), "Using primary SMTP address '{0}' for user {1} because it is a federated domain", base.PrimarySmtpAddress, base.Id);
					return base.PrimarySmtpAddress;
				}
			}
			foreach (ProxyAddress proxyAddress2 in base.EmailAddresses)
			{
				if (proxyAddress2.Prefix == ProxyAddressPrefix.Smtp)
				{
					SmtpAddress smtpAddress2 = new SmtpAddress(proxyAddress2.AddressString);
					if (source.Contains(smtpAddress2.Domain, StringComparer.OrdinalIgnoreCase))
					{
						ExTraceGlobals.FederatedIdentityTracer.TraceDebug<SmtpAddress, ADObjectId>((long)this.GetHashCode(), "Using secondary SMTP address '{0}' for user {1} because it is a federated domain", smtpAddress2, base.Id);
						return smtpAddress2;
					}
				}
			}
			ExTraceGlobals.FederatedIdentityTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "Could not find a SMTP proxy address corresponding to a federated accepted domain for user {0}.", base.Id);
			throw new UserWithoutFederatedProxyAddressException();
		}

		internal List<string> GetFederatedEmailAddresses()
		{
			OrganizationId organizationId = base.OrganizationId ?? OrganizationId.ForestWideOrgId;
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			if (organizationIdCacheValue.FederatedDomains == null)
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "No federated domains found for tenant '{0}'", organizationId);
				return null;
			}
			List<string> list = new List<string>();
			List<string> source = new List<string>(organizationIdCacheValue.FederatedDomains);
			foreach (ProxyAddress proxyAddress in base.EmailAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
				{
					SmtpAddress arg = new SmtpAddress(proxyAddress.AddressString);
					if (source.Contains(arg.Domain, StringComparer.OrdinalIgnoreCase))
					{
						ExTraceGlobals.FederatedIdentityTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "Adding address {0} to the list of email addresses", arg);
						list.Add(proxyAddress.AddressString);
					}
				}
			}
			return list;
		}

		internal FederatedOrganizationId LookupFederatedOrganizationId()
		{
			OrganizationId organizationId = base.OrganizationId ?? OrganizationId.ForestWideOrgId;
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			FederatedOrganizationId federatedOrganizationId = organizationIdCacheValue.FederatedOrganizationId;
			if (federatedOrganizationId == null)
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceError<ADObjectId>((long)this.GetHashCode(), "Unable to find federated organization id for user {0}", base.Id);
				throw new InvalidFederatedOrganizationIdException(DirectoryStrings.FederatedOrganizationIdNotFound);
			}
			if (!federatedOrganizationId.Enabled)
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceError<ADObjectId>((long)this.GetHashCode(), "Federated organization id {0} is not enabled", federatedOrganizationId.Id);
				throw new InvalidFederatedOrganizationIdException(DirectoryStrings.FederatedOrganizationIdNotEnabled);
			}
			if (federatedOrganizationId.AccountNamespace == null)
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceError<ADObjectId>((long)this.GetHashCode(), "Federated organization id {0} doesn't have AccountNamespace set", federatedOrganizationId.Id);
				throw new InvalidFederatedOrganizationIdException(DirectoryStrings.FederatedOrganizationIdNoNamespaceAccount);
			}
			if (organizationIdCacheValue.FederatedDomains == null)
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "Organization is not federated: {0}", organizationId);
				throw new InvalidFederatedOrganizationIdException(DirectoryStrings.FederatedOrganizationIdNoFederatedDomains);
			}
			return federatedOrganizationId;
		}

		internal static ExchangeObjectVersion GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails recipientTypeDetails, bool forReadObject = false)
		{
			if (recipientTypeDetails <= RecipientTypeDetails.DiscoveryMailbox)
			{
				if (recipientTypeDetails <= RecipientTypeDetails.MailUser)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.SharedMailbox)
					{
						if (recipientTypeDetails != RecipientTypeDetails.UserMailbox && recipientTypeDetails != RecipientTypeDetails.SharedMailbox)
						{
							goto IL_175;
						}
					}
					else if (recipientTypeDetails != RecipientTypeDetails.RoomMailbox && recipientTypeDetails != RecipientTypeDetails.EquipmentMailbox)
					{
						if (recipientTypeDetails != RecipientTypeDetails.MailUser)
						{
							goto IL_175;
						}
						goto IL_165;
					}
					return ExchangeObjectVersion.Exchange2012;
				}
				if (recipientTypeDetails <= RecipientTypeDetails.DisabledUser)
				{
					if (recipientTypeDetails != RecipientTypeDetails.User && recipientTypeDetails != RecipientTypeDetails.DisabledUser)
					{
						goto IL_175;
					}
					goto IL_16D;
				}
				else if (recipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox)
				{
					if (recipientTypeDetails == RecipientTypeDetails.LinkedUser)
					{
						goto IL_16D;
					}
					if (recipientTypeDetails != RecipientTypeDetails.DiscoveryMailbox)
					{
						goto IL_175;
					}
				}
			}
			else
			{
				if (recipientTypeDetails > RecipientTypeDetails.PublicFolderMailbox)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.RemoteTeamMailbox)
					{
						if (recipientTypeDetails != RecipientTypeDetails.TeamMailbox)
						{
							if (recipientTypeDetails != RecipientTypeDetails.RemoteTeamMailbox)
							{
								goto IL_175;
							}
							goto IL_165;
						}
					}
					else if (recipientTypeDetails != RecipientTypeDetails.GroupMailbox)
					{
						if (recipientTypeDetails == RecipientTypeDetails.LinkedRoomMailbox)
						{
							goto IL_165;
						}
						if (recipientTypeDetails != RecipientTypeDetails.AuditLogMailbox)
						{
							goto IL_175;
						}
						goto IL_15D;
					}
					return ADRecipient.TeamMailboxObjectVersion;
				}
				if (recipientTypeDetails <= RecipientTypeDetails.RemoteRoomMailbox)
				{
					if (recipientTypeDetails != (RecipientTypeDetails)((ulong)-2147483648) && recipientTypeDetails != RecipientTypeDetails.RemoteRoomMailbox)
					{
						goto IL_175;
					}
					goto IL_165;
				}
				else
				{
					if (recipientTypeDetails == RecipientTypeDetails.RemoteEquipmentMailbox || recipientTypeDetails == RecipientTypeDetails.RemoteSharedMailbox)
					{
						goto IL_165;
					}
					if (recipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
					{
						goto IL_175;
					}
					goto IL_16D;
				}
			}
			IL_15D:
			return ADRecipient.ArbitrationMailboxObjectVersion;
			IL_165:
			return ExchangeObjectVersion.Exchange2012;
			IL_16D:
			return ADRecipient.PublicFolderMailboxObjectVersion;
			IL_175:
			return forReadObject ? ADRecipient.PublicFolderMailboxObjectVersion : ExchangeObjectVersion.Exchange2012;
		}

		public string C
		{
			get
			{
				return (string)this[ADUserSchema.C];
			}
			set
			{
				this[ADUserSchema.C] = value;
			}
		}

		public string City
		{
			get
			{
				return (string)this[ADUserSchema.City];
			}
			set
			{
				this[ADUserSchema.City] = value;
			}
		}

		public string Co
		{
			get
			{
				return (string)this[ADUserSchema.Co];
			}
			set
			{
				this[ADUserSchema.Co] = value;
			}
		}

		public string Company
		{
			get
			{
				return (string)this[ADUserSchema.Company];
			}
			set
			{
				this[ADUserSchema.Company] = value;
			}
		}

		public int CountryCode
		{
			get
			{
				return (int)this[ADUserSchema.CountryCode];
			}
			set
			{
				this[ADUserSchema.CountryCode] = value;
			}
		}

		public string CountryOrRegionDisplayName
		{
			get
			{
				return (string)this[ADUserSchema.Co];
			}
		}

		public CountryInfo CountryOrRegion
		{
			get
			{
				return (CountryInfo)this[ADUserSchema.CountryOrRegion];
			}
			set
			{
				this[ADUserSchema.CountryOrRegion] = value;
			}
		}

		public string Department
		{
			get
			{
				return (string)this[ADUserSchema.Department];
			}
			set
			{
				this[ADUserSchema.Department] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> DirectReports
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADUserSchema.DirectReports];
			}
		}

		public string Fax
		{
			get
			{
				return (string)this[ADUserSchema.Fax];
			}
			set
			{
				this[ADUserSchema.Fax] = value;
			}
		}

		public string FirstName
		{
			get
			{
				return (string)this[ADUserSchema.FirstName];
			}
			set
			{
				this[ADUserSchema.FirstName] = value;
			}
		}

		public string HomePhone
		{
			get
			{
				return (string)this[ADUserSchema.HomePhone];
			}
			set
			{
				this[ADUserSchema.HomePhone] = value;
			}
		}

		public MultiValuedProperty<string> IndexedPhoneNumbers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.IndexedPhoneNumbers];
			}
		}

		public string Initials
		{
			get
			{
				return (string)this[ADUserSchema.Initials];
			}
			set
			{
				this[ADUserSchema.Initials] = value;
			}
		}

		public MultiValuedProperty<CultureInfo> Languages
		{
			get
			{
				return (MultiValuedProperty<CultureInfo>)this[ADUserSchema.Languages];
			}
			set
			{
				this[ADUserSchema.Languages] = value;
			}
		}

		public string LastName
		{
			get
			{
				return (string)this[ADUserSchema.LastName];
			}
			set
			{
				this[ADUserSchema.LastName] = value;
			}
		}

		public ADObjectId Manager
		{
			get
			{
				return (ADObjectId)this[ADUserSchema.Manager];
			}
			set
			{
				this[ADUserSchema.Manager] = value;
			}
		}

		public string MobilePhone
		{
			get
			{
				return (string)this[ADUserSchema.MobilePhone];
			}
			set
			{
				this[ADUserSchema.MobilePhone] = value;
			}
		}

		public string Office
		{
			get
			{
				return (string)this[ADUserSchema.Office];
			}
			set
			{
				this[ADUserSchema.Office] = value;
			}
		}

		public MultiValuedProperty<string> OtherFax
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.OtherFax];
			}
			set
			{
				this[ADUserSchema.OtherFax] = value;
			}
		}

		public MultiValuedProperty<string> OtherHomePhone
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.OtherHomePhone];
			}
			set
			{
				this[ADUserSchema.OtherHomePhone] = value;
			}
		}

		public MultiValuedProperty<string> OtherTelephone
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.OtherTelephone];
			}
			set
			{
				this[ADUserSchema.OtherTelephone] = value;
			}
		}

		public string Pager
		{
			get
			{
				return (string)this[ADUserSchema.Pager];
			}
			set
			{
				this[ADUserSchema.Pager] = value;
			}
		}

		public string Phone
		{
			get
			{
				return (string)this[ADUserSchema.Phone];
			}
			set
			{
				this[ADUserSchema.Phone] = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return (string)this[ADUserSchema.PostalCode];
			}
			set
			{
				this[ADUserSchema.PostalCode] = value;
			}
		}

		public MultiValuedProperty<string> PostOfficeBox
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.PostOfficeBox];
			}
			set
			{
				this[ADUserSchema.PostOfficeBox] = value;
			}
		}

		public string RtcSipLine
		{
			get
			{
				return (string)this[ADUserSchema.RtcSipLine];
			}
		}

		public MultiValuedProperty<string> SanitizedPhoneNumbers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.SanitizedPhoneNumbers];
			}
		}

		public string StateOrProvince
		{
			get
			{
				return (string)this[ADUserSchema.StateOrProvince];
			}
			set
			{
				this[ADUserSchema.StateOrProvince] = value;
			}
		}

		public string StreetAddress
		{
			get
			{
				return (string)this[ADUserSchema.StreetAddress];
			}
			set
			{
				this[ADUserSchema.StreetAddress] = value;
			}
		}

		public string TelephoneAssistant
		{
			get
			{
				return (string)this[ADUserSchema.TelephoneAssistant];
			}
			set
			{
				this[ADUserSchema.TelephoneAssistant] = value;
			}
		}

		public string Title
		{
			get
			{
				return (string)this[ADUserSchema.Title];
			}
			set
			{
				this[ADUserSchema.Title] = value;
			}
		}

		public MultiValuedProperty<string> UMCallingLineIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.UMCallingLineIds];
			}
			set
			{
				this[ADUserSchema.UMCallingLineIds] = value;
			}
		}

		public MultiValuedProperty<string> VoiceMailSettings
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADUserSchema.VoiceMailSettings];
			}
			set
			{
				this[ADUserSchema.VoiceMailSettings] = value;
			}
		}

		public object[][] GetManagementChainView(bool getPeers, params PropertyDefinition[] returnProperties)
		{
			return ADOrgPerson.GetManagementChainView(base.Session, this, getPeers, returnProperties);
		}

		public object[][] GetDirectReportsView(params PropertyDefinition[] returnProperties)
		{
			return ADOrgPerson.GetDirectReportsView(base.Session, this, returnProperties);
		}

		public override void PopulateDtmfMap(bool create)
		{
			string text = (this.FirstName + this.LastName).Trim();
			string lastFirst = (this.LastName + this.FirstName).Trim();
			if (string.IsNullOrEmpty(text))
			{
				text = base.DisplayName;
				lastFirst = base.DisplayName;
			}
			base.PopulateDtmfMap(create, text, lastFirst, base.PrimarySmtpAddress, this.SanitizedPhoneNumbers);
		}

		private const byte DowngradeMailBitMask = 128;

		private const int DowngradeMailBitIndex = 3;

		internal const string UserPrincipalNamePattern = "^.*@[^@]+$";

		private static readonly ADUserSchema schema = ObjectSchema.GetInstance<ADUserSchema>();

		internal static string MostDerivedClass = "user";

		internal static string ObjectCategoryNameInternal = "person";

		private string netIDSuffixCopy = string.Empty;

		internal static QueryFilter ImplicitFilterInternal = new AndFilter(new QueryFilter[]
		{
			ADObject.ObjectClassFilter(ADUser.MostDerivedClass, true),
			ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal)
		});

		public static readonly string SamAccountNameInvalidCharacters = "\"\\/[]:|<>+=;?,*\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f@";
	}
}
