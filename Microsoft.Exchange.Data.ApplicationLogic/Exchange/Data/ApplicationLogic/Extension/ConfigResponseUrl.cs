using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class ConfigResponseUrl
	{
		internal string ServiceName { get; set; }

		internal string Url { get; set; }

		internal ConfigResponseUrl()
		{
		}

		internal ConfigResponseUrl(XElement element, Dictionary<string, XElement> tokenDictionary, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			this.ServiceName = ConfigResponseUrl.ParseServiceName(element, OmexConstants.OfficeNamespace + "name", logParseFailureCallback);
			if (!string.IsNullOrWhiteSpace(this.ServiceName))
			{
				this.Url = ConfigResponseUrl.ParseUrl(element, OmexConstants.OfficeNamespace + "url", tokenDictionary, this.ServiceName, logParseFailureCallback);
			}
		}

		private static string ParseServiceName(XElement element, XName serviceNameKey, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			string text = (string)element.Attribute(serviceNameKey);
			if (string.IsNullOrWhiteSpace(text))
			{
				ConfigResponseUrl.Tracer.TraceError<XElement>(0L, "ConfigResponseUrl.ParseServiceName: Unable to parse serviceName for: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_ConfigResponseServiceNameParseFailed, null, element);
			}
			return text;
		}

		private static string ParseUrl(XElement element, XName urlKey, Dictionary<string, XElement> tokenDictionary, string serviceName, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			XElement xelement = element.Element(urlKey);
			if (xelement == null || xelement.Value == null)
			{
				ConfigResponseUrl.Tracer.TraceError<XElement>(0L, "ConfigResponseUrl.ParseUrl: Unable to parse url for: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_ConfigResponseUrlParseFailed, serviceName, element);
				return null;
			}
			string value = xelement.Value;
			string[] array = value.Split(new char[]
			{
				'[',
				']'
			});
			if (array.Length == 1)
			{
				ConfigResponseUrl.Tracer.TraceDebug<XElement>(0L, "ConfigResponseUrl.ParseUrl: Url contains no tokens: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_ConfigResponseUrlNoTokens, serviceName, element);
				return value;
			}
			if (array.Length != 3)
			{
				ConfigResponseUrl.Tracer.TraceError<XElement>(0L, "ConfigResponseUrl.ParseUrl: Expected one token in the response element. Unable to parse url for: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_ConfigResponseUrlTooManyTokens, serviceName, element);
				return null;
			}
			if (tokenDictionary == null || tokenDictionary.Count == 0)
			{
				ConfigResponseUrl.Tracer.TraceError<XElement>(0L, "ConfigResponseUrl.ParseUrl: No tokens in the response. Unable to parse url for: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_ConfigResponseUrlTokenNotFound, serviceName, element);
				return null;
			}
			XElement xelement2 = null;
			if (!tokenDictionary.TryGetValue(array[1], out xelement2))
			{
				ConfigResponseUrl.Tracer.TraceError<XElement>(0L, "ConfigResponseUrl.ParseUrl: Token not found. Unable to parse url for: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_ConfigResponseUrlTokenNotFound, serviceName, element);
				return null;
			}
			string value2 = xelement2.Value;
			string text = array[0] + value2 + array[2];
			if (!Uri.IsWellFormedUriString(text, UriKind.Absolute))
			{
				ConfigResponseUrl.Tracer.TraceError<XElement>(0L, "ConfigResponseUrl.ParseUrl: Constructed url is not well formed for: {0}", element);
				logParseFailureCallback(ApplicationLogicEventLogConstants.Tuple_ConfigResponseUrlNotWellFormed, serviceName, element);
				return null;
			}
			return text;
		}

		private const char LeftTokenDelimiter = '[';

		private const char RightTokenDelimiter = ']';

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;
	}
}
