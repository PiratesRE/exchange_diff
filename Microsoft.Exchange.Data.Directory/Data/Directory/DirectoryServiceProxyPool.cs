using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

namespace Microsoft.Exchange.Data.Directory
{
	internal class DirectoryServiceProxyPool<TClient> : ServiceProxyPool<TClient>
	{
		private DirectoryServiceProxyPool(string endpointName, string hostName, Trace tracer, int maxNumberOfClientProxies, ChannelFactory<TClient> factory, GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate, GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate, ExEventLog.EventTuple errorEvent, bool useDisposeTracker) : base(endpointName, hostName, maxNumberOfClientProxies, factory, useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("getPermanentWrappedExceptionDelegate", getPermanentWrappedExceptionDelegate);
			ArgumentValidator.ThrowIfNull("getTransientWrappedExceptionDelegate", getTransientWrappedExceptionDelegate);
			ArgumentValidator.ThrowIfNull("errorEvent", errorEvent);
			this.getPermanentWrappedExceptionDelegate = getPermanentWrappedExceptionDelegate;
			this.getTransientWrappedExceptionDelegate = getTransientWrappedExceptionDelegate;
			this.errorEvent = errorEvent;
			this.tracer = tracer;
		}

		private DirectoryServiceProxyPool(string endpointName, string hostName, Trace tracer, int maxNumberOfClientProxies, List<ChannelFactory<TClient>> factoryList, GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate, GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate, ExEventLog.EventTuple errorEvent, bool useDisposeTracker) : base(endpointName, hostName, maxNumberOfClientProxies, factoryList, useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("getPermanentWrappedExceptionDelegate", getPermanentWrappedExceptionDelegate);
			ArgumentValidator.ThrowIfNull("getTransientWrappedExceptionDelegate", getTransientWrappedExceptionDelegate);
			ArgumentValidator.ThrowIfNull("errorEvent", errorEvent);
			this.getPermanentWrappedExceptionDelegate = getPermanentWrappedExceptionDelegate;
			this.getTransientWrappedExceptionDelegate = getTransientWrappedExceptionDelegate;
			this.errorEvent = errorEvent;
			this.tracer = tracer;
		}

		internal static DirectoryServiceProxyPool<TClient> CreateDirectoryServiceProxyPool(string endpointName, EndpointAddress endpointAddress, Trace tracer, int maxNumberOfClientProxies, Binding defaultBinding, GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate, GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate, ExEventLog.EventTuple errorEvent, bool useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("endpointAddress", endpointAddress);
			ChannelFactory<TClient> factory = DirectoryServiceProxyPool<TClient>.CreateChannelFactory(endpointName, endpointAddress, defaultBinding, tracer);
			return new DirectoryServiceProxyPool<TClient>(endpointName, endpointAddress.Uri.Host ?? "localhost", tracer, maxNumberOfClientProxies, factory, getTransientWrappedExceptionDelegate, getPermanentWrappedExceptionDelegate, errorEvent, useDisposeTracker);
		}

		internal static DirectoryServiceProxyPool<TClient> CreateDirectoryServiceProxyPool(string endpointName, ServiceEndpoint serviceEndpoint, Trace tracer, int maxNumberOfClientProxies, Binding defaultBinding, GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate, GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate, ExEventLog.EventTuple errorEvent, bool useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("serviceEndpoint", serviceEndpoint);
			ChannelFactory<TClient> factory = DirectoryServiceProxyPool<TClient>.CreateChannelFactory(endpointName, serviceEndpoint, defaultBinding, tracer);
			return new DirectoryServiceProxyPool<TClient>(endpointName, serviceEndpoint.Uri.Host, tracer, maxNumberOfClientProxies, factory, getTransientWrappedExceptionDelegate, getPermanentWrappedExceptionDelegate, errorEvent, useDisposeTracker);
		}

