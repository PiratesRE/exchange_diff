using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class RecipientData
	{
		public long ExchangePrincipalLatency { get; private set; }

		public override string ToString()
		{
			if (this.initialEmailAddress == null)
			{
				if (this.emailAddress == null)
				{
					return "null";
				}
				return this.emailAddress.Address;
			}
			else
			{
				if (this.emailAddress == null)
				{
					return this.initialEmailAddress.Address;
				}
				if (StringComparer.OrdinalIgnoreCase.Equals(this.initialEmailAddress.Address, this.emailAddress.Address))
				{
					return this.emailAddress.Address;
				}
				return this.emailAddress.Address + " (" + this.initialEmailAddress.Address + ")";
			}
		}

		private RecipientData(EmailAddress emailAddress)
		{
			this.initialEmailAddress = emailAddress;
			this.emailAddress = emailAddress;
		}

		internal static RecipientData Create(EmailAddress emailAddress)
		{
			return new RecipientData(emailAddress);
		}

		internal static RecipientData Create(EmailAddress emailAddress, ConfigurableObject configurableObject, ICollection<PropertyDefinition> propertyDefinitionCollection)
		{
			if (Testability.HandleSmtpAddressAsContact(emailAddress.Address))
			{
				return RecipientData.CreateAsContact(emailAddress);
			}
			RecipientData recipientData = new RecipientData(emailAddress);
			recipientData.ParseConfigurableObject(configurableObject, propertyDefinitionCollection);
			return recipientData;
		}

		internal static RecipientData Create(EmailAddress emailAddress, Dictionary<PropertyDefinition, object> propertyMap)
		{
			if (Testability.HandleSmtpAddressAsContact(emailAddress.Address))
			{
				return RecipientData.CreateAsContact(emailAddress);
			}
			return new RecipientData(emailAddress)
			{
				propertyMap = propertyMap
			};
		}

		internal static RecipientData Create(EmailAddress emailAddress, Exception exception)
		{
			return new RecipientData(emailAddress)
			{
				exception = exception
			};
		}

		internal static RecipientData Create(EmailAddress emailAddress, ProviderError providerError)
		{
			return new RecipientData(emailAddress)
			{
				providerError = providerError
			};
		}

		internal EmailAddress InitialEmailAddress
		{
			get
			{
				return this.initialEmailAddress;
			}
		}

		internal EmailAddress EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
			set
			{
				this.emailAddress = value;
			}
		}

		internal StoreObjectId AssociatedFolderId
		{
			get
			{
				return this.associatedFolderId;
			}
			set
			{
				this.associatedFolderId = value;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.exception = value;
			}
		}

		internal ProviderError ProviderError
		{
			get
			{
				return this.providerError;
			}
		}

		internal bool IsEmpty
		{
			get
			{
				return null == this.propertyMap;
			}
		}

		internal RecipientType RecipientType
		{
			get
			{
				return (RecipientType)this[ADRecipientSchema.RecipientType];
			}
		}

		internal RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return (RecipientDisplayType?)this[ADRecipientSchema.RecipientDisplayType];
			}
		}

		internal SmtpAddress PrimarySmtpAddress
		{
			get
			{
				object obj = this[ADRecipientSchema.PrimarySmtpAddress];
				if (obj == null)
				{
					return SmtpAddress.Empty;
				}
				return (SmtpAddress)obj;
			}
		}

		internal string LegacyExchangeDN
		{
			get
			{
				return (string)this[ADRecipientSchema.LegacyExchangeDN];
			}
		}

		internal string DisplayName
		{
			get
			{
				return (string)this[ADRecipientSchema.DisplayName];
			}
		}

		internal ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[ADRecipientSchema.ExternalEmailAddress];
			}
		}

		internal string ServerLegacyDN
		{
			get
			{
				return (string)this[ADMailboxRecipientSchema.ServerLegacyDN];
			}
		}

		internal Guid MailboxGuid
		{
			get
			{
				return (Guid)this[ADMailboxRecipientSchema.ExchangeGuid];
			}
		}

		internal Guid MdbGuid
		{
			get
			{
				ADObjectId adobjectId = this[ADMailboxRecipientSchema.Database] as ADObjectId;
				if (adobjectId != null)
				{
					return adobjectId.ObjectGuid;
				}
				return Guid.Empty;
			}
		}

		internal ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[ADObjectSchema.ExchangeVersion];
			}
		}

		internal Guid Guid
		{
			get
			{
				return (Guid)this[ADObjectSchema.Guid];
			}
		}

		internal ADObjectId Id
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.Id];
			}
		}

		internal int DistributionGroupMembersCount
		{
			get
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty = this[ADGroupSchema.Members] as MultiValuedProperty<ADObjectId>;
				if (multiValuedProperty != null)
				{
					return multiValuedProperty.Count;
				}
				return 0;
			}
		}

		internal int GroupMemberCount
		{
			get
			{
				return (int)this[ADGroupSchema.GroupMemberCount];
			}
		}

		internal int GroupExternalMemberCount
		{
			get
			{
				return (int)this[ADGroupSchema.GroupExternalMemberCount];
			}
		}

		internal MultiValuedProperty<ADObjectId> RejectMessagesFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.RejectMessagesFrom];
			}
		}

		internal MultiValuedProperty<ADObjectId> RejectMessagesFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.RejectMessagesFromDLMembers];
			}
		}

		internal MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.AcceptMessagesOnlyFrom];
			}
		}

		internal MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers];
			}
		}

		internal MultiValuedProperty<ADObjectId> BypassModerationFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.BypassModerationFrom];
			}
		}

		internal MultiValuedProperty<ADObjectId> BypassModerationFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.BypassModerationFromDLMembers];
			}
		}

		internal MultiValuedProperty<ADObjectId> ModeratedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.ModeratedBy];
			}
		}

		internal MultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADGroupSchema.ManagedBy];
			}
		}

		internal Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADRecipientSchema.MaxReceiveSize];
			}
		}

		internal OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[ADObjectSchema.OrganizationId];
			}
		}

		internal SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[ADMailboxRecipientSchema.Sid];
			}
		}

		internal SecurityIdentifier MasterAccountSid
		{
			get
			{
				return (SecurityIdentifier)this[ADRecipientSchema.MasterAccountSid];
			}
		}

		internal RawSecurityDescriptor ExchangeSecurityDescriptor
		{
			get
			{
				return (RawSecurityDescriptor)this[ADMailboxRecipientSchema.ExchangeSecurityDescriptor];
			}
		}

		internal MultiValuedProperty<string> MailTipTranslations
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.MailTipTranslations];
			}
		}

		internal bool ModerationEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.ModerationEnabled];
			}
		}

		internal bool IsDistributionGroup
		{
			get
			{
				switch (this.RecipientType)
				{
				case RecipientType.Group:
				case RecipientType.MailUniversalDistributionGroup:
				case RecipientType.MailUniversalSecurityGroup:
				case RecipientType.MailNonUniversalGroup:
				case RecipientType.DynamicDistributionGroup:
					return true;
				default:
					return false;
				}
			}
		}

		internal bool IsIndividual
		{
			get
			{
				switch (this.RecipientType)
				{
				case RecipientType.User:
				case RecipientType.UserMailbox:
				case RecipientType.MailUser:
				case RecipientType.Contact:
				case RecipientType.MailContact:
					return true;
				default:
					return false;
				}
			}
		}

		internal bool IsRemoteMailbox
		{
			get
			{
				if (this.RecipientDisplayType == null)
				{
					return false;
				}
				RecipientDisplayType value = this.RecipientDisplayType.Value;
				RecipientDisplayType recipientDisplayType = value;
				if (recipientDisplayType <= Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.SyncedPublicFolder)
				{
					if (recipientDisplayType != Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.SyncedMailboxUser && recipientDisplayType != Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.SyncedPublicFolder)
					{
						return false;
					}
				}
				else if (recipientDisplayType != Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.SyncedConferenceRoomMailbox && recipientDisplayType != Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.SyncedEquipmentMailbox && recipientDisplayType != Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.ACLableSyncedMailboxUser)
				{
					return false;
				}
				return true;
			}
		}

		internal ExchangePrincipal ExchangePrincipal
		{
			get
			{
				if (this.exchangePrincipal == null)
				{
					Stopwatch stopwatch = Stopwatch.StartNew();
					this.exchangePrincipal = ExchangePrincipal.FromAnyVersionMailboxData(this.DisplayName, this.MailboxGuid, this.MdbGuid, this.PrimarySmtpAddress.ToString(), this.LegacyExchangeDN, this.Id, this.RecipientType, this.MasterAccountSid, this.OrganizationId, RemotingOptions.AllowCrossSite, false);
					stopwatch.Stop();
					this.ExchangePrincipalLatency = stopwatch.ElapsedMilliseconds;
				}
				return this.exchangePrincipal;
			}
		}

		internal byte[] ThumbnailPhoto
		{
			get
			{
				return (byte[])this[ADRecipientSchema.ThumbnailPhoto];
			}
		}

		private object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				if (this.HasPropertyDefinition(propertyDefinition))
				{
					return this.propertyMap[propertyDefinition];
				}
				return null;
			}
		}

		private void ParseConfigurableObject(ConfigurableObject configurableObject, ICollection<PropertyDefinition> propertyDefinitionCollection)
		{
			if (configurableObject == null)
			{
				ExTraceGlobals.RequestRoutingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Trying to parse null configurable object.", new object[]
				{
					TraceContext.Get()
				});
				return;
			}
			if (0 < propertyDefinitionCollection.Count)
			{
				this.propertyMap = new Dictionary<PropertyDefinition, object>(propertyDefinitionCollection.Count);
				using (IEnumerator<PropertyDefinition> enumerator = propertyDefinitionCollection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PropertyDefinition propertyDefinition = enumerator.Current;
						if (configurableObject[propertyDefinition] == null)
						{
							ExTraceGlobals.RequestRoutingTracer.TraceDebug<object, PropertyDefinition>((long)this.GetHashCode(), "{0}: {1} property was null.", TraceContext.Get(), propertyDefinition);
						}
						this.propertyMap[propertyDefinition] = configurableObject[propertyDefinition];
					}
					return;
				}
			}
			ExTraceGlobals.RequestRoutingTracer.TraceError<object, int>((long)this.GetHashCode(), "{0}: Property definition collection contains {1} property definitions, nothing is parsed in RecipientData.", TraceContext.Get(), propertyDefinitionCollection.Count);
		}

		private bool HasPropertyDefinition(PropertyDefinition propertyDefinition)
		{
			return this.propertyMap != null && 0 < this.propertyMap.Count && this.propertyMap.ContainsKey(propertyDefinition);
		}

		internal static RecipientData CreateAsContact(EmailAddress emailAddress)
		{
			RecipientData recipientData = new RecipientData(emailAddress);
			SmtpProxyAddress value = new SmtpProxyAddress(emailAddress.Address, true);
			recipientData.propertyMap = new Dictionary<PropertyDefinition, object>();
			recipientData.propertyMap[ADRecipientSchema.RecipientType] = RecipientType.MailUser;
			recipientData.propertyMap[ADRecipientSchema.ExternalEmailAddress] = value;
			SmtpAddress smtpAddress = new SmtpAddress(emailAddress.Address);
			recipientData.propertyMap[ADRecipientSchema.PrimarySmtpAddress] = smtpAddress;
			return recipientData;
		}

		private Dictionary<PropertyDefinition, object> propertyMap;

		private EmailAddress emailAddress;

		private EmailAddress initialEmailAddress;

		private StoreObjectId associatedFolderId;

		private Exception exception;

		private ExchangePrincipal exchangePrincipal;

		private ProviderError providerError;
	}
}
