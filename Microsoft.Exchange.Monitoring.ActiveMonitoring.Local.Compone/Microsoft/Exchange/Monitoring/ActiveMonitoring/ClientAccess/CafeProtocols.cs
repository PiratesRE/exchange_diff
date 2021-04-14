using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	internal static class CafeProtocols
	{
		internal static IEnumerable<ADVirtualDirectory> VirtualDirectories { get; set; }

		internal static ProtocolDescriptor Get(string appPool)
		{
			ProtocolDescriptor protocolDescriptor = CafeProtocols.Protocols.SingleOrDefault((ProtocolDescriptor p) => p.AppPool == appPool);
			if (protocolDescriptor != null)
			{
				return protocolDescriptor;
			}
			throw new InvalidOperationException("Unknown app pool name: " + appPool);
		}

		internal static ProtocolDescriptor Get(HttpProtocol httpProtocol)
		{
			ProtocolDescriptor protocolDescriptor = CafeProtocols.Protocols.SingleOrDefault((ProtocolDescriptor p) => p.HttpProtocol == httpProtocol);
			if (protocolDescriptor != null)
			{
				return protocolDescriptor;
			}
			throw new ApplicationException(string.Format("No descriptor found for a well-defined HTTP Protocol: {0}. It should be there!", httpProtocol));
		}

		// Note: this type is marked as 'beforefieldinit'.
		static CafeProtocols()
		{
			ProtocolDescriptor[] array = new ProtocolDescriptor[13];
			ProtocolDescriptor[] array2 = array;
			int num = 0;
			ProtocolDescriptor protocolDescriptor = new ProtocolDescriptor();
			protocolDescriptor.HttpProtocol = HttpProtocol.AutoDiscover;
			protocolDescriptor.AppPool = "MSExchangeAutodiscoverAppPool";
			protocolDescriptor.VirtualDirectory = "AutoDiscover";
			protocolDescriptor.HealthSet = ExchangeComponent.AutodiscoverProxy;
			protocolDescriptor.DeferAlertOnCafeWideFailure = true;
			ProtocolDescriptor protocolDescriptor2 = protocolDescriptor;
			AuthenticationMethod[] authPreferenceOrderDatacenter = new AuthenticationMethod[1];
			protocolDescriptor2.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter;
			protocolDescriptor.ProtocolPriority = 1;
			array2[num] = protocolDescriptor;
			ProtocolDescriptor[] array3 = array;
			int num2 = 1;
			ProtocolDescriptor protocolDescriptor3 = new ProtocolDescriptor();
			protocolDescriptor3.HttpProtocol = HttpProtocol.EAS;
			protocolDescriptor3.AppPool = "MSExchangeSyncAppPool";
			protocolDescriptor3.VirtualDirectory = "Microsoft-Server-ActiveSync";
			protocolDescriptor3.HealthSet = ExchangeComponent.ActiveSyncProxy;
			protocolDescriptor3.DeferAlertOnCafeWideFailure = true;
			ProtocolDescriptor protocolDescriptor4 = protocolDescriptor3;
			AuthenticationMethod[] authPreferenceOrderDatacenter2 = new AuthenticationMethod[1];
			protocolDescriptor4.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter2;
			protocolDescriptor3.ProtocolPriority = 0;
			array3[num2] = protocolDescriptor3;
			ProtocolDescriptor[] array4 = array;
			int num3 = 2;
			ProtocolDescriptor protocolDescriptor5 = new ProtocolDescriptor();
			protocolDescriptor5.HttpProtocol = HttpProtocol.ECP;
			protocolDescriptor5.AppPool = "MSExchangeECPAppPool";
			protocolDescriptor5.VirtualDirectory = "ecp";
			protocolDescriptor5.IsRedirectOK = true;
			protocolDescriptor5.HealthSet = ExchangeComponent.EcpProxy;
			protocolDescriptor5.DeferAlertOnCafeWideFailure = true;
			protocolDescriptor5.AuthPreferenceOrderEnterprise = new AuthenticationMethod[0];
			ProtocolDescriptor protocolDescriptor6 = protocolDescriptor5;
			AuthenticationMethod[] authPreferenceOrderDatacenter3 = new AuthenticationMethod[1];
			protocolDescriptor6.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter3;
			protocolDescriptor5.ProtocolPriority = 2;
			array4[num3] = protocolDescriptor5;
			ProtocolDescriptor[] array5 = array;
			int num4 = 3;
			ProtocolDescriptor protocolDescriptor7 = new ProtocolDescriptor();
			protocolDescriptor7.HttpProtocol = HttpProtocol.EWS;
			protocolDescriptor7.AppPool = "MSExchangeServicesAppPool";
			protocolDescriptor7.VirtualDirectory = "ews";
			protocolDescriptor7.HealthSet = ExchangeComponent.EwsProxy;
			protocolDescriptor7.DeferAlertOnCafeWideFailure = false;
			ProtocolDescriptor protocolDescriptor8 = protocolDescriptor7;
			AuthenticationMethod[] authPreferenceOrderDatacenter4 = new AuthenticationMethod[1];
			protocolDescriptor8.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter4;
			protocolDescriptor7.ProtocolPriority = 0;
			array5[num4] = protocolDescriptor7;
			ProtocolDescriptor[] array6 = array;
			int num5 = 4;
			ProtocolDescriptor protocolDescriptor9 = new ProtocolDescriptor();
			protocolDescriptor9.HttpProtocol = HttpProtocol.Mapi;
			protocolDescriptor9.AppPool = "MSExchangeMapiFrontEndAppPool";
			protocolDescriptor9.VirtualDirectory = "mapi";
			protocolDescriptor9.HealthSet = ExchangeComponent.OutlookMapiProxy;
			protocolDescriptor9.DeferAlertOnCafeWideFailure = true;
			ProtocolDescriptor protocolDescriptor10 = protocolDescriptor9;
			AuthenticationMethod[] authPreferenceOrderDatacenter5 = new AuthenticationMethod[1];
			protocolDescriptor10.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter5;
			protocolDescriptor9.ProtocolPriority = 0;
			array6[num5] = protocolDescriptor9;
			ProtocolDescriptor[] array7 = array;
			int num6 = 5;
			ProtocolDescriptor protocolDescriptor11 = new ProtocolDescriptor();
			protocolDescriptor11.HttpProtocol = HttpProtocol.OAB;
			protocolDescriptor11.AppPool = "MSExchangeOABAppPool";
			protocolDescriptor11.VirtualDirectory = "OAB";
			protocolDescriptor11.HealthSet = ExchangeComponent.OabProxy;
			protocolDescriptor11.DeferAlertOnCafeWideFailure = true;
			ProtocolDescriptor protocolDescriptor12 = protocolDescriptor11;
			AuthenticationMethod[] authPreferenceOrderDatacenter6 = new AuthenticationMethod[1];
			protocolDescriptor12.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter6;
			protocolDescriptor11.ProtocolPriority = 2;
			array7[num6] = protocolDescriptor11;
			ProtocolDescriptor[] array8 = array;
			int num7 = 6;
			ProtocolDescriptor protocolDescriptor13 = new ProtocolDescriptor();
			protocolDescriptor13.HttpProtocol = HttpProtocol.OWA;
			protocolDescriptor13.AppPool = "MSExchangeOWAAppPool";
			protocolDescriptor13.VirtualDirectory = "OWA";
			protocolDescriptor13.IsRedirectOK = true;
			protocolDescriptor13.HealthSet = ExchangeComponent.OwaProxy;
			protocolDescriptor13.DeferAlertOnCafeWideFailure = true;
			protocolDescriptor13.AuthPreferenceOrderEnterprise = new AuthenticationMethod[]
			{
				AuthenticationMethod.Fba
			};
			ProtocolDescriptor protocolDescriptor14 = protocolDescriptor13;
			AuthenticationMethod[] authPreferenceOrderDatacenter7 = new AuthenticationMethod[1];
			protocolDescriptor14.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter7;
			protocolDescriptor13.ProtocolPriority = 0;
			array8[num7] = protocolDescriptor13;
			array[7] = new ProtocolDescriptor
			{
				HttpProtocol = HttpProtocol.OWACalendar,
				AppPool = "MSExchangeOWACalendarAppPool",
				VirtualDirectory = "OWA/Calendar",
				HealthSet = ExchangeComponent.OwaProxy,
				DeferAlertOnCafeWideFailure = true,
				AuthPreferenceOrderDatacenter = new AuthenticationMethod[]
				{
					AuthenticationMethod.Misconfigured
				},
				ProtocolPriority = 2
			};
			array[8] = new ProtocolDescriptor
			{
				HttpProtocol = HttpProtocol.PowerShell,
				AppPool = "MSExchangePowerShellFrontEndAppPool",
				VirtualDirectory = "PowerShell",
				HealthSet = ExchangeComponent.RpsProxy,
				DeferAlertOnCafeWideFailure = true,
				ProtocolPriority = 1
			};
			ProtocolDescriptor[] array9 = array;
			int num8 = 9;
			ProtocolDescriptor protocolDescriptor15 = new ProtocolDescriptor();
			protocolDescriptor15.HttpProtocol = HttpProtocol.PowerShellLiveID;
			protocolDescriptor15.AppPool = "MSExchangePowerShellLiveIDFrontEndAppPool";
			protocolDescriptor15.VirtualDirectory = "PowerShell-LiveID";
			protocolDescriptor15.HealthSet = ExchangeComponent.RpsProxy;
			protocolDescriptor15.DeferAlertOnCafeWideFailure = true;
			ProtocolDescriptor protocolDescriptor16 = protocolDescriptor15;
			AuthenticationMethod[] authPreferenceOrderDatacenter8 = new AuthenticationMethod[1];
			protocolDescriptor16.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter8;
			protocolDescriptor15.ProtocolPriority = 2;
			array9[num8] = protocolDescriptor15;
			ProtocolDescriptor[] array10 = array;
			int num9 = 10;
			ProtocolDescriptor protocolDescriptor17 = new ProtocolDescriptor();
			protocolDescriptor17.HttpProtocol = HttpProtocol.Reporting;
			protocolDescriptor17.AppPool = "MSExchangeReportingWebServiceAppPool";
			protocolDescriptor17.VirtualDirectory = "ecp/ReportingWebService";
			protocolDescriptor17.IsRedirectOK = true;
			protocolDescriptor17.HealthSet = ExchangeComponent.RwsProxy;
			protocolDescriptor17.DeferAlertOnCafeWideFailure = true;
			ProtocolDescriptor protocolDescriptor18 = protocolDescriptor17;
			AuthenticationMethod[] authPreferenceOrderDatacenter9 = new AuthenticationMethod[1];
			protocolDescriptor18.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter9;
			protocolDescriptor17.LogFolderName = "ReportingWebService";
			protocolDescriptor17.ProtocolPriority = 2;
			array10[num9] = protocolDescriptor17;
			ProtocolDescriptor[] array11 = array;
			int num10 = 11;
			ProtocolDescriptor protocolDescriptor19 = new ProtocolDescriptor();
			protocolDescriptor19.HttpProtocol = HttpProtocol.RPC;
			protocolDescriptor19.AppPool = "MSExchangeRpcProxyFrontEndAppPool";
			protocolDescriptor19.VirtualDirectory = "RPC";
			protocolDescriptor19.HealthSet = ExchangeComponent.OutlookProxy;
			protocolDescriptor19.DeferAlertOnCafeWideFailure = true;
			ProtocolDescriptor protocolDescriptor20 = protocolDescriptor19;
			AuthenticationMethod[] authPreferenceOrderDatacenter10 = new AuthenticationMethod[1];
			protocolDescriptor20.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter10;
			protocolDescriptor19.LogFolderName = "RpcHttp";
			protocolDescriptor19.ProtocolPriority = 0;
			array11[num10] = protocolDescriptor19;
			ProtocolDescriptor[] array12 = array;
			int num11 = 12;
			ProtocolDescriptor protocolDescriptor21 = new ProtocolDescriptor();
			protocolDescriptor21.HttpProtocol = HttpProtocol.XRop;
			protocolDescriptor21.AppPool = "MSExchangeXRopAppPool";
			protocolDescriptor21.VirtualDirectory = "xrop";
			protocolDescriptor21.HealthSet = ExchangeComponent.XropProxy;
			protocolDescriptor21.DeferAlertOnCafeWideFailure = true;
			ProtocolDescriptor protocolDescriptor22 = protocolDescriptor21;
			AuthenticationMethod[] authPreferenceOrderDatacenter11 = new AuthenticationMethod[1];
			protocolDescriptor22.AuthPreferenceOrderDatacenter = authPreferenceOrderDatacenter11;
			protocolDescriptor21.ProtocolPriority = 0;
			array12[num11] = protocolDescriptor21;
			CafeProtocols.Protocols = array;
		}

		internal static readonly ProtocolDescriptor[] Protocols;
	}
}