		internal static DirectoryServiceProxyPool<TClient> CreateDirectoryServiceProxyPool(string endpointName, ServiceEndpoint serviceEndpoint, Trace tracer, int maxNumberOfClientProxies, List<WSHttpBinding> httpBindings, GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate, GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate, ExEventLog.EventTuple errorEvent, bool useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("serviceEndpoint", serviceEndpoint);
			List<ChannelFactory<TClient>> list = new List<ChannelFactory<TClient>>();
			foreach (Binding defaultBinding in httpBindings)
			{
				list.Add(DirectoryServiceProxyPool<TClient>.CreateChannelFactory(endpointName, serviceEndpoint, defaultBinding, tracer));
			}
			return new DirectoryServiceProxyPool<TClient>(endpointName, serviceEndpoint.Uri.Host, tracer, maxNumberOfClientProxies, list, getTransientWrappedExceptionDelegate, getPermanentWrappedExceptionDelegate, errorEvent, useDisposeTracker);
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
			if (Globals.ProcessInstanceType != InstanceType.NotInitialized)
			{
				string text = error.ToString();
				if (error is FaultException<TopologyServiceFault>)
				{
					text = ((FaultException<TopologyServiceFault>)error).Detail.ToString();
				}
				Globals.LogEvent(this.errorEvent, priodicKey, new object[]
				{
					debugMessage,
					base.TargetInfo,
					numberOfRetries,
					text
				});
			}
		}

		protected override bool IsTransientException(Exception ex)
		{
			ArgumentValidator.ThrowIfNull("ex", ex);
			if (ex is FaultException<TopologyServiceFault>)
			{
				return ((FaultException<TopologyServiceFault>)ex).Detail.CanRetry;
			}
			if (ex is FaultException<LocatorServiceFault>)
			{
				return ((FaultException<LocatorServiceFault>)ex).Detail.CanRetry;
			}
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
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_WcfClientConfigError, endpointName, new object[]
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
			DirectoryServiceProxyPool<TClient>.ConfigWCFServicePointManager();
			return channelFactory;
		}

		private static ChannelFactory<TClient> CreateChannelFactory(string endpointName, ServiceEndpoint serviceEndpoint, Binding defaultBinding, Trace tracer)
		{
			ArgumentValidator.ThrowIfNull("endpointName", endpointName);
			ArgumentValidator.ThrowIfNull("serviceEndpoint", serviceEndpoint);
			ArgumentValidator.ThrowIfNull("defaultBinding", defaultBinding);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ChannelFactory<TClient> channelFactory = null;
			try
			{
				channelFactory = WcfUtils.TryCreateChannelFactoryFromConfig<TClient>(endpointName);
			}
			catch (Exception ex)
			{
				tracer.TraceError<string, string>(0L, "ServiceProxyPool - Error Creating channel factory from config file for {0}. Details {1}", endpointName, ex.ToString());
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_WcfClientConfigError, endpointName, new object[]
				{
					endpointName,
					ex.Message
				});
			}
			if (channelFactory == null)
			{
				channelFactory = new ChannelFactory<TClient>(defaultBinding, serviceEndpoint.Uri.ToString());
			}
			WSHttpBinding wshttpBinding = defaultBinding as WSHttpBinding;
			if (wshttpBinding != null && wshttpBinding.Security.Transport.ClientCredentialType == HttpClientCredentialType.Certificate)
			{
				try
				{
					channelFactory.Credentials.ClientCertificate.Certificate = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(serviceEndpoint.CertificateSubject);
				}
				catch (ArgumentException ex2)
				{
					throw new GlsPermanentException(DirectoryStrings.PermanentGlsError(ex2.Message));
				}
			}
			DirectoryServiceProxyPool<TClient>.ConfigWCFServicePointManager();
			return channelFactory;
		}

		private static void ConfigWCFServicePointManager()
		{
			ServicePointManager.DefaultConnectionLimit = Math.Max(ServicePointManager.DefaultConnectionLimit, 8 * Environment.ProcessorCount);
			ServicePointManager.EnableDnsRoundRobin = true;
		}

		private readonly GetWrappedExceptionDelegate getTransientWrappedExceptionDelegate;

		private readonly GetWrappedExceptionDelegate getPermanentWrappedExceptionDelegate;

		private readonly ExEventLog.EventTuple errorEvent;

		private readonly Trace tracer;
	}
}
