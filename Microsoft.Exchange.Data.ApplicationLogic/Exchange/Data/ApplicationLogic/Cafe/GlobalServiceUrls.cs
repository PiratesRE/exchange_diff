using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class GlobalServiceUrls
	{
		public static MiniVirtualDirectory GetRpcHttpVdir()
		{
			GlobalServiceUrls.DiscoverUrlsIfNeeded();
			return GlobalServiceUrls.rpcHttpVdir;
		}

		public static Uri GetExternalUrl<Service>() where Service : HttpService
		{
			ServiceType key = GlobalServiceUrls.ConvertHttpServiceType<Service>();
			GlobalServiceUrls.DiscoverUrlsIfNeeded();
			return GlobalServiceUrls.E15Services[key].ExternalUri;
		}

		public static Uri GetInternalUrl<Service>(string serverFqdn) where Service : HttpService
		{
			ServiceType key = GlobalServiceUrls.ConvertHttpServiceType<Service>();
			GlobalServiceUrls.DiscoverUrlsIfNeeded();
			VDirInfo vdirInfo = GlobalServiceUrls.E15Services[key];
			Uri externalUri = vdirInfo.ExternalUri;
			UriBuilder uriBuilder = new UriBuilder(externalUri.Scheme, serverFqdn, 444, vdirInfo.Path);
			return uriBuilder.Uri;
		}

		public static Uri GetExternalUrl<Service>(string serverFqdn) where Service : HttpService
		{
			ServiceType key = GlobalServiceUrls.ConvertHttpServiceType<Service>();
			GlobalServiceUrls.DiscoverUrlsIfNeeded();
			VDirInfo vdirInfo = GlobalServiceUrls.E15Services[key];
			Uri externalUri = vdirInfo.ExternalUri;
			UriBuilder uriBuilder = new UriBuilder(externalUri.Scheme, serverFqdn, 443, vdirInfo.Path);
			return uriBuilder.Uri;
		}

		public static ProtocolConnectionSettings GetExternalProtocolSettingsForLocalServer<ProtocolServiceType>() where ProtocolServiceType : Service
		{
			ServiceType key = GlobalServiceUrls.ConvertProtocolServiceType<ProtocolServiceType>();
			GlobalServiceUrls.DiscoverProtocolSettingsIfNeeded();
			ProtocolConnectionSettings result = null;
			GlobalServiceUrls.E15Protocols.TryGetValue(key, out result);
			return result;
		}

		public static ProtocolConnectionSettings GetInternalProtocolSettingsForLocalServer<ProtocolServiceType>() where ProtocolServiceType : Service
		{
			ServiceType key = GlobalServiceUrls.ConvertProtocolServiceType<ProtocolServiceType>();
			GlobalServiceUrls.DiscoverProtocolSettingsIfNeeded();
			ProtocolConnectionSettings result = null;
			GlobalServiceUrls.E15ProtocolsInternal.TryGetValue(key, out result);
			return result;
		}

		private static ServiceType ConvertHttpServiceType<Service>() where Service : HttpService
		{
			if (typeof(Service) == typeof(OwaService))
			{
				return ServiceType.OutlookWebAccess;
			}
			if (typeof(Service) == typeof(EcpService))
			{
				return ServiceType.ExchangeControlPanel;
			}
			if (typeof(Service) == typeof(WebServicesService))
			{
				return ServiceType.WebServices;
			}
			if (typeof(Service) == typeof(MobileSyncService))
			{
				return ServiceType.MobileSync;
			}
			if (typeof(Service) == typeof(OabService))
			{
				return ServiceType.OfflineAddressBook;
			}
			if (typeof(Service) == typeof(MapiHttpService))
			{
				return ServiceType.MapiHttp;
			}
			throw new NotSupportedException(typeof(Service).Name + " is not supported");
		}

		private static ServiceType ConvertProtocolServiceType<ProtocolServiceType>() where ProtocolServiceType : Service
		{
			if (typeof(ProtocolServiceType) == typeof(Pop3Service))
			{
				return ServiceType.Pop3;
			}
			if (typeof(ProtocolServiceType) == typeof(Imap4Service))
			{
				return ServiceType.Imap4;
			}
			if (typeof(ProtocolServiceType) == typeof(SmtpService))
			{
				return ServiceType.Smtp;
			}
			throw new NotSupportedException(typeof(ProtocolServiceType).Name + " is not supported");
		}

		private static void DiscoverUrlsIfNeeded()
		{
			if (GlobalServiceUrls.E15Services == null)
			{
				ExTraceGlobals.CafeTracer.TraceDebug<int>(0L, "[GlobalServiceUrls.DiscoverUrlsIfNeeded] VDir information is not yet cached, starting discovery of up to {0} Cafe servers", 20);
				lock (GlobalServiceUrls.urlLockRoot)
				{
					if (GlobalServiceUrls.E15Services == null)
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 284, "DiscoverUrlsIfNeeded", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\GlobalServiceUrls.cs");
						ADPagedReader<MiniServer> adpagedReader = topologyConfigurationSession.FindAllServersWithVersionNumber(Server.E15MinVersion, GlobalServiceUrls.CafeFilter, null);
						adpagedReader.PageSize = 20;
						int num = 0;
						string text = string.Empty;
						bool flag2 = false;
						bool flag3 = false;
						bool flag4 = false;
						bool flag5 = false;
						bool flag6 = false;
						bool flag7 = false;
						foreach (MiniServer miniServer in adpagedReader)
						{
							text = miniServer.Name;
							MiniVirtualDirectory[] array = topologyConfigurationSession.FindMiniVirtualDirectories(miniServer.Id);
							ExTraceGlobals.CafeTracer.TraceDebug<int, string, int>(0L, "[GlobalServiceUrls.DiscoverUrlsIfNeeded] Attempt {0}: Processing {1} and its {2} VDirs", num, text, array.Length);
							Dictionary<ServiceType, VDirInfo> e15Services = new Dictionary<ServiceType, VDirInfo>(6);
							flag2 = false;
							flag3 = false;
							flag4 = false;
							flag5 = false;
							flag6 = false;
							flag7 = false;
							bool flag8 = false;
							foreach (MiniVirtualDirectory miniVirtualDirectory in array)
							{
								if (miniVirtualDirectory.IsEcp)
								{
									GlobalServiceUrls.SelectUrl(ServiceType.ExchangeControlPanel, miniVirtualDirectory, e15Services, ref flag3);
								}
								else if (miniVirtualDirectory.IsOwa)
								{
									GlobalServiceUrls.SelectUrl(ServiceType.OutlookWebAccess, miniVirtualDirectory, e15Services, ref flag2);
								}
								else if (miniVirtualDirectory.IsWebServices)
								{
									GlobalServiceUrls.SelectUrl(ServiceType.WebServices, miniVirtualDirectory, e15Services, ref flag4);
								}
								else if (miniVirtualDirectory.IsMobile)
								{
									GlobalServiceUrls.SelectUrl(ServiceType.MobileSync, miniVirtualDirectory, e15Services, ref flag5);
								}
								else if (miniVirtualDirectory.IsOab)
								{
									GlobalServiceUrls.SelectUrl(ServiceType.OfflineAddressBook, miniVirtualDirectory, e15Services, ref flag6);
								}
								else if (miniVirtualDirectory.IsMapi)
								{
									GlobalServiceUrls.SelectUrl(ServiceType.MapiHttp, miniVirtualDirectory, e15Services, ref flag7);
								}
								else if (miniVirtualDirectory.IsRpcHttp)
								{
									GlobalServiceUrls.rpcHttpVdir = miniVirtualDirectory;
									flag8 = true;
								}
								if (flag2 && flag3 && flag4 && flag5 && flag6 && flag7 && flag8)
								{
									ExTraceGlobals.CafeTracer.TraceDebug(0L, "[GlobalServiceUrls.DiscoverUrlsIfNeeded] Successfully found all needed VDirs");
									GlobalServiceUrls.E15Services = e15Services;
									return;
								}
							}
							if (++num >= 20)
							{
								ExTraceGlobals.CafeTracer.TraceError(0L, "[GlobalServiceUrls.DiscoverUrlsIfNeeded] Retry limit reached, Could not find needed VDirs.");
								break;
							}
						}
						throw new VDirConfigurationMissingException(text, GlobalServiceUrls.IsDatacenter ? "ExternalUrl" : "InternalUrl", (flag2 ? string.Empty : "OWA,") + (flag3 ? string.Empty : "ECP,") + (flag4 ? string.Empty : "EWS"));
					}
					ExTraceGlobals.CafeTracer.TraceDebug(0L, "[GlobalServiceUrls.DiscoverUrlsIfNeeded] Another thread completed discovery first, nothing to do.");
				}
			}
		}

		private static void DiscoverProtocolSettingsIfNeeded()
		{
			if (GlobalServiceUrls.E15Protocols == null)
			{
				ExTraceGlobals.CafeTracer.TraceDebug<int>(0L, "[GlobalServiceUrls.DiscoverProtocolSettingsIfNeeded] Protocol settings information is not yet cached, starting discovery of up to {0} Cafe servers", 20);
				lock (GlobalServiceUrls.protocolLockRoot)
				{
					if (GlobalServiceUrls.E15Protocols != null)
					{
						ExTraceGlobals.CafeTracer.TraceDebug(0L, "[GlobalServiceUrls.DiscoverProtocolSettingsIfNeeded] Another thread completed discovery first, nothing to do.");
					}
					else if (GlobalServiceUrls.TryDiscoverProtocolSettings(true))
					{
						ExTraceGlobals.CafeTracer.TraceDebug(0L, "[GlobalServiceUrls.DiscoverProtocolSettingsIfNeeded] Completed the discovery within the local site.");
					}
					else
					{
						GlobalServiceUrls.TryDiscoverProtocolSettings(false);
					}
				}
			}
		}

		private static bool TryDiscoverProtocolSettings(bool useLocalSite)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 413, "TryDiscoverProtocolSettings", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\GlobalServiceUrls.cs");
			ADObjectId id = LocalSiteCache.LocalSite.Id;
			QueryFilter queryFilter = new ComparisonFilter(useLocalSite ? ComparisonOperator.Equal : ComparisonOperator.NotEqual, ServerSchema.ServerSite, id);
			QueryFilter additionalFilter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				GlobalServiceUrls.CafeFilter
			});
			ADPagedReader<MiniServer> adpagedReader = topologyConfigurationSession.FindAllServersWithVersionNumber(Server.E15MinVersion, additionalFilter, null);
			adpagedReader.PageSize = 20;
			int num = 0;
			string text = string.Empty;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			Dictionary<ServiceType, ProtocolConnectionSettings> dictionary = null;
			Dictionary<ServiceType, ProtocolConnectionSettings> dictionary2 = null;
			foreach (MiniServer miniServer in adpagedReader)
			{
				text = miniServer.Name;
				ADEmailTransport[] source = topologyConfigurationSession.Find<ADEmailTransport>(miniServer.Id, QueryScope.SubTree, null, null, 1000, null);
				IEnumerable<ADEmailTransport> enumerable = from adEmailTransport in source
				where adEmailTransport is Pop3AdConfiguration
				select adEmailTransport;
				IEnumerable<ADEmailTransport> enumerable2 = from adEmailTransport in source
				where adEmailTransport is Imap4AdConfiguration
				select adEmailTransport;
				ReceiveConnector[] array = topologyConfigurationSession.Find<ReceiveConnector>(miniServer.Id, QueryScope.SubTree, GlobalServiceUrls.ReceiveConnectorFilter, null, 1000, null);
				ExTraceGlobals.CafeTracer.TraceDebug<int, string, int>(0L, "[GlobalServiceUrls.TryDiscoverProtocolSettings] Attempt {0}: Processing {1} and its {2} connectors", num, text, array.Length);
				dictionary = new Dictionary<ServiceType, ProtocolConnectionSettings>(3);
				dictionary2 = new Dictionary<ServiceType, ProtocolConnectionSettings>(3);
				flag = false;
				flag2 = false;
				flag3 = false;
				flag4 = false;
				flag5 = false;
				foreach (ADEmailTransport ademailTransport in enumerable)
				{
					Pop3AdConfiguration popImapConfiguration = (Pop3AdConfiguration)ademailTransport;
					if (!flag && GlobalServiceUrls.TrySelectProtocolConnectionSettings(ServiceType.Pop3, popImapConfiguration, true, dictionary))
					{
						flag = true;
					}
					if (!flag2 && GlobalServiceUrls.TrySelectProtocolConnectionSettings(ServiceType.Pop3, popImapConfiguration, false, dictionary2))
					{
						flag2 = true;
					}
					if (flag && flag2)
					{
						break;
					}
				}
				foreach (ADEmailTransport ademailTransport2 in enumerable2)
				{
					Imap4AdConfiguration popImapConfiguration2 = (Imap4AdConfiguration)ademailTransport2;
					if (!flag3 && GlobalServiceUrls.TrySelectProtocolConnectionSettings(ServiceType.Imap4, popImapConfiguration2, true, dictionary))
					{
						flag3 = true;
					}
					if (!flag4 && GlobalServiceUrls.TrySelectProtocolConnectionSettings(ServiceType.Imap4, popImapConfiguration2, false, dictionary2))
					{
						flag4 = true;
					}
					if (flag3 && flag4)
					{
						break;
					}
				}
				ProtocolConnectionSettings protocolConnectionSettings = null;
				foreach (ReceiveConnector receiveConnector in array)
				{
					Hostname hostname;
					if (!Hostname.TryParse(receiveConnector.ServiceDiscoveryFqdn, out hostname) && !Hostname.TryParse(receiveConnector.Fqdn, out hostname))
					{
						ExTraceGlobals.CafeTracer.TraceWarning<ADObjectId>(0L, "[GlobalServiceUrls.TryDiscoverProtocolSettings] Smtp connector {0} has no valid ServiceDiscoveryFqdn or Fqdn", receiveConnector.Id);
					}
					else
					{
						int port = receiveConnector.Bindings[0].Port;
						bool flag6 = (receiveConnector.AuthMechanism & (AuthMechanisms.Tls | AuthMechanisms.BasicAuthRequireTLS)) != AuthMechanisms.None;
						if (flag6)
						{
							protocolConnectionSettings = new ProtocolConnectionSettings(hostname, port, new EncryptionType?(EncryptionType.TLS));
							break;
						}
						if (protocolConnectionSettings == null)
						{
							protocolConnectionSettings = new ProtocolConnectionSettings(hostname, port, null);
						}
					}
				}
				if (protocolConnectionSettings != null)
				{
					dictionary[ServiceType.Smtp] = protocolConnectionSettings;
					dictionary2[ServiceType.Smtp] = protocolConnectionSettings;
					flag5 = true;
				}
				if (flag2 && flag4 && flag5)
				{
					ExTraceGlobals.CafeTracer.TraceDebug(0L, "[GlobalServiceUrls.TryDiscoverProtocolSettings] Successfully found all needed internal protocol settings");
					GlobalServiceUrls.E15ProtocolsInternal = dictionary2;
				}
				if (flag && flag3 && flag5)
				{
					ExTraceGlobals.CafeTracer.TraceDebug(0L, "[GlobalServiceUrls.TryDiscoverProtocolSettings] Successfully found all needed external protocol settings");
					GlobalServiceUrls.E15Protocols = dictionary;
					return true;
				}
				if (++num >= 20)
				{
					ExTraceGlobals.CafeTracer.TraceError(0L, "[GlobalServiceUrls.TryDiscoverProtocolSettings] Retry limit reached, Could not find all of the needed Protocol configurations.");
					break;
				}
			}
			if (!GlobalServiceUrls.IsDatacenter)
			{
				if (dictionary != null && dictionary.Count > 0)
				{
					ExTraceGlobals.CafeTracer.TraceWarning(0L, "[GlobalServiceUrls.TryDiscoverProtocolSettings] Found only some of the needed external protocol settings. Missing:" + (flag ? string.Empty : "POP3,") + (flag3 ? string.Empty : "IMAP4,") + (flag5 ? string.Empty : "SMTP"));
					GlobalServiceUrls.E15Protocols = dictionary;
				}
				if (dictionary2 != null && dictionary2.Count > 0)
				{
					ExTraceGlobals.CafeTracer.TraceWarning(0L, "[GlobalServiceUrls.TryDiscoverProtocolSettings] Found only some of the needed internal protocol settings. Missing:" + (flag2 ? string.Empty : "POP3,") + (flag4 ? string.Empty : "IMAP4,") + (flag5 ? string.Empty : "SMTP"));
					GlobalServiceUrls.E15ProtocolsInternal = dictionary2;
				}
			}
			if (!useLocalSite && GlobalServiceUrls.E15Protocols == null)
			{
				if (GlobalServiceUrls.IsDatacenter)
				{
					throw new ProtocolConfigurationMissingException(text, "ExternalSettings");
				}
				GlobalServiceUrls.E15Protocols = new Dictionary<ServiceType, ProtocolConnectionSettings>(0);
			}
			if (!useLocalSite && GlobalServiceUrls.E15ProtocolsInternal == null)
			{
				if (GlobalServiceUrls.IsDatacenter)
				{
					throw new ProtocolConfigurationMissingException(text, "InternalSettings");
				}
				GlobalServiceUrls.E15ProtocolsInternal = new Dictionary<ServiceType, ProtocolConnectionSettings>(0);
			}
			return false;
		}

		private static void SelectUrl(ServiceType serviceType, MiniVirtualDirectory vdir, Dictionary<ServiceType, VDirInfo> e15Services, ref bool found)
		{
			if (found)
			{
				return;
			}
			Uri uri = vdir.ExternalUrl;
			if (uri == null && !GlobalServiceUrls.IsDatacenter)
			{
				uri = vdir.InternalUrl;
			}
			if (uri != null)
			{
				found = true;
				VDirInfo value = new VDirInfo(uri);
				e15Services[serviceType] = value;
				ExTraceGlobals.CafeTracer.TraceDebug<Uri>(0L, "[GlobalServiceUrls.SelectUrl] Found URL: {0}", uri);
			}
		}

		private static bool TrySelectProtocolConnectionSettings(ServiceType serviceType, PopImapAdConfiguration popImapConfiguration, bool useExternalPopImapSettings, Dictionary<ServiceType, ProtocolConnectionSettings> e15Protocols)
		{
			IList<ProtocolConnectionSettings> list = useExternalPopImapSettings ? popImapConfiguration.ExternalConnectionSettings : popImapConfiguration.InternalConnectionSettings;
			ProtocolConnectionSettings protocolConnectionSettings = null;
			foreach (ProtocolConnectionSettings protocolConnectionSettings2 in list)
			{
				if (protocolConnectionSettings == null)
				{
					protocolConnectionSettings = protocolConnectionSettings2;
				}
				else
				{
					if (protocolConnectionSettings2.EncryptionType == EncryptionType.SSL)
					{
						protocolConnectionSettings = protocolConnectionSettings2;
						break;
					}
					if (protocolConnectionSettings2.EncryptionType == EncryptionType.TLS && protocolConnectionSettings.EncryptionType == null)
					{
						protocolConnectionSettings = protocolConnectionSettings2;
					}
				}
			}
			if (protocolConnectionSettings != null)
			{
				e15Protocols[serviceType] = protocolConnectionSettings;
				ExTraceGlobals.CafeTracer.TraceDebug<string, ProtocolConnectionSettings>(0L, "[GlobalServiceUrls.TrySelectProtocolConnectionSettings] Found {0} settings {1}", typeof(ServiceType).Name, protocolConnectionSettings);
				return true;
			}
			return false;
		}

		private const int MaxServers = 20;

		private const int BackEndServicesHttpPort = 444;

		private const int FrontEndServicesHttpPort = 443;

		private static readonly int E15Version = new ServerVersion(15, 0, 0, 0).ToInt();

		private static Dictionary<ServiceType, VDirInfo> E15Services;

		private static readonly QueryFilter CafeFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.IsCafeServer, true),
			new ComparisonFilter(ComparisonOperator.Equal, ActiveDirectoryServerSchema.IsOutOfService, false)
		});

		private static readonly QueryFilter ReceiveConnectorFilter = new ComparisonFilter(ComparisonOperator.Equal, ReceiveConnectorSchema.AdvertiseClientSettings, true);

		private static readonly bool IsDatacenter = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;

		private static readonly object urlLockRoot = new object();

		private static readonly object protocolLockRoot = new object();

		private static Dictionary<ServiceType, ProtocolConnectionSettings> E15Protocols;

		private static Dictionary<ServiceType, ProtocolConnectionSettings> E15ProtocolsInternal;

		private static MiniVirtualDirectory rpcHttpVdir;
	}
}
