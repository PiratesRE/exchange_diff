using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxyHeaderXmlWriter
	{
		public static void AddProxyHeader(Message message, ProxyHeaderValue proxyHeaderValue)
		{
			ProxyHeaderXmlWriter.RemoveAnyWSHeaders(message);
			ProxyHeaderXmlWriter.RemoveAnyProxyHeaders(message);
			string text = null;
			switch (proxyHeaderValue.ProxyHeaderType)
			{
			case ProxyHeaderType.SuggesterSid:
				text = "ProxySuggesterSid";
				break;
			case ProxyHeaderType.FullToken:
				text = "ProxySecurityContext";
				break;
			case ProxyHeaderType.PartnerToken:
				text = "ProxyPartnerToken";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				MessageHeader header = MessageHeader.CreateHeader(text, "http://schemas.microsoft.com/exchange/services/2006/types", Convert.ToBase64String(proxyHeaderValue.Value));
				message.Headers.Add(header);
			}
		}

		public static void RemoveProxyHeaders(Message message)
		{
			ProxyHeaderXmlWriter.RemoveAnyWSHeaders(message);
			ProxyHeaderXmlWriter.RemoveAnyProxyHeaders(message);
		}

		private static void RemoveAnyWSHeaders(Message message)
		{
			ProxyHeaderXmlWriter.RemoveHeaders(message, "http://schemas.microsoft.com/ws/2005/05/addressing/none");
			ProxyHeaderXmlWriter.RemoveHeaders(message, "http://www.w3.org/2005/08/addressing");
			ProxyHeaderXmlWriter.RemoveHeaders(message, "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
		}

		private static void RemoveHeaders(Message message, string xmlNamespace)
		{
			List<string> list = new List<string>(message.Headers.Count);
			foreach (MessageHeaderInfo messageHeaderInfo in message.Headers)
			{
				if (messageHeaderInfo.Namespace == xmlNamespace)
				{
					list.Add(messageHeaderInfo.Name);
				}
			}
			foreach (string name in list)
			{
				message.Headers.RemoveAll(name, xmlNamespace);
			}
		}

		private static void RemoveAnyProxyHeaders(Message message)
		{
			int count = message.Headers.Count;
			message.Headers.RemoveAll("ProxySecurityContext", "http://schemas.microsoft.com/exchange/services/2006/types");
			message.Headers.RemoveAll("ProxySuggesterSid", "http://schemas.microsoft.com/exchange/services/2006/types");
			message.Headers.RemoveAll("ProxyPartnerToken", "http://schemas.microsoft.com/exchange/services/2006/types");
		}
	}
}
