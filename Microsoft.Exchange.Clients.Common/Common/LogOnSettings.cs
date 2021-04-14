using System;
using System.Configuration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Common
{
	public static class LogOnSettings
	{
		public static LogOnSettings.SignOutKind SignOut { get; private set; }

		public static string SignOutPageUrl { get; private set; }

		public static bool IsLegacyLogOff { get; private set; }

		static LogOnSettings()
		{
			string text = null;
			LogOnSettings.SignOut = LogOnSettings.SignOutKind.DefaultSignOut;
			try
			{
				text = ConfigurationManager.AppSettings["LogonSettings.SignOutKind"];
				LogOnSettings.SignOut = (LogOnSettings.SignOutKind)Enum.Parse(typeof(LogOnSettings.SignOutKind), text, true);
			}
			catch (Exception arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<Exception>(0L, "LogonSettings::LogonSettings() Exception='{0}'", arg);
			}
			LogOnSettings.SignOutKind signOut = LogOnSettings.SignOut;
			if (signOut == LogOnSettings.SignOutKind.LegacyLogOff)
			{
				LogOnSettings.IsLegacyLogOff = true;
				LogOnSettings.SignOutPageUrl = LogOnSettings.logOnPageUrl;
			}
			else
			{
				LogOnSettings.IsLegacyLogOff = false;
				LogOnSettings.SignOutPageUrl = LogOnSettings.signOutPageUrl;
			}
			ExTraceGlobals.CoreTracer.TraceDebug<string, LogOnSettings.SignOutKind, string>(0L, "LogonSettings::LogonSettings() web.config.SignOut='{0}'; SignOut='{1}',SignOutPageUrl='{2}'", text, LogOnSettings.SignOut, LogOnSettings.SignOutPageUrl);
		}

		private static string signOutPageUrl = "auth/signout.aspx";

		private static string logOnPageUrl = "auth/logoff.aspx";

		public enum SignOutKind
		{
			DefaultSignOut,
			LegacyLogOff
		}
	}
}
