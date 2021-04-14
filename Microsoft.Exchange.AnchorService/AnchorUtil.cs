using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AnchorUtil
	{
		public static void AssertOrThrow(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				throw new MigrationDataCorruptionException(string.Format(formatString, parameters));
			}
		}

		public static void RunOperationWithCulture(CultureInfo culture, Action operation)
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			try
			{
				Thread.CurrentThread.CurrentCulture = culture;
				Thread.CurrentThread.CurrentUICulture = culture;
				operation();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
		}

		internal static void RunTimedOperation(AnchorContext context, Action operation, object debugInfo)
		{
			AnchorUtil.RunTimedOperation<int>(context, delegate()
			{
				operation();
				return 0;
			}, debugInfo);
		}

		internal static T RunTimedOperation<T>(AnchorContext context, Func<T> operation, object debugInfo)
		{
			TimedOperationRunner timedOperationRunner = context.CreateOperationRunner();
			return timedOperationRunner.RunOperation<T>(operation, debugInfo);
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
			if (!argument.GetEnumerator().MoveNext())
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

		internal static string GetCurrentStackTrace()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StackTrace stackTrace = new StackTrace(2, false);
			foreach (StackFrame stackFrame in stackTrace.GetFrames().Take(10))
			{
				MethodBase method = stackFrame.GetMethod();
				stringBuilder.AppendFormat("{0}:{1};", method.DeclaringType, method);
			}
			return stringBuilder.ToString();
		}

		internal static bool IsTransientException(Exception exception)
		{
			return exception is TransientException;
		}

		internal static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> entries, int batchSize)
		{
			if (batchSize <= 0)
			{
				throw new ArgumentException("batchSize must be greater than 0.", "batchSize");
			}
			List<T> batch = new List<T>(batchSize);
			foreach (T entry in entries)
			{
				batch.Add(entry);
				if (batch.Count >= batchSize)
				{
					yield return batch;
					batch = new List<T>(batchSize);
				}
			}
			if (batch.Count > 0)
			{
				yield return batch;
			}
			yield break;
		}
	}
}
