using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADOwaVirtualDirectory : ExchangeWebAppVirtualDirectory
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
				return ADOwaVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADOwaVirtualDirectory.MostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		private static ITopologyConfigurationSession ConfigurationSession
		{
			get
			{
				if (ADOwaVirtualDirectory.configurationSession == null)
				{
					ADOwaVirtualDirectory.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1944, "ConfigurationSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADOWAVirtualDirectory.cs");
				}
				return ADOwaVirtualDirectory.configurationSession;
			}
		}

		private bool IsOnExchange2007RTM
		{
			get
			{
				if (this.isOnExchange2007RTM == null)
				{
					Server server = ADOwaVirtualDirectory.ConfigurationSession.FindServerByName(base.Server.Name);
					int versionNumber = server.VersionNumber;
					int num = versionNumber >> 16 & 63;
					int num2 = versionNumber >> 22 & 63;
					if (num2 == Microsoft.Exchange.Data.Directory.SystemConfiguration.Server.Exchange2007MajorVersion && num == 0)
					{
						this.isOnExchange2007RTM = new bool?(true);
					}
					else
					{
						this.isOnExchange2007RTM = new bool?(false);
					}
				}
				return this.isOnExchange2007RTM.Value;
			}
		}

		internal bool IsExchange2007OrLater
		{
			get
			{
				return this.OwaVersion >= OwaVersions.Exchange2007;
			}
		}

		internal bool IsExchange2009OrLater
		{
			get
			{
				return this.OwaVersion >= OwaVersions.Exchange2010;
			}
		}

		internal bool IsExchange2013OrLater
		{
			get
			{
				return this.OwaVersion >= OwaVersions.Exchange2013;
			}
		}

		[Parameter]
		public bool? DirectFileAccessOnPublicComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.DirectFileAccessOnPublicComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.DirectFileAccessOnPublicComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? DirectFileAccessOnPrivateComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.DirectFileAccessOnPrivateComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.DirectFileAccessOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? WebReadyDocumentViewingOnPublicComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.WebReadyDocumentViewingOnPublicComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WebReadyDocumentViewingOnPublicComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? WebReadyDocumentViewingOnPrivateComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.WebReadyDocumentViewingOnPrivateComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WebReadyDocumentViewingOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? ForceWebReadyDocumentViewingFirstOnPublicComputers
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ForceWebReadyDocumentViewingFirstOnPublicComputers]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ForceWebReadyDocumentViewingFirstOnPublicComputers] = value;
			}
		}

		[Parameter]
		public bool? ForceWebReadyDocumentViewingFirstOnPrivateComputers
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ForceWebReadyDocumentViewingFirstOnPrivateComputers]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ForceWebReadyDocumentViewingFirstOnPrivateComputers] = value;
			}
		}

		[Parameter]
		public bool? WacViewingOnPublicComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.WacViewingOnPublicComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WacViewingOnPublicComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? WacViewingOnPrivateComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.WacViewingOnPrivateComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WacViewingOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? ForceWacViewingFirstOnPublicComputers
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ForceWacViewingFirstOnPublicComputers]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ForceWacViewingFirstOnPublicComputers] = value;
			}
		}

		[Parameter]
		public bool? ForceWacViewingFirstOnPrivateComputers
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ForceWacViewingFirstOnPrivateComputers]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ForceWacViewingFirstOnPrivateComputers] = value;
			}
		}

		[Parameter]
		public RemoteDocumentsActions? RemoteDocumentsActionForUnknownServers
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new RemoteDocumentsActions?((RemoteDocumentsActions)this[ADOwaVirtualDirectorySchema.RemoteDocumentsActionForUnknownServers]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.RemoteDocumentsActionForUnknownServers] = value;
			}
		}

		[Parameter]
		public AttachmentBlockingActions? ActionForUnknownFileAndMIMETypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new AttachmentBlockingActions?((AttachmentBlockingActions)this[ADOwaVirtualDirectorySchema.ActionForUnknownFileAndMIMETypes]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ActionForUnknownFileAndMIMETypes] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> WebReadyFileTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.WebReadyFileTypes];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WebReadyFileTypes] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> WebReadyMimeTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.WebReadyMimeTypes];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WebReadyMimeTypes] = value;
			}
		}

		[Parameter]
		public bool? WebReadyDocumentViewingForAllSupportedTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.WebReadyDocumentViewingForAllSupportedTypes]);
			}
			set
			{
				if (value != null)
				{
					this[ADOwaVirtualDirectorySchema.WebReadyDocumentViewingForAllSupportedTypes] = value;
				}
			}
		}

		[Parameter]
		public MultiValuedProperty<string> WebReadyDocumentViewingSupportedMimeTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				if (!this.IsOnExchange2007RTM)
				{
					return ADOwaVirtualDirectory.webReadyDocumentViewingSupportedMimeTypes;
				}
				return ADOwaVirtualDirectory.exchange2007RTMWebReadyDocumentViewingSupportedMimeTypes;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> WebReadyDocumentViewingSupportedFileTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				if (!this.IsOnExchange2007RTM)
				{
					return ADOwaVirtualDirectory.webReadyDocumentViewingSupportedFileTypes;
				}
				return ADOwaVirtualDirectory.exchange2007RTMWebReadyDocumentViewingSupportedFileTypes;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> AllowedFileTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.AllowedFileTypes];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.AllowedFileTypes] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> AllowedMimeTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.AllowedMimeTypes];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.AllowedMimeTypes] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ForceSaveFileTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.ForceSaveFileTypes];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ForceSaveFileTypes] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ForceSaveMimeTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.ForceSaveMimeTypes];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ForceSaveMimeTypes] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> BlockedFileTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.BlockedFileTypes];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.BlockedFileTypes] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> BlockedMimeTypes
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.BlockedMimeTypes];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.BlockedMimeTypes] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RemoteDocumentsAllowedServers
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.RemoteDocumentsAllowedServers];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.RemoteDocumentsAllowedServers] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RemoteDocumentsBlockedServers
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.RemoteDocumentsBlockedServers];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.RemoteDocumentsBlockedServers] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RemoteDocumentsInternalDomainSuffixList
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.RemoteDocumentsInternalDomainSuffixList];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.RemoteDocumentsInternalDomainSuffixList] = value;
			}
		}

		internal static object WebReadyFileTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADWebReadyFileTypes], ADOwaVirtualDirectorySchema.ADWebReadyFileTypes);
		}

		internal static void WebReadyFileTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADWebReadyFileTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADWebReadyFileTypes, propertyBag);
		}

		internal static object WebReadyMimeTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADWebReadyMimeTypes], ADOwaVirtualDirectorySchema.ADWebReadyMimeTypes);
		}

		internal static void WebReadyMimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADWebReadyMimeTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADWebReadyMimeTypes, propertyBag);
		}

		internal static object AllowedFileTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADAllowedFileTypes], ADOwaVirtualDirectorySchema.ADAllowedFileTypes);
		}

		internal static void AllowedFileTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADAllowedFileTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADAllowedFileTypes, propertyBag);
		}

		internal static object AllowedMimeTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADAllowedMimeTypes], ADOwaVirtualDirectorySchema.ADAllowedMimeTypes);
		}

		internal static void AllowedMimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADAllowedMimeTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADAllowedMimeTypes, propertyBag);
		}

		internal static object ForceSaveFileTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADForceSaveFileTypes], ADOwaVirtualDirectorySchema.ADForceSaveFileTypes);
		}

		internal static void ForceSaveFileTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADForceSaveFileTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADForceSaveFileTypes, propertyBag);
		}

		internal static object ForceSaveMimeTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADForceSaveMimeTypes], ADOwaVirtualDirectorySchema.ADForceSaveMimeTypes);
		}

		internal static void ForceSaveMimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADForceSaveMimeTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADForceSaveMimeTypes, propertyBag);
		}

		internal static object BlockedFileTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADBlockedFileTypes], ADOwaVirtualDirectorySchema.ADBlockedFileTypes);
		}

		internal static void BlockedFileTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADBlockedFileTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADBlockedFileTypes, propertyBag);
		}

		internal static object BlockedMimeTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADBlockedMimeTypes], ADOwaVirtualDirectorySchema.ADBlockedMimeTypes);
		}

		internal static void BlockedMimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADBlockedMimeTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADBlockedMimeTypes, propertyBag);
		}

		internal static object RemoteDocumentsAllowedServersGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADRemoteDocumentsAllowedServers], ADOwaVirtualDirectorySchema.ADRemoteDocumentsAllowedServers);
		}

		internal static void RemoteDocumentsAllowedServersSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADRemoteDocumentsAllowedServers] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADRemoteDocumentsAllowedServers, propertyBag);
		}

		internal static object RemoteDocumentsBlockedServersGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADRemoteDocumentsBlockedServers], ADOwaVirtualDirectorySchema.ADRemoteDocumentsBlockedServers);
		}

		internal static void RemoteDocumentsBlockedServersSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADRemoteDocumentsBlockedServers] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADRemoteDocumentsBlockedServers, propertyBag);
		}

		internal static object RemoteDocumentsInternalDomainSuffixListGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[ADOwaVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList], ADOwaVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList);
		}

		internal static void RemoteDocumentsInternalDomainSuffixListSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADOwaVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, ADOwaVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList, propertyBag);
		}

		public string FolderPathname
		{
			get
			{
				if (this.IsExchange2007OrLater)
				{
					return null;
				}
				return (string)this[ADOwaVirtualDirectorySchema.FolderPathname];
			}
			internal set
			{
				this[ADOwaVirtualDirectorySchema.FolderPathname] = value;
			}
		}

		public MultiValuedProperty<string> Url
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (MultiValuedProperty<string>)this[ADOwaVirtualDirectorySchema.Url];
			}
			internal set
			{
				this[ADOwaVirtualDirectorySchema.Url] = value;
			}
		}

		[Parameter]
		public LogonFormats LogonFormat
		{
			get
			{
				return (LogonFormats)this[ADOwaVirtualDirectorySchema.LogonFormat];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.LogonFormat] = value;
			}
		}

		[Parameter]
		public ClientAuthCleanupLevels ClientAuthCleanupLevel
		{
			get
			{
				return (ClientAuthCleanupLevels)this[ADOwaVirtualDirectorySchema.ClientAuthCleanupLevel];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ClientAuthCleanupLevel] = value;
			}
		}

		[Parameter]
		public bool? LogonPagePublicPrivateSelectionEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.LogonPagePublicPrivateSelectionEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.LogonPagePublicPrivateSelectionEnabled] = value;
			}
		}

		[Parameter]
		public bool? LogonPageLightSelectionEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.LogonPageLightSelectionEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.LogonPageLightSelectionEnabled] = value;
			}
		}

		[Parameter]
		public bool? IsPublic
		{
			get
			{
				if (!this.IsExchange2013OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.IsPublic]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.IsPublic] = value;
			}
		}

		[Parameter]
		public WebBeaconFilterLevels? FilterWebBeaconsAndHtmlForms
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new WebBeaconFilterLevels?((WebBeaconFilterLevels)this[ADOwaVirtualDirectorySchema.FilterWebBeaconsAndHtmlForms]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.FilterWebBeaconsAndHtmlForms] = value;
			}
		}

		[Parameter]
		public int? NotificationInterval
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (int?)this[ADOwaVirtualDirectorySchema.NotificationInterval];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.NotificationInterval] = value;
			}
		}

		[Parameter]
		public string DefaultTheme
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (string)this[ADOwaVirtualDirectorySchema.DefaultTheme];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.DefaultTheme] = value;
			}
		}

		[Parameter]
		public int? UserContextTimeout
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (int?)this[ADOwaVirtualDirectorySchema.UserContextTimeout];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.UserContextTimeout] = value;
			}
		}

		[Parameter]
		public ExchwebProxyDestinations? ExchwebProxyDestination
		{
			get
			{
				if (this.IsExchange2007OrLater)
				{
					return null;
				}
				return new ExchwebProxyDestinations?((ExchwebProxyDestinations)this[ADOwaVirtualDirectorySchema.ExchwebProxyDestination]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ExchwebProxyDestination] = value;
			}
		}

		[Parameter]
		public VirtualDirectoryTypes? VirtualDirectoryType
		{
			get
			{
				if (this.IsExchange2007OrLater)
				{
					return null;
				}
				return new VirtualDirectoryTypes?((VirtualDirectoryTypes)this[ADOwaVirtualDirectorySchema.VirtualDirectoryType]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.VirtualDirectoryType] = value;
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
			internal set
			{
				this[ADOwaVirtualDirectorySchema.OwaVersion] = value;
			}
		}

		public string ServerName
		{
			get
			{
				if (base.Server != null)
				{
					return base.Server.Name;
				}
				return null;
			}
		}

		[Parameter]
		public string InstantMessagingCertificateThumbprint
		{
			get
			{
				return (string)this[ADOwaVirtualDirectorySchema.InstantMessagingCertificateThumbprint];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.InstantMessagingCertificateThumbprint] = value;
			}
		}

		[Parameter]
		public string InstantMessagingServerName
		{
			get
			{
				return (string)this[ADOwaVirtualDirectorySchema.InstantMessagingServerName];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.InstantMessagingServerName] = value;
			}
		}

		[Parameter]
		public bool? RedirectToOptimalOWAServer
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((RedirectToOptimalOWAServerOptions)this[ADOwaVirtualDirectorySchema.RedirectToOptimalOWAServer] == RedirectToOptimalOWAServerOptions.Enabled);
			}
			set
			{
				if (value != null)
				{
					this[ADOwaVirtualDirectorySchema.RedirectToOptimalOWAServer] = (value.Value ? RedirectToOptimalOWAServerOptions.Enabled : RedirectToOptimalOWAServerOptions.Disabled);
				}
			}
		}

		[Parameter]
		public int? DefaultClientLanguage
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return (int?)this[ADOwaVirtualDirectorySchema.DefaultClientLanguage];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.DefaultClientLanguage] = value;
			}
		}

		[Parameter]
		public int LogonAndErrorLanguage
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return 0;
				}
				return (int)this[ADOwaVirtualDirectorySchema.LogonAndErrorLanguage];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.LogonAndErrorLanguage] = value;
			}
		}

		[Parameter]
		public bool? UseGB18030
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((int?)this[ADOwaVirtualDirectorySchema.UseGB18030] == 1);
			}
			set
			{
				if (value != null)
				{
					this[ADOwaVirtualDirectorySchema.UseGB18030] = (value.Value ? 1 : 0);
				}
			}
		}

		[Parameter]
		public bool? UseISO885915
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((int?)this[ADOwaVirtualDirectorySchema.UseISO885915] == 1);
			}
			set
			{
				if (value != null)
				{
					this[ADOwaVirtualDirectorySchema.UseISO885915] = (value.Value ? 1 : 0);
				}
			}
		}

		[Parameter]
		public OutboundCharsetOptions? OutboundCharset
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new OutboundCharsetOptions?((OutboundCharsetOptions)this[ADOwaVirtualDirectorySchema.OutboundCharset]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.OutboundCharset] = value;
			}
		}

		[Parameter]
		public bool? GlobalAddressListEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.GlobalAddressListEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.GlobalAddressListEnabled] = value;
			}
		}

		[Parameter]
		public bool? OrganizationEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.OrganizationEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.OrganizationEnabled] = value;
			}
		}

		[Parameter]
		public bool? ExplicitLogonEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ExplicitLogonEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ExplicitLogonEnabled] = value;
			}
		}

		[Parameter]
		public bool? OWALightEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.OWALightEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.OWALightEnabled] = value;
			}
		}

		[Parameter]
		public bool? DelegateAccessEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.DelegateAccessEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.DelegateAccessEnabled] = value;
			}
		}

		[Parameter]
		public bool? IRMEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.IRMEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.IRMEnabled] = value;
			}
		}

		[Parameter]
		public bool? CalendarEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.CalendarEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.CalendarEnabled] = value;
			}
		}

		[Parameter]
		public bool? ContactsEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ContactsEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ContactsEnabled] = value;
			}
		}

		[Parameter]
		public bool? TasksEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.TasksEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.TasksEnabled] = value;
			}
		}

		[Parameter]
		public bool? JournalEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.JournalEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.JournalEnabled] = value;
			}
		}

		[Parameter]
		public bool? NotesEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.NotesEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.NotesEnabled] = value;
			}
		}

		[Parameter]
		public bool? RemindersAndNotificationsEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.RemindersAndNotificationsEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.RemindersAndNotificationsEnabled] = value;
			}
		}

		[Parameter]
		public bool? PremiumClientEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.PremiumClientEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.PremiumClientEnabled] = value;
			}
		}

		[Parameter]
		public bool? SpellCheckerEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.SpellCheckerEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.SpellCheckerEnabled] = value;
			}
		}

		[Parameter]
		public bool? SearchFoldersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.SearchFoldersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.SearchFoldersEnabled] = value;
			}
		}

		[Parameter]
		public bool? SignaturesEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.SignaturesEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.SignaturesEnabled] = value;
			}
		}

		[Parameter]
		public bool? ThemeSelectionEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ThemeSelectionEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ThemeSelectionEnabled] = value;
			}
		}

		[Parameter]
		public bool? JunkEmailEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.JunkEmailEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.JunkEmailEnabled] = value;
			}
		}

		[Parameter]
		public bool? UMIntegrationEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.UMIntegrationEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.UMIntegrationEnabled] = value;
			}
		}

		[Parameter]
		public bool? WSSAccessOnPublicComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.WSSAccessOnPublicComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WSSAccessOnPublicComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? WSSAccessOnPrivateComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.WSSAccessOnPrivateComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WSSAccessOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? ChangePasswordEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ChangePasswordEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ChangePasswordEnabled] = value;
			}
		}

		[Parameter]
		public bool? UNCAccessOnPublicComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.UNCAccessOnPublicComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.UNCAccessOnPublicComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? UNCAccessOnPrivateComputersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.UNCAccessOnPrivateComputersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.UNCAccessOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter]
		public bool? ActiveSyncIntegrationEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ActiveSyncIntegrationEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ActiveSyncIntegrationEnabled] = value;
			}
		}

		[Parameter]
		public bool? AllAddressListsEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.AllAddressListsEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.AllAddressListsEnabled] = value;
			}
		}

		[Parameter]
		public bool? RulesEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.RulesEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.RulesEnabled] = value;
			}
		}

		[Parameter]
		public bool? PublicFoldersEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.PublicFoldersEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.PublicFoldersEnabled] = value;
			}
		}

		[Parameter]
		public bool? SMimeEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.SMimeEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.SMimeEnabled] = value;
			}
		}

		[Parameter]
		public bool? RecoverDeletedItemsEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.RecoverDeletedItemsEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.RecoverDeletedItemsEnabled] = value;
			}
		}

		[Parameter]
		public bool? InstantMessagingEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.InstantMessagingEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.InstantMessagingEnabled] = value;
			}
		}

		[Parameter]
		public bool? TextMessagingEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.TextMessagingEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.TextMessagingEnabled] = value;
			}
		}

		[Parameter]
		public bool? ForceSaveAttachmentFilteringEnabled
		{
			get
			{
				if (!this.IsExchange2007OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.ForceSaveAttachmentFilteringEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.ForceSaveAttachmentFilteringEnabled] = value;
			}
		}

		[Parameter]
		public bool? SilverlightEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.SilverlightEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.SilverlightEnabled] = value;
			}
		}

		[Parameter]
		public bool? PlacesEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.PlacesEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.PlacesEnabled] = value;
			}
		}

		[Parameter]
		public bool? WeatherEnabled
		{
			get
			{
				if (!this.IsExchange2013OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.WeatherEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WeatherEnabled] = value;
			}
		}

		[Parameter]
		public bool? AllowCopyContactsToDeviceAddressBook
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.AllowCopyContactsToDeviceAddressBook]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.AllowCopyContactsToDeviceAddressBook] = value;
			}
		}

		[Parameter]
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
			set
			{
				this[ADOwaVirtualDirectorySchema.AnonymousFeaturesEnabled] = value;
			}
		}

		[Parameter]
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
			set
			{
				this[ADOwaVirtualDirectorySchema.IntegratedFeaturesEnabled] = value;
			}
		}

		[Parameter]
		public bool? DisplayPhotosEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.DisplayPhotosEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.DisplayPhotosEnabled] = value;
			}
		}

		[Parameter]
		public bool? SetPhotoEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.SetPhotoEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.SetPhotoEnabled] = value;
			}
		}

		[Parameter]
		public bool? PredictedActionsEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.PredictedActionsEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.PredictedActionsEnabled] = value;
			}
		}

		[Parameter]
		public bool? UserDiagnosticEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[ADOwaVirtualDirectorySchema.UserDiagnosticEnabled]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.UserDiagnosticEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? ReportJunkEmailEnabled
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new bool?((bool)this[OwaMailboxPolicySchema.ReportJunkEmailEnabled]);
			}
			set
			{
				this[OwaMailboxPolicySchema.ReportJunkEmailEnabled] = value;
			}
		}

		[Parameter]
		public WebPartsFrameOptions? WebPartsFrameOptionsType
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new WebPartsFrameOptions?((WebPartsFrameOptions)this[ADOwaVirtualDirectorySchema.WebPartsFrameOptionsType]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WebPartsFrameOptionsType] = (int)value.Value;
			}
		}

		[Parameter(Mandatory = false)]
		public AllowOfflineOnEnum AllowOfflineOn
		{
			get
			{
				return (AllowOfflineOnEnum)this[ADOwaVirtualDirectorySchema.AllowOfflineOn];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.AllowOfflineOn] = value;
			}
		}

		[Parameter]
		public string SetPhotoURL
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return (string)this[ADOwaVirtualDirectorySchema.SetPhotoURL];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.SetPhotoURL] = value;
			}
		}

		[Parameter]
		public InstantMessagingTypeOptions? InstantMessagingType
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return new InstantMessagingTypeOptions?((InstantMessagingTypeOptions)this[ADOwaVirtualDirectorySchema.InstantMessagingType]);
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.InstantMessagingType] = value;
			}
		}

		[Parameter]
		public Uri Exchange2003Url
		{
			get
			{
				if (!this.IsExchange2009OrLater)
				{
					return null;
				}
				return (Uri)this[ADOwaVirtualDirectorySchema.Exchange2003Url];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.Exchange2003Url] = value;
			}
		}

		[Parameter]
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
			set
			{
				this[ADOwaVirtualDirectorySchema.FailbackUrl] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.LogonFormat == LogonFormats.UserName && (base.DefaultDomain == null || base.DefaultDomain.Length == 0) && !base.ADPropertiesOnly)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.OwaDefaultDomainRequiredWhenLogonFormatIsUserName, ADOwaVirtualDirectorySchema.LogonFormat, this.LogonFormat));
			}
			if (base.MetabasePath.Length != 0)
			{
				if (base.MetabasePath.ToUpper().IndexOf("IIS://") != 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.OwaMetabasePathIsInvalid(base.Id.Name, base.MetabasePath), ExchangeVirtualDirectorySchema.MetabasePath, base.MetabasePath));
				}
				if (!base.ADPropertiesOnly)
				{
					using (DirectoryEntry directoryEntry = new DirectoryEntry(base.MetabasePath))
					{
						if (directoryEntry == null)
						{
							errors.Add(new PropertyValidationError(DirectoryStrings.OwaAdOrphanFound(base.Id.Name), ExchangeVirtualDirectorySchema.MetabasePath, base.MetabasePath));
						}
					}
				}
				if (this.propertyBag.IsModified(ADVirtualDirectorySchema.InternalUrl) && !this.IsExchange2007OrLater)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("InternalUrl"), ADVirtualDirectorySchema.InternalUrl, base.InternalUrl));
				}
				if (this.propertyBag.IsModified(ADVirtualDirectorySchema.ExternalUrl) && !this.IsExchange2007OrLater)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("ExternalUrl"), ADVirtualDirectorySchema.ExternalUrl, base.ExternalUrl));
				}
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.RemoteDocumentsActionForUnknownServers) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("RemoteDocumentsActionForUnknownServers"), ADOwaVirtualDirectorySchema.RemoteDocumentsActionForUnknownServers, this.RemoteDocumentsActionForUnknownServers));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.ActionForUnknownFileAndMIMETypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("ActionForUnknownFileAndMIMETypes"), ADOwaVirtualDirectorySchema.ActionForUnknownFileAndMIMETypes, this.ActionForUnknownFileAndMIMETypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADWebReadyFileTypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("WebReadyFileTypes"), ADOwaVirtualDirectorySchema.WebReadyFileTypes, this.WebReadyFileTypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADWebReadyMimeTypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("WebReadyMimeTypes"), ADOwaVirtualDirectorySchema.WebReadyMimeTypes, this.WebReadyFileTypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADAllowedFileTypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("AllowedFileTypes"), ADOwaVirtualDirectorySchema.AllowedFileTypes, this.AllowedFileTypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADAllowedMimeTypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("AllowedMimeTypes"), ADOwaVirtualDirectorySchema.AllowedMimeTypes, this.AllowedMimeTypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADForceSaveFileTypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("ForceSaveFileTypes"), ADOwaVirtualDirectorySchema.ForceSaveFileTypes, this.ForceSaveFileTypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADForceSaveMimeTypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("ForceSaveMimeTypes"), ADOwaVirtualDirectorySchema.ForceSaveMimeTypes, this.ForceSaveMimeTypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADBlockedFileTypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("BlockedFileTypes"), ADOwaVirtualDirectorySchema.BlockedFileTypes, this.BlockedFileTypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADBlockedMimeTypes) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("BlockedMimeTypes"), ADOwaVirtualDirectorySchema.BlockedMimeTypes, this.BlockedMimeTypes));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADRemoteDocumentsAllowedServers) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("RemoteDocumentsAllowedServers"), ADOwaVirtualDirectorySchema.RemoteDocumentsAllowedServers, this.RemoteDocumentsAllowedServers));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADRemoteDocumentsBlockedServers) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("RemoteDocumentsBlockedServers"), ADOwaVirtualDirectorySchema.RemoteDocumentsBlockedServers, this.RemoteDocumentsBlockedServers));
			}
			if (this.propertyBag.IsChanged(ADOwaVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("RemoteDocumentsInternalDomainSuffixList"), ADOwaVirtualDirectorySchema.RemoteDocumentsInternalDomainSuffixList, this.RemoteDocumentsInternalDomainSuffixList));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.FilterWebBeaconsAndHtmlForms) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("FilterWebBeaconsAndHtmlForms"), ADOwaVirtualDirectorySchema.FilterWebBeaconsAndHtmlForms, this.FilterWebBeaconsAndHtmlForms));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.NotificationInterval) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("NotificationInterval"), ADOwaVirtualDirectorySchema.NotificationInterval, this.NotificationInterval));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.DefaultTheme) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("DefaultTheme"), ADOwaVirtualDirectorySchema.DefaultTheme, this.DefaultTheme));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.UserContextTimeout) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("UserContextTimeout"), ADOwaVirtualDirectorySchema.UserContextTimeout, this.UserContextTimeout));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.RedirectToOptimalOWAServer) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("RedirectToOptimalOWAServer"), ADOwaVirtualDirectorySchema.RedirectToOptimalOWAServer, this.RedirectToOptimalOWAServer));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.DefaultClientLanguage) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("DefaultClientLanguage"), ADOwaVirtualDirectorySchema.DefaultClientLanguage, this.DefaultClientLanguage));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.UseGB18030) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("UseGB18030"), ADOwaVirtualDirectorySchema.UseGB18030, this.UseGB18030));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.UseISO885915) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("UseISO885915"), ADOwaVirtualDirectorySchema.UseISO885915, this.UseISO885915));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.OutboundCharset) && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("OutboundCharset"), ADOwaVirtualDirectorySchema.OutboundCharset, this.OutboundCharset));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.InstantMessagingType) && !this.IsExchange2009OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("InstantMessagingType"), ADOwaVirtualDirectorySchema.InstantMessagingType, this.InstantMessagingType));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.LogonAndErrorLanguage) && this.LogonAndErrorLanguage != 0 && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("LogonAndErrorLanguage"), ADOwaVirtualDirectorySchema.LogonAndErrorLanguage, this.LogonAndErrorLanguage));
			}
			if (this.propertyBag.IsModified(ExchangeWebAppVirtualDirectorySchema.DigestAuthentication) && base.DigestAuthentication && !this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory("DigestAuthentication"), ExchangeWebAppVirtualDirectorySchema.DigestAuthentication, base.DigestAuthentication));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.ExchwebProxyDestination) && !this.ExchwebProxyDestination.Equals(ExchwebProxyDestinations.NotSpecified) && this.VirtualDirectoryType != VirtualDirectoryTypes.Exchweb)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnVirtualDirectoryOtherThanExchweb, ADOwaVirtualDirectorySchema.ExchwebProxyDestination, this.ExchwebProxyDestination));
			}
			if (this.propertyBag.IsModified(ADOwaVirtualDirectorySchema.VirtualDirectoryType) && this.IsExchange2007OrLater)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnE12VirtualDirectory("VirtualDirectoryType"), ADOwaVirtualDirectorySchema.VirtualDirectoryType, this.VirtualDirectoryType));
			}
		}

		private static readonly ADOwaVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADOwaVirtualDirectorySchema>();

		private static readonly MultiValuedProperty<string> webReadyDocumentViewingSupportedFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultWebReadyFileTypes);

		private static readonly MultiValuedProperty<string> exchange2007RTMWebReadyDocumentViewingSupportedFileTypes = new MultiValuedProperty<string>(ADOwaVirtualDirectorySchema.Exchange2007RTMWebReadyDocumentViewingSupportedFileTypes);

		private static readonly MultiValuedProperty<string> webReadyDocumentViewingSupportedMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultWebReadyMimeTypes);

		private static readonly MultiValuedProperty<string> exchange2007RTMWebReadyDocumentViewingSupportedMimeTypes = new MultiValuedProperty<string>(ADOwaVirtualDirectorySchema.Exchange2007RTMWebReadyDocumentViewingSupportedMimeTypes);

		public static readonly string MostDerivedClass = "msExchOwaVirtualDirectory";

		private static ITopologyConfigurationSession configurationSession;

		private bool? isOnExchange2007RTM = null;
	}
}
