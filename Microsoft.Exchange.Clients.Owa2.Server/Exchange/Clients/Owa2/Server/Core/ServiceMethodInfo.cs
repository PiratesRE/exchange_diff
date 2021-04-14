using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Web;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ServiceMethodInfo
	{
		internal string Name { get; set; }

		internal Type RequestType { get; set; }

		internal Type ResponseType { get; set; }

		internal Type WrappedRequestType { get; set; }

		internal Dictionary<string, string> WrappedRequestTypeParameterMap { get; set; }

		internal bool IsAsyncPattern { get; set; }

		internal bool IsAsyncAwait { get; set; }

		internal bool IsWrappedRequest { get; set; }

		internal bool IsWrappedResponse { get; set; }

		internal bool IsStreamedResponse { get; set; }

		internal bool ShouldAutoDisposeRequest { get; set; }

		internal bool ShouldAutoDisposeResponse { get; set; }

		internal bool IsResponseCacheable { get; set; }

		internal bool IsHttpGet { get; set; }

		internal UriTemplate UriTemplate { get; set; }

		internal MethodInfo SyncMethod { get; set; }

		internal MethodInfo BeginMethod { get; set; }

		internal MethodInfo EndMethod { get; set; }

		internal MethodInfo GenericAsyncTaskMethod { get; set; }

		internal JsonRequestFormat JsonRequestFormat { get; set; }

		internal WebMessageFormat WebMethodResponseFormat { get; set; }

		internal WebMessageFormat WebMethodRequestFormat { get; set; }
	}
}
