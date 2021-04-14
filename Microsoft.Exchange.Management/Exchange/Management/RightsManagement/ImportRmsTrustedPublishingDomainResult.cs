using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Serializable]
	public sealed class ImportRmsTrustedPublishingDomainResult : ADPresentationObject
	{
		public ImportRmsTrustedPublishingDomainResult(RMSTrustedPublishingDomain rmsTpd) : base(rmsTpd)
		{
			this.IntranetLicensingUrl = rmsTpd.IntranetLicensingUrl;
			this.ExtranetLicensingUrl = rmsTpd.ExtranetLicensingUrl;
			this.IntranetCertificationUrl = rmsTpd.IntranetCertificationUrl;
			this.ExtranetCertificationUrl = rmsTpd.ExtranetCertificationUrl;
			this.Default = rmsTpd.Default;
			this.CSPType = rmsTpd.CSPType;
			this.CSPName = rmsTpd.CSPName;
			this.KeyContainerName = rmsTpd.KeyContainerName;
			this.KeyId = rmsTpd.KeyId;
			this.KeyIdType = rmsTpd.KeyIdType;
			this.KeyNumber = rmsTpd.KeyNumber;
			base.Name = rmsTpd.Name;
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ImportRmsTrustedPublishingDomainResult.schema;
			}
		}

		public Uri IntranetLicensingUrl
		{
			get
			{
				return (Uri)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.IntranetLicensingUrl];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.IntranetLicensingUrl] = value;
			}
		}

		public Uri ExtranetLicensingUrl
		{
			get
			{
				return (Uri)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.ExtranetLicensingUrl];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.ExtranetLicensingUrl] = value;
			}
		}

		public Uri IntranetCertificationUrl
		{
			get
			{
				return (Uri)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.IntranetCertificationUrl];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.IntranetCertificationUrl] = value;
			}
		}

		public Uri ExtranetCertificationUrl
		{
			get
			{
				return (Uri)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.ExtranetCertificationUrl];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.ExtranetCertificationUrl] = value;
			}
		}

		public bool Default
		{
			get
			{
				return (bool)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.Default];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.Default] = value;
			}
		}

		public int CryptoMode
		{
			get
			{
				return (int)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.CryptoMode];
			}
		}

		public int CSPType
		{
			get
			{
				return (int)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.CSPType];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.CSPType] = value;
			}
		}

		public string CSPName
		{
			get
			{
				return (string)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.CSPName];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.CSPName] = value;
			}
		}

		public string KeyContainerName
		{
			get
			{
				return (string)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.KeyContainerName];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.KeyContainerName] = value;
			}
		}

		public string KeyId
		{
			get
			{
				return (string)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.KeyId];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.KeyId] = value;
			}
		}

		public string KeyIdType
		{
			get
			{
				return (string)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.KeyIdType];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.KeyIdType] = value;
			}
		}

		public int KeyNumber
		{
			get
			{
				return (int)this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.KeyNumber];
			}
			set
			{
				this[ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema.KeyNumber] = value;
			}
		}

		public MultiValuedProperty<string> AddedTemplates
		{
			get
			{
				return this.addedTemplates;
			}
		}

		public MultiValuedProperty<string> UpdatedTemplates
		{
			get
			{
				return this.updatedTemplates;
			}
		}

		public MultiValuedProperty<string> RemovedTemplates
		{
			get
			{
				return this.removedTemplates;
			}
		}

		private static readonly ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema schema = ObjectSchema.GetInstance<ImportRmsTrustedPublishingDomainResult.ImportRmsTrustedPublishingDomainResultSchema>();

		private readonly MultiValuedProperty<string> addedTemplates = new MultiValuedProperty<string>();

		private readonly MultiValuedProperty<string> updatedTemplates = new MultiValuedProperty<string>();

		private readonly MultiValuedProperty<string> removedTemplates = new MultiValuedProperty<string>();

		private sealed class ImportRmsTrustedPublishingDomainResultSchema : ADPresentationSchema
		{
			internal override ADObjectSchema GetParentSchema()
			{
				return ObjectSchema.GetInstance<RMSTrustedPublishingDomainSchema>();
			}

			public static readonly ADPropertyDefinition CSPName = RMSTrustedPublishingDomainSchema.CSPName;

			public static readonly ADPropertyDefinition CSPType = RMSTrustedPublishingDomainSchema.CSPType;

			public static readonly ADPropertyDefinition KeyId = RMSTrustedPublishingDomainSchema.KeyId;

			public static readonly ADPropertyDefinition KeyIdType = RMSTrustedPublishingDomainSchema.KeyIdType;

			public static readonly ADPropertyDefinition KeyContainerName = RMSTrustedPublishingDomainSchema.KeyContainerName;

			public static readonly ADPropertyDefinition KeyNumber = RMSTrustedPublishingDomainSchema.KeyNumber;

			public static readonly ADPropertyDefinition IntranetLicensingUrl = RMSTrustedPublishingDomainSchema.IntranetLicensingUrl;

			public static readonly ADPropertyDefinition ExtranetLicensingUrl = RMSTrustedPublishingDomainSchema.ExtranetLicensingUrl;

			public static readonly ADPropertyDefinition IntranetCertificationUrl = RMSTrustedPublishingDomainSchema.IntranetCertificationUrl;

			public static readonly ADPropertyDefinition ExtranetCertificationUrl = RMSTrustedPublishingDomainSchema.ExtranetCertificationUrl;

			public static readonly ADPropertyDefinition Default = RMSTrustedPublishingDomainSchema.Default;

			public static readonly ADPropertyDefinition CryptoMode = RMSTrustedPublishingDomainSchema.CryptoMode;
		}
	}
}
