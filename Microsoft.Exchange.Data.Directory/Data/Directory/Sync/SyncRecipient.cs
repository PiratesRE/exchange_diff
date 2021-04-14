using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal abstract class SyncRecipient : SyncObject
	{
		public SyncRecipient(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> AcceptMessagesOnlyFrom
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncRecipientSchema.AcceptMessagesOnlyFrom];
			}
			set
			{
				base[SyncRecipientSchema.AcceptMessagesOnlyFrom] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncRecipientSchema.AcceptMessagesOnlyFromDLMembers];
			}
			set
			{
				base[SyncRecipientSchema.AcceptMessagesOnlyFromDLMembers] = value;
			}
		}

		public SyncProperty<string> Alias
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.Alias];
			}
			set
			{
				base[SyncRecipientSchema.Alias] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> BypassModerationFrom
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncRecipientSchema.BypassModerationFrom];
			}
			set
			{
				base[SyncRecipientSchema.BypassModerationFrom] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> BypassModerationFromDLMembers
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncRecipientSchema.BypassModerationFromDLMembers];
			}
			set
			{
				base[SyncRecipientSchema.BypassModerationFromDLMembers] = value;
			}
		}

		public SyncProperty<string> Cn
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.Cn];
			}
			set
			{
				base[SyncRecipientSchema.Cn] = value;
			}
		}

		public string UpdatedCnFromMSO { get; set; }

		public string CloudLegacyExchangeDN
		{
			get
			{
				return (string)base[SyncRecipientSchema.CloudLegacyExchangeDN];
			}
			set
			{
				base[SyncRecipientSchema.CloudLegacyExchangeDN] = value;
			}
		}

		public SyncProperty<int?> RecipientDisplayType
		{
			get
			{
				return (SyncProperty<int?>)base[SyncRecipientSchema.RecipientDisplayType];
			}
			set
			{
				base[SyncRecipientSchema.RecipientDisplayType] = value;
			}
		}

		public SyncProperty<string> CustomAttribute1
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute1];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute1] = value;
			}
		}

		public SyncProperty<string> CustomAttribute10
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute10];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute10] = value;
			}
		}

		public SyncProperty<string> CustomAttribute11
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute11];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute11] = value;
			}
		}

		public SyncProperty<string> CustomAttribute12
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute12];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute12] = value;
			}
		}

		public SyncProperty<string> CustomAttribute13
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute13];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute13] = value;
			}
		}

		public SyncProperty<string> CustomAttribute14
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute14];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute14] = value;
			}
		}

		public SyncProperty<string> CustomAttribute15
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute15];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute15] = value;
			}
		}

		public SyncProperty<string> CustomAttribute2
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute2];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute2] = value;
			}
		}

		public SyncProperty<string> CustomAttribute3
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute3];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute3] = value;
			}
		}

		public SyncProperty<string> CustomAttribute4
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute4];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute4] = value;
			}
		}

		public SyncProperty<string> CustomAttribute5
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute5];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute5] = value;
			}
		}

		public SyncProperty<string> CustomAttribute6
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute6];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute6] = value;
			}
		}

		public SyncProperty<string> CustomAttribute7
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute7];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute7] = value;
			}
		}

		public SyncProperty<string> CustomAttribute8
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute8];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute8] = value;
			}
		}

		public SyncProperty<string> CustomAttribute9
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.CustomAttribute9];
			}
			set
			{
				base[SyncRecipientSchema.CustomAttribute9] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> ExtensionCustomAttribute1
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncRecipientSchema.ExtensionCustomAttribute1];
			}
			set
			{
				base[SyncRecipientSchema.ExtensionCustomAttribute1] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> ExtensionCustomAttribute2
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncRecipientSchema.ExtensionCustomAttribute2];
			}
			set
			{
				base[SyncRecipientSchema.ExtensionCustomAttribute2] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> ExtensionCustomAttribute3
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncRecipientSchema.ExtensionCustomAttribute3];
			}
			set
			{
				base[SyncRecipientSchema.ExtensionCustomAttribute3] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> ExtensionCustomAttribute4
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncRecipientSchema.ExtensionCustomAttribute4];
			}
			set
			{
				base[SyncRecipientSchema.ExtensionCustomAttribute4] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> ExtensionCustomAttribute5
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncRecipientSchema.ExtensionCustomAttribute5];
			}
			set
			{
				base[SyncRecipientSchema.ExtensionCustomAttribute5] = value;
			}
		}

		public SyncProperty<string> DisplayName
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.DisplayName];
			}
			set
			{
				base[SyncRecipientSchema.DisplayName] = value;
			}
		}

		public SyncProperty<ProxyAddressCollection> EmailAddresses
		{
			get
			{
				return (SyncProperty<ProxyAddressCollection>)base[SyncRecipientSchema.EmailAddresses];
			}
			set
			{
				base[SyncRecipientSchema.EmailAddresses] = value;
			}
		}

		public SyncProperty<ProxyAddress> ExternalEmailAddress
		{
			get
			{
				return (SyncProperty<ProxyAddress>)base[SyncRecipientSchema.ExternalEmailAddress];
			}
			set
			{
				base[SyncRecipientSchema.ExternalEmailAddress] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> GrantSendOnBehalfTo
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncRecipientSchema.GrantSendOnBehalfTo];
			}
			set
			{
				base[SyncRecipientSchema.GrantSendOnBehalfTo] = value;
			}
		}

		public SyncProperty<bool> HiddenFromAddressListsEnabled
		{
			get
			{
				return (SyncProperty<bool>)base[SyncRecipientSchema.HiddenFromAddressListsEnabled];
			}
			set
			{
				base[SyncRecipientSchema.HiddenFromAddressListsEnabled] = value;
			}
		}

		public SyncProperty<bool> IsDirSynced
		{
			get
			{
				return (SyncProperty<bool>)base[SyncRecipientSchema.IsDirSynced];
			}
			set
			{
				base[SyncRecipientSchema.IsDirSynced] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> DirSyncAuthorityMetadata
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncRecipientSchema.DirSyncAuthorityMetadata];
			}
			set
			{
				base[SyncRecipientSchema.DirSyncAuthorityMetadata] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> MailTipTranslations
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncRecipientSchema.MailTipTranslations];
			}
			set
			{
				base[SyncRecipientSchema.MailTipTranslations] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> ModeratedBy
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncRecipientSchema.ModeratedBy];
			}
			set
			{
				base[SyncRecipientSchema.ModeratedBy] = value;
			}
		}

		public SyncProperty<bool> ModerationEnabled
		{
			get
			{
				return (SyncProperty<bool>)base[SyncRecipientSchema.ModerationEnabled];
			}
			set
			{
				base[SyncRecipientSchema.ModerationEnabled] = value;
			}
		}

		public SyncProperty<int> ModerationFlags
		{
			get
			{
				return (SyncProperty<int>)base[SyncRecipientSchema.ModerationFlags];
			}
			set
			{
				base[SyncRecipientSchema.ModerationFlags] = value;
			}
		}

		public SyncProperty<string> OnPremisesObjectId
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.OnPremisesObjectId];
			}
			set
			{
				base[SyncRecipientSchema.OnPremisesObjectId] = value;
			}
		}

		public SyncProperty<string> PhoneticDisplayName
		{
			get
			{
				return (SyncProperty<string>)base[SyncRecipientSchema.PhoneticDisplayName];
			}
			set
			{
				base[SyncRecipientSchema.PhoneticDisplayName] = value;
			}
		}

		public virtual SyncProperty<RecipientTypeDetails> RecipientTypeDetailsValue
		{
			get
			{
				return (SyncProperty<RecipientTypeDetails>)base[SyncRecipientSchema.RecipientTypeDetailsValue];
			}
			set
			{
				base[SyncRecipientSchema.RecipientTypeDetailsValue] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> RejectMessagesFrom
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncRecipientSchema.RejectMessagesFrom];
			}
			set
			{
				base[SyncRecipientSchema.RejectMessagesFrom] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> RejectMessagesFromDLMembers
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncRecipientSchema.RejectMessagesFromDLMembers];
			}
			set
			{
				base[SyncRecipientSchema.RejectMessagesFromDLMembers] = value;
			}
		}

		public SyncProperty<bool> BypassNestedModerationEnabled
		{
			get
			{
				return (SyncProperty<bool>)base[SyncRecipientSchema.BypassNestedModerationEnabled];
			}
		}

		public SyncProperty<TransportModerationNotificationFlags> SendModerationNotifications
		{
			get
			{
				return (SyncProperty<TransportModerationNotificationFlags>)base[SyncRecipientSchema.SendModerationNotifications];
			}
		}

		public SyncProperty<bool> RequireAllSendersAreAuthenticated
		{
			get
			{
				return (SyncProperty<bool>)base[SyncRecipientSchema.RequireAllSendersAreAuthenticated];
			}
			set
			{
				base[SyncRecipientSchema.RequireAllSendersAreAuthenticated] = value;
			}
		}

		public SyncProperty<int?> SeniorityIndex
		{
			get
			{
				return (SyncProperty<int?>)base[SyncRecipientSchema.SeniorityIndex];
			}
			set
			{
				base[SyncRecipientSchema.SeniorityIndex] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<byte[]>> UserCertificate
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<byte[]>>)base[SyncRecipientSchema.UserCertificate];
			}
			set
			{
				base[SyncRecipientSchema.UserCertificate] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<byte[]>> UserSMimeCertificate
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<byte[]>>)base[SyncRecipientSchema.UserSMimeCertificate];
			}
			set
			{
				base[SyncRecipientSchema.UserSMimeCertificate] = value;
			}
		}

		public SyncProperty<ProxyAddressCollection> SipAddresses
		{
			get
			{
				return (SyncProperty<ProxyAddressCollection>)base[SyncRecipientSchema.SipAddresses];
			}
			set
			{
				base[SyncRecipientSchema.SipAddresses] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<ValidationErrorValue>> ValidationError
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<ValidationErrorValue>>)base[SyncRecipientSchema.ValidationError];
			}
			set
			{
				base[SyncRecipientSchema.ValidationError] = value;
			}
		}

		public SyncRecipient CreateObjectWithMinimumForwardSyncPropertySet()
		{
			SyncRecipient syncRecipient = (SyncRecipient)SyncObject.CreateBlankObjectByClass(this.ObjectClass, SyncDirection.Forward);
			syncRecipient[SyncObjectSchema.ObjectId] = base.ObjectId;
			syncRecipient[SyncObjectSchema.ContextId] = base.ContextId;
			IDictionary<SyncPropertyDefinition, object> changedProperties = base.GetChangedProperties();
			foreach (SyncPropertyDefinition syncPropertyDefinition in this.MinimumForwardSyncProperties)
			{
				object value = null;
				if (changedProperties.TryGetValue(syncPropertyDefinition, out value))
				{
					syncRecipient[syncPropertyDefinition] = value;
				}
			}
			return syncRecipient;
		}

		public virtual SyncRecipient CreatePlaceHolder()
		{
			SyncRecipient syncRecipient = (SyncRecipient)SyncObject.CreateBlankObjectByClass(this.ObjectClass, SyncDirection.Forward);
			syncRecipient[SyncObjectSchema.ObjectId] = base.ObjectId;
			syncRecipient[SyncObjectSchema.ContextId] = base.ContextId;
			if (this.IsDirSynced.HasValue)
			{
				syncRecipient[SyncRecipientSchema.IsDirSynced] = this.IsDirSynced;
			}
			if (this.OnPremisesObjectId.HasValue)
			{
				syncRecipient[SyncRecipientSchema.OnPremisesObjectId] = this.OnPremisesObjectId;
			}
			return syncRecipient;
		}

		protected virtual SyncPropertyDefinition[] MinimumForwardSyncProperties
		{
			get
			{
				return new SyncPropertyDefinition[]
				{
					SyncRecipientSchema.IsDirSynced,
					SyncRecipientSchema.OnPremisesObjectId,
					SyncRecipientSchema.Alias,
					SyncRecipientSchema.EmailAddresses
				};
			}
		}

		internal static void ProxyAddressesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[SyncRecipientSchema.EmailAddresses] = value;
		}

		internal static ProxyAddressCollection GetEmailAddressesByPrefix(IPropertyBag propertyBag, ProxyAddressPrefix proxyAddressPrefix, SyncPropertyDefinition propertyDefinition)
		{
			List<ProxyAddress> list = new List<ProxyAddress>();
			ADPropertyDefinition propertyDefinition2 = SyncRecipientSchema.EmailAddresses;
			if ((bool)propertyBag[SyncRecipientSchema.UseShadow])
			{
				propertyDefinition2 = SyncRecipientSchema.EmailAddresses.ShadowProperty;
			}
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)propertyBag[propertyDefinition2];
			foreach (ProxyAddress proxyAddress in proxyAddressCollection)
			{
				if (proxyAddress.Prefix == proxyAddressPrefix)
				{
					list.Add(proxyAddress);
				}
			}
			return new ProxyAddressCollection(false, propertyDefinition, list);
		}

		internal static object SendModerationNotificationsGetter(IPropertyBag propertyBag)
		{
			int moderationFlags = (int)propertyBag[SyncRecipientSchema.ModerationFlags];
			return ADRecipient.GetSendModerationNotificationsFromModerationFlags(moderationFlags);
		}

		internal static DirectoryObjectClass GetRecipientType(PropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
			if (multiValuedProperty.Contains(ADUser.MostDerivedClass))
			{
				return DirectoryObjectClass.User;
			}
			if (multiValuedProperty.Contains(ADGroup.MostDerivedClass))
			{
				return DirectoryObjectClass.Group;
			}
			if (multiValuedProperty.Contains(ADContact.MostDerivedClass))
			{
				return DirectoryObjectClass.Contact;
			}
			throw new InvalidOperationException("Unexpected recipient type");
		}

		internal static readonly QueryFilter SyncRecipientObjectTypeFilter = new OrFilter(new QueryFilter[]
		{
			ADObject.ObjectClassFilter(ADUser.MostDerivedClass),
			ADObject.ObjectClassFilter(ADGroup.MostDerivedClass),
			ADObject.ObjectClassFilter(ADContact.MostDerivedClass)
		});

		internal static readonly QueryFilter SyncRecipientObjectTypeFilterOptDisabled = new OrFilter(false, true, new QueryFilter[]
		{
			ADObject.ObjectClassFilter(ADUser.MostDerivedClass),
			ADObject.ObjectClassFilter(ADGroup.MostDerivedClass),
			ADObject.ObjectClassFilter(ADContact.MostDerivedClass)
		});

		internal static ProxyAddressPrefix SipPrefix = ProxyAddressPrefix.GetPrefix("sip");
	}
}
