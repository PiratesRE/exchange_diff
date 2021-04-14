using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MiniVirtualDirectory : MiniObject
	{
		static MiniVirtualDirectory()
		{
			ADVirtualDirectory advirtualDirectory = new ADVirtualDirectory();
			MiniVirtualDirectory.implicitFilter = advirtualDirectory.ImplicitFilter;
			MiniVirtualDirectory.mostDerivedClass = advirtualDirectory.MostDerivedObjectClass;
			MiniVirtualDirectory.schema = ObjectSchema.GetInstance<MiniVirtualDirectorySchema>();
		}

		public bool IsAvailabilityForeignConnector
		{
			get
			{
				return base.ObjectClass.Contains(ADAvailabilityForeignConnectorVirtualDirectory.MostDerivedClass);
			}
		}

		public bool IsE12UM
		{
			get
			{
				return base.ObjectClass.Contains(ADE12UMVirtualDirectory.MostDerivedClass);
			}
		}

		public bool IsEcp
		{
			get
			{
				return base.ObjectClass.Contains(ADEcpVirtualDirectory.MostDerivedClass);
			}
		}

		public bool IsMapi
		{
			get
			{
				return base.ObjectClass.Contains(ADMapiVirtualDirectory.MostDerivedClass);
			}
		}

		public bool IsMobile
		{
			get
			{
				return base.ObjectClass.Contains("msExchMobileVirtualDirectory");
			}
		}

		public bool IsOab
		{
			get
			{
				return base.ObjectClass.Contains(ADOabVirtualDirectory.MostDerivedClass);
			}
		}

		public bool IsOwa
		{
			get
			{
				return base.ObjectClass.Contains(ADOwaVirtualDirectory.MostDerivedClass);
			}
		}

		public bool IsRpcHttp
		{
			get
			{
				return base.ObjectClass.Contains(ADRpcHttpVirtualDirectory.MostDerivedClass);
			}
		}

		public bool IsWebServices
		{
			get
			{
				return base.ObjectClass.Contains(ADWebServicesVirtualDirectory.MostDerivedClass);
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[ADVirtualDirectorySchema.Server];
			}
		}

		public Uri InternalUrl
		{
			get
			{
				return (Uri)this[ADVirtualDirectorySchema.InternalUrl];
			}
		}

		public MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)this[ADVirtualDirectorySchema.InternalAuthenticationMethods];
			}
		}

		public Uri ExternalUrl
		{
			get
			{
				return (Uri)this[ADVirtualDirectorySchema.ExternalUrl];
			}
		}

		public MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)this[ADVirtualDirectorySchema.ExternalAuthenticationMethods];
			}
		}

		public string MetabasePath
		{
			get
			{
				return (string)this[ExchangeVirtualDirectorySchema.MetabasePath];
			}
		}

		public bool LiveIdAuthentication
		{
			get
			{
				return (bool)this[ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication];
			}
		}

		public string AvailabilityForeignConnectorType
		{
			get
			{
				return (string)this[ADAvailabilityForeignConnectorVirtualDirectorySchema.AvailabilityForeignConnectorType];
			}
		}

		public MultiValuedProperty<string> AvailabilityForeignConnectorDomains
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADAvailabilityForeignConnectorVirtualDirectorySchema.AvailabilityForeignConnectorDomains];
			}
		}

		public bool AdminEnabled
		{
			get
			{
				return (bool)this[ADEcpVirtualDirectorySchema.AdminEnabled];
			}
		}

		public bool OwaOptionsEnabled
		{
			get
			{
				return (bool)this[ADEcpVirtualDirectorySchema.OwaOptionsEnabled];
			}
		}

		public bool MobileClientCertificateProvisioningEnabled
		{
			get
			{
				return (bool)this[ADMobileVirtualDirectorySchema.MobileClientCertificateProvisioningEnabled];
			}
		}

		public string MobileClientCertificateAuthorityURL
		{
			get
			{
				return (string)this[ADMobileVirtualDirectorySchema.MobileClientCertificateAuthorityURL];
			}
		}

		public string MobileClientCertTemplateName
		{
			get
			{
				return (string)this[ADMobileVirtualDirectorySchema.MobileClientCertTemplateName];
			}
		}

		public MultiValuedProperty<ADObjectId> OfflineAddressBooks
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADOabVirtualDirectorySchema.OfflineAddressBooks];
			}
		}

		public OwaVersions OwaVersion
		{
			get
			{
				OwaVersions owaVersions = (OwaVersions)this[ADOwaVirtualDirectorySchema.OwaVersion];
				if (base.ExchangeVersion.ExchangeBuild.Major < 14 && owaVersions >= OwaVersions.Exchange2007)
				{
					return OwaVersions.Exchange2007;
				}
				return owaVersions;
			}
		}

		public bool? AnonymousFeaturesEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.AnonymousFeaturesEnabled]);
			}
		}

		public Uri FailbackUrl
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return (Uri)this[ADOwaVirtualDirectorySchema.FailbackUrl];
			}
		}

		public bool? IntegratedFeaturesEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.IntegratedFeaturesEnabled]);
			}
		}

		public AuthenticationMethod ExternalClientAuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)this[ADRpcHttpVirtualDirectorySchema.ExternalClientAuthenticationMethod];
			}
		}

		public AuthenticationMethod InternalClientAuthenticationMethod
		{
			get
			{
				int num = (int)this[ADEcpVirtualDirectorySchema.ADFeatureSet];
				if (num == -1)
				{
					return AuthenticationMethod.Ntlm;
				}
				return (AuthenticationMethod)num;
			}
		}

		public MultiValuedProperty<AuthenticationMethod> IISAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)this[ADRpcHttpVirtualDirectorySchema.IISAuthenticationMethods];
			}
		}

		public Uri XropUrl
		{
			get
			{
				MultiValuedProperty<Uri> multiValuedProperty = (MultiValuedProperty<Uri>)this[ADRpcHttpVirtualDirectorySchema.XropUrl];
				if (multiValuedProperty != null && multiValuedProperty.Count != 0)
				{
					return multiValuedProperty[0];
				}
				return null;
			}
		}

		public Uri InternalNLBBypassUrl
		{
			get
			{
				return (Uri)this[ADWebServicesVirtualDirectorySchema.InternalNLBBypassUrl];
			}
		}

		public bool MRSProxyEnabled
		{
			get
			{
				return (bool)this[ADWebServicesVirtualDirectorySchema.MRSProxyEnabled];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MiniVirtualDirectory.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return MiniVirtualDirectory.implicitFilter;
			}
		}

		internal bool IsExchange2009OrLater
		{
			get
			{
				return this.OwaVersion >= OwaVersions.Exchange2010;
			}
		}

		private int ADFeatureSet
		{
			get
			{
				return (int)this[ADEcpVirtualDirectorySchema.ADFeatureSet];
			}
		}

		internal static MiniVirtualDirectory CreateFrom(ADObject virtualDirectory, ICollection<PropertyDefinition> propertyDefinitions, object[] propertyValues)
		{
			MiniVirtualDirectory miniVirtualDirectory = new MiniVirtualDirectory();
			IEnumerable<PropertyDefinition> allProperties = miniVirtualDirectory.Schema.AllProperties;
			ADPropertyBag adpropertyBag = new ADPropertyBag();
			adpropertyBag.SetIsReadOnly(false);
			foreach (PropertyDefinition propertyDefinition in allProperties)
			{
				ADPropertyDefinition key = (ADPropertyDefinition)propertyDefinition;
				object value = virtualDirectory.propertyBag.Contains(key) ? virtualDirectory.propertyBag[key] : null;
				adpropertyBag.SetField(key, value);
			}
			MultiValuedProperty<string> multiValuedProperty = adpropertyBag[ADObjectSchema.ObjectClass] as MultiValuedProperty<string>;
			if (multiValuedProperty == null || multiValuedProperty.Count == 0)
			{
				multiValuedProperty = new MultiValuedProperty<string>(virtualDirectory.MostDerivedObjectClass);
				adpropertyBag.SetField(ADObjectSchema.ObjectClass, multiValuedProperty);
			}
			if (adpropertyBag[ADObjectSchema.WhenChangedUTC] == null)
			{
				DateTime utcNow = DateTime.UtcNow;
				adpropertyBag.SetField(ADObjectSchema.WhenChangedUTC, utcNow);
				adpropertyBag.SetField(ADObjectSchema.WhenCreatedUTC, utcNow);
			}
			if (propertyDefinitions != null && propertyValues != null)
			{
				adpropertyBag.SetProperties(propertyDefinitions, propertyValues);
			}
			adpropertyBag.SetIsReadOnly(true);
			miniVirtualDirectory.propertyBag = adpropertyBag;
			return miniVirtualDirectory;
		}

		private static QueryFilter implicitFilter;

		private static ADObjectSchema schema;

		private static string mostDerivedClass;
	}
}
