using System;
using System.ServiceModel;

namespace Microsoft.Exchange.DxStore.Common
{
	public abstract class WcfExceptionTranslator<TService>
	{
		public static Exception TranslateException(Exception ex, Func<Exception, Exception> transient, Func<Exception, Exception> permanent)
		{
			FaultException<DxStoreServerFault> faultException = ex as FaultException<DxStoreServerFault>;
			if (faultException != null)
			{
				if (faultException.Detail.IsTransientError)
				{
					return transient(ex);
				}
				return permanent(ex);
			}
			else
			{
				if (ex is TimeoutException || ex is CommunicationException)
				{
					return transient(ex);
				}
				return null;
			}
		}

		public abstract Exception GenerateTransientException(Exception exception);

		public abstract Exception GeneratePermanentException(Exception exception);

		public void Execute(CachedChannelFactory<TService> factory, TimeSpan? timeout, Action<TService> action)
		{
			this.Execute<int>(factory, timeout, delegate(TService service)
			{
				action(service);
				return 0;
			});
		}

		public TReturnType Execute<TReturnType>(CachedChannelFactory<TService> factory, TimeSpan? timeout, Func<TService, TReturnType> action)
		{
			TReturnType result;
			try
			{
				result = WcfUtils.Run<TService, TReturnType>(factory, timeout, action);
			}
			catch (Exception ex)
			{
				Exception ex2 = WcfExceptionTranslator<TService>.TranslateException(ex, new Func<Exception, Exception>(this.GenerateTransientException), new Func<Exception, Exception>(this.GeneratePermanentException));
				if (ex2 != null)
				{
					throw ex2;
				}
				throw;
			}
			return result;
		}
	}
}
