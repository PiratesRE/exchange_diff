using System;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security;
using System.Threading;
using System.Transactions;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal static class RetryHelper
	{
		public static bool IsSystemFatal(Exception ex)
		{
			return ex is AccessViolationException || ex is AppDomainUnloadedException || ex is BadImageFormatException || ex is DataMisalignedException || ex is InsufficientExecutionStackException || ex is InvalidOperationException || ex is MemberAccessException || ex is OutOfMemoryException || ex is StackOverflowException || ex is TypeInitializationException || ex is TypeLoadException || ex is TypeUnloadedException || ex is UnauthorizedAccessException || ex is ThreadAbortException || ex is SecurityTokenException || ex is InternalBufferOverflowException || ex is SecurityException;
		}

		public static bool IsFatalDefect(Exception ex)
		{
			return ex is ArgumentException || ex is ArithmeticException || ex is FormatException || ex is IndexOutOfRangeException || ex is InvalidCastException || ex is NotImplementedException || ex is NotSupportedException || ex is NullReferenceException;
		}

		public static T Invoke<T>(TimeSpan slowResponseTime, TimeSpan sleepInterval, int maxRetryCount, Func<T> action, Predicate<Exception> isRetriableException, Action<Exception, int> onRetry, Action<Exception, int> onMaxRetry, Action<TimeSpan> onSlowResponse, Action<Exception> onUnhandledException)
		{
			return RetryHelper.Invoke<T>(slowResponseTime, sleepInterval, maxRetryCount, action, isRetriableException, onRetry, onMaxRetry, onSlowResponse, onUnhandledException, RetryHelper.DefaultIsStoreUnavailablePredicate);
		}

		public static T Invoke<T>(TimeSpan slowResponseTime, TimeSpan sleepInterval, int maxRetryCount, Func<T> action, Predicate<Exception> isRetriableException, Action<Exception, int> onRetry, Action<Exception, int> onMaxRetry, Action<TimeSpan> onSlowResponse, Action<Exception> onUnhandledException, Predicate<Exception> isDataStoreUnavailableException)
		{
			int num = 0;
			T result = default(T);
			try
			{
				bool flag;
				do
				{
					flag = false;
					result = RetryHelper.InvokeOnce<T>(ref num, maxRetryCount, slowResponseTime, sleepInterval, action, isRetriableException, isDataStoreUnavailableException, onRetry, onMaxRetry, onSlowResponse, out flag);
				}
				while (flag);
			}
			catch (Exception obj)
			{
				onUnhandledException(obj);
				throw;
			}
			return result;
		}

		public static void Invoke(TimeSpan slowResponseTime, TimeSpan sleepInterval, int maxRetryCount, Action retryableAction, Predicate<Exception> isRetriableException, Action<Exception, int> onRetry, Action<Exception, int> onMaxRetry, Action<TimeSpan> onSlowResponse, Action<Exception> onUnhandledException)
		{
			RetryHelper.Invoke(slowResponseTime, sleepInterval, maxRetryCount, retryableAction, isRetriableException, onRetry, onMaxRetry, onSlowResponse, onUnhandledException, RetryHelper.DefaultIsStoreUnavailablePredicate);
		}

		public static void Invoke(TimeSpan slowResponseTime, TimeSpan sleepInterval, int maxRetryCount, Action retryableAction, Predicate<Exception> isRetriableException, Action<Exception, int> onRetry, Action<Exception, int> onMaxRetry, Action<TimeSpan> onSlowResponse, Action<Exception> onUnhandledException, Predicate<Exception> isDataStoreUnavailableException)
		{
			int num = 0;
			try
			{
				bool flag;
				do
				{
					flag = false;
					RetryHelper.InvokeOnce(ref num, maxRetryCount, slowResponseTime, sleepInterval, retryableAction, isRetriableException, isDataStoreUnavailableException, onRetry, onMaxRetry, onSlowResponse, out flag);
				}
				while (flag);
			}
			catch (Exception obj)
			{
				onUnhandledException(obj);
				throw;
			}
		}

		private static T InvokeOnce<T>(ref int retryCount, int maxRetryCount, TimeSpan slowResponseTime, TimeSpan sleepInterval, Func<T> action, Predicate<Exception> isRetriableException, Predicate<Exception> isDataStoreUnavailableException, Action<Exception, int> onRetry, Action<Exception, int> onMaxRetry, Action<TimeSpan> onSlowResponse, out bool needsRetry)
		{
			T result = default(T);
			needsRetry = false;
			RetryHelper.InvokeOnce(ref retryCount, maxRetryCount, slowResponseTime, sleepInterval, delegate()
			{
				result = action();
			}, isRetriableException, isDataStoreUnavailableException, onRetry, onMaxRetry, onSlowResponse, out needsRetry);
			return result;
		}

		private static void InvokeOnce(ref int retryCount, int maxRetryCount, TimeSpan slowResponseTime, TimeSpan sleepInterval, Action action, Predicate<Exception> isRetriableException, Predicate<Exception> isDataStoreUnavailableException, Action<Exception, int> onRetry, Action<Exception, int> onMaxRetry, Action<TimeSpan> onSlowResponse, out bool needsRetry)
		{
			needsRetry = false;
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				action();
				stopwatch.Stop();
				if (stopwatch.Elapsed > slowResponseTime)
				{
					onSlowResponse(stopwatch.Elapsed);
				}
			}
			catch (Exception ex)
			{
				if (RetryHelper.IsSystemFatal(ex) || RetryHelper.IsFatalDefect(ex))
				{
					throw;
				}
				if (ex is TransientDALException || ex is PermanentDALException)
				{
					throw;
				}
				if (!isRetriableException(ex))
				{
					throw new PermanentDALException(Strings.ErrorPermanentDALException, ex);
				}
				if (Transaction.Current != null)
				{
					throw new TransientDALException(Strings.ErrorTransientDALExceptionAmbientTransaction, ex);
				}
				if (retryCount >= maxRetryCount)
				{
					onMaxRetry(ex, maxRetryCount);
					if (isDataStoreUnavailableException(ex))
					{
						throw new TransientDataProviderUnavailableException(Strings.ErrorDataStoreUnavailable, ex);
					}
					throw new TransientDALException(Strings.ErrorTransientDALExceptionMaxRetries, ex);
				}
				else
				{
					retryCount++;
					onRetry(ex, retryCount);
					Thread.Sleep(sleepInterval);
					needsRetry = true;
				}
			}
		}

		private static readonly Predicate<Exception> DefaultIsStoreUnavailablePredicate = (Exception isDataStoreUnavailable) => false;
	}
}
