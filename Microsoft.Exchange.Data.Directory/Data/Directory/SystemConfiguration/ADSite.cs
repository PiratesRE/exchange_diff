using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class ADSite : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSite.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADSite.mostDerivedClass;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HubSiteEnabled
		{
			get
			{
				return (bool)this[ADSiteSchema.HubSiteEnabled];
			}
			set
			{
				this[ADSiteSchema.HubSiteEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InboundMailEnabled
		{
			get
			{
				return !(bool)this[ADSiteSchema.InboundMailDisabled];
			}
			set
			{
				this[ADSiteSchema.InboundMailDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PartnerId
		{
			get
			{
				return (int)this[ADSiteSchema.PartnerId];
			}
			set
			{
				this[ADSiteSchema.PartnerId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MinorPartnerId
		{
			get
			{
				return (int)this[ADSiteSchema.MinorPartnerId];
			}
			set
			{
				this[ADSiteSchema.MinorPartnerId] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ResponsibleForSites
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADSiteSchema.ResponsibleForSites];
			}
			set
			{
				this[ADSiteSchema.ResponsibleForSites] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		public const int InvalidPartnerId = -1;

		private static ADSiteSchema schema = ObjectSchema.GetInstance<ADSiteSchema>();

		private static string mostDerivedClass = "site";
	}
}
