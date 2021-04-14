using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Web;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.DispatchPipe.Base
{
	internal class ServiceMethodInfo
	{
		internal string Name { get; set; }

		internal Type RequestType { get; set; }

		internal Type RequestBodyType { get; set; }

		internal Type ResponseType { get; set; }

		internal Type ResponseBodyType { get; set; }

		internal Type WrappedRequestType { get; set; }

		internal Type WrappedResponseType { get; set; }

		internal FieldInfo WrappedResponseBodyField { get; set; }

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

		internal SafeXmlSerializer GetOrCreateXmlSerializer(Type type, XmlRootAttribute root)
		{
			SafeXmlSerializer safeXmlSerializer = null;
			if (!this.xmlSerializers.TryGetValue(type, out safeXmlSerializer))
			{
				lock (this.xmlSerializersLock)
				{
					if (!this.xmlSerializers.TryGetValue(type, out safeXmlSerializer))
					{
						safeXmlSerializer = new SafeXmlSerializer(type, root);
						this.xmlSerializers.Add(type, safeXmlSerializer);
					}
				}
			}
			return safeXmlSerializer;
		}

		private object xmlSerializersLock = new object();

		private Dictionary<Type, SafeXmlSerializer> xmlSerializers = new Dictionary<Type, SafeXmlSerializer>();
	}
}
