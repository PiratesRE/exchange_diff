using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FrontEndLocator : IFrontEndLocator
	{
		public static Uri GetDatacenterMapiHttpUrl()
		{
			return GlobalServiceUrls.GetExternalUrl<MapiHttpService>();
		}

		public static MiniVirtualDirectory GetDatacenterRpcHttpVdir()
		{
			return GlobalServiceUrls.GetRpcHttpVdir();
		}

		public static Uri GetDatacenterFrontEndWebServicesUrl()
		{
			return GlobalServiceUrls.GetExternalUrl<WebServicesService>();
		}

		public static Uri GetDatacenterFrontEndOwaUrl()
		{
			return GlobalServiceUrls.GetExternalUrl<OwaService>();
		}

		public static Uri GetDatacenterFrontEndEcpUrl()
		{
			return GlobalServiceUrls.GetExternalUrl<EcpService>();
		}

		public static Uri GetDatacenterFrontEndEasUrl()
		{
			return GlobalServiceUrls.GetExternalUrl<MobileSyncService>();
		}

		public static Uri GetDatacenterFrontEndOabUrl()
		{
			return GlobalServiceUrls.GetExternalUrl<OabService>();
		}

		public static Uri GetFrontEndWebServicesUrl(IExchangePrincipal exchangePrincipal)
		{
			return FrontEndLocator.GetFrontEndHttpServiceUrl<WebServicesService>(exchangePrincipal);
		}

		public static Uri GetFrontEndWebServicesUrl(string serverFqdn)
		{
			return GlobalServiceUrls.GetExternalUrl<WebServicesService>(serverFqdn);
		}

		public static Uri GetFrontEndOwaUrl(IExchangePrincipal exchangePrincipal)
		{
			return FrontEndLocator.GetFrontEndHttpServiceUrl<OwaService>(exchangePrincipal);
		}

		public static Uri GetFrontEndEcpUrl(IExchangePrincipal exchangePrincipal)
		{
			return FrontEndLocator.GetFrontEndHttpServiceUrl<EcpService>(exchangePrincipal);
		}

		public static Uri GetFrontEndEasUrl(IExchangePrincipal exchangePrincipal)
		{
			return FrontEndLocator.GetFrontEndHttpServiceUrl<MobileSyncService>(exchangePrincipal);
		}

		public static ProtocolConnectionSettings GetFrontEndPop3SettingsForLocalServer()
		{
			return GlobalServiceUrls.GetExternalProtocolSettingsForLocalServer<Pop3Service>();
		}

		public static ProtocolConnectionSettings GetFrontEndImap4SettingsForLocalServer()
		{
			return GlobalServiceUrls.GetExternalProtocolSettingsForLocalServer<Imap4Service>();
		}

		public static ProtocolConnectionSettings GetFrontEndSmtpSettingsForLocalServer()
		{
			return GlobalServiceUrls.GetExternalProtocolSettingsForLocalServer<SmtpService>();
		}

		public static ProtocolConnectionSettings GetInternalPop3SettingsForLocalServer()
		{
			return GlobalServiceUrls.GetInternalProtocolSettingsForLocalServer<Pop3Service>();
		}

		public static ProtocolConnectionSettings GetInternalImap4SettingsForLocalServer()
		{
			return GlobalServiceUrls.GetInternalProtocolSettingsForLocalServer<Imap4Service>();
		}

		public static ProtocolConnectionSettings GetInternalSmtpSettingsForLocalServer()
		{
			return GlobalServiceUrls.GetInternalProtocolSettingsForLocalServer<SmtpService>();
		}

		public Uri GetWebServicesUrl(IExchangePrincipal exchangePrincipal)
		{
			return FrontEndLocator.GetFrontEndWebServicesUrl(exchangePrincipal);
		}

		public Uri GetOwaUrl(IExchangePrincipal exchangePrincipal)
		{
			return FrontEndLocator.GetFrontEndOwaUrl(exchangePrincipal);
		}

		private static Uri GetFrontEndHttpServiceUrl<ServiceType>(IExchangePrincipal exchangePrincipal) where ServiceType : HttpService
		{
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			if (exchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion && FrontEndLocator.IsDatacenter)
			{
				return GlobalServiceUrls.GetExternalUrl<ServiceType>();
			}
			ServiceTopology serviceTopology = FrontEndLocator.IsDatacenter ? ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\FrontEndLocator.cs", "GetFrontEndHttpServiceUrl", 276) : ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\FrontEndLocator.cs", "GetFrontEndHttpServiceUrl", 276);
			ServerVersion serverVersion = new ServerVersion(exchangePrincipal.MailboxInfo.Location.ServerVersion);
			int majorversion = serverVersion.Major;
			IList<ServiceType> services = serviceTopology.FindAll<ServiceType>(exchangePrincipal, ClientAccessType.External, (ServiceType service) => new ServerVersion(service.ServerVersionNumber).Major == majorversion, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\FrontEndLocator.cs", "GetFrontEndHttpServiceUrl", 281);
			Uri uri = FrontEndLocator.FindServiceInList<ServiceType>(services);
			if (uri == null)
			{
				services = serviceTopology.FindAll<ServiceType>(exchangePrincipal, ClientAccessType.Internal, (ServiceType service) => new ServerVersion(service.ServerVersionNumber).Major == majorversion, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\FrontEndLocator.cs", "GetFrontEndHttpServiceUrl", 285);
				uri = FrontEndLocator.FindServiceInList<ServiceType>(services);
			}
			if (uri != null)
			{
				ExTraceGlobals.CafeTracer.TraceDebug<string>(0L, "[FrontEndLocator.GetFrontEndHttpServiceUrl] Found HTTP service for the specified back end server {0}.", exchangePrincipal.MailboxInfo.Location.ServerFqdn);
				return uri;
			}
			throw new ServerNotFoundException("Unable to find proper HTTP service.");
		}

		private static Uri FindServiceInList<ServiceType>(IList<ServiceType> services) where ServiceType : HttpService
		{
			foreach (ServiceType serviceType in services)
			{
				if (serviceType != null)
				{
					return serviceType.Url;
				}
			}
			return null;
		}

		private static bool IsDatacenter
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			}
		}
	}
}
