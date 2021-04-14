using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class CallIdTracer
	{
		static CallIdTracer()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole", false))
			{
				CallIdTracer.traceToFileEnabled = (registryKey != null && registryKey.GetValue("TraceToFile") != null && registryKey.GetValueKind("TraceToFile") == RegistryValueKind.DWord && 1 == (int)registryKey.GetValue("TraceToFile", 0));
			}
			CallIdTracer.traceFileLogger = new TraceFileLogger(CallIdTracer.traceToFileEnabled);
		}

		internal static void AddBreadcrumb(string message)
		{
			if (CallIdTracer.crumbedEnabled)
			{
				Breadcrumbs.AddBreadcrumb(message);
			}
		}

		internal static string FormatMessage(string message, params object[] args)
		{
			if (message == null)
			{
				message = "Null message";
			}
			else
			{
				try
				{
					if (args != null)
					{
						message = string.Format(CultureInfo.InvariantCulture, message, args);
					}
				}
				catch (FormatException)
				{
					message = string.Format("Badly formatted string - {0} args provided for message '{1}'", args.Length, message);
				}
			}
			return message;
		}

		internal static string FormatMessageWithContextAndCallId(object context, string message)
		{
			if (CallId.Id != null)
			{
				message = string.Format(CultureInfo.InvariantCulture, "Call-ID {0} : {1}", new object[]
				{
					CallId.Id,
					message
				});
			}
			if (context != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "({0}) : {1}", new object[]
				{
					context.GetHashCode(),
					message
				});
			}
			return message;
		}

		internal static string FormatMessageWithPIIData(string message, PIIMessage[] piiData)
		{
			if (piiData == null)
			{
				return message;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("​");
			foreach (PIIMessage piimessage in piiData)
			{
				stringBuilder.Append(piimessage.ToString());
				stringBuilder.Append("​");
			}
			stringBuilder.Append(PIIType._Message.ToString());
			stringBuilder.Append("=");
			stringBuilder.Append(message);
			return stringBuilder.ToString();
		}

		internal static void TraceDebug(Trace tracer, object context, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			CallIdTracer.traceFileLogger.TraceDebug(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TraceDebug((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TraceDebug(Trace tracer, object context, PIIMessage[] data, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			message = CallIdTracer.FormatMessageWithPIIData(message, data);
			CallIdTracer.traceFileLogger.TraceDebug(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TraceDebug((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TraceDebug(Trace tracer, object context, PIIMessage data, string message, params object[] args)
		{
			PIIMessage[] data2 = new PIIMessage[]
			{
				data
			};
			CallIdTracer.TraceDebug(tracer, context, data2, message, args);
		}

		internal static void TraceError(Trace tracer, object context, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			CallIdTracer.traceFileLogger.TraceError(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TraceError((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TraceError(Trace tracer, object context, PIIMessage[] data, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			message = CallIdTracer.FormatMessageWithPIIData(message, data);
			CallIdTracer.traceFileLogger.TraceError(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TraceError((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TraceError(Trace tracer, object context, PIIMessage data, string message, params object[] args)
		{
			PIIMessage[] data2 = new PIIMessage[]
			{
				data
			};
			CallIdTracer.TraceError(tracer, context, data2, message, args);
		}

		internal static void TracePfd(Trace tracer, object context, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			CallIdTracer.traceFileLogger.TracePfd(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TracePfd((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TracePfd(Trace tracer, object context, PIIMessage[] data, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			message = CallIdTracer.FormatMessageWithPIIData(message, data);
			CallIdTracer.traceFileLogger.TracePfd(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TracePfd((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TracePfd(Trace tracer, object context, PIIMessage data, string message, params object[] args)
		{
			PIIMessage[] data2 = new PIIMessage[]
			{
				data
			};
			CallIdTracer.TracePfd(tracer, context, data2, message, args);
		}

		internal static void TraceWarning(Trace tracer, object context, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			CallIdTracer.traceFileLogger.TraceWarning(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TraceWarning((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TraceWarning(Trace tracer, object context, PIIMessage[] data, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			message = CallIdTracer.FormatMessageWithPIIData(message, data);
			CallIdTracer.traceFileLogger.TraceWarning(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TraceWarning((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TraceWarning(Trace tracer, object context, PIIMessage data, string message, params object[] args)
		{
			PIIMessage[] data2 = new PIIMessage[]
			{
				data
			};
			CallIdTracer.TraceWarning(tracer, context, data2, message, args);
		}

		internal static void TracePerformance(Trace tracer, object context, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			CallIdTracer.traceFileLogger.TracePerformance(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TracePerformance((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void TracePerformance(Trace tracer, object context, PIIMessage[] data, string message, params object[] args)
		{
			message = CallIdTracer.FormatMessage(message, args);
			message = CallIdTracer.FormatMessageWithPIIData(message, data);
			CallIdTracer.traceFileLogger.TracePerformance(context, CallId.Id, message);
			message = CallIdTracer.FormatMessageWithContextAndCallId(context, message);
			CallIdTracer.AddBreadcrumb(message);
			tracer.TracePerformance((long)((context != null) ? context.GetHashCode() : 0), message);
		}

		internal static void Flush()
		{
			CallIdTracer.traceFileLogger.Flush();
		}

		internal static void TracePerformance(Trace tracer, object context, PIIMessage data, string message, params object[] args)
		{
			PIIMessage[] data2 = new PIIMessage[]
			{
				data
			};
			CallIdTracer.TracePerformance(tracer, context, data2, message, args);
		}

		private static bool crumbedEnabled = true;

		private static bool traceToFileEnabled;

		private static TraceFileLogger traceFileLogger;
	}
}
