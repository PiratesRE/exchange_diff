using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class GlobalLocatorServiceTenant : ConfigurableObject
	{
		internal GlobalLocatorServiceTenant() : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2012);
		}

		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)this[GlobalLocatorServiceTenantSchema.ExternalDirectoryOrganizationId];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.ExternalDirectoryOrganizationId] = value;
			}
		}

		public MultiValuedProperty<string> DomainNames
		{
			get
			{
				return (MultiValuedProperty<string>)this[GlobalLocatorServiceTenantSchema.DomainNames];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.DomainNames] = value;
			}
		}

		public string ResourceForest
		{
			get
			{
				return (string)this[GlobalLocatorServiceTenantSchema.ResourceForest];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.ResourceForest] = value;
			}
		}

		public string AccountForest
		{
			get
			{
				return (string)this[GlobalLocatorServiceTenantSchema.AccountForest];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.AccountForest] = value;
			}
		}

		public string PrimarySite
		{
			get
			{
				return (string)this[GlobalLocatorServiceTenantSchema.PrimarySite];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.PrimarySite] = value;
			}
		}

		public SmtpDomain SmtpNextHopDomain
		{
			get
			{
				return (SmtpDomain)this[GlobalLocatorServiceTenantSchema.SmtpNextHopDomain];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.SmtpNextHopDomain] = value;
			}
		}

		public GlsTenantFlags TenantFlags
		{
			get
			{
				return (GlsTenantFlags)this[GlobalLocatorServiceTenantSchema.TenantFlags];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.TenantFlags] = value;
			}
		}

		public string TenantContainerCN
		{
			get
			{
				return (string)this[GlobalLocatorServiceTenantSchema.TenantContainerCN];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.TenantContainerCN] = value;
			}
		}

		public string ResumeCache
		{
			get
			{
				return (string)this[GlobalLocatorServiceTenantSchema.ResumeCache];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.ResumeCache] = value;
			}
		}

		public bool IsOfflineData
		{
			get
			{
				return (bool)this[GlobalLocatorServiceTenantSchema.IsOfflineData];
			}
			set
			{
				this[GlobalLocatorServiceTenantSchema.IsOfflineData] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<GlobalLocatorServiceTenantSchema>();
			}
		}
	}
}
