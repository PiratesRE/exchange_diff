using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class JsonMessageInspectorManager : MessageInspectorManager
	{
		internal JsonMessageInspectorManager()
		{
		}

		protected override object InternalAfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext, MessageBuffer buffer)
		{
			string text = "http://schemas.microsoft.com/exchange/services/2006/messages";
			string methodName = JsonMessageInspectorManager.GetMethodName(request);
			request.Headers.Action = string.Format("{0}/{1}", text, methodName);
			JsonMessageHeaderProcessor jsonMessageHeaderProcessor = new JsonMessageHeaderProcessor();
			if (JsonMessageInspectorManager.RequestNeedsHeaderProcessing(methodName))
			{
				if (buffer == null)
				{
					buffer = base.CreateMessageBuffer(request);
				}
				Message request2 = buffer.CreateMessage();
				jsonMessageHeaderProcessor.ProcessMessageHeaders(request2);
				request = buffer.CreateMessage();
			}
			request.Properties.Add("MethodName", methodName);
			request.Properties.Add("MethodNamespace", text);
			request.Properties.Add("MessageHeaderProcessor", jsonMessageHeaderProcessor);
			request.Properties.Add("ConnectionCostType", CostType.Connection);
			if (JsonMessageInspectorManager.IsOperationGetUserPhotoViaHttpGet(request))
			{
				request.Properties.Add("WebMethodEntry", WebMethodMetadata.Entries["GetUserPhoto:GET"]);
			}
			else
			{
				request.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
			}
			return base.InternalAfterReceiveRequest(ref request, channel, instanceContext, buffer);
		}

		protected override void InternalBeforeSendReply(ref Message reply, object correlationState)
		{
			base.InternalBeforeSendReply(ref reply, correlationState);
		}

		protected override void AddRequestResponseInspectors()
		{
		}

		private static string GetMethodName(Message request)
		{
			string result = "Json";
			object obj;
			if (request.Properties.TryGetValue("HttpOperationName", out obj) && obj is string)
			{
				result = (string)obj;
			}
			return result;
		}

		private static bool RequestNeedsHeaderProcessing(string methodName)
		{
			return !string.IsNullOrEmpty(methodName) && !JsonMessageInspectorManager.noHeaderProcessingMethodMap.Value.Contains(methodName.ToLowerInvariant());
		}

		private static bool IsOperationGetUserPhotoViaHttpGet(Message request)
		{
			if (request == null)
			{
				return false;
			}
			if (!"GetUserPhoto".Equals(JsonMessageInspectorManager.GetMethodName(request), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			object obj;
			if (!request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj) || obj == null)
			{
				return false;
			}
			HttpRequestMessageProperty httpRequestMessageProperty = obj as HttpRequestMessageProperty;
			return httpRequestMessageProperty != null && "GET".Equals(httpRequestMessageProperty.Method, StringComparison.OrdinalIgnoreCase);
		}

		private const string GetUserPhotoOperationName = "GetUserPhoto";

		private const string GetUserPhotoHttpGetMetadataKey = "GetUserPhoto:GET";

		private static Lazy<HashSet<string>> noHeaderProcessingMethodMap = new Lazy<HashSet<string>>(() => JsonMessageHeaderProcessor.BuildNoHeaderProcessingMap(typeof(JsonService)));
	}
}
