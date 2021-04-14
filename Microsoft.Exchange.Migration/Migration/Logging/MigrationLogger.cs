using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MigrationLogger
	{
		internal static Action<string, MigrationEventType, object, string> InMemoryLogger { get; set; }

		public static void Log(MigrationEventType eventType, Exception exception, string format, params object[] args)
		{
			if (exception != null)
			{
				MigrationLogger.Log(eventType, format + ", exception " + MigrationLogger.GetDiagnosticInfo(exception, null), args);
				return;
			}
			MigrationLogger.Log(eventType, format, args);
		}

		public static void Log(MigrationEventType eventType, string format, params object[] args)
		{
			MigrationLogContext migrationLogContext = MigrationLogContext.Current;
			MigrationLogger.Log(migrationLogContext.Source, eventType, migrationLogContext, format, args);
		}

		public static void Log(string source, MigrationEventType eventType, object context, string format, params object[] args)
		{
			lock (MigrationLogger.objLock)
			{
				if (MigrationLogger.log != null)
				{
					MigrationLogger.log.Log(source, eventType, context, format, args);
				}
				if (MigrationLogger.InMemoryLogger != null)
				{
					string arg = string.Format(format, args);
					MigrationLogger.InMemoryLogger(source, eventType, context, arg);
				}
			}
		}

		public static void Flush()
		{
			lock (MigrationLogger.objLock)
			{
				if (MigrationLogger.log != null)
				{
					MigrationLogger.log.Flush();
				}
			}
		}

		public static void Close()
		{
			lock (MigrationLogger.objLock)
			{
				MigrationLogger.refCount--;
				if (MigrationLogger.refCount <= 0)
				{
					if (MigrationLogger.log != null)
					{
						MigrationLogger.log.Close();
						MigrationLogger.log.Dispose();
						MigrationLogger.log = null;
					}
					MigrationLogger.refCount = 0;
				}
			}
		}

		public static string PropertyBagToString(PropertyBag bag)
		{
			MigrationUtil.ThrowOnNullArgument(bag, "bag");
			StringBuilder stringBuilder = new StringBuilder(bag.Count * 128);
			foreach (object obj in bag.Keys)
			{
				PropertyDefinition propertyDefinition = obj as PropertyDefinition;
				if (propertyDefinition != null)
				{
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "[{0}:{1}]", new object[]
					{
						propertyDefinition.Name,
						bag[propertyDefinition]
					}));
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetDiagnosticInfo(Exception ex, string diagnosticInfo)
		{
			string internalError = null;
			MigrationApplication.RunOperationWithCulture(CultureInfo.InvariantCulture, delegate
			{
				internalError = MigrationLogger.InternalGetDiagnosticInfo(ex, diagnosticInfo);
			});
			return internalError;
		}

		public static string GetDiagnosticInfo(StackTrace st, string diagnosticInfo)
		{
			MigrationUtil.ThrowOnNullArgument(st, "st");
			MigrationUtil.ThrowOnNullOrEmptyArgument(diagnosticInfo, "diagnosticInfo");
			string internalError = null;
			MigrationApplication.RunOperationWithCulture(CultureInfo.InvariantCulture, delegate
			{
				internalError = MigrationLogger.ConcatAndSanitizeDiagnosticFields(new object[]
				{
					st,
					diagnosticInfo
				});
			});
			return internalError;
		}

		public static string SanitizeDiagnosticInfo(string diagnosticInfo)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(diagnosticInfo, "diagnosticInfo");
			diagnosticInfo = diagnosticInfo.Replace("  ", " ");
			diagnosticInfo = diagnosticInfo.Replace("\n", " ");
			diagnosticInfo = diagnosticInfo.Replace("\r", " ");
			diagnosticInfo = diagnosticInfo.Replace("\t", " ");
			diagnosticInfo = diagnosticInfo.Replace("{", "[");
			diagnosticInfo = diagnosticInfo.Replace("}", "]");
			if (diagnosticInfo.Length > 32768)
			{
				return diagnosticInfo.Substring(0, 32765) + "...";
			}
			return diagnosticInfo;
		}

		public static void Initialize()
		{
			lock (MigrationLogger.objLock)
			{
				if (MigrationLogger.log == null)
				{
					MigrationLogger.log = new MigrationLog();
				}
				MigrationLogger.refCount++;
			}
		}

		public static string GetInternalError(Exception ex)
		{
			string result = null;
			MigrationPermanentException ex2 = ex as MigrationPermanentException;
			MigrationTransientException ex3 = ex as MigrationTransientException;
			MigrationDataCorruptionException ex4 = ex as MigrationDataCorruptionException;
			if (ex2 != null)
			{
				result = ex2.InternalError;
			}
			else if (ex3 != null)
			{
				result = ex3.InternalError;
			}
			else if (ex4 != null)
			{
				result = ex4.InternalError;
			}
			return result;
		}

		public static string CombineInternalError(string internalError, Exception ex)
		{
			string internalError2 = MigrationLogger.GetInternalError(ex);
			string result = internalError;
			if (!string.IsNullOrEmpty(internalError2))
			{
				result = internalError + " -- " + internalError2;
			}
			return result;
		}

		private static string ExtractMapiDiagnostic(Exception ex)
		{
			string text = null;
			Exception innerException = ex.InnerException;
			int num = 0;
			while (num < 10 && innerException != null)
			{
				MapiPermanentException ex2 = innerException as MapiPermanentException;
				MapiRetryableException ex3 = innerException as MapiRetryableException;
				string text2 = innerException.Message;
				if (ex2 != null)
				{
					text2 = ex2.DiagCtx.ToCompactString();
				}
				else if (ex3 != null)
				{
					text2 = ex3.DiagCtx.ToCompactString();
				}
				if (!string.IsNullOrEmpty(text2))
				{
					if (text == null)
					{
						text = string.Format(CultureInfo.InvariantCulture, "InnerException:{0}:{1}", new object[]
						{
							innerException.GetType().Name,
							text2
						});
					}
					else
					{
						text = string.Format(CultureInfo.InvariantCulture, "{0} InnerException:{1}:{2}", new object[]
						{
							text,
							innerException.GetType().Name,
							text2
						});
					}
				}
				num++;
				innerException = innerException.InnerException;
			}
			if (text != null)
			{
				text = text.Replace("  ", " ");
			}
			return text;
		}

		private static string InternalGetDiagnosticInfo(Exception ex, string internalContext)
		{
			string internalError = MigrationLogger.GetInternalError(ex);
			return MigrationLogger.ConcatAndSanitizeDiagnosticFields(new object[]
			{
				ex.StackTrace,
				internalContext,
				internalError,
				ex.GetType(),
				ex.Message,
				MigrationLogger.ExtractMapiDiagnostic(ex),
				ex.InnerException
			});
		}

		private static string ConcatAndSanitizeDiagnosticFields(params object[] sourceFields)
		{
			MigrationUtil.ThrowOnCollectionEmptyArgument(sourceFields, "sourceFields");
			int num = 0;
			List<string> list = new List<string>(sourceFields.Length);
			foreach (object obj in sourceFields)
			{
				if (obj != null)
				{
					string text = obj.ToString();
					int num2 = 32768 - num;
					if (text.Length >= num2)
					{
						if (num2 > 0)
						{
							list.Add(text.Substring(0, num2));
						}
						num = 32768;
						break;
					}
					num += text.Length;
					list.Add(text);
				}
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			foreach (string text2 in list)
			{
				stringBuilder.Append(text2.Replace('|', '_'));
				stringBuilder.Append('|');
			}
			return MigrationLogger.SanitizeDiagnosticInfo(stringBuilder.ToString());
		}

		public const char FieldDelimiter = '|';

		private const int MaxDiagnosticInfoLength = 32768;

		private static readonly object objLock = new object();

		private static MigrationLog log;

		private static int refCount;
	}
}
