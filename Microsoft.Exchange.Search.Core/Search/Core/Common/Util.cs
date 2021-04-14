using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal static class Util
	{
		internal static void ThrowOnNullArgument(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		internal static void ThrowOnConditionFailed(bool condition, string message)
		{
			if (!condition)
			{
				throw new InvalidOperationException(message);
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

		internal static void ThrowOnNullOrEmptyArgument(ICollection argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			if (argument.Count == 0)
			{
				throw new ArgumentException("Argument should not be empty.", argumentName);
			}
		}

		internal static void ThrowOnNullOrEmptyArgument<TElement>(ICollection<TElement> argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			if (argument.Count == 0)
			{
				throw new ArgumentException("Argument should not be empty.", argumentName);
			}
		}

		internal static void ThrowOnMismatchType<T>(object o, string argumentName)
		{
			Type typeFromHandle = typeof(T);
			Type type = o.GetType();
			if (type != typeFromHandle && !typeFromHandle.IsAssignableFrom(type))
			{
				throw new ArgumentException(string.Format("Type mismatch for object: {0}, expected: {1}, actual: {2}", argumentName, typeof(T), o.GetType()));
			}
		}

		internal static string StringizeException(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Util.StringizeException(stringBuilder, exception, 0U);
			return stringBuilder.ToString();
		}

		internal static DateTime NormalizeDateTime(DateTime datetime)
		{
			return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, 0, datetime.Kind);
		}

		internal static DateTime NormalizeDateTimeToMinutes(DateTime datetime)
		{
			return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0, 0, datetime.Kind);
		}

		internal static bool ShouldRethrowException(Exception ex)
		{
			return ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException;
		}

		internal static bool TryGetExceptionOrInnerOfType<TException>(Exception exception, out Exception matchedException) where TException : Exception
		{
			matchedException = null;
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				if (ex is TException)
				{
					matchedException = ex;
					return true;
				}
			}
			return false;
		}

		private static void StringizeException(StringBuilder builder, Exception exception, uint depth)
		{
			if (depth > 10U)
			{
				builder.AppendLine("-----------------------------------------------------------");
				builder.AppendLine("  There's at least one more inner exception (not shown)");
				builder.AppendLine("-----------------------------------------------------------");
				return;
			}
			builder.AppendLine("-----------------------------------------------------------");
			builder.AppendLine(string.Format("  {0} exception: {1}", (depth == 0U) ? "Main" : "Inner", exception.GetType().ToString()));
			builder.AppendLine("-----------------------------------------------------------");
			builder.AppendLine(exception.Message);
			builder.AppendLine(exception.StackTrace);
			if (exception.InnerException != null)
			{
				Util.StringizeException(builder, exception.InnerException, depth + 1U);
			}
		}

		private const int StringizedExceptionMaxDepth = 10;
	}
}
