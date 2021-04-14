using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Compliance.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class GccProtocolActivityLoggerSingleton
	{
		public static GccProtocolActivityLogger Get(string clientInfoString)
		{
			string protocolName = GccProtocolActivityLoggerSingleton.GetProtocolName(clientInfoString);
			if (!GccProtocolActivityLoggerSingleton.DoWeNeedToLog(protocolName))
			{
				return null;
			}
			if (Interlocked.Exchange(ref GccProtocolActivityLoggerSingleton.creationNotMyJob, 1) == 0)
			{
				GccProtocolActivityLogger gccProtocolActivityLogger = new GccProtocolActivityLogger(protocolName);
				gccProtocolActivityLogger.Initialize();
				AppDomain.CurrentDomain.DomainUnload += GccProtocolActivityLoggerSingleton.CloseLogSingleton;
				GccProtocolActivityLoggerSingleton.instance = gccProtocolActivityLogger;
				GccProtocolActivityLoggerSingleton.busyCreating.Set();
			}
			else if (GccProtocolActivityLoggerSingleton.instance == null)
			{
				GccProtocolActivityLoggerSingleton.busyCreating.WaitOne();
			}
			return GccProtocolActivityLoggerSingleton.instance;
		}

		private static void CloseLogSingleton(object sender, EventArgs e)
		{
			if (GccProtocolActivityLoggerSingleton.instance != null)
			{
				GccProtocolActivityLogger gccProtocolActivityLogger = Interlocked.Exchange<GccProtocolActivityLogger>(ref GccProtocolActivityLoggerSingleton.instance, null);
				GccProtocolActivityLoggerSingleton.creationNotMyJob = 0;
				gccProtocolActivityLogger.Close();
			}
		}

		private static string GetProtocolName(string clientInfoString)
		{
			int num = clientInfoString.IndexOf(';');
			if (num >= 0)
			{
				clientInfoString = clientInfoString.Substring(0, num);
			}
			if (clientInfoString.StartsWith("Client=", StringComparison.OrdinalIgnoreCase))
			{
				clientInfoString = clientInfoString.Substring("Client=".Length);
			}
			else if (clientInfoString.StartsWith("MSExchangeRPC", StringComparison.OrdinalIgnoreCase))
			{
				clientInfoString = "MSExchangeRPC";
			}
			return clientInfoString;
		}

		private static bool DoWeNeedToLog(string protocol)
		{
			if (GccProtocolActivityLogger.Config.Disabled)
			{
				return false;
			}
			int num = GccProtocolActivityLoggerSingleton.loggedProtocols.Length;
			for (int i = 0; i < num; i++)
			{
				if (GccProtocolActivityLoggerSingleton.loggedProtocols[i].Equals(protocol, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static readonly string[] loggedProtocols = new string[]
		{
			"activesync",
			"owa",
			"pop3/imap4",
			"webservices",
			"msexchangerpc",
			"mailboxsessionwrapper"
		};

		private static int creationNotMyJob;

		private static ManualResetEvent busyCreating = new ManualResetEvent(false);

		private static GccProtocolActivityLogger instance;
	}
}
