using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MigrationUtil
	{
		public static void AssertOrThrow(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				throw new MigrationPermanentException(Strings.MigrationGenericError)
				{
					InternalError = string.Format(formatString, parameters)
				};
			}
		}

		internal static void ThrowOnNullArgument(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		internal static void ThrowOnNullOrEmptyArgument(string argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			if (argument.Length == 0)
			{
				throw new ArgumentException(argumentName);
			}
		}

		internal static void ThrowOnCollectionEmptyArgument(IEnumerable argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			bool flag = argument is ICollection;
			if ((flag && ((ICollection)argument).Count == 0) || (!flag && !argument.GetEnumerator().MoveNext()))
			{
				throw new ArgumentException(argumentName);
			}
		}

		internal static void ThrowOnLessThanZeroArgument(long argument, string argumentName)
		{
			if (argument < 0L)
			{
				throw new ArgumentOutOfRangeException(argumentName);
			}
		}

		internal static void ThrowOnGuidEmptyArgument(Guid argument, string argumentName)
		{
			if (Guid.Empty == argument)
			{
				throw new ArgumentException(argumentName);
			}
		}

		internal static string EncryptedStringToClearText(string encryptedString)
		{
			if (string.IsNullOrEmpty(encryptedString))
			{
				return null;
			}
			string result;
			using (SecureString secureString = MigrationServiceFactory.Instance.GetCryptoAdapter().EncryptedStringToSecureString(encryptedString))
			{
				result = secureString.AsUnsecureString();
			}
			return result;
		}

		internal static string GetCurrentStackTrace()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StackTrace stackTrace = new StackTrace(2, false);
			foreach (StackFrame stackFrame in stackTrace.GetFrames().Take(10))
			{
				MethodBase method = stackFrame.GetMethod();
				stringBuilder.AppendFormat("{0}:{1};", method.DeclaringType, method.ToString());
			}
			return stringBuilder.ToString();
		}

		internal static TimeSpan MinTimeSpan(TimeSpan timespan1, TimeSpan timespan2)
		{
			if (timespan1 < timespan2)
			{
				return timespan1;
			}
			return timespan2;
		}

		internal static void RunTimedOperation(Action operation, object debugInfo)
		{
			MigrationUtil.RunTimedOperation<int>(delegate()
			{
				operation();
				return 0;
			}, debugInfo);
		}

		internal static T RunTimedOperation<T>(Func<T> operation, object debugInfo)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			T result;
			try
			{
				result = operation();
			}
			finally
			{
				stopwatch.Stop();
				TimeSpan config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("MigrationSlowOperationThreshold");
				if (config < stopwatch.Elapsed)
				{
					MigrationLogger.Log(MigrationEventType.Error, "SLOW Operation: took {0}s using '{1}' stack trace {2}", new object[]
					{
						stopwatch.Elapsed.Seconds,
						debugInfo,
						MigrationUtil.GetCurrentStackTrace()
					});
				}
				else
				{
					MigrationLogger.Log(MigrationEventType.Instrumentation, "Operation: took {0} using '{1}'", new object[]
					{
						stopwatch.Elapsed,
						debugInfo
					});
				}
			}
			return result;
		}

		internal static bool IsFeatureBlocked(MigrationFeature features)
		{
			MigrationFeature config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<MigrationFeature>("BlockedMigrationFeatures");
			return (config & features) != MigrationFeature.None;
		}

		internal static bool HasUnicodeCharacters(string token)
		{
			return !string.IsNullOrEmpty(token) && SyncUtilities.HasUnicodeCharacters(token);
		}

		internal static bool IsTransientException(Exception exception)
		{
			return exception is TransientException || CommonUtils.IsTransientException(exception) || (exception.InnerException != null && CommonUtils.IsTransientException(exception.InnerException));
		}
	}
}
