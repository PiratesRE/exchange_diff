using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class MailboxInfo
	{
		public MailboxInfo(Dictionary<PropertyDefinition, object> propertyMap, MailboxType type)
		{
			Util.ThrowOnNull(propertyMap, "propertyMap");
			this.propertyMap = propertyMap;
			this.type = type;
		}

		public MailboxInfo(ConfigurableObject configurableObject, MailboxType type)
		{
			Util.ThrowOnNull(configurableObject, "configurableObject");
			this.type = type;
			this.ParseConfigurableObject(configurableObject);
		}

		public ADObjectId OwnerId
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.Id];
			}
		}

		public MailboxType Type
		{
			get
			{
				return this.type;
			}
		}

		public bool IsPrimary
		{
			get
			{
				return this.type == MailboxType.Primary;
			}
		}

		public bool IsArchive
		{
			get
			{
				return this.type == MailboxType.Archive;
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

		internal RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[ADRecipientSchema.RecipientTypeDetails];
			}
		}

		internal SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[ADRecipientSchema.PrimarySmtpAddress];
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

		internal string DistinguishedName
		{
			get
			{
				return this.OwnerId.DistinguishedName;
			}
		}

		internal ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[ADRecipientSchema.ExternalEmailAddress];
			}
		}

		internal ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[ADRecipientSchema.EmailAddresses];
			}
		}

		internal Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[ADMailboxRecipientSchema.ExchangeGuid];
			}
		}

		internal ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[ADObjectSchema.ExchangeVersion];
			}
		}

		internal SecurityIdentifier MasterAccountSid
		{
			get
			{
				return (SecurityIdentifier)this[ADRecipientSchema.MasterAccountSid];
			}
		}

		internal OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[ADObjectSchema.OrganizationId];
			}
		}

		internal Guid ArchiveGuid
		{
			get
			{
				return (Guid)this[ADUserSchema.ArchiveGuid];
			}
		}

		internal ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return (ArchiveStatusFlags)this[ADUserSchema.ArchiveStatus];
			}
		}

		internal Guid MailboxGuid
		{
			get
			{
				if (this.Type != MailboxType.Primary)
				{
					return this.ArchiveGuid;
				}
				return this.ExchangeGuid;
			}
		}

		internal SmtpDomain ArchiveDomain
		{
			get
			{
				return (SmtpDomain)this[ADUserSchema.ArchiveDomain];
			}
		}

		internal Guid ArchiveDatabase
		{
			get
			{
				ADObjectId adobjectId = this[ADUserSchema.ArchiveDatabaseRaw] as ADObjectId;
				if (adobjectId != null)
				{
					return adobjectId.ObjectGuid;
				}
				return Guid.Empty;
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

		internal bool IsRemoteMailbox
		{
			get
			{
				return this.IsCrossPremiseMailbox || this.IsCrossForestMailbox;
			}
		}

		internal virtual bool IsCrossPremiseMailbox
		{
			get
			{
				switch (this.RecipientType)
				{
				case RecipientType.UserMailbox:
					return !this.IsPrimary && this.ArchiveDomain != null;
				case RecipientType.MailUser:
					return this.MdbGuid == Guid.Empty && ((this.RecipientTypeDetails & (RecipientTypeDetails)((ulong)int.MinValue)) == (RecipientTypeDetails)((ulong)int.MinValue) || (this.RecipientTypeDetails & RecipientTypeDetails.RemoteRoomMailbox) == RecipientTypeDetails.RemoteRoomMailbox || (this.RecipientTypeDetails & RecipientTypeDetails.RemoteEquipmentMailbox) == RecipientTypeDetails.RemoteEquipmentMailbox || (this.RecipientTypeDetails & RecipientTypeDetails.RemoteTeamMailbox) == RecipientTypeDetails.RemoteTeamMailbox || (this.RecipientTypeDetails & RecipientTypeDetails.RemoteSharedMailbox) == RecipientTypeDetails.RemoteSharedMailbox);
				default:
					return false;
				}
			}
		}

		internal virtual bool IsCrossForestMailbox
		{
			get
			{
				switch (this.RecipientType)
				{
				case RecipientType.MailUser:
				case RecipientType.MailContact:
					return !this.IsCrossPremiseMailbox && !this.IsCloudArchive;
				}
				return false;
			}
		}

		internal virtual bool IsCloudArchive
		{
			get
			{
				return this.RecipientType == RecipientType.MailUser && this.ArchiveGuid != Guid.Empty && this.ArchiveStatus == ArchiveStatusFlags.Active;
			}
		}

		internal ExchangePrincipal ExchangePrincipal
		{
			get
			{
				if (this.exchangePrincipal == null)
				{
					this.exchangePrincipal = ExchangePrincipal.FromAnyVersionMailboxData(this.DisplayName, (this.Type == MailboxType.Primary) ? this.ExchangeGuid : this.ArchiveGuid, (this.Type == MailboxType.Primary) ? this.MdbGuid : this.ArchiveDatabase, this.PrimarySmtpAddress.ToString(), this.LegacyExchangeDN, this.OwnerId, this.RecipientType, this.MasterAccountSid, this.OrganizationId, RemotingOptions.AllowCrossSite, this.Type == MailboxType.Archive);
				}
				return this.exchangePrincipal;
			}
		}

		internal string Folder { get; set; }

		internal object SourceMailbox { get; set; }

		internal Dictionary<PropertyDefinition, object> PropertyMap
		{
			get
			{
				return this.propertyMap;
			}
		}

		internal object this[PropertyDefinition propertyDefinition]
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

		public string GetDomain()
		{
			if (!this.IsRemoteMailbox)
			{
				Factory.Current.GeneralTracer.TraceDebug((long)this.GetHashCode(), "MailboxInfo.GetDomain: Not a remote mailbox, returning primary smtp domain.");
				return this.PrimarySmtpAddress.Domain;
			}
			if (this.IsCrossForestMailbox || (this.IsCrossPremiseMailbox && this.IsPrimary))
			{
				Factory.Current.GeneralTracer.TraceDebug((long)this.GetHashCode(), "MailboxInfo.GetDomain: Remote primary mailbox, returning external email address domain.");
				return new SmtpAddress(this.ExternalEmailAddress.AddressString).Domain;
			}
			if (this.RecipientType == RecipientType.MailUser)
			{
				return new SmtpAddress(this.ExternalEmailAddress.AddressString).Domain;
			}
			if (this.RecipientType != RecipientType.UserMailbox)
			{
				return null;
			}
			if (this.ArchiveDomain == null)
			{
				return null;
			}
			return this.ArchiveDomain.ToString();
		}

		public string GetUniqueKey()
		{
			return string.Format("{0}{1}", this.LegacyExchangeDN, this.Folder);
		}

		private void ParseConfigurableObject(ConfigurableObject configurableObject)
		{
			Util.ThrowOnNull(configurableObject, "configurableObject");
			this.propertyMap = new Dictionary<PropertyDefinition, object>(MailboxInfo.PropertyDefinitionCollection.Length);
			foreach (PropertyDefinition propertyDefinition in MailboxInfo.PropertyDefinitionCollection)
			{
				object obj = null;
				if (!configurableObject.TryGetValueWithoutDefault(propertyDefinition, out obj))
				{
					Factory.Current.GeneralTracer.TraceDebug<string>((long)this.GetHashCode(), "PropertyDefinition {0} not found while creating MailboxInfo", propertyDefinition.ToString());
				}
				this.propertyMap[propertyDefinition] = configurableObject[propertyDefinition];
			}
		}

		protected bool HasPropertyDefinition(PropertyDefinition propertyDefinition)
		{
			return this.propertyMap != null && 0 < this.propertyMap.Count && this.propertyMap.ContainsKey(propertyDefinition);
		}

		internal static PropertyDefinition[] PropertyDefinitionCollection = new PropertyDefinition[]
		{
			ADObjectSchema.ExchangeVersion,
			ADObjectSchema.Id,
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.ExternalEmailAddress,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.RecipientTypeDetails,
			ADUserSchema.ArchiveGuid,
			ADUserSchema.ArchiveDomain,
			ADUserSchema.ArchiveDatabaseRaw,
			ADUserSchema.ArchiveStatus,
			ADMailboxRecipientSchema.Database,
			ADMailboxRecipientSchema.ExchangeGuid,
			ADRecipientSchema.MasterAccountSid
		};

		protected Dictionary<PropertyDefinition, object> propertyMap;

		private MailboxType type;

		private ExchangePrincipal exchangePrincipal;
	}
}
