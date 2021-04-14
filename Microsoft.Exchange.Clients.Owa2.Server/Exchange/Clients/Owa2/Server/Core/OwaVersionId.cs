using System;
using System.Timers;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaVersionId
	{
		public static string Current
		{
			get
			{
				return OwaVersionId.current;
			}
		}

		public static string VNext
		{
			get
			{
				return OwaVersionId.vNext;
			}
		}

		public static bool Matches(string clientVersionId, bool isVNext)
		{
			if (string.IsNullOrEmpty(clientVersionId))
			{
				ExTraceGlobals.CommonAlgorithmTracer.Information(0L, "[OwavVersionId::MatchesClientVersionString Ignoring Null or Missing ClientVersionId");
				return true;
			}
			string text = isVNext ? OwaVersionId.VNext : OwaVersionId.Current;
			string[] array = text.Split(new char[]
			{
				'.'
			});
			if (array.Length < 3)
			{
				ExTraceGlobals.CommonAlgorithmTracer.Information<string>(0L, "[OwavVersionId::MatchesClientVersionString Ignoring Invalid Server ID={0}", text);
				return true;
			}
			string[] array2 = clientVersionId.Split(new char[]
			{
				'.'
			});
			if (array2.Length < 3)
			{
				ExTraceGlobals.CommonAlgorithmTracer.Information<string>(0L, "[OwavVersionId::MatchesClientVersionString Ignoring Invalid Client ID={0}", clientVersionId);
				return true;
			}
			for (int i = 0; i < 3; i++)
			{
				if (!array[i].Equals(array2[i], StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceWarning<string, string>(0L, "[OwavVersionId::MatchesClientVersionString returning FALSE.  Client ID={0}, Server ID={1}", clientVersionId, text);
					return false;
				}
			}
			ExTraceGlobals.CommonAlgorithmTracer.Information<string, string>(0L, "[OwavVersionId::MatchesClientVersionString returning TRUE.  Client ID={0}, Server ID={1}", clientVersionId, text);
			return true;
		}

		public static bool Supports(string clientOwsVersionString, bool isVNext)
		{
			if (string.IsNullOrEmpty(clientOwsVersionString))
			{
				ExTraceGlobals.CommonAlgorithmTracer.Information<bool>(0L, "[OwaVersionId::Supports Ignoring Null or Missing clientOwsVersionString, IsVNext={0}", isVNext);
				return true;
			}
			ExchangeVersionType exchangeVersionType;
			if (!Enum.TryParse<ExchangeVersionType>(clientOwsVersionString, out exchangeVersionType))
			{
				ExTraceGlobals.CommonAlgorithmTracer.Information<string, bool>(0L, "[OwaVersionId::Supports returning FALSE. Unrecognized clientOwsVersion={0}, IsVNext={1}", clientOwsVersionString, isVNext);
				return false;
			}
			if (exchangeVersionType < ExchangeVersionType.V2_6)
			{
				ExTraceGlobals.CommonAlgorithmTracer.Information<string, ExchangeVersionType, bool>(0L, "[OwavVersionId::Supports returning FALSE.  Client ver={0}, Server min supported ver={1}, IsVNext={2}", clientOwsVersionString, ExchangeVersionType.V2_6, isVNext);
				return false;
			}
			if (!isVNext && exchangeVersionType > ExchangeVersion.Latest.Version)
			{
				ExTraceGlobals.CommonAlgorithmTracer.Information<string, ExchangeVersionType, bool>(0L, "[OwavVersionId::Supports returning FALSE.  Client ver={0}, Server latest ver={1}, IsVNext={2}", clientOwsVersionString, ExchangeVersion.Latest.Version, isVNext);
				return false;
			}
			ExTraceGlobals.CommonAlgorithmTracer.Information<string, ExchangeVersionType, bool>(0L, "[OwavVersionId::Supports returning TRUE.  Client ver={0}, Server min supported ver={1}, IsVNext={2}", clientOwsVersionString, ExchangeVersionType.V2_6, isVNext);
			return true;
		}

		public static void InitializeOwaVersionReadingTimer()
		{
			OwaVersionId.owaVersionReader = new Timer(Globals.OwaVersionReadingInterval);
			OwaVersionId.owaVersionReader.Elapsed += OwaVersionId.UpdateCurrentOwaVersion;
			OwaVersionId.owaVersionReader.Enabled = true;
		}

		private static void UpdateCurrentOwaVersion(object source, ElapsedEventArgs e)
		{
			OwaRegistryKeys.UpdateOwaSetupVersionsCache();
			OwaVersionId.current = OwaVersionId.GetInstalledOwaVersion();
			OwaVersionId.vNext = OwaVersionId.GetInstalledNextOwaVersion();
		}

		private static string GetInstalledOwaVersion()
		{
			string text;
			if (Globals.IsPreCheckinApp)
			{
				text = Globals.ApplicationVersion;
			}
			else
			{
				text = OwaRegistryKeys.InstalledOwaVersion;
			}
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "OwaVersionId.Current: No registry setting for OwaVersion found.");
				throw new ApplicationException("No registry setting for OwaVersion found");
			}
			return text;
		}

		private static string GetInstalledNextOwaVersion()
		{
			string text;
			if (Globals.IsPreCheckinApp)
			{
				text = Globals.ApplicationVersion;
			}
			else
			{
				text = OwaRegistryKeys.InstalledNextOwaVersion;
			}
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "OwaVersionId.VNext: No registry setting for NextOwaVersion found.");
				throw new ApplicationException("No registry setting for NextOwaVersion found");
			}
			return text;
		}

		public const string InboundVersionIdHeaderName = "X-OWA-VersionId";

		public const string InboundClientOWSVersionHeaderName = "X-OWA-ClientOWSVersion";

		public static double OwaVersionReadingInterval = (double)TimeSpan.FromMinutes(5.0).Milliseconds;

		private static string current = OwaVersionId.GetInstalledOwaVersion();

		private static string vNext = OwaVersionId.GetInstalledNextOwaVersion();

		private static Timer owaVersionReader;
	}
}
