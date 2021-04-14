using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADMobileVirtualDirectory : ExchangeVirtualDirectory
	{
		internal static ExchangeObjectVersion MinimumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADMobileVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMobileVirtualDirectory";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public MobileClientFlagsType MobileClientFlags
		{
			get
			{
				return (MobileClientFlagsType)this[ADMobileVirtualDirectorySchema.MobileClientFlags];
			}
			internal set
			{
				this[ADMobileVirtualDirectorySchema.MobileClientFlags] = value;
			}
		}

		public bool MobileClientCertificateProvisioningEnabled
		{
			get
			{
				return (bool)this[ADMobileVirtualDirectorySchema.MobileClientCertificateProvisioningEnabled];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.MobileClientCertificateProvisioningEnabled] = value;
			}
		}

		public bool BadItemReportingEnabled
		{
			get
			{
				return (bool)this[ADMobileVirtualDirectorySchema.BadItemReportingEnabled];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.BadItemReportingEnabled] = value;
			}
		}

		public bool SendWatsonReport
		{
			get
			{
				return (bool)this[ADMobileVirtualDirectorySchema.SendWatsonReport];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.SendWatsonReport] = value;
			}
		}

		public string MobileClientCertificateAuthorityURL
		{
			get
			{
				return (string)this[ADMobileVirtualDirectorySchema.MobileClientCertificateAuthorityURL];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.MobileClientCertificateAuthorityURL] = value;
			}
		}

		public string MobileClientCertTemplateName
		{
			get
			{
				return (string)this[ADMobileVirtualDirectorySchema.MobileClientCertTemplateName];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.MobileClientCertTemplateName] = value;
			}
		}

		public string ActiveSyncServer
		{
			get
			{
				if (!(base.ExternalUrl == null))
				{
					return base.ExternalUrl.ToString();
				}
				return string.Empty;
			}
			set
			{
				try
				{
					base.ExternalUrl = new Uri(value);
				}
				catch (UriFormatException ex)
				{
					PropertyValidationError error = new PropertyValidationError(new LocalizedString(ex.Message), ADVirtualDirectorySchema.ExternalUrl, value);
					throw new DataValidationException(error, ex);
				}
			}
		}

		public RemoteDocumentsActions? RemoteDocumentsActionForUnknownServers
		{
			get
			{
				return (RemoteDocumentsActions?)this[ADMobileVirtualDirectorySchema.RemoteDocumentsActionForUnknownServers];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.RemoteDocumentsActionForUnknownServers] = value;
			}
		}

		public MultiValuedProperty<string> RemoteDocumentsAllowedServers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADMobileVirtualDirectorySchema.RemoteDocumentsAllowedServers];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.RemoteDocumentsAllowedServers] = value;
			}
		}

		public MultiValuedProperty<string> RemoteDocumentsBlockedServers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADMobileVirtualDirectorySchema.RemoteDocumentsBlockedServers];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.RemoteDocumentsBlockedServers] = value;
			}
		}

		public MultiValuedProperty<string> RemoteDocumentsInternalDomainSuffixList
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADMobileVirtualDirectorySchema.RemoteDocumentsInternalDomainSuffixList];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.RemoteDocumentsInternalDomainSuffixList] = value;
			}
		}

		public new string MetabasePath
		{
			get
			{
				return (string)this[ExchangeVirtualDirectorySchema.MetabasePath];
			}
		}

		internal static object RemoteDocumentsInternalDomainSuffixListGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADMobileVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList], ADMobileVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList);
		}

		internal static void RemoteDocumentsInternalDomainSuffixListSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADMobileVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADMobileVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList, propertyBag);
		}

		internal static object RemoteDocumentsAllowedServersGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADMobileVirtualDirectorySchema.ADRemoteDocumentsAllowedServers], ADMobileVirtualDirectorySchema.ADRemoteDocumentsAllowedServers);
		}

		internal static void RemoteDocumentsAllowedServersSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADMobileVirtualDirectorySchema.ADRemoteDocumentsAllowedServers] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADMobileVirtualDirectorySchema.ADRemoteDocumentsAllowedServers, propertyBag);
		}

		internal static object RemoteDocumentsBlockedServersGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADMobileVirtualDirectorySchema.ADRemoteDocumentsBlockedServers], ADMobileVirtualDirectorySchema.ADRemoteDocumentsBlockedServers);
		}

		internal static void RemoteDocumentsBlockedServersSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADMobileVirtualDirectorySchema.ADRemoteDocumentsBlockedServers] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADMobileVirtualDirectorySchema.ADRemoteDocumentsBlockedServers, propertyBag);
		}

		public bool BasicAuthEnabled
		{
			get
			{
				return (bool)this[ADMobileVirtualDirectorySchema.BasicAuthEnabled];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.BasicAuthEnabled] = value;
			}
		}

		public bool WindowsAuthEnabled
		{
			get
			{
				return (bool)this[ADMobileVirtualDirectorySchema.WindowsAuthEnabled];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.WindowsAuthEnabled] = value;
			}
		}

		public bool CompressionEnabled
		{
			get
			{
				return (bool)this[ADMobileVirtualDirectorySchema.CompressionEnabled];
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.CompressionEnabled] = value;
			}
		}

		public ClientCertAuthTypes? ClientCertAuth
		{
			get
			{
				return new ClientCertAuthTypes?((ClientCertAuthTypes)this[ADMobileVirtualDirectorySchema.ClientCertAuth]);
			}
			set
			{
				this[ADMobileVirtualDirectorySchema.ClientCertAuth] = value;
			}
		}

		public string WebsiteName
		{
			get
			{
				return (string)this[ADMobileVirtualDirectorySchema.WebsiteName];
			}
			internal set
			{
				this[ADMobileVirtualDirectorySchema.WebsiteName] = value;
			}
		}

		public bool WebSiteSSLEnabled
		{
			get
			{
				return (bool)this[ADMobileVirtualDirectorySchema.WebSiteSSLEnabled];
			}
			internal set
			{
				this[ADMobileVirtualDirectorySchema.WebSiteSSLEnabled] = value;
			}
		}

		public string VirtualDirectoryName
		{
			get
			{
				return (string)this[ADMobileVirtualDirectorySchema.VirtualDirectoryName];
			}
			internal set
			{
				this[ADMobileVirtualDirectorySchema.VirtualDirectoryName] = value;
			}
		}

		internal ExchangeVirtualDirectory ProxyVirtualDirectoryObject
		{
			get
			{
				return this.proxyVirtualDirectoryObject;
			}
		}

		internal void InitProxyVDirDataObject()
		{
			if (this.proxyVirtualDirectoryObject == null)
			{
				this.proxyVirtualDirectoryObject = new ExchangeVirtualDirectory();
				this.proxyVirtualDirectoryObject.SetExchangeVersion(base.ExchangeVersion);
				this.proxyVirtualDirectoryObject.MetabasePath = string.Format("{0}/{1}", this.MetabasePath, "Proxy");
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.MetabasePath.Length != 0)
			{
				if (this.MetabasePath.ToUpper().IndexOf("IIS://") != 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.MobileMetabasePathIsInvalid(base.Id.Name, this.MetabasePath), ExchangeVirtualDirectorySchema.MetabasePath, this.MetabasePath));
				}
				if (!base.ADPropertiesOnly)
				{
					bool flag;
					try
					{
						flag = DirectoryEntry.Exists(this.MetabasePath);
					}
					catch (COMException)
					{
						flag = false;
					}
					if (!flag)
					{
						errors.Add(new PropertyValidationError(DirectoryStrings.MobileAdOrphanFound(base.Id.Name), ExchangeVirtualDirectorySchema.MetabasePath, this.MetabasePath));
					}
				}
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(ADMobileVirtualDirectorySchema.ClientCertAuth))
			{
				this[ADMobileVirtualDirectorySchema.ClientCertAuth] = ClientCertAuthTypes.Ignore;
			}
			if (!base.IsModified(ADMobileVirtualDirectorySchema.WebsiteName))
			{
				this[ADMobileVirtualDirectorySchema.WebsiteName] = "Default Web Site";
			}
			if (!base.IsModified(ADMobileVirtualDirectorySchema.VirtualDirectoryName))
			{
				this[ADMobileVirtualDirectorySchema.VirtualDirectoryName] = "Microsoft-Server-ActiveSync";
			}
			base.StampPersistableDefaultValues();
		}

		public const string IisProxySubDir = "Proxy";

		public const string MostDerivedClass = "msExchMobileVirtualDirectory";

		private static readonly ADMobileVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADMobileVirtualDirectorySchema>();

		private ExchangeVirtualDirectory proxyVirtualDirectoryObject;
	}
}
