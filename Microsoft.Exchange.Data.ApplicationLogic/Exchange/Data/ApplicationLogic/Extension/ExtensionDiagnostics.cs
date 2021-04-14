using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal static class ExtensionDiagnostics
	{
		internal static ExEventLog Logger
		{
			get
			{
				return ExtensionDiagnostics.logger;
			}
		}

		internal static string GetLoggedExceptionString(Exception exception)
		{
			string text = exception.ToString();
			if (text.Length > 32000)
			{
				text = text.Substring(0, 2000) + "...\n" + text.Substring(text.Length - 20000, 20000);
			}
			return text;
		}

		internal static object HandleNullObjectTrace(object objectToDisplay)
		{
			return objectToDisplay ?? "Object is null";
		}

		internal static string GetLoggedMailboxIdentifier(IExchangePrincipal exchangePrincipal)
		{
			return exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
		}

		internal static void LogToDatacenterOnly(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			if (Globals.IsDatacenter)
			{
				ExtensionDiagnostics.Logger.LogEvent(tuple, periodicKey, messageArgs);
			}
		}

		private static Guid eventLogComponentGuid = new Guid("{2C1EB772-38A2-4812-903B-244EAB5169A6}");

		private static readonly ExEventLog logger = new ExEventLog(ExtensionDiagnostics.eventLogComponentGuid, "MSExchangeApplicationLogic");
	}
}
