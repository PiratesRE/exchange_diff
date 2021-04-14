using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.OnlineMeetings.ResourceContract;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class ResourceJsonSerializer
	{
		public MemoryStream Serialize(object value, MemoryStream stream)
		{
			Resource resource = value as Resource;
			if (resource == null && value != null)
			{
				throw new ArgumentException("The object to be serialized is not derived from Resource.", "value");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			try
			{
				object obj = this.ConvertResourceToDictionary(resource, null);
				string text = ResourceJsonSerializer.jsSerializer.Serialize(obj);
				ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string>(0, 0L, "[OnlineMeetings][ResourceJsonSerializer.Serialize] Serialized object: {0}", text);
				StreamWriter streamWriter = new StreamWriter(stream);
				streamWriter.Write(text);
				streamWriter.Flush();
			}
			catch (Exception innerException)
			{
				throw new SerializationException("The object could not be serialized correctly. See InnerException for details.", innerException);
			}
			return stream;
		}

		public object Deserialize(Type type, Stream stream)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!typeof(Resource).IsAssignableFrom(type))
			{
				throw new ArgumentException("The requested Type needs to be derived from ResourceBase", "type");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			object result;
			try
			{
				StreamReader streamReader = new StreamReader(stream);
				string text = streamReader.ReadToEnd();
				ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string>(0, 0L, "[OnlineMeetings][ResourceJsonSerializer.Deserialize] Response Body:{0}", text);
				Dictionary<string, object> dictionary = ResourceJsonSerializer.jsSerializer.Deserialize<Dictionary<string, object>>(text);
				Resource resource = Resource.FromDictionary(type, dictionary);
				result = resource;
			}
			catch (Exception innerException)
			{
				throw new SerializationException("The object could not be deserialized correctly. See InnerException for details.", innerException);
			}
			return result;
		}

		public object ConvertResourceToDictionary(Resource value, List<EmbeddedPart> mimeParts)
		{
			if (value == null)
			{
				return new Dictionary<string, string>();
			}
			return value.ToDictionary(mimeParts);
		}

		public string GetRequestContentType(Type requestType)
		{
			if (typeof(Resource).IsAssignableFrom(requestType))
			{
				return "application/vnd.microsoft.com.ucwa+json";
			}
			throw new ArgumentException("Unsupported request type. This Transformer only supports Resource classes.", "requestType");
		}

		public string GetResponseContentType(Type responseType)
		{
			if (typeof(Resource).IsAssignableFrom(responseType))
			{
				return "application/vnd.microsoft.com.ucwa+json";
			}
			throw new ArgumentException("Unsupported response type. This Transformer only supports ConferenceServicesResponse.", "responseType");
		}

		private static readonly JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
	}
}
