using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DxStore.Common
{
	public static class WcfUtils
	{
		public static IEnumerable<Exception> EnumerateInner(this Exception root)
		{
			if (root != null)
			{
				Queue<Exception> errors = new Queue<Exception>();
				errors.Enqueue(root);
				while (errors.Count > 0)
				{
					Exception error = errors.Dequeue();
					yield return error;
					AggregateException aggregate = error as AggregateException;
					if (aggregate != null)
					{
						using (IEnumerator<Exception> enumerator = aggregate.InnerExceptions.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Exception item = enumerator.Current;
								errors.Enqueue(item);
							}
							continue;
						}
					}
					if (error.InnerException != null)
					{
						errors.Enqueue(error.InnerException);
					}
				}
			}
			yield break;
		}

		public static bool IsChannelException(Exception error)
		{
			return error.EnumerateInner().Any(delegate(Exception e)
			{
				Type type = e.GetType();
				return !typeof(FaultException).IsAssignableFrom(type) && (typeof(CommunicationException).IsAssignableFrom(type) || typeof(TimeoutException).IsAssignableFrom(type));
			});
		}

		public static void CloseChannel(IClientChannel channel)
		{
			if (channel != null)
			{
				try
				{
					channel.Close();
				}
				catch (Exception error)
				{
					channel.Abort();
					if (!WcfUtils.IsChannelException(error))
					{
						throw;
					}
				}
			}
		}

		public static void Run<T>(CachedChannelFactory<T> factory, TimeSpan? timeout, Action<T> methodToCall)
		{
			WcfUtils.Run<T, int>(factory, timeout, delegate(T service)
			{
				methodToCall(service);
				return 0;
			});
		}

		public static R Run<T, R>(CachedChannelFactory<T> factory, TimeSpan? timeout, Func<T, R> methodToCall)
		{
			T t = factory.Factory.CreateChannel();
			R result;
			using (IClientChannel clientChannel = (IClientChannel)((object)t))
			{
				if (timeout != null)
				{
					clientChannel.OperationTimeout = timeout.Value;
				}
				CommunicationState state = clientChannel.State;
				bool flag = false;
				if (state != CommunicationState.Created)
				{
					if (state != CommunicationState.Closed)
					{
						goto IL_51;
					}
				}
				try
				{
					clientChannel.Open();
					flag = true;
				}
				catch
				{
					clientChannel.Abort();
					throw;
				}
				IL_51:
				bool flag2 = false;
				try
				{
					result = methodToCall(t);
				}
				catch (Exception error)
				{
					if (WcfUtils.IsChannelException(error))
					{
						flag2 = true;
						clientChannel.Abort();
					}
					throw;
				}
				finally
				{
					if (!flag2 && flag)
					{
						WcfUtils.CloseChannel(clientChannel);
					}
				}
			}
			return result;
		}

		public static DxStoreServerFault ConvertExceptionToDxStoreFault(Exception exception)
		{
			DxStoreFaultCode faultCode = DxStoreFaultCode.General;
			bool isTransientError = true;
			if (exception is DxStoreInstanceNotReadyException)
			{
				faultCode = DxStoreFaultCode.InstanceNotReady;
			}
			else if (exception is DxStoreInstanceStaleStoreException)
			{
				faultCode = DxStoreFaultCode.Stale;
			}
			else if (exception is DxStoreCommandConstraintFailedException)
			{
				faultCode = DxStoreFaultCode.ConstraintNotSatisfied;
			}
			else if (exception is TimeoutException)
			{
				faultCode = DxStoreFaultCode.ServerTimeout;
			}
			else
			{
				isTransientError = false;
			}
			bool isLocalized = exception is LocalizedException;
			return new DxStoreServerFault(exception, faultCode, isTransientError, isLocalized);
		}
	}
}
