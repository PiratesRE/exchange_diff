using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class TenantInboundConnector : ADConfigurationObject
	{
		internal static object SenderDomainsGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[TenantInboundConnectorSchema.SenderDomainString];
			if (string.IsNullOrEmpty(text))
			{
				return new MultiValuedProperty<AddressSpace>(false, TenantInboundConnectorSchema.SenderDomains, new AddressSpace[0]);
			}
			List<AddressSpace> senderDomainsFromString = TenantInboundConnector.GetSenderDomainsFromString(text);
			return new MultiValuedProperty<AddressSpace>(false, TenantInboundConnectorSchema.SenderDomains, senderDomainsFromString);
		}

		internal static void SenderDomainsSetter(object value, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				propertyBag[TenantInboundConnectorSchema.SenderDomainString] = string.Empty;
				return;
			}
			MultiValuedProperty<AddressSpace> senderDomains = (MultiValuedProperty<AddressSpace>)value;
			string value2 = TenantInboundConnector.ConvertSenderdomainsToString(senderDomains);
			propertyBag[TenantInboundConnectorSchema.SenderDomainString] = value2;
		}

		internal static string ConvertSenderdomainsToString(IList<AddressSpace> senderDomains)
		{
			if (senderDomains == null || senderDomains.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (AddressSpace addressSpace in senderDomains)
			{
				num++;
				stringBuilder.Append(addressSpace.ToString());
				if (num < senderDomains.Count)
				{
					stringBuilder.Append(',');
				}
			}
			return stringBuilder.ToString();
		}

		internal static List<AddressSpace> GetSenderDomainsFromString(string senderDomainString)
		{
			List<AddressSpace> list = new List<AddressSpace>();
			if (!string.IsNullOrEmpty(senderDomainString))
			{
				string[] array = senderDomainString.Split(new char[]
				{
					TenantInboundConnector.senderDomainsDelimiter
				});
				foreach (string address in array)
				{
					AddressSpace item = null;
					if (!AddressSpace.TryParse(address, out item))
					{
						return new List<AddressSpace>();
					}
					list.Add(item);
				}
			}
			return list;
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return TenantInboundConnector.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TenantInboundConnector.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return TenantInboundConnector.RootId;
			}
		}

		internal override void InitializeSchema()
		{
		}

		public TenantInboundConnector()
		{
		}

		internal TenantInboundConnector(IConfigurationSession session, string tenantId)
		{
			this.m_Session = session;
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal TenantInboundConnector(string tenantId)
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[TenantInboundConnectorSchema.Enabled];
			}
			set
			{
				this[TenantInboundConnectorSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TenantConnectorType ConnectorType
		{
			get
			{
				return (TenantConnectorType)this[TenantInboundConnectorSchema.ConnectorType];
			}
			set
			{
				this[TenantInboundConnectorSchema.ConnectorType] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public TenantConnectorSource ConnectorSource
		{
			get
			{
				return (TenantConnectorSource)this[TenantInboundConnectorSchema.ConnectorSourceFlags];
			}
			set
			{
				this[TenantInboundConnectorSchema.ConnectorSourceFlags] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)this[TenantInboundConnectorSchema.Comment];
			}
			set
			{
				this[TenantInboundConnectorSchema.Comment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> SenderIPAddresses
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[TenantInboundConnectorSchema.RemoteIPRanges];
			}
			set
			{
				this[TenantInboundConnectorSchema.RemoteIPRanges] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<AddressSpace> SenderDomains
		{
			get
			{
				return (MultiValuedProperty<AddressSpace>)this[TenantInboundConnectorSchema.SenderDomains];
			}
			set
			{
				this[TenantInboundConnectorSchema.SenderDomains] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AssociatedAcceptedDomains
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[TenantInboundConnectorSchema.AssociatedAcceptedDomains];
			}
			set
			{
				this[TenantInboundConnectorSchema.AssociatedAcceptedDomains] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireTls
		{
			get
			{
				return (bool)this[TenantInboundConnectorSchema.RequireTls];
			}
			set
			{
				this[TenantInboundConnectorSchema.RequireTls] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RestrictDomainsToIPAddresses
		{
			get
			{
				return (bool)this[TenantInboundConnectorSchema.RestrictDomainsToIPAddresses];
			}
			set
			{
				this[TenantInboundConnectorSchema.RestrictDomainsToIPAddresses] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RestrictDomainsToCertificate
		{
			get
			{
				return (bool)this[TenantInboundConnectorSchema.RestrictDomainsToCertificate];
			}
			set
			{
				this[TenantInboundConnectorSchema.RestrictDomainsToCertificate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CloudServicesMailEnabled
		{
			get
			{
				return (bool)this[TenantInboundConnectorSchema.CloudServicesMailEnabled];
			}
			set
			{
				this[TenantInboundConnectorSchema.CloudServicesMailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TlsCertificate TlsSenderCertificateName
		{
			get
			{
				return (TlsCertificate)this[TenantInboundConnectorSchema.TlsSenderCertificateName];
			}
			set
			{
				this[TenantInboundConnectorSchema.TlsSenderCertificateName] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.ConnectorType == TenantConnectorType.OnPremises && MultiValuedPropertyBase.IsNullOrEmpty(this.SenderIPAddresses) && this.TlsSenderCertificateName == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InboundConnectorMissingTlsCertificateOrSenderIP, TenantInboundConnectorSchema.RemoteIPRanges, this));
			}
			if (this.TlsSenderCertificateName != null && !this.RequireTls)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InboundConnectorRequiredTlsSettingsInvalid, TenantInboundConnectorSchema.RequireTls, this));
			}
			if (this.RestrictDomainsToIPAddresses && MultiValuedPropertyBase.IsNullOrEmpty(this.SenderIPAddresses))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InboundConnectorInvalidRestrictDomainsToIPAddresses, TenantInboundConnectorSchema.RestrictDomainsToIPAddresses, this));
			}
			if (this.RestrictDomainsToIPAddresses && this.RestrictDomainsToCertificate)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InboundConnectorInvalidIPCertificateCombinations, TenantInboundConnectorSchema.RestrictDomainsToCertificate, this));
			}
			if (this.RestrictDomainsToCertificate && this.TlsSenderCertificateName == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InboundConnectorInvalidRestrictDomainsToCertificate, TenantInboundConnectorSchema.RestrictDomainsToCertificate, this));
			}
			if (this.ConnectorType != TenantConnectorType.OnPremises && this.CloudServicesMailEnabled)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InboundConnectorIncorrectCloudServicesMailEnabled, TenantInboundConnectorSchema.CloudServicesMailEnabled, this));
			}
		}

		internal const int MinCidrLength = 24;

		private static readonly char senderDomainsDelimiter = ',';

		private static readonly ADObjectId RootId = new ADObjectId("CN=Transport Settings");

		private static readonly TenantInboundConnectorSchema schema = ObjectSchema.GetInstance<TenantInboundConnectorSchema>();

		private static string mostDerivedClass = "msExchSMTPInboundConnector";
	}
}
