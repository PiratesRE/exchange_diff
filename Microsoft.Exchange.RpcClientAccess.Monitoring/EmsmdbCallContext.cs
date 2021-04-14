using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class EmsmdbCallContext<TResult> : RpcCallContext<TResult> where TResult : EmsmdbCallResult
	{
		public EmsmdbCallContext(IExchangeAsyncDispatch protocolClient, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(timeout, asyncCallback, asyncState)
		{
			Util.ThrowOnNullArgument(protocolClient, "protocolClient");
			this.protocolClient = protocolClient;
		}

		protected IExchangeAsyncDispatch ProtocolClient
		{
			get
			{
				return this.protocolClient;
			}
		}

		protected IPropertyBag GetHttpResponseInformation()
		{
			PropertyBag propertyBag = new PropertyBag();
			EmsmdbHttpClient emsmdbHttpClient = this.ProtocolClient as EmsmdbHttpClient;
			if (emsmdbHttpClient != null)
			{
				propertyBag.Set(ContextPropertySchema.ResponseHeaderCollection, emsmdbHttpClient.LastHttpResponseHeaders);
				propertyBag.Set(ContextPropertySchema.ResponseStatusCode, emsmdbHttpClient.LastResponseStatusCode);
				propertyBag.Set(ContextPropertySchema.ResponseStatusCodeDescription, emsmdbHttpClient.LastResponseStatusDescription);
				propertyBag.Set(ContextPropertySchema.RequestHeaderCollection, emsmdbHttpClient.LastHttpRequestHeaders);
				return propertyBag;
			}
			return null;
		}

		protected IPropertyBag GetHttpInformationFromProtocolException(ProtocolException protocolException)
		{
			PropertyBag propertyBag = new PropertyBag();
			propertyBag.Set(ContextPropertySchema.RequestHeaderCollection, protocolException.HttpRequestHeaders);
			propertyBag.Set(ContextPropertySchema.ResponseHeaderCollection, protocolException.HttpResponseHeaders);
			ProtocolFailureException ex = protocolException as ProtocolFailureException;
			if (ex != null)
			{
				propertyBag.Set(ContextPropertySchema.ResponseStatusCode, ex.HttpStatusCode);
				propertyBag.Set(ContextPropertySchema.ResponseStatusCodeDescription, ex.HttpStatusDescription);
			}
			return propertyBag;
		}

		private readonly IExchangeAsyncDispatch protocolClient;
	}
}
