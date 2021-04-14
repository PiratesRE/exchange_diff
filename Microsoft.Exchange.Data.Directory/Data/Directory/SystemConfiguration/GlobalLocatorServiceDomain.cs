using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class GlobalLocatorServiceDomain : ConfigurableObject
	{
		internal GlobalLocatorServiceDomain() : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2012);
		}

		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)this[GlobalLocatorServiceDomainSchema.ExternalDirectoryOrganizationId];
			}
			set
			{
				this[GlobalLocatorServiceDomainSchema.ExternalDirectoryOrganizationId] = value;
			}
		}

		public SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)this[GlobalLocatorServiceDomainSchema.DomainName];
			}
			set
			{
				this[GlobalLocatorServiceDomainSchema.DomainName] = value;
			}
		}

		public GlsDomainFlags? DomainFlags
		{
			get
			{
				return (GlsDomainFlags?)this[GlobalLocatorServiceDomainSchema.DomainFlags];
			}
			set
			{
				this[GlobalLocatorServiceDomainSchema.DomainFlags] = value;
			}
		}

		public bool DomainInUse
		{
			get
			{
				return (bool)this[GlobalLocatorServiceDomainSchema.DomainInUse];
			}
			set
			{
				this[GlobalLocatorServiceDomainSchema.DomainInUse] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<GlobalLocatorServiceDomainSchema>();
			}
		}
	}
}
