using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	internal static class ResponseMessageBuilder
	{
		internal static Microsoft.Exchange.Services.Core.Types.ResponseMessage ResponseMessageFromExchangeException(LocalizedException exception)
		{
			if (exception == null)
			{
				return new Microsoft.Exchange.Services.Core.Types.ResponseMessage(ServiceResultCode.Success, null);
			}
			ProxyQueryFailureException ex = exception as ProxyQueryFailureException;
			string messageText;
			XmlNodeArray messageXmlFromException;
			Microsoft.Exchange.Services.Core.Types.ResponseCodeType messageKey;
			if (ex != null)
			{
				messageText = ex.ResponseMessage.MessageText;
				messageXmlFromException = ResponseMessageBuilder.GetMessageXmlFromException(ex);
				messageKey = ResponseMessageBuilder.GetResponseCodeFromResponseMessage(ex.ResponseMessage);
			}
			else
			{
				messageText = ResponseMessageBuilder.GetMessageTextFromException(exception);
				messageXmlFromException = ResponseMessageBuilder.GetMessageXmlFromException(exception);
				messageKey = ResponseMessageBuilder.GetResponseCodeFromException(exception);
			}
			ServiceError error = new ServiceError(messageText, messageKey, 0, ExchangeVersion.Exchange2007);
			return new Microsoft.Exchange.Services.Core.Types.ResponseMessage(ServiceResultCode.Error, error)
			{
				MessageXml = messageXmlFromException
			};
		}

		private static string GetMessageTextFromException(LocalizedException exception)
		{
			if (exception.InnerException != null && exception.InnerException.Message != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}, inner exception: {1}", new object[]
				{
					exception.Message,
					exception.InnerException.ToString()
				});
			}
			return exception.ToString();
		}

		private static XmlNodeArray GetMessageXmlFromException(LocalizedException exception)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateNode(XmlNodeType.Element, "ExceptionType", "http://schemas.microsoft.com/exchange/services/2006/errors");
			xmlNode.InnerText = exception.GetType().Name;
			XmlNode xmlNode2 = safeXmlDocument.CreateNode(XmlNodeType.Element, "ExceptionCode", "http://schemas.microsoft.com/exchange/services/2006/errors");
			xmlNode2.InnerText = exception.ErrorCode.ToString(CultureInfo.InvariantCulture);
			XmlNode xmlNode3 = safeXmlDocument.CreateNode(XmlNodeType.Element, "ExceptionMessage", "http://schemas.microsoft.com/exchange/services/2006/errors");
			if (exception.InnerException != null && exception.InnerException.Message != null)
			{
				xmlNode3.InnerText = string.Format(CultureInfo.InvariantCulture, "{0}, inner exception: {1}", new object[]
				{
					exception.Message,
					exception.InnerException.Message
				});
			}
			else
			{
				xmlNode3.InnerText = exception.Message.ToString(CultureInfo.InvariantCulture);
			}
			XmlNodeArray xmlNodeArray = new XmlNodeArray();
			xmlNodeArray.Nodes.Add(xmlNode);
			xmlNodeArray.Nodes.Add(xmlNode2);
			xmlNodeArray.Nodes.Add(xmlNode3);
			AvailabilityException ex = exception as AvailabilityException;
			if (ex != null)
			{
				XmlNode xmlNode4 = safeXmlDocument.CreateNode(XmlNodeType.Element, "ExceptionServerName", "http://schemas.microsoft.com/exchange/services/2006/errors");
				xmlNode4.InnerText = ex.ServerName.ToString(CultureInfo.InvariantCulture);
				xmlNodeArray.Nodes.Add(xmlNode4);
			}
			return xmlNodeArray;
		}

		private static Microsoft.Exchange.Services.Core.Types.ResponseCodeType GetResponseCodeFromException(LocalizedException exception)
		{
			if (!Enum.IsDefined(typeof(Microsoft.Exchange.Services.Core.Types.ResponseCodeType), exception.ErrorCode))
			{
				return Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorWin32InteropError;
			}
			return (Microsoft.Exchange.Services.Core.Types.ResponseCodeType)exception.ErrorCode;
		}

		private static XmlNodeArray GetMessageXmlFromException(ProxyQueryFailureException proxyQueryFailureException)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNodeArray xmlNodeArray = new XmlNodeArray();
			if (proxyQueryFailureException.ResponseMessage.MessageXml != null && proxyQueryFailureException.ResponseMessage.MessageXml.HasChildNodes)
			{
				foreach (object obj in proxyQueryFailureException.ResponseMessage.MessageXml.ChildNodes)
				{
					XmlNode item = (XmlNode)obj;
					xmlNodeArray.Nodes.Add(item);
				}
			}
			XmlNode xmlNode = safeXmlDocument.CreateNode(XmlNodeType.Element, "ResponseSource", "http://schemas.microsoft.com/exchange/services/2006/errors");
			xmlNode.InnerText = ((proxyQueryFailureException.ResponseSource != null) ? proxyQueryFailureException.ResponseSource : "unknown");
			xmlNodeArray.Nodes.Add(xmlNode);
			return xmlNodeArray;
		}

		private static Microsoft.Exchange.Services.Core.Types.ResponseCodeType GetResponseCodeFromResponseMessage(Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ResponseMessage responseMessage)
		{
			if (string.IsNullOrEmpty(responseMessage.ResponseCode))
			{
				return Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorProxyRequestProcessingFailed;
			}
			Microsoft.Exchange.Services.Core.Types.ResponseCodeType result;
			try
			{
				result = (Microsoft.Exchange.Services.Core.Types.ResponseCodeType)Enum.Parse(typeof(Microsoft.Exchange.Services.Core.Types.ResponseCodeType), responseMessage.ResponseCode, true);
			}
			catch (ArgumentException)
			{
				result = Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorProxyRequestProcessingFailed;
			}
			return result;
		}
	}
}
