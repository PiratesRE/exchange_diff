using System;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class HttpProxyGlobals
	{
		public static bool IsPartnerHostedOnly
		{
			get
			{
				return HttpProxyGlobals.isPartnerHostedOnly.Member;
			}
		}

		public static ProtocolType ProtocolType
		{
			get
			{
				return HttpProxyGlobals.LazyProtocolType.Member;
			}
		}

		public static bool OnlyProxySecureConnections
		{
			get
			{
				return HttpProxySettings.OnlyProxySecureConnectionsAppSetting.Value;
			}
		}

		public static readonly LazyMember<string> LocalMachineFqdn = new LazyMember<string>(() => LocalServer.GetServer().Fqdn.ToUpper());

		public static readonly LazyMember<string> LocalMachineForest = new LazyMember<string>(() => LocalServer.GetServer().Domain.ToUpper());

		private static readonly LazyMember<ProtocolType> LazyProtocolType = new LazyMember<ProtocolType>(delegate()
		{
			if (HttpProxySettings.ProtocolTypeAppSetting.Value == ProtocolType.Owa && HttpRuntime.AppDomainAppVirtualPath.ToLower().Contains("calendar"))
			{
				return ProtocolType.OwaCalendar;
			}
			return HttpProxySettings.ProtocolTypeAppSetting.Value;
		});

		private static LazyMember<bool> isPartnerHostedOnly = new LazyMember<bool>(delegate()
		{
			try
			{
				if (Datacenter.IsPartnerHostedOnly(true))
				{
					return true;
				}
			}
			catch (CannotDetermineExchangeModeException)
			{
			}
			return false;
		});
	}
}
