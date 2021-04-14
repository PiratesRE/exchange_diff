using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class ExchangeVersionDeterminer
	{
		public static ExchangeVersionDeterminer.ExchangeServerVersion LocalServerVersion
		{
			get
			{
				return ExchangeVersionDeterminer.localServerVersion.Member;
			}
		}

		public static bool MatchesLocalServerVersion(int exchangeServerVersionInt)
		{
			return exchangeServerVersionInt == 0 || ExchangeVersionDeterminer.LocalServerVersion == ExchangeVersionDeterminer.IntToServerVersion(exchangeServerVersionInt);
		}

		public static bool IsCurrentOrOlderThanLocalServer(int exchangeServerVersionInt)
		{
			ExchangeVersionDeterminer.ExchangeServerVersion exchangeServerVersion = ExchangeVersionDeterminer.IntToServerVersion(exchangeServerVersionInt);
			return exchangeServerVersion != ExchangeVersionDeterminer.ExchangeServerVersion.Legacy && exchangeServerVersion <= ExchangeVersionDeterminer.LocalServerVersion;
		}

		public static bool ServerSupportsRequestVersion(int exchangeServerVersionInt)
		{
			if (exchangeServerVersionInt == 0)
			{
				return true;
			}
			switch (ExchangeVersionDeterminer.IntToServerVersion(exchangeServerVersionInt))
			{
			case ExchangeVersionDeterminer.ExchangeServerVersion.Legacy:
				return false;
			case ExchangeVersionDeterminer.ExchangeServerVersion.E12:
				return ExchangeVersion.Current.Version <= ExchangeVersionType.Exchange2007_SP1;
			case ExchangeVersionDeterminer.ExchangeServerVersion.E14:
				return ExchangeVersion.Current.Version <= ExchangeVersionType.Exchange2010_SP2;
			case ExchangeVersionDeterminer.ExchangeServerVersion.E15:
				return true;
			default:
				return false;
			}
		}

		public static bool AreSameVersion(int exchangeServerVersionInt1, int exchangeServerVersionInt2)
		{
			return ExchangeVersionDeterminer.IntToServerVersion(exchangeServerVersionInt1) == ExchangeVersionDeterminer.IntToServerVersion(exchangeServerVersionInt2);
		}

		public static bool AreSameVersion(ExchangePrincipal principal1, ExchangePrincipal principal2)
		{
			return ExchangeVersionDeterminer.AreSameVersion(principal1.MailboxInfo.Location.ServerVersion, principal2.MailboxInfo.Location.ServerVersion);
		}

		public static bool AllowCrossVersionAccess(int exchangeServerVersionInt)
		{
			return ExchangeVersionDeterminer.LocalServerVersion == ExchangeVersionDeterminer.ExchangeServerVersion.E14 && ExchangeVersionDeterminer.IntToServerVersion(exchangeServerVersionInt) == ExchangeVersionDeterminer.ExchangeServerVersion.E15;
		}

		public static ExchangeVersionDeterminer.ExchangeServerVersion IntToServerVersion(int exchangeServerVersionInt)
		{
			if (exchangeServerVersionInt < 0)
			{
				throw new ArgumentException("Version integer cannot be negative", "exchangeServerVersionInt");
			}
			if (exchangeServerVersionInt >= Server.E2007MinVersion && exchangeServerVersionInt < Server.E14MinVersion)
			{
				return ExchangeVersionDeterminer.ExchangeServerVersion.E12;
			}
			if (exchangeServerVersionInt >= Server.E14MinVersion && exchangeServerVersionInt < Server.E15MinVersion)
			{
				return ExchangeVersionDeterminer.ExchangeServerVersion.E14;
			}
			if (exchangeServerVersionInt >= Server.E15MinVersion)
			{
				return ExchangeVersionDeterminer.ExchangeServerVersion.E15;
			}
			return ExchangeVersionDeterminer.ExchangeServerVersion.Legacy;
		}

		private static LazyMember<ExchangeVersionDeterminer.ExchangeServerVersion> localServerVersion = new LazyMember<ExchangeVersionDeterminer.ExchangeServerVersion>(() => ExchangeVersionDeterminer.IntToServerVersion(LocalServer.GetServer().VersionNumber));

		public enum ExchangeServerVersion
		{
			Legacy,
			E12,
			E14,
			E15
		}
	}
}
