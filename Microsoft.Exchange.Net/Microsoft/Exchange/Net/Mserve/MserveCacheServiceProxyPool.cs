using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Mserve
{
	internal class MserveCacheServiceProxyPool<TClient> : ServiceProxyPool<TClient>
	{
		private MserveCacheServiceProxyPool(string endpointName, string hostName, Trace tracer, int maxNumberOfClientProxies, ChannelFactory<TClient> factory, GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate, GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate, ExEventLog.EventTuple errorEvent, bool useDisposeTracker) : base(endpointName, hostName, maxNumberOfClientProxies, factory, useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("getPermanentWrappedExceptionDelegate", getPermanentWrappedExceptionDelegate);
			ArgumentValidator.ThrowIfNull("getTransientWrappedExceptionDelegate", getTransientWrappedExceptionDelegate);
			ArgumentValidator.ThrowIfNull("errorEvent", errorEvent);
			this.getPermanentWrappedExceptionDelegate = getPermanentWrappedExceptionDelegate;
			this.getTransientWrappedExceptionDelegate = getTransientWrappedExceptionDelegate;
			this.errorEvent = errorEvent;
			this.tracer = tracer;
		}

		internal static MserveCacheServiceProxyPool<TClient> CreateMserveCacheServiceProxyPool(string endpointName, EndpointAddress endpointAddress, Trace tracer, int maxNumberOfClientProxies, Binding defaultBinding, GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate, GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate, ExEventLog.EventTuple errorEvent, bool useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("endpointAddress", endpointAddress);
			ChannelFactory<TClient> factory = MserveCacheServiceProxyPool<TClient>.CreateChannelFactory(endpointName, endpointAddress, defaultBinding, tracer);
			return new MserveCacheServiceProxyPool<TClient>(endpointName, endpointAddress.Uri.Host ?? "localhost", tracer, maxNumberOfClientProxies, factory, getTransientWrappedExceptionDelegate, getPermanentWrappedExceptionDelegate, errorEvent, useDisposeTracker);
		}

		protected override Trace Tracer
		{
			get
			{
				return this.tracer;
			}
		}

		protected override Exception GetTransientWrappedException(Exception wcfException)
		{
			return this.getTransientWrappedExceptionDelegate(wcfException, base.TargetInfo);
		}

		protected override Exception GetPermanentWrappedException(Exception wcfException)
		{
			return this.getPermanentWrappedExceptionDelegate(wcfException, base.TargetInfo);
		}

		protected override void LogCallServiceError(Exception error, string priodicKey, string debugMessage, int numberOfRetries)
		{
			MserveCacheServiceProvider.EventLog.LogEvent(this.errorEvent, error.Message, new object[]
			{
				error.Message,
				numberOfRetries,
				error.ToString()
			});
		}

		protected override bool IsTransientException(Exception ex)
		{
			ArgumentValidator.ThrowIfNull("ex", ex);
			return ex is SecurityNegotiationException || base.IsTransientException(ex);
		}

		private static ChannelFactory<TClient> CreateChannelFactory(string endpointName, EndpointAddress endpointAddress, Binding defaultBinding, Trace tracer)
		{
			ArgumentValidator.ThrowIfNull("endpointName", endpointName);
			ArgumentValidator.ThrowIfNull("endpointAddress", endpointAddress);
			ArgumentValidator.ThrowIfNull("defaultBinding", endpointAddress);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ChannelFactory<TClient> channelFactory = null;
			try
			{
				channelFactory = WcfUtils.TryCreateChannelFactoryFromConfig<TClient>(endpointName);
			}
			catch (Exception ex)
			{
				tracer.TraceError<string, string>(0L, "ServiceProxyPool - Error Creating channel factory from config file for {0}. Details {1}", endpointName, ex.ToString());
				MserveCacheServiceProvider.EventLog.LogEvent(CommonEventLogConstants.Tuple_WcfClientConfigError, endpointName, new object[]
				{
					endpointName,
					ex.Message
				});
			}
			if (channelFactory != null)
			{
				string host = endpointAddress.Uri.Host;
				Uri uri = channelFactory.Endpoint.Address.Uri;
				string uri2 = string.Format("{0}://{1}:{2}{3}", new object[]
				{
					uri.Scheme,
					host,
					uri.Port,
					uri.PathAndQuery
				});
				channelFactory.Endpoint.Address = new EndpointAddress(uri2);
			}
			else
			{
				tracer.TraceDebug<string>(0L, "ServiceProxyPool - Creating channel factory for {0} using default configuration", endpointName);
				channelFactory = new ChannelFactory<TClient>(defaultBinding, endpointAddress);
			}
			return channelFactory;
		}

		private readonly GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate;

		private readonly GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate;

		private readonly ExEventLog.EventTuple errorEvent;

		private readonly Trace tracer;
	}
}
