using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AuditingOpticsLogger
	{
		static AuditingOpticsLogger()
		{
			AuditingOpticsLogger.auditingOpticsLoggerInstanceHook = Hookable<IAuditingOpticsLoggerInstance>.Create(true, null);
		}

		private static void Stop(AuditingOpticsLoggerType loggerType)
		{
			EnumValidator.AssertValid<AuditingOpticsLoggerType>(loggerType);
			lock (AuditingOpticsLogger.instanceLock)
			{
				AuditingOpticsLoggerInstance auditingOpticsLoggerInstance = AuditingOpticsLogger.instances[(int)loggerType];
				if (auditingOpticsLoggerInstance != null)
				{
					auditingOpticsLoggerInstance.Stop();
					AuditingOpticsLogger.instances[(int)loggerType] = null;
				}
			}
		}

		internal static IAuditingOpticsLoggerInstance GetInstance(AuditingOpticsLoggerType loggerType)
		{
			EnumValidator.AssertValid<AuditingOpticsLoggerType>(loggerType);
			if (AuditingOpticsLogger.auditingOpticsLoggerInstanceHook.Value != null)
			{
				return AuditingOpticsLogger.auditingOpticsLoggerInstanceHook.Value;
			}
			IAuditingOpticsLoggerInstance result;
			lock (AuditingOpticsLogger.instanceLock)
			{
				AuditingOpticsLoggerInstance auditingOpticsLoggerInstance = AuditingOpticsLogger.instances[(int)loggerType];
				if (auditingOpticsLoggerInstance == null)
				{
					auditingOpticsLoggerInstance = new AuditingOpticsLoggerInstance(loggerType);
					AuditingOpticsLogger.instances[(int)loggerType] = auditingOpticsLoggerInstance;
				}
				result = auditingOpticsLoggerInstance;
			}
			return result;
		}

		internal static IDisposable SetLoggerInstanceTestHook(IAuditingOpticsLoggerInstance loggerInstance)
		{
			return AuditingOpticsLogger.auditingOpticsLoggerInstanceHook.SetTestHook(loggerInstance);
		}

		public static void LogAuditOpticsEntry(AuditingOpticsLoggerType loggerType, List<KeyValuePair<string, object>> customData)
		{
			EnumValidator.AssertValid<AuditingOpticsLoggerType>(loggerType);
			AuditingOpticsLogger.GetInstance(loggerType).InternalLogRow(customData);
		}

		public static List<KeyValuePair<string, object>> GetLogColumns<T>(T data, LogTableSchema<T>[] schema)
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>(schema.Length);
			foreach (LogTableSchema<T> logTableSchema in schema)
			{
				string text = logTableSchema.Getter(data);
				if (text == null)
				{
					text = string.Empty;
				}
				list.Add(new KeyValuePair<string, object>(logTableSchema.ColumnName, text));
			}
			return list;
		}

		public static IExceptionLogFormatter DefaultExceptionFormatter
		{
			get
			{
				return AuditingOpticsLogger.defaultFormatter;
			}
		}

		public static string GetExceptionNamesForTrace(Exception exception)
		{
			return AuditingOpticsLogger.GetExceptionNamesForTrace(exception, AuditingOpticsLogger.DefaultExceptionFormatter);
		}

		public static string GetExceptionNamesForTrace(Exception exception, IExceptionLogFormatter formatter)
		{
			if (exception == null)
			{
				return string.Empty;
			}
			if (formatter == null)
			{
				formatter = AuditingOpticsLogger.DefaultExceptionFormatter;
			}
			StringBuilder stringBuilder = null;
			do
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(256);
				}
				else
				{
					stringBuilder.Append("+");
				}
				string value = formatter.FormatException(exception);
				if (string.IsNullOrEmpty(value))
				{
					value = AuditingOpticsLogger.DefaultExceptionFormatter.FormatException(exception);
				}
				stringBuilder.Append(value);
				exception = exception.InnerException;
			}
			while (exception != null);
			return stringBuilder.ToString();
		}

		public static string GetDiagnosticContextFromException(Exception exception)
		{
			string result = string.Empty;
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				DiagnosticContext diagnosticContext = null;
				MapiPermanentException ex2 = ex as MapiPermanentException;
				MapiRetryableException ex3 = ex as MapiRetryableException;
				if (ex2 != null)
				{
					diagnosticContext = ex2.DiagCtx;
				}
				else if (ex3 != null)
				{
					diagnosticContext = ex3.DiagCtx;
				}
				if (diagnosticContext != null)
				{
					result = string.Format("[e::{0}]", diagnosticContext.ToCompactString());
					break;
				}
			}
			return result;
		}

		public static string GetDiagnosticContextFromThread()
		{
			if (!DiagnosticContext.HasData)
			{
				return string.Empty;
			}
			byte[] array = DiagnosticContext.PackInfo();
			byte[] array2 = new byte[array.Length + 6];
			int num = 0;
			ExBitConverter.Write(0, array2, num);
			num += 2;
			ExBitConverter.Write((uint)array.Length, array2, num);
			num += 4;
			Array.Copy(array, 0, array2, num, array.Length);
			return string.Format("[diag::{0}]", Convert.ToBase64String(array2));
		}

		public static string GetDiagnosticContext(Exception exception)
		{
			string result = string.Empty;
			if (exception != null)
			{
				result = string.Format("{0}{1}", AuditingOpticsLogger.GetDiagnosticContextFromThread(), AuditingOpticsLogger.GetDiagnosticContextFromException(exception));
			}
			return result;
		}

		private static AuditingOpticsLoggerInstance[] instances = new AuditingOpticsLoggerInstance[4];

		private static readonly object instanceLock = new object();

		private static Hookable<IAuditingOpticsLoggerInstance> auditingOpticsLoggerInstanceHook;

		private static readonly IExceptionLogFormatter defaultFormatter = new AuditingOpticsLogger._DefaultExceptionFormatter();

		private class _DefaultExceptionFormatter : IExceptionLogFormatter
		{
			public string FormatException(Exception exception)
			{
				if (exception == null)
				{
					return string.Empty;
				}
				return exception.GetType().ToString();
			}
		}
	}
}
