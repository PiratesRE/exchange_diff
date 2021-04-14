using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MSERVEntry : ConfigurableObject
	{
		internal MSERVEntry() : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2012);
		}

		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)this[MSERVEntrySchema.ExternalDirectoryOrganizationId];
			}
			set
			{
				this[MSERVEntrySchema.ExternalDirectoryOrganizationId] = value;
			}
		}

		public string DomainName
		{
			get
			{
				return (string)this[MSERVEntrySchema.DomainName];
			}
			set
			{
				this[MSERVEntrySchema.DomainName] = value;
			}
		}

		public string AddressForPartnerId
		{
			get
			{
				return (string)this[MSERVEntrySchema.AddressForPartnerId];
			}
			set
			{
				this[MSERVEntrySchema.AddressForPartnerId] = value;
			}
		}

		public int PartnerId
		{
			get
			{
				return (int)this[MSERVEntrySchema.PartnerId];
			}
			set
			{
				this[MSERVEntrySchema.PartnerId] = value;
			}
		}

		public string AddressForMinorPartnerId
		{
			get
			{
				return (string)this[MSERVEntrySchema.AddressForMinorPartnerId];
			}
			set
			{
				this[MSERVEntrySchema.AddressForMinorPartnerId] = value;
			}
		}

		public int MinorPartnerId
		{
			get
			{
				return (int)this[MSERVEntrySchema.MinorPartnerId];
			}
			set
			{
				this[MSERVEntrySchema.MinorPartnerId] = value;
			}
		}

		public string Forest
		{
			get
			{
				return (string)this[MSERVEntrySchema.Forest];
			}
			set
			{
				this[MSERVEntrySchema.Forest] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MSERVEntrySchema>();
			}
		}
	}
}
