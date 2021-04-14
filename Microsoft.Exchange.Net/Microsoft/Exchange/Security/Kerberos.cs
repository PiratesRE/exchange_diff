using System;
using System.Security;

namespace Microsoft.Exchange.Security
{
	internal static class Kerberos
	{
		public static void FlushTicketCache()
		{
			using (SafeLsaUntrustedHandle safeLsaUntrustedHandle = SafeLsaUntrustedHandle.Create())
			{
				int packageId = safeLsaUntrustedHandle.LookupPackage("Kerberos");
				safeLsaUntrustedHandle.PurgeTicketCache(packageId);
			}
		}

		public static void AddExtraCredentials(string username, string domain, SecureString password)
		{
			Kerberos.AddExtraCredentials(username, domain, password, Kerberos.LuidSelf);
		}

		public static void AddExtraCredentials(string username, string domain, SecureString password, LsaNativeMethods.LUID luid)
		{
			using (SafeLsaUntrustedHandle safeLsaUntrustedHandle = SafeLsaUntrustedHandle.Create())
			{
				int packageId = safeLsaUntrustedHandle.LookupPackage("Kerberos");
				safeLsaUntrustedHandle.AddExtraCredentials(packageId, username, domain, password, LsaNativeMethods.KerbRequestCredentialFlags.Add, luid);
			}
		}

		public static void RemoveExtraCredentials(string username, string domain)
		{
			Kerberos.RemoveExtraCredentials(username, domain, Kerberos.LuidSelf);
		}

		public static void RemoveExtraCredentials(string username, string domain, LsaNativeMethods.LUID luid)
		{
			using (SafeLsaUntrustedHandle safeLsaUntrustedHandle = SafeLsaUntrustedHandle.Create())
			{
				int packageId = safeLsaUntrustedHandle.LookupPackage("Kerberos");
				safeLsaUntrustedHandle.AddExtraCredentials(packageId, username, domain, null, LsaNativeMethods.KerbRequestCredentialFlags.Remove, luid);
			}
		}

		public static void ReplaceExtraCredentials(string username, string domain, SecureString password)
		{
			Kerberos.ReplaceExtraCredentials(username, domain, password, Kerberos.LuidSelf);
		}

		public static void ReplaceExtraCredentials(string username, string domain, SecureString password, LsaNativeMethods.LUID luid)
		{
			using (SafeLsaUntrustedHandle safeLsaUntrustedHandle = SafeLsaUntrustedHandle.Create())
			{
				int packageId = safeLsaUntrustedHandle.LookupPackage("Kerberos");
				safeLsaUntrustedHandle.AddExtraCredentials(packageId, username, domain, password, LsaNativeMethods.KerbRequestCredentialFlags.Replace, luid);
			}
		}

		private const string PackageName = "Kerberos";

		public static readonly LsaNativeMethods.LUID LuidSelf = new LsaNativeMethods.LUID(0, 0);

		public static readonly LsaNativeMethods.LUID LuidLocalSystem = new LsaNativeMethods.LUID(999, 0);

		public static readonly LsaNativeMethods.LUID LuidNetworkService = new LsaNativeMethods.LUID(996, 0);
	}
}
