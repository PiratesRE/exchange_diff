using System;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	[Serializable]
	public class TenantRelocationSyncObject : ADRawEntry
	{
		public TenantRelocationSyncObject()
		{
		}

		internal TenantRelocationSyncObject(ADPropertyBag propertyBag, DirectoryAttribute[] directoryAttributes) : base(propertyBag)
		{
			this.RawLdapSearchResult = directoryAttributes;
		}

		internal DirectoryAttribute[] RawLdapSearchResult { get; private set; }

		internal bool IsDeleted
		{
			get
			{
				return (bool)this[SyncObjectSchema.Deleted];
			}
		}

		internal Guid CorrelationId
		{
			get
			{
				return (Guid)this[ADObjectSchema.CorrelationId];
			}
		}

		internal Guid Guid
		{
			get
			{
				return (Guid)this[ADObjectSchema.Guid];
			}
		}

		internal Guid ExchangeObjectId
		{
			get
			{
				return (Guid)this[ADObjectSchema.ExchangeObjectId];
			}
		}

		internal string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[ADRecipientSchema.ExternalDirectoryObjectId];
			}
		}

		internal string ConfigurationXMLRaw
		{
			get
			{
				return (string)this[ADRecipientSchema.ConfigurationXMLRaw];
			}
		}

		internal MultiValuedProperty<LinkMetadata> LinkValueMetadata
		{
			get
			{
				return (MultiValuedProperty<LinkMetadata>)this[ADRecipientSchema.LinkMetadata];
			}
		}
	}
}
