using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal static class DocumentLibraryUtility
	{
		private static MultiValuedProperty<string> RemoteDocumentsAllowedServers
		{
			get
			{
				return GlobalSettings.RemoteDocumentsAllowedServers;
			}
		}

		private static MultiValuedProperty<string> RemoteDocumentsBlockedServers
		{
			get
			{
				return GlobalSettings.RemoteDocumentsBlockedServers;
			}
		}

		private static MultiValuedProperty<string> RemoteDocumentsInternalDomainSuffixList
		{
			get
			{
				return GlobalSettings.RemoteDocumentsInternalDomainSuffixList;
			}
		}

		private static RemoteDocumentsActions RemoteDocumentsActionForUnknownServers
		{
			get
			{
				if (GlobalSettings.RemoteDocumentsActionForUnknownServers == null)
				{
					return RemoteDocumentsActions.Allow;
				}
				return GlobalSettings.RemoteDocumentsActionForUnknownServers.Value;
			}
		}

		public static bool IsTrustedProtocol(string protocol)
		{
			AirSyncDiagnostics.Assert(!string.IsNullOrEmpty(protocol));
			for (int i = 0; i < DocumentLibraryUtility.trustedProtocols.Length; i++)
			{
				if (string.Equals(protocol, DocumentLibraryUtility.trustedProtocols[i], StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsInternalUri(Uri uri)
		{
			AirSyncDiagnostics.Assert(uri != null);
			string host = uri.Host;
			if (DocumentLibraryUtility.RemoteDocumentsInternalDomainSuffixList != null)
			{
				foreach (string value in DocumentLibraryUtility.RemoteDocumentsInternalDomainSuffixList)
				{
					if (host.EndsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			IPAddress ipaddress = new IPAddress(0L);
			bool result;
			try
			{
				result = (!IPAddress.TryParse(host, out ipaddress) && host.IndexOf('.') < 0);
			}
			catch (ArgumentException)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, null, "Invalid Uri Format in internal URI determination");
				result = false;
			}
			return result;
		}

		public static bool IsUncAccessEnabled(IAirSyncUser user)
		{
			AirSyncDiagnostics.Assert(user != null);
			PolicyData policyData = ADNotificationManager.GetPolicyData(user);
			return policyData == null || policyData.UNCAccessEnabled;
		}

		public static bool IsWssAccessEnabled(IAirSyncUser user)
		{
			AirSyncDiagnostics.Assert(user != null);
			PolicyData policyData = ADNotificationManager.GetPolicyData(user);
			return policyData == null || policyData.WSSAccessEnabled;
		}

		public static bool IsBlockedHostName(string hostName)
		{
			AirSyncDiagnostics.Assert(!string.IsNullOrEmpty(hostName));
			if (DocumentLibraryUtility.RemoteDocumentsBlockedServers != null)
			{
				foreach (string b in DocumentLibraryUtility.RemoteDocumentsBlockedServers)
				{
					if (string.Equals(hostName, b, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			if (DocumentLibraryUtility.RemoteDocumentsAllowedServers != null)
			{
				foreach (string b2 in DocumentLibraryUtility.RemoteDocumentsAllowedServers)
				{
					if (string.Equals(hostName, b2, StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
				}
			}
			return DocumentLibraryUtility.RemoteDocumentsActionForUnknownServers == RemoteDocumentsActions.Block;
		}

		private static readonly string[] trustedProtocols = new string[]
		{
			"http",
			"https",
			"file"
		};
	}
}
