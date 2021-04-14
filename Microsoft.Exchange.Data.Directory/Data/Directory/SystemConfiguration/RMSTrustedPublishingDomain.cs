using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class RMSTrustedPublishingDomain : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RMSTrustedPublishingDomain.adSchema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchControlPointTrustedPublishingDomain";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return RMSTrustedPublishingDomain.parentPath;
			}
		}

		public Uri IntranetLicensingUrl
		{
			get
			{
				return (Uri)this[RMSTrustedPublishingDomainSchema.IntranetLicensingUrl];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.IntranetLicensingUrl] = value;
			}
		}

		public Uri ExtranetLicensingUrl
		{
			get
			{
				return (Uri)this[RMSTrustedPublishingDomainSchema.ExtranetLicensingUrl];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.ExtranetLicensingUrl] = value;
			}
		}

		public Uri IntranetCertificationUrl
		{
			get
			{
				return (Uri)this[RMSTrustedPublishingDomainSchema.IntranetCertificationUrl];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.IntranetCertificationUrl] = value;
			}
		}

		public Uri ExtranetCertificationUrl
		{
			get
			{
				return (Uri)this[RMSTrustedPublishingDomainSchema.ExtranetCertificationUrl];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.ExtranetCertificationUrl] = value;
			}
		}

		public bool Default
		{
			get
			{
				return (bool)this[RMSTrustedPublishingDomainSchema.Default];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.Default] = value;
			}
		}

		public int CryptoMode
		{
			get
			{
				return (int)this[RMSTrustedPublishingDomainSchema.CryptoMode];
			}
		}

		public int CSPType
		{
			get
			{
				return (int)this[RMSTrustedPublishingDomainSchema.CSPType];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.CSPType] = value;
			}
		}

		public string CSPName
		{
			get
			{
				return (string)this[RMSTrustedPublishingDomainSchema.CSPName];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.CSPName] = value;
			}
		}

		public bool IsRMSOnline
		{
			get
			{
				return (bool)this[RMSTrustedPublishingDomainSchema.IsRMSOnline];
			}
		}

		public string KeyContainerName
		{
			get
			{
				return (string)this[RMSTrustedPublishingDomainSchema.KeyContainerName];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.KeyContainerName] = value;
			}
		}

		public string KeyId
		{
			get
			{
				return (string)this[RMSTrustedPublishingDomainSchema.KeyId];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.KeyId] = value;
			}
		}

		public string KeyIdType
		{
			get
			{
				return (string)this[RMSTrustedPublishingDomainSchema.KeyIdType];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.KeyIdType] = value;
			}
		}

		public int KeyNumber
		{
			get
			{
				return (int)this[RMSTrustedPublishingDomainSchema.KeyNumber];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.KeyNumber] = value;
			}
		}

		internal string SLCCertChain
		{
			get
			{
				return (string)this[RMSTrustedPublishingDomainSchema.SLCCertChain];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.SLCCertChain] = value;
			}
		}

		internal string PrivateKey
		{
			get
			{
				return (string)this[RMSTrustedPublishingDomainSchema.PrivateKey];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.PrivateKey] = value;
			}
		}

		[ValidateCount(0, 25)]
		internal MultiValuedProperty<string> RMSTemplates
		{
			get
			{
				return (MultiValuedProperty<string>)this[RMSTrustedPublishingDomainSchema.RMSTemplates];
			}
			set
			{
				this[RMSTrustedPublishingDomainSchema.RMSTemplates] = value;
			}
		}

		private const int MaxRMSTemplates = 25;

		private const string MostDerivedClass = "msExchControlPointTrustedPublishingDomain";

		private static readonly RMSTrustedPublishingDomainSchema adSchema = ObjectSchema.GetInstance<RMSTrustedPublishingDomainSchema>();

		private static readonly ADObjectId parentPath = new ADObjectId("CN=ControlPoint Config,CN=Transport Settings");
	}
}
