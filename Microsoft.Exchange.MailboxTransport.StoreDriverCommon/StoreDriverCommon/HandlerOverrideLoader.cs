using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal class HandlerOverrideLoader
	{
		public static void ApplyConfiguredOverrides(ExceptionHandler exceptionHandler, NameValueCollection configurationAppSettings)
		{
			foreach (string text in from key in configurationAppSettings.AllKeys
			where key.StartsWith("ExceptionHandler_Override_")
			select key)
			{
				HandlerOverrideLoader.Diag.TraceDebug<string>(0L, "ExceptionHandlerOverrides: Begin processing override for key {0}", text);
				string[] array = text.Split(new char[]
				{
					'_'
				});
				if (array.Length != 4)
				{
					throw new HandlerParseException(string.Format("Unable to parse configured override because there were not the correct number of components in the key. Key: {0}", text));
				}
				string name = array[2].Trim();
				Type type = null;
				string text2 = array[3].Trim();
				foreach (string arg in HandlerOverrideLoader.wellKnownExceptionAssemblies)
				{
					type = Type.GetType(string.Format("{0}, {1}", text2, arg));
					if (type != null)
					{
						break;
					}
				}
				if (type == null)
				{
					type = Type.GetType(text2);
				}
				if (!(type != null))
				{
					throw new HandlerParseException(string.Format("ExceptionHandlerOverrides: Unable to determine the following exception type: {0}", text2));
				}
				exceptionHandler.AddOrUpdateOverrideHandler(name, type, Handler.Parse(configurationAppSettings[text]));
				HandlerOverrideLoader.Diag.TraceDebug<string>(0L, "ExceptionHandlerOverrides: Applied override for key {0}", text);
				HandlerOverrideLoader.Diag.TraceDebug<string>(0L, "ExceptionHandlerOverrides: Completed processing override for key {0}", text);
			}
		}

		private const string AppSettingsOverrideKeyNamePrefix = "ExceptionHandler_Override_";

		private const string ComponentKeyName = "Component";

		private const string ExceptionKeyName = "Exception";

		private static readonly Trace Diag = ExTraceGlobals.StoreDriverDeliveryTracer;

		private static string[] wellKnownExceptionAssemblies = new string[]
		{
			"Microsoft.Exchange.StoreProvider",
			"Microsoft.Exchange.Data.Storage"
		};
	}
}
