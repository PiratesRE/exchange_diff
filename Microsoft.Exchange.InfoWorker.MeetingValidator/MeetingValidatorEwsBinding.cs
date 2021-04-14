using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MeetingValidatorEwsBinding : ExchangeServiceBinding
	{
		internal MeetingValidatorEwsBinding(ExchangePrincipal principal, Uri endpoint) : base("ExchangeCalendarRepairAssistant", new RemoteCertificateValidationCallback(MeetingValidatorEwsBinding.CertificateErrorHandler))
		{
			this.RequestServerVersionValue = MeetingValidatorEwsBinding.RequestServerVersionExchange2013;
			base.UserAgent = "ExchangeCalendarRepairAssistant";
			base.Url = endpoint.AbsoluteUri;
			base.Timeout = (int)TimeSpan.FromSeconds((double)Configuration.WebRequestTimeoutInSeconds).TotalMilliseconds;
			NetworkServiceImpersonator.Initialize();
			if (NetworkServiceImpersonator.Exception == null)
			{
				base.Authenticator = SoapHttpClientAuthenticator.CreateNetworkService();
				base.Authenticator.AdditionalSoapHeaders.Add(new OpenAsAdminOrSystemServiceType
				{
					ConnectingSID = new ConnectingSIDType
					{
						Item = new SmtpAddressType
						{
							Value = principal.MailboxInfo.PrimarySmtpAddress.ToString()
						}
					},
					LogonType = SpecialLogonType.SystemService,
					BudgetType = 1,
					BudgetTypeSpecified = true
				});
			}
		}

		private static bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (SslConfiguration.AllowInternalUntrustedCerts || Configuration.IgnoreCertificateErrors)
			{
				Globals.ConsistencyChecksTracer.TraceDebug<string, SslPolicyErrors>((long)sender.GetHashCode(), "MeetingValidatorEwsBinding. Allowed SSL certificate {0} with error {1}", certificate.Subject, sslPolicyErrors);
				return true;
			}
			return false;
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetClientIntent", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("GetClientIntentResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetClientIntentResponseMessageType GetClientIntent([XmlElement("GetClientIntent", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetClientIntentType request)
		{
			object[] array = this.Invoke("GetClientIntent", new object[]
			{
				request
			});
			return (GetClientIntentResponseMessageType)array[0];
		}

		private const string ComponentId = "ExchangeCalendarRepairAssistant";

		private static readonly RequestServerVersion RequestServerVersionExchange2013 = new RequestServerVersion
		{
			Version = ExchangeVersionType.Exchange2013
		};
	}
}
