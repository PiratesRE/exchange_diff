using System;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaMessageHeaderProcessor : JsonMessageHeaderProcessor
	{
		internal override void ProcessMessageHeaders(Message request)
		{
			OwaServiceMessage owaServiceMessage = request as OwaServiceMessage;
			BaseJsonRequest baseJsonRequest = owaServiceMessage.Request as BaseJsonRequest;
			if (baseJsonRequest != null && baseJsonRequest.Header != null)
			{
				base.RequestVersion = JsonMessageHeaderProcessor.ReadRequestVersionHeader(baseJsonRequest.Header.RequestServerVersion.ToString());
				base.MailboxCulture = baseJsonRequest.Header.MailboxCulture;
				base.TimeZoneContext = baseJsonRequest.Header.TimeZoneContext;
				base.DateTimePrecision = new DateTimePrecision?(baseJsonRequest.Header.DateTimePrecision);
				base.IsBackgroundLoad = baseJsonRequest.Header.BackgroundLoad;
				base.ManagementRoleType = baseJsonRequest.Header.ManagementRole;
			}
			base.ProcessRequestVersion(request);
		}

		internal override void ProcessMessageHeadersFromQueryString(Message request)
		{
			OwaServiceMessage owaServiceMessage = request as OwaServiceMessage;
			HttpRequest httpRequest = owaServiceMessage.HttpRequest;
			bool flag = false;
			foreach (object obj in httpRequest.QueryString.Keys)
			{
				string text = (string)obj;
				string a;
				if ((a = text) != null)
				{
					if (!(a == "ManagementRole"))
					{
						if (a == "RequestServerVersion")
						{
							base.RequestVersion = JsonMessageHeaderProcessor.ReadRequestVersionHeader(httpRequest.QueryString.Get(text));
						}
					}
					else
					{
						base.QueryStringXmlDictionaryReaderAction(httpRequest.QueryString.Get(text), delegate(XmlDictionaryReader reader)
						{
							base.ManagementRoleType = JsonMessageHeaderProcessor.ReadManagementRoleHeader(reader);
						});
						flag = true;
					}
				}
			}
			if (flag)
			{
				base.ProcessRequestVersion(request);
			}
		}

		internal override void ProcessHttpHeaders(Message request, ExchangeVersion defaultVersion)
		{
			OwaServiceMessage owaServiceMessage = request as OwaServiceMessage;
			HttpRequest httpRequest = owaServiceMessage.HttpRequest;
			if (httpRequest == null)
			{
				return;
			}
			string text = httpRequest.Headers["X-MailboxCulture"];
			if (!string.IsNullOrEmpty(text))
			{
				base.MailboxCulture = text;
			}
			string text2 = httpRequest.Headers["X-TimeZoneContext"];
			if (!string.IsNullOrEmpty(text2))
			{
				base.TimeZoneContext = new TimeZoneContextType
				{
					TimeZoneDefinition = new TimeZoneDefinitionType
					{
						Id = text2
					}
				};
			}
			string value = httpRequest.Headers["X-DateTimePrecision"];
			if (!string.IsNullOrEmpty(value))
			{
				base.DateTimePrecision = new DateTimePrecision?(JsonMessageHeaderProcessor.ReadDateTimePrecisionHeader(value));
			}
			string value2 = httpRequest.Headers["X-BackgroundLoad"];
			if (!string.IsNullOrEmpty(value2))
			{
				base.IsBackgroundLoad = bool.Parse(value2);
			}
			ExchangeVersion ewsVersionFromHttpHeaders = this.GetEwsVersionFromHttpHeaders(request, httpRequest);
			if (ewsVersionFromHttpHeaders != null)
			{
				base.RequestVersion = ewsVersionFromHttpHeaders;
			}
			else
			{
				string text3 = httpRequest.Headers["X-RequestServerVersion"];
				base.RequestVersion = (string.IsNullOrEmpty(text3) ? defaultVersion : JsonMessageHeaderProcessor.ReadRequestVersionHeader(text3));
			}
			base.ProcessRequestVersion(request);
		}

		internal override void ProcessEwsVersionFromHttpHeaders(Message request)
		{
			OwaServiceMessage owaServiceMessage = request as OwaServiceMessage;
			HttpRequest httpRequest = owaServiceMessage.HttpRequest;
			if (httpRequest == null)
			{
				return;
			}
			ExchangeVersion ewsVersionFromHttpHeaders = this.GetEwsVersionFromHttpHeaders(request, httpRequest);
			if (ewsVersionFromHttpHeaders != null)
			{
				base.RequestVersion = ewsVersionFromHttpHeaders;
				base.ProcessRequestVersion(request);
			}
		}

		internal override ProxyRequestType? ProcessRequestTypeHeader(Message request)
		{
			OwaServiceMessage owaServiceMessage = request as OwaServiceMessage;
			if (owaServiceMessage != null)
			{
				return base.ParseProxyRequestType(owaServiceMessage.HttpRequest.Headers["RequestType"]);
			}
			return null;
		}

		private ExchangeVersion GetEwsVersionFromHttpHeaders(Message request, HttpRequest httpRequest)
		{
			ExchangeVersion result = null;
			string headerValue = httpRequest.Headers["X-EWS-TargetVersion"];
			ExchangeVersionHeader exchangeVersionHeader = new ExchangeVersionHeader(headerValue);
			if (!exchangeVersionHeader.IsMissing)
			{
				ExchangeVersionType version = exchangeVersionHeader.CheckAndGetRequestedVersion();
				result = new ExchangeVersion(version);
			}
			return result;
		}
	}
}
