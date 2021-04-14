using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Net.XropService
{
	internal static class ClientFactory
	{
		internal static IService GetClient(Uri endpoint, Uri internetWebProxy, FederatedClientCredentials clientCredentials, string targetSmtpAddress, TimeSpan timeout, IClientDiagnosticsHandler clientDiagnosticsHandler)
		{
			if (null == endpoint)
			{
				throw new ArgumentNullException("endpoint");
			}
			if (clientCredentials == null)
			{
				throw new ArgumentNullException("clientCredentials");
			}
			Uri uri = endpoint;
			string text = ClientFactory.NewXropCookieHeader();
			if (Binding.AddSessionIdToQueryString.Value)
			{
				UriBuilder uriBuilder = new UriBuilder(endpoint);
				if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
				{
					uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + text;
				}
				else
				{
					uriBuilder.Query = text;
				}
				uri = uriBuilder.Uri;
			}
			if (!string.IsNullOrEmpty(targetSmtpAddress))
			{
				UriBuilder uriBuilder2 = new UriBuilder(uri);
				if (uriBuilder2.Query != null && uriBuilder2.Query.Length > 1)
				{
					uriBuilder2.Query = string.Format("{0}&{1}={2}", uriBuilder2.Query.Substring(1), WellKnownHeader.AnchorMailbox, HttpUtility.UrlEncode(targetSmtpAddress));
				}
				else
				{
					uriBuilder2.Query = string.Format("{0}={1}", WellKnownHeader.AnchorMailbox, HttpUtility.UrlEncode(targetSmtpAddress));
				}
				uri = uriBuilder2.Uri;
			}
			IService service = ClientFactory.GetChannelFactory(endpoint, internetWebProxy, timeout, clientDiagnosticsHandler).CreateChannel(new EndpointAddress(uri, new AddressHeader[0]));
			ChannelParameterCollection property = ((IChannel)service).GetProperty<ChannelParameterCollection>();
			if (property == null)
			{
				ExTraceGlobals.XropServiceClientTracer.TraceError(0L, "ClientFactory unable to acquire channel parameter collection.");
				throw new InvalidOperationException();
			}
			property.Add(clientCredentials);
			WebHeaderCollection property2 = ((IChannel)service).GetProperty<WebHeaderCollection>();
			if (property2 != null)
			{
				property2.Add(HttpRequestHeader.Cookie, text);
			}
			ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)service.GetHashCode(), "ClientFactory created new IService channel instance.");
			return service;
		}

		private static ChannelFactory<IService> ConfigureDefaultChannelFactory(Uri endpoint, Uri internetWebProxy, IClientDiagnosticsHandler clientDiagnosticsHandler)
		{
			EndpointAddress remoteAddress = new EndpointAddress(endpoint, new AddressHeader[0]);
			BindingElementCollection clientBindingElements = Binding.GetClientBindingElements();
			if (null != internetWebProxy && !string.IsNullOrEmpty(internetWebProxy.Host))
			{
				HttpsTransportBindingElement httpsTransportBindingElement = clientBindingElements.Find<HttpsTransportBindingElement>();
				httpsTransportBindingElement.ProxyAddress = internetWebProxy;
				httpsTransportBindingElement.UseDefaultWebProxy = false;
			}
			ChannelFactory<IService> channelFactory = new ChannelFactory<IService>(new Binding(clientBindingElements), remoteAddress);
			channelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
			channelFactory.Endpoint.Behaviors.Add(new FactoryClientCredentials());
			channelFactory.Endpoint.Behaviors.Add(new ClientFactory.ClientEndpointBehavior(clientDiagnosticsHandler));
			ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)channelFactory.GetHashCode(), "ClientFactory created new IService channel factory instance.");
			return channelFactory;
		}

		private static string NewXropCookieHeader()
		{
			return "OutlookSession=" + Guid.NewGuid().ToString("N");
		}

		private static string GetCacheIdentifier(Uri endpoint, Uri internetWebProxy)
		{
			if (null == internetWebProxy || string.IsNullOrEmpty(internetWebProxy.Host))
			{
				return endpoint.Host.ToString();
			}
			return endpoint.Host.ToString() + internetWebProxy.Host.ToString();
		}

		private static ChannelFactory<IService> GetChannelFactory(Uri endpoint, Uri internetWebProxy, TimeSpan timeout, IClientDiagnosticsHandler clientDiagnosticsHandler)
		{
			ClientFactory.ChannelFactoryRef channelFactoryRef = null;
			string cacheIdentifier = ClientFactory.GetCacheIdentifier(endpoint, internetWebProxy);
			if (clientDiagnosticsHandler == null && ClientFactory.sharedDefaultFactories.TryGetValue(cacheIdentifier, out channelFactoryRef))
			{
				if (!channelFactoryRef.ShouldFactoryBeRetired)
				{
					return channelFactoryRef.ChannelFactory;
				}
				ClientFactory.RetireChannelFactory(cacheIdentifier, channelFactoryRef);
			}
			lock (ClientFactory.sharedFactoryLock)
			{
				if (clientDiagnosticsHandler != null || !ClientFactory.sharedDefaultFactories.TryGetValue(cacheIdentifier, out channelFactoryRef))
				{
					ChannelFactory<IService> channelFactory = ClientFactory.ConfigureDefaultChannelFactory(endpoint, internetWebProxy, clientDiagnosticsHandler);
					try
					{
						channelFactory.Open(timeout);
					}
					catch (Exception ex)
					{
						ExTraceGlobals.XropServiceClientTracer.TraceDebug<string>((long)channelFactory.GetHashCode(), "Xrop Service factory open failure {0}", ex.Message);
						throw ex;
					}
					ExTraceGlobals.XropServiceClientTracer.TraceDebug<string>((long)channelFactory.GetHashCode(), "Xrop Service created a new factory for {0}", cacheIdentifier);
					if (Binding.DoNotCacheFactories.Value || clientDiagnosticsHandler != null)
					{
						return channelFactory;
					}
					channelFactoryRef = new ClientFactory.ChannelFactoryRef(channelFactory);
					Dictionary<string, ClientFactory.ChannelFactoryRef> dictionary = new Dictionary<string, ClientFactory.ChannelFactoryRef>(ClientFactory.sharedDefaultFactories, StringComparer.OrdinalIgnoreCase);
					dictionary[cacheIdentifier] = channelFactoryRef;
					ClientFactory.sharedDefaultFactories = dictionary;
				}
			}
			return channelFactoryRef.ChannelFactory;
		}

		private static void RetireChannelFactory(string cacheKey, ClientFactory.ChannelFactoryRef factoryRef)
		{
			lock (ClientFactory.sharedFactoryLock)
			{
				if (ClientFactory.sharedDefaultFactories.TryGetValue(cacheKey, out factoryRef))
				{
					ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)factoryRef.ChannelFactory.GetHashCode(), "Retiring factory for " + cacheKey);
					Dictionary<string, ClientFactory.ChannelFactoryRef> dictionary = new Dictionary<string, ClientFactory.ChannelFactoryRef>(ClientFactory.sharedDefaultFactories, StringComparer.OrdinalIgnoreCase);
					dictionary.Remove(cacheKey);
					ClientFactory.sharedDefaultFactories = dictionary;
				}
			}
		}

		private const string XropSessionCookieHeader = "OutlookSession=";

		private const string ComponentId = "XropClient";

		private static object sharedFactoryLock = new object();

		private static Dictionary<string, ClientFactory.ChannelFactoryRef> sharedDefaultFactories = new Dictionary<string, ClientFactory.ChannelFactoryRef>();

		private sealed class ClientEndpointBehavior : IEndpointBehavior
		{
			public ClientEndpointBehavior(IClientDiagnosticsHandler clientDiagnosticsHandler)
			{
				this.clientDiagnosticsHandler = clientDiagnosticsHandler;
			}

			public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
			{
			}

			public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
			{
				clientRuntime.MessageInspectors.Add(new ClientFactory.ClientMessageInspector(endpoint.Address.Uri, this.clientDiagnosticsHandler));
			}

			public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
			{
			}

			public void Validate(ServiceEndpoint endpoint)
			{
			}

			private readonly IClientDiagnosticsHandler clientDiagnosticsHandler;
		}

		private sealed class ClientMessageInspector : IClientMessageInspector
		{
			public ClientMessageInspector(Uri endpoint, IClientDiagnosticsHandler clientDiagnosticsHandler)
			{
				this.endpoint = endpoint;
				this.clientDiagnosticsHandler = clientDiagnosticsHandler;
			}

			public object BeforeSendRequest(ref Message request, IClientChannel channel)
			{
				object result = null;
				HttpRequestMessageProperty httpRequestMessageProperty = ClientFactory.ClientMessageInspector.SetWebHeaders(request, channel);
				string text = httpRequestMessageProperty.Headers[HttpRequestHeader.Cookie];
				if (text != null)
				{
					try
					{
						this.cookieContainer.SetCookies(this.endpoint, text);
					}
					catch (CookieException)
					{
					}
				}
				string cookieHeader = this.cookieContainer.GetCookieHeader(this.endpoint);
				if (!string.IsNullOrEmpty(cookieHeader))
				{
					httpRequestMessageProperty.Headers[HttpRequestHeader.Cookie] = cookieHeader;
				}
				if (this.clientDiagnosticsHandler != null)
				{
					result = this.clientDiagnosticsHandler.BeforeSendRequest(httpRequestMessageProperty.Headers, request.ToString());
				}
				ExTraceGlobals.XropServiceClientTracer.TraceDebug<Message>((long)this.GetHashCode(), "Request={0}", request);
				return result;
			}

			public void AfterReceiveReply(ref Message reply, object correlationState)
			{
				object obj = null;
				HttpResponseMessageProperty httpResponseMessageProperty = null;
				if (reply.Properties.TryGetValue(HttpResponseMessageProperty.Name, out obj))
				{
					httpResponseMessageProperty = (obj as HttpResponseMessageProperty);
				}
				string arg = null;
				if (httpResponseMessageProperty != null)
				{
					arg = httpResponseMessageProperty.Headers["X-DiagInfo"];
				}
				string text = httpResponseMessageProperty.Headers[HttpResponseHeader.SetCookie];
				if (text != null)
				{
					try
					{
						this.cookieContainer.SetCookies(this.endpoint, text);
					}
					catch (CookieException)
					{
					}
				}
				if (reply.IsFault)
				{
					ExTraceGlobals.XropServiceClientTracer.TraceError<string, Message>((long)this.GetHashCode(), "Diagnostic={0};Fault={1}", arg, reply);
				}
				else
				{
					ExTraceGlobals.XropServiceClientTracer.TraceDebug<string, Message>((long)this.GetHashCode(), "Diagnostic={0};Reply={1}", arg, reply);
				}
				if (this.clientDiagnosticsHandler != null && httpResponseMessageProperty != null && reply != null)
				{
					this.clientDiagnosticsHandler.AfterRecieveReply(httpResponseMessageProperty.Headers, httpResponseMessageProperty.StatusCode, reply.IsFault, reply.ToString(), correlationState);
				}
			}

			private static HttpRequestMessageProperty SetWebHeaders(Message request, IClientChannel channel)
			{
				HttpRequestMessageProperty httpRequestMessageProperty = null;
				object obj = null;
				if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
				{
					httpRequestMessageProperty = (obj as HttpRequestMessageProperty);
				}
				if (httpRequestMessageProperty == null)
				{
					httpRequestMessageProperty = new HttpRequestMessageProperty();
					request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessageProperty);
				}
				WebHeaderCollection property = channel.GetProperty<WebHeaderCollection>();
				if (property != null && property.Count > 0)
				{
					httpRequestMessageProperty.Headers.Add(property);
					for (int i = 0; i < property.Count; i++)
					{
						ExTraceGlobals.XropServiceClientTracer.TraceDebug<string, string, Message>(0L, "Added HTTP header {0}={1} to the message {2}", property.GetKey(i), property.Get(i), request);
					}
				}
				return httpRequestMessageProperty;
			}

			private readonly IClientDiagnosticsHandler clientDiagnosticsHandler;

			private readonly Uri endpoint;

			private readonly CookieContainer cookieContainer = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
		}

		private sealed class ChannelFactoryRef : IDisposable
		{
			internal ChannelFactoryRef(ChannelFactory<IService> channelFactory)
			{
				this.channelFactory = channelFactory;
			}

			internal bool ShouldFactoryBeRetired
			{
				get
				{
					return this.ChannelFactory.State != CommunicationState.Opened;
				}
			}

			internal ChannelFactory<IService> ChannelFactory
			{
				get
				{
					return this.channelFactory;
				}
			}

			void IDisposable.Dispose()
			{
			}

			private ChannelFactory<IService> channelFactory;
		}
	}
}
